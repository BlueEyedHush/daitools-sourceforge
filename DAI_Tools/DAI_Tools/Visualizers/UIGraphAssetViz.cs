﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAI_Tools.Frostbite;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using Color = Microsoft.Msagl.Drawing.Color;

namespace DAI_Tools.EBXExplorer
{
    public partial class UIGraphAssetViz : Form
    {
        public UIGraphAssetViz(EbxDataContainers ebxDataContainers, string assetGuid)
        {
            this.ebxDataContainers = ebxDataContainers;
            this.assetGuid = assetGuid;
            InitializeComponent();
        }

        private EbxDataContainers ebxDataContainers;
        private string assetGuid;
        private GViewer viewer;

        private void UIGraphAssetViz_Load(object sender, EventArgs e)
        {
            //create a viewer object 
            viewer = new GViewer();
                
            //create the graph content 
            drawGraphSafely();

            //associate the viewer with the form 
            this.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            this.graphVizPanel.Controls.Add(viewer);
            this.ResumeLayout();
        }

        private class PortDesc
        {
            public PortDesc(int portIdx, string portName, string nodeLabel)
            {
                this.portIdx = portIdx;
                this.portName = portName;
                this.nodeLabel = nodeLabel;
            }

            public int refCount = 0;
            public int portIdx;
            public string portName;
            public string nodeLabel;
        }

        private void drawGraphSafely()
        {
            try {
                drawGraph();
            } catch (Exception ex)
            {
                MessageBox.Show("Exception:\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void drawGraph()
        {
            var uiGraphAsset = ebxDataContainers.instances[assetGuid];
            var assetName = uiGraphAsset.data.get("Name").castTo<ASimpleValue>().Val;
            
            Graph graph = new Graph(assetName);
            
            AArray nodes = uiGraphAsset.data.get("Nodes").castTo<AArray>();
            var splitNodes = new HashSet<string>();
            var portsGuidToPortDesc = new Dictionary<string, PortDesc>();
            /* target port guid, source vertex name */
            var jumpNodeEdges = new List<Tuple<string, string>>();

            int nodeNextIdx = 0;
            int portNextIdx = 0;

            foreach (var nodeRef in nodes.elements)
            {
                var nodeInRef = nodeRef.castTo<AIntRef>();
                var nodeAStruct = ebxDataContainers.instances[nodeInRef.instanceGuid].data;
                var nodeName = nodeAStruct.get("Name").castTo<ASimpleValue>().Val;
                bool isRoot = Convert.ToBoolean(nodeAStruct.get("IsRootNode").castTo<ASimpleValue>().Val);
                var nodeContainer = ebxDataContainers.instances[nodeInRef.instanceGuid];
                var nodeType = nodeContainer.data.name;
                var nodeLabel = "N" + nodeNextIdx.ToString() + ": " + nodeName + "\n[" + nodeType + "]";
                nodeNextIdx += 1;
                var nodeNode = graph.AddNode(nodeLabel);
                
                var ports = ebxDataContainers.getIntRefedObjsByTypeFor(nodeInRef.instanceGuid, "UINodePort");

                foreach (var dataContainer in ports)
                {
                    var portName = dataContainer.data.get("Name").castTo<ASimpleValue>().Val;
                    var portDesc = new PortDesc(portNextIdx, portName, nodeLabel);
                    portNextIdx += 1;

                    if (!portsGuidToPortDesc.ContainsKey(dataContainer.guid))
                        portsGuidToPortDesc.Add(dataContainer.guid, portDesc);
                }

                /* handling special nodes */
                if (nodeContainer.hasPartial("JumpNode"))
                {
                    var targetPortGuid = nodeAStruct.get("TargetPort").castTo<AIntRef>().instanceGuid;
                    jumpNodeEdges.Add(new Tuple<string, string>(targetPortGuid, nodeLabel));
                }
                else if (nodeContainer.hasPartial("SplitterNode"))
                    splitNodes.Add(nodeLabel);

                /* some visual formatting */
                nodeNode.Attr.LabelMargin = 3;
                nodeNode.Attr.Padding = 2;
                nodeNode.Attr.Shape = Shape.Box;

                if (isRoot)
                    nodeNode.Attr.FillColor = Color.PaleVioletRed;
                else if (nodeContainer.hasPartial("InstanceOutputNode"))
                    nodeNode.Attr.FillColor = Color.LightBlue;
                else if (nodeContainer.hasPartial("InstanceInputNode"))
                    nodeNode.Attr.FillColor = Color.MediumPurple;
                else
                    nodeNode.Attr.FillColor = Color.LightGreen;
            }

            /* draw ordinary edges */
            var connections = uiGraphAsset.data.get("Connections").castTo<AArray>();

            foreach (var connRef in connections.elements)
            {
                var conn = ebxDataContainers.instances[connRef.castTo<AIntRef>().instanceGuid].data;
                var srcGuid = conn.get("SourcePort").castTo<AIntRef>().instanceGuid;
                var targetGuid = conn.get("TargetPort").castTo<AIntRef>().instanceGuid;
                var srcPortDesc = portsGuidToPortDesc[srcGuid];
                var targetPortDesc = portsGuidToPortDesc[targetGuid];

                var connLabel = "";
                if (srcPortDesc.portName.Length > 0 || targetPortDesc.portName.Length > 0)
                    connLabel = srcPortDesc.portName + " -> " + targetPortDesc.portName;

                var edge = graph.AddEdge(srcPortDesc.nodeLabel, connLabel, targetPortDesc.nodeLabel);
                edge.Attr.Weight = 10;

                srcPortDesc.refCount += 1;
                targetPortDesc.refCount += 1;
            }

            /* draw special edges */
            foreach (var jumpEdge in jumpNodeEdges)
            {
                var sourceNode = jumpEdge.Item2;
                var targetPortGuid = jumpEdge.Item1;

                var targetPortDesc = portsGuidToPortDesc[targetPortGuid];
                var edge = graph.AddEdge(sourceNode, "", targetPortDesc.nodeLabel);
                targetPortDesc.refCount += 1;

                /* visual formatting */
                edge.Attr.Weight = 1;
                edge.Attr.AddStyle(Style.Dotted);
                edge.Attr.Color = Color.OrangeRed;
            }

            foreach (var portDesc in portsGuidToPortDesc.Values)
            {
                if (portDesc.refCount < 1)
                {
                    var portLabel = "P" + portDesc.portIdx + ": " + portDesc.portName;
                    var portNode = graph.AddNode(portLabel);

                    var portEdge = graph.AddEdge(portDesc.nodeLabel, "", portLabel);

                    /* visual formatting */
                    portNode.Attr.FillColor = Color.Orange;
                    portNode.Attr.Shape = Shape.Box;
                    portEdge.Attr.ArrowheadAtSource = ArrowStyle.None;
                    portEdge.Attr.ArrowheadAtTarget = ArrowStyle.None;
                }
            }
            
            this.SuspendLayout();
            /* bind graph to viewer; must be bound for RemoveNode/Edge to work */
            viewer.Graph = graph;

            /* remove split nodes */
            if (hideSplittersCheckbox.Checked)
            {
                var nodesToRemove = new List<Node>();
                var edgesToRemove = new List<Edge>();
                
                foreach (var splitNode in splitNodes)
                {
                    var node = graph.FindNode(splitNode);

                    foreach (var inEdge in node.InEdges)
                    {
                        var inNode = inEdge.SourceNode;

                        foreach (var outEdge in node.OutEdges)
                        {
                            var newEdge = graph.AddEdge(inNode.Id, outEdge.LabelText, outEdge.TargetNode.Id);
                            foreach (var style in outEdge.Attr.Styles)
                                newEdge.Attr.AddStyle(style);
                            newEdge.Attr.Color = outEdge.Attr.Color;

                            edgesToRemove.Add(outEdge);
                        }

                        edgesToRemove.Add(inEdge);
                    }

                    nodesToRemove.Add(node);
                }

                foreach (var node in nodesToRemove)
                    graph.RemoveNode(node);
            }

            /* some visual formatting */
            var layoutSettings = new SugiyamaLayoutSettings();
            graph.LayoutAlgorithmSettings = layoutSettings;

            this.ResumeLayout();
            /* assign it once again... */
            viewer.Graph = graph;
        }

        private void hideSplittersCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (viewer != null)
                drawGraphSafely();
        }
    }
}

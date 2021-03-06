﻿namespace DAI_Tools.EBXExplorer
{
    partial class BlueprintViz
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.contentContainer = new System.Windows.Forms.SplitContainer();
            this.showUnconnPortsCbkb = new System.Windows.Forms.CheckBox();
            this.showLinkConnsCheckbox = new System.Windows.Forms.CheckBox();
            this.showEventConnsCheckbox = new System.Windows.Forms.CheckBox();
            this.toolsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.showPropertyConnsCheckbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.contentContainer)).BeginInit();
            this.contentContainer.Panel2.SuspendLayout();
            this.contentContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolsSplitContainer)).BeginInit();
            this.toolsSplitContainer.Panel1.SuspendLayout();
            this.toolsSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentContainer
            // 
            this.contentContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentContainer.Location = new System.Drawing.Point(0, 0);
            this.contentContainer.Name = "contentContainer";
            // 
            // contentContainer.Panel2
            // 
            this.contentContainer.Panel2.Controls.Add(this.toolsSplitContainer);
            this.contentContainer.Size = new System.Drawing.Size(1264, 682);
            this.contentContainer.SplitterDistance = 1007;
            this.contentContainer.TabIndex = 0;
            // 
            // showUnconnPortsCbkb
            // 
            this.showUnconnPortsCbkb.AutoSize = true;
            this.showUnconnPortsCbkb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.showUnconnPortsCbkb.Location = new System.Drawing.Point(19, 88);
            this.showUnconnPortsCbkb.Name = "showUnconnPortsCbkb";
            this.showUnconnPortsCbkb.Size = new System.Drawing.Size(124, 17);
            this.showUnconnPortsCbkb.TabIndex = 3;
            this.showUnconnPortsCbkb.Text = "Show unconn ports?";
            this.showUnconnPortsCbkb.UseVisualStyleBackColor = false;
            this.showUnconnPortsCbkb.CheckedChanged += new System.EventHandler(this.showUnconnPortsCbkb_CheckedChanged);
            // 
            // showLinkConnsCheckbox
            // 
            this.showLinkConnsCheckbox.AutoSize = true;
            this.showLinkConnsCheckbox.BackColor = System.Drawing.Color.Red;
            this.showLinkConnsCheckbox.Checked = true;
            this.showLinkConnsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showLinkConnsCheckbox.Location = new System.Drawing.Point(19, 65);
            this.showLinkConnsCheckbox.Name = "showLinkConnsCheckbox";
            this.showLinkConnsCheckbox.Size = new System.Drawing.Size(112, 17);
            this.showLinkConnsCheckbox.TabIndex = 2;
            this.showLinkConnsCheckbox.Text = "Show LinkConns?";
            this.showLinkConnsCheckbox.UseVisualStyleBackColor = false;
            this.showLinkConnsCheckbox.CheckedChanged += new System.EventHandler(this.showLinkConnsCheckbox_CheckedChanged);
            // 
            // showEventConnsCheckbox
            // 
            this.showEventConnsCheckbox.AutoSize = true;
            this.showEventConnsCheckbox.BackColor = System.Drawing.Color.Blue;
            this.showEventConnsCheckbox.Checked = true;
            this.showEventConnsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showEventConnsCheckbox.Location = new System.Drawing.Point(19, 19);
            this.showEventConnsCheckbox.Name = "showEventConnsCheckbox";
            this.showEventConnsCheckbox.Size = new System.Drawing.Size(120, 17);
            this.showEventConnsCheckbox.TabIndex = 0;
            this.showEventConnsCheckbox.Text = "Show EventConns?";
            this.showEventConnsCheckbox.UseVisualStyleBackColor = false;
            this.showEventConnsCheckbox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // toolsSplitContainer
            // 
            this.toolsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.toolsSplitContainer.Name = "toolsSplitContainer";
            this.toolsSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // toolsSplitContainer.Panel1
            // 
            this.toolsSplitContainer.Panel1.Controls.Add(this.showPropertyConnsCheckbox);
            this.toolsSplitContainer.Panel1.Controls.Add(this.showUnconnPortsCbkb);
            this.toolsSplitContainer.Panel1.Controls.Add(this.showEventConnsCheckbox);
            this.toolsSplitContainer.Panel1.Controls.Add(this.showLinkConnsCheckbox);
            this.toolsSplitContainer.Size = new System.Drawing.Size(253, 682);
            this.toolsSplitContainer.SplitterDistance = 128;
            this.toolsSplitContainer.TabIndex = 4;
            // 
            // showPropertyConnsCheckbox
            // 
            this.showPropertyConnsCheckbox.AutoSize = true;
            this.showPropertyConnsCheckbox.BackColor = System.Drawing.Color.Green;
            this.showPropertyConnsCheckbox.Checked = true;
            this.showPropertyConnsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showPropertyConnsCheckbox.Location = new System.Drawing.Point(19, 42);
            this.showPropertyConnsCheckbox.Name = "showPropertyConnsCheckbox";
            this.showPropertyConnsCheckbox.Size = new System.Drawing.Size(131, 17);
            this.showPropertyConnsCheckbox.TabIndex = 1;
            this.showPropertyConnsCheckbox.Text = "Show PropertyConns?";
            this.showPropertyConnsCheckbox.UseVisualStyleBackColor = false;
            this.showPropertyConnsCheckbox.CheckedChanged += new System.EventHandler(this.showPropertyConnsCheckbox_CheckedChanged);
            // 
            // BlueprintViz
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 682);
            this.Controls.Add(this.contentContainer);
            this.Name = "BlueprintViz";
            this.Text = "BlueprintViz";
            this.Load += new System.EventHandler(this.BlueprintViz_Load);
            this.contentContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.contentContainer)).EndInit();
            this.contentContainer.ResumeLayout(false);
            this.toolsSplitContainer.Panel1.ResumeLayout(false);
            this.toolsSplitContainer.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toolsSplitContainer)).EndInit();
            this.toolsSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer contentContainer;
        private System.Windows.Forms.CheckBox showEventConnsCheckbox;
        private System.Windows.Forms.CheckBox showLinkConnsCheckbox;
        private System.Windows.Forms.CheckBox showUnconnPortsCbkb;
        private System.Windows.Forms.SplitContainer toolsSplitContainer;
        private System.Windows.Forms.CheckBox showPropertyConnsCheckbox;
    }
}
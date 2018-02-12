﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAI_Tools.Frostbite
{
    public class STR
    {
        public uint ID;
        public String Value;
    }

    public class Talktable
    {
        public List<STR> Strings;

        List<int> NodeList;
        List<StringID> StringList;
        List<uint> BitStream;

        public class HuffNode
        {
            public HuffNode e0;
            public HuffNode e1;
            public char c;
            public long w;
            public int index;
            public bool hasIndex;
            public HuffNode(char chr, long weight)
            {
                e0 = e1 = null;
                c = chr;
                w = weight;
                hasIndex = false;
            }
        }

        public class AlphaEntry
        {
            public bool[] list;
            public char c;
        }

        public class StringID
        {
            public uint ID;
            public uint offset;
        }

        public uint magic;
        public int unk01;
        public int DataOffset;
        public int unk02;
        public int unk03;
        public int unk04;
        public int Data1Count;
        public int Data1Offset;
        public int Data2Count;
        public int Data2Offset;
        public int Data3Count;
        public int Data3Offset;
        public int Data4Count;
        public int Data4Offset;
        public int Data5Count;
        public int Data5Offset;
        public List<int> Data1;
        public List<uint> StringIDs;
        public List<int> StringData;
        public List<uint> Data;

        public bool Read(MemoryStream s)
        {
            magic = Tools.ReadUInt(s);
            if (magic != 0xD78B40EB)
                return false;

            unk01 = Tools.ReadInt(s);
            DataOffset = Tools.ReadInt(s);
            unk02 = Tools.ReadInt(s);
            unk03 = Tools.ReadInt(s);
            unk04 = Tools.ReadInt(s);
            Data1Count = Tools.ReadInt(s);
            Data1Offset = Tools.ReadInt(s);
            Data2Count = Tools.ReadInt(s);
            Data2Offset = Tools.ReadInt(s);
            Data3Count = Tools.ReadInt(s);
            Data3Offset = Tools.ReadInt(s);
            Data4Count = Tools.ReadInt(s);
            Data4Offset = Tools.ReadInt(s);
            if (Data4Count > 0)
            {
                Data5Count = Tools.ReadInt(s);
                Data5Offset = Tools.ReadInt(s);
            }
            s.Seek(Data1Offset, SeekOrigin.Begin);
            Data1 = new List<int>();
            for (int i = 0; i < Data1Count; i++)
                Data1.Add(Tools.ReadInt(s));
            s.Seek(Data2Offset, SeekOrigin.Begin);
            StringIDs = new List<uint>();
            StringData = new List<int>();
            for (int i = 0; i < Data2Count; i++)
            {
                StringIDs.Add(Tools.ReadUInt(s));
                StringData.Add(Tools.ReadInt(s));
            }
            s.Seek(DataOffset, SeekOrigin.Begin);
            Data = new List<uint>();
            while (s.Position < s.Length)
                Data.Add(Tools.ReadUInt(s));
            Strings = new List<STR>();
            for (int i = 0; i < StringIDs.Count; i++)
            {
                STR ValueString = new STR();
                ValueString.ID = StringIDs[i];
                ValueString.Value = "";
                int Index = StringData[i] >> 5;
                int Shift = StringData[i] & 0x1F;
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    int e = (Data1.Count / 2) - 1;
                    while (e >= 0)
                    {
                        uint d = Data[Index];
                        int offset = (int)((d>> Shift) & 1);
                        e = Data1[(e * 2) + offset];

                        Shift++;
                        Index += (Shift >> 5);
                        Shift %= 32;
                    }

                    ushort c = (ushort)(0xFFFFFFFF - (uint)e);
                    if (c == 0)
                        break;
                    //if (c == 10)
                    //    sb.Append("[/n]");
                    //else if (c == 13)
                    //    sb.Append("[/r]");
                    else
                        sb.Append((char)c);
                }
                ValueString.Value = sb.ToString();
                Strings.Add(ValueString);
            }
            return true;
        }

        public void Save(Stream s)
        {
            long[] weights = new long[256 * 256];
            foreach (STR line in Strings)
            {
                weights[0]++;   //string terminator for line
                foreach (char c in line.Value)
                    weights[(ushort)c]++;
            }
            Dictionary<char, long> weighttable = new Dictionary<char, long>();
            for (int i = 0; i < 256 * 256; i++)
                if (weights[i] > 0)
                    weighttable.Add((char)i, weights[i]);
            List<HuffNode> nodes = new List<HuffNode>();
            foreach (KeyValuePair<char, long> w in weighttable)
                nodes.Add(new HuffNode(w.Key, w.Value));
            while (nodes.Count > 1)
            {
                bool run = true;
                while (run)
                {
                    run = false;
                    for (int i = 0; i < nodes.Count - 1; i++)
                        if (nodes[i].w > nodes[i + 1].w)
                        {
                            run = true;
                            HuffNode t = nodes[i];
                            nodes[i] = nodes[i + 1];
                            nodes[i + 1] = t;
                        }
                }
                HuffNode e0 = nodes[0];
                HuffNode e1 = nodes[1];
                HuffNode combine = new HuffNode(' ', e0.w + e1.w);
                combine.e0 = e0;
                combine.e1 = e1;
                nodes.RemoveAt(1);
                nodes.RemoveAt(0);
                nodes.Add(combine);
            }
            HuffNode root = nodes[0];
            NodeList = new List<int>();
            while (!root.hasIndex)
                CalcIndex(root);
            AlphaEntry[] alphabet = GetAlphabet(root, new List<bool>());
            BitStream = new List<uint>();
            StringList = new List<StringID>();
            uint curr = 0;
            uint index = 0;
            byte shift = 0;
            foreach (STR str in Strings)
            {
                StringID t = new StringID();
                t.ID = str.ID;
                t.offset = index << 5;
                t.offset += shift;
                string line = str.Value + "\0";
                foreach (char c in line)
                {
                    AlphaEntry alpha = null;
                    foreach (AlphaEntry a in alphabet)
                        if (a.c == c)
                            alpha = a;
                    foreach (bool step in alpha.list)
                    {
                        byte b = 0;
                        if (step)
                            b = 1;
                        if (shift < 32)
                        {
                            curr += (uint)(b << shift);
                            shift++;
                        }
                        if (shift == 32)
                        {
                            BitStream.Add(curr);
                            index++;
                            shift = 0;
                            curr = 0;
                        }
                    }
                }
                StringList.Add(t);
            }
            BitStream.Add(curr);
            Tools.WriteInt(s, (int)magic);                       
            Tools.WriteInt(s, (int)unk01);
            Tools.WriteInt(s, 0x38 + NodeList.Count * 4 + StringList.Count * 8);
            Tools.WriteInt(s, (int)unk02);
            Tools.WriteInt(s, (int)unk03);                        
            Tools.WriteInt(s, (int)unk04);
            Tools.WriteInt(s, NodeList.Count);
            Tools.WriteInt(s, 0x38);
            Tools.WriteInt(s, StringList.Count);
            Tools.WriteInt(s, 0x38 + NodeList.Count * 4);
            Tools.WriteInt(s, 0);
            Tools.WriteInt(s, 0x38 + NodeList.Count * 4 + StringList.Count * 8);
            Tools.WriteInt(s, 0);
            Tools.WriteInt(s, 0x38 + NodeList.Count * 4 + StringList.Count * 8);
            foreach (int i in NodeList)
                Tools.WriteInt(s, i);
            foreach (StringID sid in StringList)
            {
                Tools.WriteInt(s, (int)sid.ID);
                Tools.WriteInt(s, (int)sid.offset);
            }
            foreach (int i in BitStream)
                Tools.WriteInt(s, i);
        }

        public void CalcIndex(HuffNode h)
        {
            if (h.e0 == null && h.e1 == null && !h.hasIndex)
            {
                int u;
                if (((ushort)h.c) < 0x100)
                    u = (short)(0xFFFF - (short)h.c);
                else
                    u = (int)(0xFFFFFFFF - (uint)h.c);
                h.index = u;
                h.hasIndex = true;
            }
            else
            {
                CalcIndex(h.e0);
                CalcIndex(h.e1);
                if (h.e0.hasIndex && h.e1.hasIndex)
                {
                    h.index = NodeList.Count / 2;
                    h.hasIndex = true;
                    NodeList.Add(h.e0.index);
                    NodeList.Add(h.e1.index);
                }
            }
        }

        public AlphaEntry[] GetAlphabet(HuffNode h, List<bool> list)
        {
            List<AlphaEntry> result = new List<AlphaEntry>();
            if (h.e0.e0 == null)
            {
                AlphaEntry e = new AlphaEntry();
                e.c = h.e0.c;
                List<bool> t = new List<bool>();
                t.AddRange(list);
                t.Add(false);
                e.list = t.ToArray();
                result.Add(e);
            }
            else
            {
                List<bool> t = new List<bool>();
                t.AddRange(list);
                t.Add(false);
                result.AddRange(GetAlphabet(h.e0, t));
            }
            if (h.e1.e0 == null)
            {
                AlphaEntry e = new AlphaEntry();
                e.c = h.e1.c;
                List<bool> t = new List<bool>();
                t.AddRange(list);
                t.Add(true);
                e.list = t.ToArray();
                result.Add(e);
            }
            else
            {
                List<bool> t = new List<bool>();
                t.AddRange(list);
                t.Add(true);
                result.AddRange(GetAlphabet(h.e1, t));
            }
            return result.ToArray();
        }
    }
}

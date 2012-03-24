using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace FimbulwinterClient.Content
{
    public class RsmBone
    {
        private Matrix transform;
        public Matrix Transform
        {
            get { return transform; }
            set { transform = value; }
        }

        private RsmBone parent;
        public RsmBone Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private RsmBone[] children;
        public RsmBone[] Children
        {
            get { return children; }
            set { children = value; }
        }

        private int index;
        public int Index
        {
            get { return index; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        public RsmBone(int idx, ROFormats.Model.Node node)
        {
            name = node.Name;
            index = idx;
            transform = new Matrix(
                node.OffsetMT[0], node.OffsetMT[1], node.OffsetMT[2], 0.0F,
                node.OffsetMT[3], node.OffsetMT[4], node.OffsetMT[5], 0.0F,
                node.OffsetMT[6], node.OffsetMT[7], node.OffsetMT[8], 0.0F,
                0.0F, 0.0F, 0.0F, 1.0F);
        }
    }
}

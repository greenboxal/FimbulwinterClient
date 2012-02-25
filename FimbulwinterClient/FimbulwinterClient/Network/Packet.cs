using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Network
{
    public abstract class InPacket
    {
        public abstract bool Read(byte[] data);
    }

    public abstract class OutPacket
    {
        private ushort packet;
        private int size;
        private bool isFixed;

        public OutPacket(ushort packet, int size)
        {
            this.packet = packet;
            this.size = size;

            isFixed = size > 0;
        }

        public virtual bool Write(BinaryWriter bw)
        {
            bw.Write(packet);

            if (!isFixed)
            {
                ComputeSize();
                bw.Write((ushort)size);
            }

            return true;
        }

        protected virtual void ComputeSize()
        {

        }

        public ushort PacketCmd
        {
            get { return packet; }
            set { packet = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }
    }
}

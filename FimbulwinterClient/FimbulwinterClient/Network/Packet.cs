using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Network
{
    public abstract class InPacket
    {
        private ushort packet;
        private int size;
        private bool isFixed;

        public InPacket(ushort packet, int size)
        {
            this.packet = packet;
            this.size = size;

            isFixed = size > 0;
        }

        public virtual bool Read(byte[] br)
        {
            packet = BitConverter.ToUInt16(br, 0);

            if (!isFixed)
                size = BitConverter.ToInt16(br, 2);

            return true;
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
            if (!isFixed)
                ComputeSize();

            bw.Write(packet);
            bw.Write((ushort)size);

            return true;
        }

        protected abstract void ComputeSize();

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

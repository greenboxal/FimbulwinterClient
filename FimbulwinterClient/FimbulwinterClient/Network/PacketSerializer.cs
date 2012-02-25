using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using FimbulwinterClient.Network.Packets;

namespace FimbulwinterClient.Network
{
    public class PacketSerializer
    {
        public struct PacketInfo
        {
            public int Size;
            public Type Type;
        }

        private MemoryStream m_memory;
        public MemoryStream Memory
        {
            get { return m_memory; }
            set { m_memory = value; }
        }

        private static Dictionary<ushort, PacketInfo> m_packetSize;
        public static Dictionary<ushort, PacketInfo> PacketSize
        {
            get { return m_packetSize; }
        }

        static PacketSerializer()
        {
            m_packetSize = new Dictionary<ushort, PacketInfo>();

            m_packetSize.Add(0x69, new PacketInfo { Size = -1, Type = typeof(AcceptLogin) });
            m_packetSize.Add(0x6A, new PacketInfo { Size = 23, Type = typeof(RejectLogin) });
        }

        public PacketSerializer()
        {
            m_memory = new MemoryStream();
        }

        public void EnqueueBytes(byte[] data, int size)
        {
            int pos = (int)m_memory.Position;
            m_memory.Position = m_memory.Length;
            m_memory.Write(data, 0, size);
            m_memory.Position = pos;

            TryReadPackets();
        }

        public void Reset()
        {
            m_memory = new MemoryStream();
        }

        private void TryReadPackets()
        {
            while (m_memory.Length - m_memory.Position > 2)
            {
                byte[] tmp = new byte[2];

                m_memory.Read(tmp, 0, 2);
                ushort cmd = BitConverter.ToUInt16(tmp, 0);

                if (!m_packetSize.ContainsKey(cmd))
                {
                    if (InvalidPacket != null)
                        InvalidPacket();

                    m_memory.Position -= 2;

                    break;
                }
                else
                {
                    int size = m_packetSize[cmd].Size;
                    bool isFixed = true;

                    if (size <= 0)
                    {
                        isFixed = false;

                        if (m_memory.Length - m_memory.Position > 2)
                        {
                            m_memory.Read(tmp, 0, 2);
                            size = BitConverter.ToUInt16(tmp, 0);
                        }
                        else
                        {
                            m_memory.Position -= 4;

                            break;
                        }
                    }

                    if (PacketReceived != null)
                    {
                        byte[] data = new byte[size];
                        m_memory.Read(data, 0, size - (isFixed ? 2 : 4));

                        ConstructorInfo ci = m_packetSize[cmd].Type.GetConstructor(new Type[] { });
                        InPacket p = (InPacket)ci.Invoke(null);

                        if (!p.Read(data))
                        {
                            if (InvalidPacket != null)
                                InvalidPacket();

                            break;
                        }

                        if (PacketReceived != null)
                            PacketReceived(cmd, size, p);
                    }
                }
            }

            if (m_memory.Length - m_memory.Position > 0)
            {
                MemoryStream ms = new MemoryStream();

                ms.Write(m_memory.GetBuffer(), (int)m_memory.Position, (int)m_memory.Length - (int)m_memory.Position);
                m_memory.Dispose();

                m_memory = ms;
            }
        }

        public event Action InvalidPacket;
        public event Action<ushort, int, InPacket> PacketReceived;
    }
}

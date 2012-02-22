using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FimbulwinterClient.Network
{
    public class PacketSerializer
    {
        private MemoryStream m_memory;
        public MemoryStream Memory
        {
            get { return m_memory; }
            set { m_memory = value; }
        }

        private static Dictionary<ushort, int> m_packetSize;
        public static Dictionary<ushort, int> PacketSize
        {
            get { return m_packetSize; }
        }

        static PacketSerializer()
        {
            m_packetSize = new Dictionary<ushort, int>();
        }

        public PacketSerializer()
        {
            m_memory = new MemoryStream();
        }

        public void EnqueueBytes(byte[] data, int size)
        {
            m_memory.Write(data, 0, data.Length);

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
                    int size = m_packetSize[cmd];
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
                            m_memory.Position -= 2;

                            break;
                        }
                    }

                    if (PacketReceived != null)
                    {
                        m_memory.Position -= isFixed ? 2 : 4;

                        byte[] data = new byte[size];
                        m_memory.Read(data, 0, size);

                        PacketReceived(cmd, size, data);
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
        public event Action<ushort, int, byte[]> PacketReceived;
    }
}

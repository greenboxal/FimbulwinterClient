using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using FimbulwinterClient.Network.Packets;
using FimbulwinterClient.Network.Packets.Char;
using FimbulwinterClient.Network.Packets.Login;

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

        private int m_bytesToSkip;
        public int BytesToSkip
        {
            get { return m_bytesToSkip; }
            set { m_bytesToSkip = value; }
        }

        private static Dictionary<ushort, PacketInfo> m_packetSize;
        public static Dictionary<ushort, PacketInfo> PacketSize
        {
            get { return m_packetSize; }
        }

        static PacketSerializer()
        {
            m_packetSize = new Dictionary<ushort, PacketInfo>();

            // Login
            m_packetSize.Add(0x0069, new PacketInfo { Size = -1, Type = typeof(AcceptLogin) });
            m_packetSize.Add(0x006A, new PacketInfo { Size = 23, Type = typeof(RejectLogin) });

            // Char
            m_packetSize.Add(0x006C, new PacketInfo { Size = 3, Type = typeof(CSRejectLogin) });
            m_packetSize.Add(0x006B, new PacketInfo { Size = -1, Type = typeof(CSAcceptLogin) });
            m_packetSize.Add(0x08B9, new PacketInfo { Size = 12, Type = typeof(PinCodeRequest) });
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
            if (m_bytesToSkip > 0)
            {
                int skipped = Math.Min(m_bytesToSkip, (int)m_memory.Length);
                m_memory.Position += skipped;
                m_bytesToSkip -= skipped;
            }

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

using System;
using System.Diagnostics;
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

        private MemoryStream memory;
        public MemoryStream Memory
        {
            get { return memory; }
            set { memory = value; }
        }

        private int bytesToSkip;
        public int BytesToSkip
        {
            get { return bytesToSkip; }
            set { bytesToSkip = value; }
        }

        private static Dictionary<ushort, PacketInfo> packetSize;
        public static Dictionary<ushort, PacketInfo> PacketSize
        {
            get { return packetSize; }
        }

        private Dictionary<ushort, Delegate> packetHooks;
        public Dictionary<ushort, Delegate> PacketHooks
        {
            get { return packetHooks; }
            set { packetHooks = value; }
        }

        static PacketSerializer()
        {
            packetSize = new Dictionary<ushort, PacketInfo>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(type => type.GetInterface("InPacket") != null))
            {
                object[] attributes = type.GetCustomAttributes(typeof(MethodAttribute), true); // get the attributes of the packet.
                if (attributes.Length == 0) return;
                MethodAttribute ma = (MethodAttribute)attributes[0];
                packetSize.Add(ma.MethodId, new PacketInfo { Size = ma.Size, Type = type });
            }
        }

        public PacketSerializer()
        {
            memory = new MemoryStream();
            packetHooks = new Dictionary<ushort, Delegate>();
        }

        public void EnqueueBytes(byte[] data, int size)
        {
            int pos = (int)memory.Position;
            memory.Position = memory.Length;
            memory.Write(data, 0, size);
            memory.Position = pos;

            TryReadPackets();
        }

        public void Reset()
        {
            memory = new MemoryStream();
        }

        private void TryReadPackets()
        {
            if (bytesToSkip > 0)
            {
                int skipped = Math.Min(bytesToSkip, (int)memory.Length);
                memory.Position += skipped;
                bytesToSkip -= skipped;
            }

            while (memory.Length - memory.Position > 2)
            {
                byte[] tmp = new byte[2];

                memory.Read(tmp, 0, 2);
                ushort cmd = BitConverter.ToUInt16(tmp, 0);

                if (!packetSize.ContainsKey(cmd))
                {
                    if (InvalidPacket != null)
                        InvalidPacket();

                    memory.Position -= 2;

                    break;
                }
                else
                {
                    int size = packetSize[cmd].Size;
                    bool isFixed = true;

                    if (size <= 0)
                    {
                        isFixed = false;

                        if (memory.Length - memory.Position > 2)
                        {
                            memory.Read(tmp, 0, 2);
                            size = BitConverter.ToUInt16(tmp, 0);
                        }
                        else
                        {
                            memory.Position -= 4;

                            break;
                        }
                    }

                    byte[] data = new byte[size];
                    memory.Read(data, 0, size - (isFixed ? 2 : 4));

                    ConstructorInfo ci = packetSize[cmd].Type.GetConstructor(new Type[] { });
                    InPacket p = (InPacket)ci.Invoke(null);
                    Debug.Print("Packet " + p.ToString());

                    if (!p.Read(data))
                    {
                        if (InvalidPacket != null)
                            InvalidPacket();

                        break;
                    }

                    if (packetHooks.ContainsKey(cmd))
                        packetHooks[cmd].DynamicInvoke(cmd, size, p);

                    if (PacketReceived != null)
                        PacketReceived(cmd, size, p);
                }
            }

            if (memory.Length - memory.Position > 0)
            {
                MemoryStream ms = new MemoryStream();

                ms.Write(memory.GetBuffer(), (int)memory.Position, (int)memory.Length - (int)memory.Position);
                memory.Dispose();

                memory = ms;
            }
        }

        public event Action InvalidPacket;
        public event Action<ushort, int, InPacket> PacketReceived;
    }
}

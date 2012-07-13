using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MethodAttribute : Attribute
    {
        public const int packet_length_dynamic = -1;
        public enum packetdirection
        {
            pd_none,
            pd_in,
            pd_out
        }

        public ushort MethodId { get; private set; }
        public string Name { get; private set; }
        public int Size { get; private set; }
        public packetdirection Direction { get; private set; }

        public MethodAttribute(ushort methodId, string name, int size, packetdirection direction)
        {
            this.MethodId = methodId;
            this.Name = name;
            this.Size = size;
            this.Direction = direction;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FimbulwinterClient.Network.Packets
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PackerHandlerAttribute : Attribute
    {
        public const int VariableSize = -1;
        public enum PacketDirection
        {
            None,
            In,
            Out
        }

        public ushort MethodId { get; private set; }
        public string Name { get; private set; }
        public int Size { get; private set; }
        public PacketDirection Direction { get; private set; }

        public PackerHandlerAttribute(PacketHeader methodId, string name, int size, PacketDirection direction)
        {
            this.MethodId = (ushort)methodId;
            this.Name = name;
            this.Size = size;
            this.Direction = direction;
        }
    }
}

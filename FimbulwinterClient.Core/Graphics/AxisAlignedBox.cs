using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace FimbulwinterClient.Core.Graphics
{
    public class AxisAlignedBox
    {
        public Vector3 Min;
        public Vector3 Max;
        public Vector3 Range;
        public Vector3 Offset;

        public AxisAlignedBox(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public static AxisAlignedBox operator +(AxisAlignedBox box, Vector3 vertex)
        {
            box.Min.X = Math.Min(box.Min.X, vertex.X);
            box.Min.Y = Math.Min(box.Min.Y, vertex.Y);
            box.Min.Z = Math.Min(box.Min.Z, vertex.Z);

            box.Max.X = Math.Max(box.Max.X, vertex.X);
            box.Max.Y = Math.Max(box.Max.Y, vertex.Y);
            box.Max.Z = Math.Max(box.Max.Z, vertex.Z);

            return box;
        }

        public static AxisAlignedBox operator +(AxisAlignedBox box, AxisAlignedBox other)
        {
            box.Min.X = Math.Min(box.Min.X, other.Min.X);
            box.Min.Y = Math.Min(box.Min.Y, other.Min.Y);
            box.Min.Z = Math.Min(box.Min.Z, other.Min.Z);

            box.Max.X = Math.Max(box.Max.X, other.Max.X);
            box.Max.Y = Math.Max(box.Max.Y, other.Max.Y);
            box.Max.Z = Math.Max(box.Max.Z, other.Max.Z);

            return box;
        }

        public void CalculateRangeAndOffset()
        {
            Offset = (Max + Min) / 2.0F;
            Range = (Max - Min) / 2.0F;
        }
    }
}

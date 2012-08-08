#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: DefaultHardwareIndexBuffer.cs 3346 2012-07-26 16:38:17Z romeoxbm $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Diagnostics;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Graphics
{
    public class DefaultHardwareIndexBuffer : HardwareIndexBuffer
    {
        private readonly byte[] _mpData;

        public DefaultHardwareIndexBuffer(IndexType idxType, int numIndexes, BufferUsage usage)
            : base(null, idxType, numIndexes, usage, true, false)
        {
            _mpData = new byte[sizeInBytes];
        }

        public DefaultHardwareIndexBuffer(HardwareBufferManagerBase manager, IndexType idxType, int numIndexes,
                                          BufferUsage usage)
            : base(manager, idxType, numIndexes, usage, true, false)
        {
            _mpData = new byte[sizeInBytes];
        }

        public override void ReadData(int offset, int length, BufferBase dest)
        {
            Debug.Assert((offset + length) <= base.sizeInBytes);

            using (BufferBase data = BufferBase.Wrap(_mpData).Offset(offset))
                Memory.Copy(dest, data, length);
        }

        public override void WriteData(int offset, int length, Array data, bool discardWholeBuffer)
        {
            Debug.Assert((offset + length) <= base.sizeInBytes);

            using (BufferBase pSource = BufferBase.Wrap(data))
            {
                using (BufferBase pIntData = BufferBase.Wrap(_mpData).Offset(offset))
                    Memory.Copy(pSource, pIntData, length);
            }
        }

        public override void WriteData(int offset, int length, BufferBase src, bool discardWholeBuffer)
        {
            Debug.Assert((offset + length) <= base.sizeInBytes);

            using (BufferBase pIntData = BufferBase.Wrap(_mpData).Offset(offset))
                Memory.Copy(src, pIntData, length);
        }

        public override BufferBase Lock(int offset, int length, BufferLocking locking)
        {
            Debug.Assert(!isLocked);
            isLocked = true;
            return Memory.PinObject(_mpData).Offset(offset);
        }

        protected override BufferBase LockImpl(int offset, int length, BufferLocking locking)
        {
            Debug.Assert(!isLocked);
            isLocked = true;
            return Memory.PinObject(_mpData).Offset(offset);
        }

        public override void Unlock()
        {
            Debug.Assert(isLocked);
            Memory.UnpinObject(_mpData);
            isLocked = false;
        }

        protected override void UnlockImpl()
        {
            Debug.Assert(isLocked);
            Memory.UnpinObject(_mpData);
            isLocked = false;
        }
    };
}
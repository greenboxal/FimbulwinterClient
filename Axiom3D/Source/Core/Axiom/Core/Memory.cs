#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: Memory.cs 3355 2012-07-27 16:26:54Z romeoxbm $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using Axiom.Utilities;
#if !AXIOM_SAFE_ONLY
using System.Runtime.InteropServices;

#endif

#endregion Namespace Declarations

namespace Axiom.Core
{
    public static class IntPtrExtension
    {
        public static IntPtr Offset(this IntPtr p, int offset)
        {
#if !NET40
            return new IntPtr(p.ToInt64() + offset);
#else
			return p + offset;
#endif
        }
    };

    ///<summary>
    ///  Utility class for dealing with memory.
    ///</summary>
    public static class Memory
    {
        #region Copy Method

        ///<summary>
        ///  Method for copying data from one IntPtr to another.
        ///</summary>
        ///<param name="src"> Source pointer. </param>
        ///<param name="dest"> Destination pointer. </param>
        ///<param name="length"> Length of data (in bytes) to copy. </param>
        public static void Copy(BufferBase src, BufferBase dest, int length)
        {
            Copy(src, dest, 0, 0, length);
        }

        ///<summary>
        ///  Method for copying data from one IntPtr to another.
        ///</summary>
        ///<param name="src"> Source pointer. </param>
        ///<param name="dest"> Destination pointer. </param>
        ///<param name="srcOffset"> Offset at which to copy from the source pointer. </param>
        ///<param name="destOffset"> Offset at which to begin copying to the destination pointer. </param>
        ///<param name="length"> Length of data (in bytes) to copy. </param>
        public static void Copy(BufferBase src, BufferBase dest, int srcOffset, int destOffset, int length)
        {
            Contract.RequiresNotNull(dest, "dest");
            dest.Copy(src, srcOffset, destOffset, length);
        }

        #endregion Copy Method

        /// <summary>
        ///   Sets buffers to a specified value
        /// </summary>
        /// <remarks>
        ///   Sets the first length values of dest to the value "c".
        ///   Make sure that the destination buffer has enough room for at least length characters.
        /// </remarks>
        /// <param name="dest"> Destination pointer. </param>
        /// <param name="c"> Value to set. </param>
        /// <param name="length"> Number of bytes to set. </param>
        public static void Set(BufferBase dest, byte c, int length)
        {
#if !AXIOM_SAFE_ONLY
            unsafe
#endif
            {
                byte* ptr = dest.ToBytePointer();

                for (int i = 0; i < length; i++)
                {
                    ptr[i] = c;
                }
            }
        }

        public static int SizeOf(Type type)
        {
            return type.Size();
        }

        public static int SizeOf(object obj)
        {
            return obj.GetType().Size();
        }

        #region Pinned Object Access

#if AXIOM_SAFE_ONLY
		private static readonly Dictionary<object, ManagedBuffer> _pinnedReferences = new Dictionary<object, ManagedBuffer>();

		public static BufferBase PinObject( object obj )
		{
			ManagedBuffer handle;
			if ( !_pinnedReferences.TryGetValue( obj, out handle ) )
			{
				handle = obj is byte[] ? new ManagedBuffer( obj as byte[] ) : new ManagedBuffer( obj );
				_pinnedReferences.Add( obj, handle );
			}

			return handle;
		}
#else
        private static readonly Dictionary<object, GCHandle> _pinnedReferences = new Dictionary<object, GCHandle>();

        public static BufferBase PinObject(object obj)
        {
            GCHandle handle;
            if (!_pinnedReferences.TryGetValue(obj, out handle))
            {
                handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
                _pinnedReferences.Add(obj, handle);
            }

            int length = obj is byte[] ? ((byte[]) obj).Length : 0;
            return new UnsafeBuffer(handle.AddrOfPinnedObject(), length);
        }
#endif

        public static void UnpinObject(object obj)
        {
            if (_pinnedReferences.ContainsKey(obj))
            {
                GCHandle handle = _pinnedReferences[obj];
#if AXIOM_SAFE_ONLY
				handle.Dispose();
#else
                handle.Free();
#endif
                _pinnedReferences.Remove(obj);
            }
            else
            {
                LogManager.Instance.Write("MemoryManager : Attempted to unpin memory that wasn't pinned.");
            }
        }

        #endregion Pinned Object Access
    };
}
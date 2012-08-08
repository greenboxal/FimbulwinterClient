#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: AtomicWrappers.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Reflection;

#endregion Namespace Declarations

namespace Axiom.Core
{
    public class AtomicScalar<T>
    {
        #region Fields

        private readonly int _size;
        private const string ERROR_MESSAGE = "Only 16, 32, and 64 bit scalars supported in win32.";

#if WINDOWS_PHONE
        private static readonly object _mutex = new object();
#endif

        #endregion Fields

        #region Properties

        public T Value { get; set; }

        #endregion Properties

        #region Constructors

        public AtomicScalar()
        {
            Type type = typeof (T);
            this._size = type.IsEnum ? 4 : Memory.SizeOf(type);
        }

        public AtomicScalar(T initial)
            : this()
        {
            Value = initial;
        }

        public AtomicScalar(AtomicScalar<T> cousin)
            : this()
        {
            Value = cousin.Value;
        }

        #endregion Constructors

        #region Methods

        public bool Cas(T old, T nu)
        {
            if (this._size == 2 || this._size == 4 || this._size == 8)
            {
                long f = Convert.ToInt64(Value);
                long o = Convert.ToInt64(old);
                long n = Convert.ToInt64(nu);

#if !WINDOWS_PHONE
                bool result = System.Threading.Interlocked.CompareExchange(ref f, o, n).Equals(o);
#else
                bool result = false;
                lock ( _mutex )
                {
                    var oldValue = f;
                    if ( f == n )
                        f = o;

                    result = oldValue.Equals( o );
                }
#endif
                Value = _changeType(f);

                return result;
            }
            else
            {
                throw new AxiomException(ERROR_MESSAGE);
            }
        }

        [AxiomHelper(0, 9)]
        private static T _changeType(object value)
        {
            Type type = typeof (T);

            if (!type.IsEnum)
            {
                return (T) Convert.ChangeType(value, type, null);
            }
            else
            {
                FieldInfo[] fields = type.GetFields();
                int idx = ((int) Convert.ChangeType(value, typeof (int), null)) + 1;
                if (fields.Length > 0 && idx < fields.Length)
                {
                    try
                    {
                        string s = fields[idx].Name;
                        return (T) Enum.Parse(type, s, false);
                    }
                    catch
                    {
                        return default(T);
                    }
                }
                else
                {
                    return default(T);
                }
            }
        }

        #endregion Methods

        #region Operator overloads

        public static AtomicScalar<T> operator ++(AtomicScalar<T> value)
        {
            if (value._size == 2 || value._size == 4 || value._size == 8)
            {
                long v = Convert.ToInt64(value.Value);
#if !WINDOWS_PHONE
                System.Threading.Interlocked.Increment(ref v);
#else
                lock ( _mutex )
                {
                    v++;
                }
#endif
                return new AtomicScalar<T>(_changeType(v));
            }
            else
            {
                throw new AxiomException(ERROR_MESSAGE);
            }
        }

        public static AtomicScalar<T> operator --(AtomicScalar<T> value)
        {
            if (value._size == 2 || value._size == 4 || value._size == 8)
            {
                long v = Convert.ToInt64(value.Value);
#if !WINDOWS_PHONE
                System.Threading.Interlocked.Decrement(ref v);
#else
                lock ( _mutex )
                {
                    v--;
                }
#endif
                return new AtomicScalar<T>(_changeType(v));
            }
            else
            {
                throw new AxiomException(ERROR_MESSAGE);
            }
        }

        #endregion Operator overloads
    };
}
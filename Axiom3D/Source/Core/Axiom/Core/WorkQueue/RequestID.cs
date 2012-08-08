#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: RequestID.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   Numeric identifier for a workqueue request
    /// </summary>
    public struct RequestID
    {
        private readonly uint mValue;

        public uint Value
        {
            get { return this.mValue; }
        }

        public RequestID(uint reqId)
        {
            this.mValue = reqId;
        }

        public static bool operator ==(RequestID lr, RequestID rr)
        {
            return lr.Value == rr.Value;
        }

        public static bool operator !=(RequestID lr, RequestID rr)
        {
            return !(lr == rr);
        }

        public static implicit operator RequestID(uint val)
        {
            return new RequestID(val);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is RequestID)
            {
                return this == (RequestID) obj;
            }

            return false;
        }

        public override string ToString()
        {
            return this.mValue.ToString();
        }
    };
}
#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: NameValuePairList.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System.Collections.Generic;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Collections
{
    /// <summary>
    ///   Represents a collection of names and values.
    /// </summary>
    public class NameValuePairList : Dictionary<string, string>
    {
        #region Constructors

        /// <summary>
        ///   Initializes a new instance of the <see cref="NameValuePairList" /> class.
        /// </summary>
        public NameValuePairList()
            : base(new CaseInsensitiveStringComparer())
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NameValuePairList" /> class that contains elements copied from
        ///   the specified <see cref="NameValuePairList" /> and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="parameters"> The <see cref="NameValuePairList" /> whose elements are copied to the new <see
        ///    cref="NameValuePairList" /> . </param>
        public NameValuePairList(NameValuePairList parameters)
            : base(parameters)
        {
        }

        #endregion Constructors
    }
}
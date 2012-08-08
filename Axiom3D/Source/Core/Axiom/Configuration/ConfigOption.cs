#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ConfigOption.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using Axiom.Collections;
using Axiom.Graphics;

#endregion Namespace Declarations

namespace Axiom.Configuration
{
    public class ConfigOption : ConfigOption<string>
    {
        public ConfigOption(string name, string value, bool immutable)
            : base(name, value, immutable)
        {
        }
    }

    /// <summary>
    ///   Packages the details of a configuration option.
    /// </summary>
    /// <remarks>
    ///   Used for <see cref="RenderSystem.ConfigOptions" />. If immutable is true, this option must be disabled for modifying.
    /// </remarks>
    public class ConfigOption<T>
    {
        //RenderSystem _parent;

        #region Name Property

        private readonly string _name;

        /// <summary>
        ///   The name for the Configuration Option
        /// </summary>
        public string Name
        {
            get { return this._name; }
        }

        #endregion Name Property

        #region Value Property

        private T _value;

        /// <summary>
        ///   The value of the Configuration Option
        /// </summary>
        public T Value
        {
            get { return this._value; }
            set
            {
                if (this._immutable != true)
                {
                    this._value = value;
                    OnValueChanged(this._name, this._value);
                }
            }
        }

        #endregion Value Property

        #region PossibleValues Property

        private readonly ConfigOptionValuesCollection<T> _possibleValues = new ConfigOptionValuesCollection<T>();

        /// <summary>
        ///   A list of the possible values for this Configuration Option
        /// </summary>
        public ConfigOptionValuesCollection<T> PossibleValues
        {
            get { return this._possibleValues; }
        }

        #endregion PossibleValues Property

        #region Immutable Property

        private bool _immutable;

        /// <summary>
        ///   Indicates if this option can be modified.
        /// </summary>
        public bool Immutable
        {
            set { this._immutable = value; }
            get { return this._immutable; }
        }

        #endregion Immutable Property

        public ConfigOption(string name, T value, bool immutable)
        {
            this._name = name;
            this._value = value;
            this._immutable = immutable;
        }

        #region Events

        public delegate void ValueChanged(string name, string value);

        public event ValueChanged ConfigValueChanged;

        private void OnValueChanged(string name, T value)
        {
            if (ConfigValueChanged != null)
            {
                ConfigValueChanged(name, Value.ToString());
            }
        }

        #endregion Events

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, Value);
        }

        public class ConfigOptionValuesCollection<ValueType> : AxiomSortedCollection<int, ValueType>
        {
        }
    }
}
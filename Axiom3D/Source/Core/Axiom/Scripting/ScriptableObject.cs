#region SVN Version Information

// <file>
//     <license see="http://axiom3d.net/wiki/index.php/license.txt"/>
//     <id value="$Id: ScriptableObject.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using Axiom.Collections;
using System.Reflection;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Scripting
{
    public sealed class ScriptableProperties
    {
        private readonly IScriptableObject _owner;

        public ScriptableProperties(IScriptableObject owner)
        {
            this._owner = owner;
        }

        public string this[string property]
        {
            get { return this._owner[property]; }
            set { this._owner[property] = value; }
        }
    }

    /// <summary>
    ///   An interface to provide access to object properties through strings
    /// </summary>
    public interface IScriptableObject
    {
        /// <summary>
        ///   The property collection available
        /// </summary>
        ScriptableProperties Properties { get; }

        /// <summary>
        ///   Set multiple properties at once using a <see cref="NameValuePairList" />
        /// </summary>
        /// <param name="parameters"> the collection of parameters </param>
        void SetParameters(NameValuePairList parameters);

        /// <summary>
        ///   provides access to each property
        /// </summary>
        /// <param name="index"> name of the property </param>
        /// <returns> the property value </returns>
        string this[string index] { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ScriptablePropertyAttribute : Attribute
    {
        public readonly string ScriptPropertyName;

        public ScriptablePropertyAttribute(string scriptPropertyName)
        {
            this.ScriptPropertyName = scriptPropertyName;
        }

        public ScriptablePropertyAttribute(string scriptPropertyName, string description)
        {
            this.ScriptPropertyName = scriptPropertyName;
        }

        public ScriptablePropertyAttribute(string scriptPropertyName, string description, Type owner)
        {
            this.ScriptPropertyName = scriptPropertyName;
        }
    }

    public abstract class ScriptableObject : DisposableObject, IScriptableObject
    {
        private readonly Dictionary<String, IPropertyCommand> _classParameters;

        ///<summary>
        ///</summary>
        public ICollection<IPropertyCommand> Commands
        {
            get { return this._classParameters.Values; }
        }

        ///<summary>
        ///</summary>
        protected ScriptableObject()
        {
            this._classParameters = _getTypePropertyMap(GetType());
            this._properties = new ScriptableProperties(this);
        }

        #region Static Implementation

        private static readonly Dictionary<Type, Dictionary<String, IPropertyCommand>> _propertyMaps =
            new Dictionary<Type, Dictionary<string, IPropertyCommand>>();

        private static Dictionary<String, IPropertyCommand> _getTypePropertyMap(Type type)
        {
            Dictionary<String, IPropertyCommand> list;
            if (!_propertyMaps.TryGetValue(type, out list))
            {
                list = new Dictionary<string, IPropertyCommand>();
                _propertyMaps.Add(type, list);
                // Use reflection to load the mapping between script name and IPropertyCommand
                _initializeTypeProperties(type, list);
            }
            return list;
        }

        private static void _initializeTypeProperties(Type type, Dictionary<string, IPropertyCommand> list)
        {
            // Load the IPropertyCommands from the parent Type
            Type parent = type.BaseType;
            if (parent != typeof (System.Object))
            {
                foreach (KeyValuePair<string, IPropertyCommand> item in _getTypePropertyMap(parent))
                {
                    list.Add(item.Key, item.Value);
                }
                parent = parent.BaseType;
            }

            foreach (Type nestType in type.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public))
            {
#if !(SILVERLIGHT || WINDOWS_PHONE || XBOX || XBOX360)
                if (
                    nestType.FindInterfaces((typeObj, criteriaObj) => typeObj.Name == criteriaObj.ToString(),
                                            typeof (IPropertyCommand).Name).Length > 0)
#else
                foreach ( Type iface in nestType.GetInterfaces() )
					if ( iface.Name == typeof ( IPropertyCommand ).Name )
#endif
                {
                    foreach (ScriptablePropertyAttribute attr in
                        nestType.GetCustomAttributes(typeof (ScriptablePropertyAttribute), true))
                    {
                        IPropertyCommand propertyCommand = (IPropertyCommand) Activator.CreateInstance(nestType);
                        list.Add(attr.ScriptPropertyName, propertyCommand);
                    }
                }
            }
        }

        #endregion Static Implementation

        #region Implementation of IScriptableObject

        private readonly ScriptableProperties _properties;

        /// <summary>
        ///   a list of properties accessible through though a string interface
        /// </summary>
        public ScriptableProperties Properties
        {
            get { return this._properties; }
        }

        /// <summary>
        ///   Set multiple properties using a <see cref="NameValuePairList" />
        /// </summary>
        /// <param name="parameters"> the list of properties to set </param>
        public void SetParameters(NameValuePairList parameters)
        {
            foreach (KeyValuePair<string, string> item in parameters)
            {
                Properties[item.Key] = item.Value;
            }
        }

        // This is using explicit interface implementation to hide the inplementation from the public api
        // access to this indexer is provided through the Properties property
        string IScriptableObject.this[string property]
        {
            get
            {
                IPropertyCommand command;

                if (this._classParameters.TryGetValue(property, out command))
                {
                    return command.Get(this);
                }
                else
                {
                    LogManager.Instance.Write("{0}: Unrecognized parameter '{1}'", GetType().Name, property);
                }
                return null;
            }
            set
            {
                IPropertyCommand command;

                if (this._classParameters.TryGetValue(property, out command))
                {
                    command.Set(this, value);
                }
                else
                {
                    LogManager.Instance.Write("{0}: Unrecognized parameter '{1}'", GetType().Name, property);
                }
            }
        }

        #endregion Implementation of IScriptableObject
    }
}
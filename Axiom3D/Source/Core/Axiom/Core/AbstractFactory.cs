#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: AbstractFactory~1.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using Axiom.Collections;

#endregion Namespace Declarations

namespace Axiom.Core
{
    /// <summary>
    ///   Abstract factory class implementation. Provides a basic Factory
    ///   implementation that can be overriden by derivitives
    /// </summary>
    /// <typeparam name="T"> The Type to instantiate </typeparam>
    public class AbstractFactory<T> : DisposableObject, IAbstractFactory<T>
        where T : class
    {
        private static readonly List<T> _instances = new List<T>();

        #region Implementation of IAbstractFactory<T>

        /// <summary>
        ///   The factory type.
        /// </summary>
        public virtual string Type
        {
            get { return typeof (T).Name; }
            protected set { throw new NotImplementedException(); }
        }

        /// <summary>
        ///   Creates a new object.
        /// </summary>
        /// <param name="name"> Name of the object to create </param>
        /// <returns> An object created by the factory. The type of the object depends on the factory. </returns>
        public virtual T CreateInstance(string name)
        {
            return CreateInstance(name, new NameValuePairList());
        }

        /// <summary>
        ///   Creates a new object.
        /// </summary>
        /// <param name="name"> Name of the object to create </param>
        /// <param name="parms"> List of Name/Value pairs to initialize the object with </param>
        /// <returns> An object created by the factory. The type of the object depends on the factory. </returns>
        public virtual T CreateInstance(string name, NameValuePairList parms)
        {
            ObjectCreator creator = new ObjectCreator(typeof (T));
            T instance = creator.CreateInstance<T>();
            _instances.Add(instance);
            return instance;
        }

        /// <summary>
        ///   Destroys an object which was created by this factory.
        /// </summary>
        /// <param name="obj"> the object to destroy </param>
        public virtual void DestroyInstance(ref T obj)
        {
            _instances.Remove(obj);
            obj = null;
        }

        #endregion Implementation of IAbstractFactory<T>
    }
}
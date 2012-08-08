#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: IAbstractFactory~1.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.Text;
using Axiom.Collections;

namespace Axiom.Core
{

    #endregion Namespace Declarations

    /// <summary>
    ///   Abstract factory class. Does nothing by itself, but derived classes can add functionality.
    /// </summary>
    /// <typeparam name="T"> </typeparam>
    /// <ogre name="FactoryObj">
    ///   <file name="OgreFactoryObj.h" revision="1.10" lastUpdated="5/18/2006" lastUpdatedBy="Borrillis" />
    /// </ogre>
    public interface IAbstractFactory<T>
    {
        /// <summary>
        ///   The factory type.
        /// </summary>
        string Type { get; }

        /// <summary>
        ///   Creates a new object.
        /// </summary>
        /// <param name="name"> Name of the object to create </param>
        /// <returns> An object created by the factory. The type of the object depends on the factory. </returns>
        T CreateInstance(string name);

        // <summary>
        // Creates a new object.
        // </summary>
        // <param name="name">Name of the object to create</param>
        // <param name="parms">List of Name/Value pairs to initialize the object with</param>
        // <returns>
        // An object created by the factory. The type of the object depends on
        // the factory.
        // </returns>
        //T CreateInstance( string name, NameValuePairList parms );

        /// <summary>
        ///   Destroys an object which was created by this factory.
        /// </summary>
        /// <param name="obj"> the object to destroy </param>
        void DestroyInstance(ref T obj);
    }
}
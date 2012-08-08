/*
using System;
using System.Collections;
using System.Diagnostics;

using Axiom.Core;

// used to alias a type in the code for easy copying and pasting.  Come on generics!!
using T = Axiom.Core.TextureLayer;
// used to alias a key value in the code for easy copying and pasting.  Come on generics!!
using K = System.String;
// used to alias a parent type in the code for easy copying and pasting.  Come on generics!!
//using P = Axiom.Core.Entity;

namespace Axiom.Collections {
    /// <summary>
    /// Summary description for TextureLayerCollection.
    /// </summary>
    public class TextureLayerCollection : AxiomCollection {
        #region Constructors

        /// <summary>
        ///		Default constructor.
        /// </summary>
        public TextureLayerCollection() : base() {}

        /// <summary>
        ///		Constructor that takes a parent object to, and calls the base class constructor to 
        /// </summary>
        /// <param name="entity"></param>
        //public TextureLayerCollection(P parent) : base(parent) {}

        #endregion

        #region Strongly typed methods and indexers

        /// <summary>
        ///		Get/Set indexer that allows access to the collection by index.
        /// </summary>
        new public T this[int index] {
            get { return (T)base[index]; }
            set { base[index] = value; }
        }

        /// <summary>
        ///		Get/Set indexer that allows access to the collection by key value.
        /// </summary>
        public T this[K key] {
            get { return (T)base[key]; }
            set { base[key] = value; }
        }

        /// <summary>
        ///		Adds an object to the collection.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item) {
            base.Add(item);
        }

        /// <summary>
        ///		Adds a named object to the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        public void Add(K key, T item) {
            base.Add(key, item);
        }

        #endregion

    }
} */


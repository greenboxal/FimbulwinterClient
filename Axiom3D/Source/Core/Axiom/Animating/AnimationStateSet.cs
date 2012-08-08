﻿#region SVN Version Information

// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <id value="$Id: AnimationStateSet.cs 3293 2012-05-21 11:56:22Z borrillis $"/>
// </file>

#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using Axiom.Collections;
using Axiom.Core;
using Axiom.Controllers;

#endregion Namespace Declarations

namespace Axiom.Animating
{
    ///<summary>
    ///  Represents the state of an animation and the weight of it's influence.
    ///</summary>
    ///<remarks>
    ///  Other classes can hold instances of this class to store the state of any animations
    ///  they are using.
    ///  This class implements the IControllerValue interface to enable automatic update of
    ///  animation state through controllers.
    ///</remarks>
    public class AnimationStateSet : IEnumerable<KeyValuePair<string, AnimationState>>
    {
        #region Protected Fields

        ///<summary>
        ///  Mapping from string to AnimationState
        ///</summary>
        protected Dictionary<string, AnimationState> stateSet = new Dictionary<string, AnimationState>();

        ///<summary>
        ///</summary>
        protected int dirtyFrameNumber;

        ///<summary>
        ///  A list of enabled animation states
        ///</summary>
        protected List<AnimationState> enabledAnimationStates = new List<AnimationState>();

        #endregion Protected Fields

        #region Constructors

        public AnimationStateSet()
        {
            this.dirtyFrameNumber = int.MaxValue;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///   Get the latest animation state been altered frame number
        /// </summary>
        public int DirtyFrameNumber
        {
            get { return this.dirtyFrameNumber; }
            set { this.dirtyFrameNumber = value; }
        }

        /// <summary>
        ///   Get the dictionary of states
        /// </summary>
        public Dictionary<string, AnimationState> AllAnimationStates
        {
            get { return this.stateSet; }
        }

        /// <summary>
        ///   Get the list of enabled animation states
        /// </summary>
        public List<AnimationState> EnabledAnimationStates
        {
            get { return this.enabledAnimationStates; }
        }

        public ICollection<AnimationState> Values
        {
            get { return this.stateSet.Values; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        ///   Create a copy of the AnimationStateSet instance.
        /// </summary>
        public AnimationStateSet Clone()
        {
            AnimationStateSet newSet = new AnimationStateSet();

            foreach (AnimationState animationState in this.stateSet.Values)
            {
                new AnimationState(newSet, animationState);
            }

            // Clone enabled animation state list
            foreach (AnimationState animationState in this.enabledAnimationStates)
            {
                newSet.EnabledAnimationStates.Add(newSet.GetAnimationState(animationState.Name));
            }
            return newSet;
        }

        /// <summary>
        ///   Create a new AnimationState instance.
        /// </summary>
        /// <param name="name"> The name of the animation </param>
        /// <param name="time"> Starting time position </param>
        /// <param name="length"> Length of the animation to play </param>
        public AnimationState CreateAnimationState(string name, float time, float length)
        {
            return CreateAnimationState(name, time, length, 1.0f, false);
        }

        /// <summary>
        ///   Create a new AnimationState instance.
        /// </summary>
        /// <param name="name"> The name of the animation </param>
        /// <param name="time"> Starting time position </param>
        /// <param name="length"> Length of the animation to play </param>
        /// <param name="weight"> Weight to apply the animation with </param>
        /// <param name="enabled"> Whether the animation is enabled </param>
        public AnimationState CreateAnimationState(string name, float time, float length, float weight, bool enabled)
        {
            if (this.stateSet.ContainsKey(name))
            {
                throw new Exception("State for animation named '" + name + "' already exists, " +
                                    "in AnimationStateSet.CreateAnimationState");
            }
            AnimationState newState = new AnimationState(name, this, time, length, weight, enabled);
            this.stateSet[name] = newState;
            return newState;
        }

        /// <summary>
        ///   Get an animation state by the name of the animation
        /// </summary>
        public AnimationState GetAnimationState(string name)
        {
            if (!this.stateSet.ContainsKey(name))
            {
                throw new Exception("No state found for animation named '" + name + "', " +
                                    "in AnimationStateSet.CreateAnimationState");
            }
            return this.stateSet[name];
        }

        /// <summary>
        ///   Tests if state for the named animation is present
        /// </summary>
        public bool HasAnimationState(string name)
        {
            return this.stateSet.ContainsKey(name);
        }

        /// <summary>
        ///   Remove animation state with the given name
        /// </summary>
        public void RemoveAnimationState(string name)
        {
            if (this.stateSet.ContainsKey(name))
            {
                this.enabledAnimationStates.Remove(this.stateSet[name]);
                this.stateSet.Remove(name);
            }
        }

        /// <summary>
        ///   Remove all animation states
        /// </summary>
        public void RemoveAllAnimationStates()
        {
            this.stateSet.Clear();
            this.enabledAnimationStates.Clear();
        }

        /// <summary>
        ///   Copy the state of any matching animation states from this to another
        /// </summary>
        public void CopyMatchingState(AnimationStateSet target)
        {
            foreach (KeyValuePair<string, AnimationState> pair in target.AllAnimationStates)
            {
                AnimationState result;
                if (!this.stateSet.TryGetValue(pair.Key, out result))
                {
                    throw new Exception("No animation entry found named '" + pair.Key + "', in " +
                                        "AnimationStateSet.CopyMatchingState");
                }
                else
                {
                    pair.Value.CopyFrom(result);
                }
            }

            // Copy matching enabled animation state list
            target.EnabledAnimationStates.Clear();
            foreach (AnimationState state in this.enabledAnimationStates)
            {
                target.EnabledAnimationStates.Add(target.AllAnimationStates[state.Name]);
            }

            target.DirtyFrameNumber = this.dirtyFrameNumber;
        }

        /// <summary>
        ///   Set the dirty flag and dirty frame number on this state set
        /// </summary>
        public void NotifyDirty()
        {
            ++this.dirtyFrameNumber;
        }

        /// <summary>
        ///   Internal method respond to enable/disable an animation state
        /// </summary>
        public void NotifyAnimationStateEnabled(AnimationState target, bool enabled)
        {
            // Remove from enabled animation state list first
            this.enabledAnimationStates.Remove(target);

            // Add to enabled animation state list if need
            if (enabled)
            {
                this.enabledAnimationStates.Add(target);
            }

            // Set the dirty frame number
            NotifyDirty();
        }

        #endregion Methods

        public IEnumerator<KeyValuePair<string, AnimationState>> GetEnumerator()
        {
            return this.stateSet.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.stateSet.GetEnumerator();
        }
    }
}
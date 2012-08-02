#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2011 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Input;

namespace Nuclex.Input.Devices {

  /// <summary>Extended game pad state with additional buttons and axes</summary>
  public partial struct ExtendedGamePadState {

    /// <summary>Retrieves the state of the specified button</summary>
    /// <param name="buttonIndex">
    ///   Index of the button whose state will be retrieved
    /// </param>
    /// <returns>The state of the queried button</returns>
    public ButtonState GetButton(int buttonIndex) {
      return IsButtonDown(buttonIndex) ? ButtonState.Pressed : ButtonState.Released;
    }

    /// <summary>Determines whether the specified button is pressed down</summary>
    /// <param name="buttonIndex">Button which will be checked</param>
    /// <returns>True if the specified button is pressed down</returns>
    public bool IsButtonDown(int buttonIndex) {
      if (buttonIndex >= 128) {
        throw new ArgumentOutOfRangeException("buttonIndex", "Button index out of range");
      } else if (buttonIndex >= 64) {
        return (this.buttonState2 & (1UL << (buttonIndex - 64))) != 0;
      } else if (buttonIndex >= 0) {
        return (this.buttonState1 & (1UL << buttonIndex)) != 0;
      } else {
        throw new ArgumentOutOfRangeException("buttonIndex", "Button index out of range");
      }
    }

    /// <summary>Determines whether the specified button is up</summary>
    /// <param name="buttonIndex">Button which will be checked</param>
    /// <returns>True if the specified button is up</returns>
    public bool IsButtonUp(int buttonIndex) {
      if (buttonIndex >= 128) {
        throw new ArgumentOutOfRangeException("buttonIndex", "Button index out of range");
      } else if (buttonIndex >= 64) {
        return (this.buttonState2 & (1UL << (buttonIndex - 64))) == 0;
      } else if (buttonIndex >= 0) {
        return (this.buttonState1 & (1UL << buttonIndex)) == 0;
      } else {
        throw new ArgumentOutOfRangeException("buttonIndex", "Button index out of range");
      }
    }

    /// <summary>Number of available axes in this state</summary>
    public int AxisCount {
      get { return countBits((uint)this.AvailableAxes); }
    }

    /// <summary>Retrieves the state of the specified axis</summary>
    /// <param name="axis">Axis whose state will be retrieved</param>
    /// <returns>The state of the specified axis</returns>
    public float GetAxis(ExtendedAxes axis) {
      switch (axis) {
        case ExtendedAxes.X: { return this.X; }
        case ExtendedAxes.Y: { return this.Y; }
        case ExtendedAxes.Z: { return this.Z; }
        case ExtendedAxes.VelocityX: { return this.VelocityX; }
        case ExtendedAxes.VelocityY: { return this.VelocityY; }
        case ExtendedAxes.VelocityZ: { return this.VelocityZ; }
        case ExtendedAxes.AccelerationX: { return this.AccelerationX; }
        case ExtendedAxes.AccelerationY: { return this.AccelerationY; }
        case ExtendedAxes.AccelerationZ: { return this.AccelerationZ; }
        case ExtendedAxes.ForceX: { return this.ForceX; }
        case ExtendedAxes.ForceY: { return this.ForceY; }
        case ExtendedAxes.ForceZ: { return this.ForceZ; }
        case ExtendedAxes.RotationX: { return this.RotationX; }
        case ExtendedAxes.RotationY: { return this.RotationY; }
        case ExtendedAxes.RotationZ: { return this.RotationZ; }
        case ExtendedAxes.AngularVelocityX: { return this.AngularVelocityX; }
        case ExtendedAxes.AngularVelocityY: { return this.AngularVelocityY; }
        case ExtendedAxes.AngularVelocityZ: { return this.AngularVelocityZ; }
        case ExtendedAxes.AngularAccelerationX: { return this.AngularAccelerationX; }
        case ExtendedAxes.AngularAccelerationY: { return this.AngularAccelerationY; }
        case ExtendedAxes.AngularAccelerationZ: { return this.AngularAccelerationZ; }
        case ExtendedAxes.TorqueX: { return this.TorqueX; }
        case ExtendedAxes.TorqueY: { return this.TorqueY; }
        case ExtendedAxes.TorqueZ: { return this.TorqueZ; }
        default: { throw new ArgumentOutOfRangeException("axis", "Invalid axis"); }
      }
    }

    /// <summary>Number of available sliders in this state</summary>
    public int SliderCount {
      get { return countBits((uint)this.AvailableSliders); }
    }

    /// <summary>Retrieves the state of the specified slider</summary>
    /// <param name="slider">Slider whose state will be retrieved</param>
    /// <returns>The state of the specified slider</returns>
    public float GetSlider(ExtendedSliders slider) {
      switch (slider) {
        case ExtendedSliders.Slider1: { return this.Slider1; }
        case ExtendedSliders.Slider2: { return this.Slider2; }
        case ExtendedSliders.Velocity1: { return this.VelocitySlider1; }
        case ExtendedSliders.Velocity2: { return this.VelocitySlider2; }
        case ExtendedSliders.Acceleration1: { return this.AccelerationSlider1; }
        case ExtendedSliders.Acceleration2: { return this.AccelerationSlider2; }
        case ExtendedSliders.Force1: { return this.ForceSlider1; }
        case ExtendedSliders.Force2: { return this.ForceSlider2; }
        default: { throw new ArgumentOutOfRangeException("slider", "Invalid slider"); }
      }
    }

    /// <summary>Retrieves the PoV controller of the specified index</summary>
    /// <param name="index">Index of the PoV controller that will be retrieved</param>
    /// <returns>The state of the PoV controller with the specified index</returns>
    public int GetPov(int index) {
      switch (index) {
        case 0: { return this.Pov1; }
        case 1: { return this.Pov2; }
        case 2: { return this.Pov3; }
        case 3: { return this.Pov4; }
        default: {
          throw new ArgumentOutOfRangeException("index", "PoV index out of range");
        }
      }
    }

    /// <summary>Converts a PoV controller state into a directional pad state</summary>
    /// <param name="pov">PoV controller state that will be converted</param>
    /// <returns>The equivalent directional pad state</returns>
    public static GamePadDPad DpadFromPov(int pov) {
      if ((ushort)pov == 0xFFFF) {
        return new GamePadDPad();
      }

      return new GamePadDPad(
        ((pov > 27000) || (pov < 9000)) ? ButtonState.Pressed : ButtonState.Released,
        ((pov > 9000) && (pov < 27000)) ? ButtonState.Pressed : ButtonState.Released,
        ((pov > 18000) && (pov < 36000)) ? ButtonState.Pressed : ButtonState.Released,
        ((pov > 0) && (pov < 18000)) ? ButtonState.Pressed : ButtonState.Released
      );
    }

    /// <summary>Converts a directional pad state into a PoV controller state</summary>
    /// <param name="dpad">Directional pad state that will be converted</param>
    /// <returns>The equivalent PoV controller pad state</returns>
    /// <remarks>
    ///   Conflicting states (eg. directional pad 'down' and 'up' at the same time)
    ///   are resolved as if the specific axis was neutral.
    /// </remarks>
    public static int PovFromDpad(GamePadDPad dpad) {
      const ButtonState pressed = ButtonState.Pressed;
      const ButtonState released = ButtonState.Released;

      if ((dpad.Up == pressed) && (dpad.Down == released)) {
        if ((dpad.Left == pressed) && (dpad.Right == released)) {
          return 31500; // Left up
        } else if ((dpad.Right == pressed) && (dpad.Left == released)) {
          return 4500; // Right up
        } else {
          return 0; // Up
        }
      } else if ((dpad.Down == pressed) && (dpad.Up == released)) {
        if ((dpad.Left == pressed) && (dpad.Right == released)) {
          return 22500; // Left down
        } else if ((dpad.Right == pressed) && (dpad.Left == released)) {
          return 13500; // Right down
        } else {
          return 18000; // Down
        }
      } else if ((dpad.Left == pressed) && (dpad.Right == released)) {
        return 27000; // Left
      } else if ((dpad.Right == pressed) && (dpad.Left == released)) {
        return 9000; // Right
      } else {
        return -1; // Neutral
      }
    }

    /// <summary>
    ///   Converts an extended button bit mask into the XNA's own button mask
    /// </summary>
    /// <param name="extendedButtons1">Button bit mask that will be converted</param>
    /// <returns>Equivalent XNA button mask for the provided button bit mask</returns>
    public static Buttons ButtonsFromExtendedButtons(ulong extendedButtons1) {
      Buttons buttons = 0;

      for (int buttonIndex = 0; buttonIndex < ButtonOrder.Length; ++buttonIndex) {
        ulong bitMask = 1UL << buttonIndex;

        if ((extendedButtons1 & bitMask) == bitMask) {
          buttons |= ButtonOrder[buttonIndex];
        }
      }

      return buttons;
    }

    /// <summary>
    ///   Converts XNA's own button mask into an extended button bit mask
    /// </summary>
    /// <param name="buttons">XNA button mask that will be converted</param>
    /// <returns>Equivalent extended button bit mask for the provided button mask</returns>
    public static ulong ExtendedButtonsFromButtons(Buttons buttons) {
      ulong extendedButtons = 0;

      for (int buttonIndex = 0; buttonIndex < ButtonOrder.Length; ++buttonIndex) {
        if ((buttons & ButtonOrder[buttonIndex]) == ButtonOrder[buttonIndex]) {
          extendedButtons |= 1UL << buttonIndex;
        }
      }

      return extendedButtons;
    }

    /// <summary>
    ///   Order in which the buttons in the extended state map to XNAs Buttons enumeration
    /// </summary>
    /// <remarks>
    ///   Tested this with an XBox 360 game pad. An older game pad used a completely
    ///   arbitrary order and there's no way to find out which button resembles what,
    ///   so I'm hoping that the XBox 360's DirectInput driver sets an inofficial
    ///   standard and others copy the order in which its buttons are listed.
    /// </remarks>
    public static readonly Buttons[] ButtonOrder = new Buttons[] {
      Buttons.A,
      Buttons.B,
      Buttons.X,
      Buttons.Y,
      Buttons.LeftShoulder,
      Buttons.RightShoulder,
      Buttons.Back,
      Buttons.Start,
      Buttons.LeftStick,
      Buttons.RightStick,
      Buttons.BigButton
    };

    /// <summary>Internal helper method that retrieves the raw button states</summary>
    /// <param name="buttons1">State of the first 64 buttons</param>
    /// <param name="buttons2">State of the second 64 buttons</param>
    internal void InternalGetButtons(out ulong buttons1, out ulong buttons2) {
      buttons1 = this.buttonState1;
      buttons2 = this.buttonState2;
    }

    /// <summary>Returns the number of bits set in an unsigned integer</summary>
    /// <param name="value">Value whose bits will be counted</param>
    /// <returns>The number of bits set in the unsigned integer</returns>
    /// <remarks>
    ///   Based on a trick revealed here:
    ///   http://stackoverflow.com/questions/109023
    /// </remarks>
    private static int countBits(uint value) {
      value = value - ((value >> 1) & 0x55555555);
      value = (value & 0x33333333) + ((value >> 2) & 0x33333333);

      return (int)unchecked(
        ((value + (value >> 4) & 0xF0F0F0F) * 0x1010101) >> 24
      );
    }

    /// <summary>Axes for which this state provides values</summary>
    public readonly ExtendedAxes AvailableAxes;
    /// <summary>State of the device's X axis</summary>
    public readonly float X;
    /// <summary>State of the device's Y axis</summary>
    public readonly float Y;
    /// <summary>State of the device's Z axis</summary>
    public readonly float Z;
    /// <summary>State of the device's X velocity axis</summary>
    public readonly float VelocityX;
    /// <summary>State of the device's Y velocity axis</summary>
    public readonly float VelocityY;
    /// <summary>State of the device's Z velocity axis</summary>
    public readonly float VelocityZ;
    /// <summary>State of the device's X acceleration axis</summary>
    public readonly float AccelerationX;
    /// <summary>State of the device's Y acceleration axis</summary>
    public readonly float AccelerationY;
    /// <summary>State of the device's Z acceleration axis</summary>
    public readonly float AccelerationZ;
    /// <summary>State of the device's X force axis</summary>
    public readonly float ForceX;
    /// <summary>State of the device's Y force axis</summary>
    public readonly float ForceY;
    /// <summary>State of the device's Z force axis</summary>
    public readonly float ForceZ;
    /// <summary>State of the device's X rotation axis</summary>
    public readonly float RotationX;
    /// <summary>State of the device's Y rotation axis</summary>
    public readonly float RotationY;
    /// <summary>State of the device's Z rotation axis</summary>
    public readonly float RotationZ;
    /// <summary>State of the device's X angular velocity axis</summary>
    public readonly float AngularVelocityX;
    /// <summary>State of the device's Y angular velocity axis</summary>
    public readonly float AngularVelocityY;
    /// <summary>State of the device's Z angular velocity axis</summary>
    public readonly float AngularVelocityZ;
    /// <summary>State of the device's X angular acceleration axis</summary>
    public readonly float AngularAccelerationX;
    /// <summary>State of the device's Y angular acceleration axis</summary>
    public readonly float AngularAccelerationY;
    /// <summary>State of the device's Z angular acceleration axis</summary>
    public readonly float AngularAccelerationZ;
    /// <summary>State of the device's X torque axis</summary>
    public readonly float TorqueX;
    /// <summary>State of the device's Y torque axis</summary>
    public readonly float TorqueY;
    /// <summary>State of the device's Z torque axis</summary>
    public readonly float TorqueZ;

    /// <summary>Number of buttons provided by the state</summary>
    public readonly int ButtonCount;

    /// <summary>Sliders for which this state provides values</summary>
    public readonly ExtendedSliders AvailableSliders;
    /// <summary>First slider, formerly the U-axis</summary>
    public readonly float Slider1;
    /// <summary>Second slider, formerly the V-axis</summary>
    public readonly float Slider2;
    /// <summary>First velocity slider</summary>
    public readonly float VelocitySlider1;
    /// <summary>second velocity slider</summary>
    public readonly float VelocitySlider2;
    /// <summary>First acceleration slider</summary>
    public readonly float AccelerationSlider1;
    /// <summary>Second acceleration slider</summary>
    public readonly float AccelerationSlider2;
    /// <summary>First force slider</summary>
    public readonly float ForceSlider1;
    /// <summary>Second force slider</summary>
    public readonly float ForceSlider2;

    /// <summary>Number of point-of-view controllers in this state</summary>
    public readonly int PovCount;
    /// <summary>Position of the first point-of-view controller</summary>
    public readonly int Pov1;
    /// <summary>Position of the second point-of-view controller</summary>
    public readonly int Pov2;
    /// <summary>Position of the third point-of-view controller</summary>
    public readonly int Pov3;
    /// <summary>Position of the fourth point-of-view controller</summary>
    public readonly int Pov4;

    /// <summary>Bitfield containing the first 64 buttons</summary>
    private ulong buttonState1;
    /// <summary>Bitfield containing the last 64 buttons</summary>
    private ulong buttonState2;

  }

} // namespace Nuclex.Input.Devices

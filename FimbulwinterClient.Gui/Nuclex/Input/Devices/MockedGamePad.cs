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
using Microsoft.Xna.Framework;

#if !XBOX360

namespace Nuclex.Input.Devices {

  /// <summary>Code-controllable game pad for unit testing</summary>
  public class MockedGamePad : GamePad {

    /// <summary>Initializes a new mocked game pad</summary>
    public MockedGamePad() {
      this.states = new Queue<ExtendedGamePadState>();
      this.current = new ExtendedGamePadState();

      this.buttonStates = new bool[128];
      this.axisStates = new float[24];
      this.sliderStates = new float[8];
      this.povStates = new int[4] { -1, -1, -1, -1 };

      this.buttonCount = 11;
      this.availableAxes =
        ExtendedAxes.X | ExtendedAxes.Y |
        ExtendedAxes.RotationX | ExtendedAxes.RotationY;
      this.availableSliders =
        ExtendedSliders.Slider1 | ExtendedSliders.Slider2;
      this.povCount = 1;
    }

    /// <summary>Retrieves the current state of the game pad</summary>
    /// <returns>The current state of the game pad</returns>
    public override GamePadState GetState() {
      ulong buttons1, buttons2;
      this.current.InternalGetButtons(out buttons1, out buttons2);

      return new GamePadState(
        new GamePadThumbSticks(
          new Vector2(this.current.X, this.current.Y),
          new Vector2(this.current.RotationX, this.current.RotationY)
        ),
        new GamePadTriggers(this.current.Slider1, this.current.Slider2),
        new GamePadButtons(ExtendedGamePadState.ButtonsFromExtendedButtons(buttons1)),
        ExtendedGamePadState.DpadFromPov(this.current.Pov1)
      );
    }

    /// <summary>Retrieves the current DirectInput joystick state</summary>
    /// <returns>The current state of the DirectInput joystick</returns>
    public override ExtendedGamePadState GetExtendedState() {
      return this.current;
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return this.isAttached; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "Mocked game pad"; }
    }

    /// <summary>Updates the state of the input device</summary>
    /// <remarks>
    ///   <para>
    ///     If this method is called with no snapshots in the queue, it will take
    ///     an immediate snapshot and make it the current state. This way, you
    ///     can use the input devices without caring for the snapshot system if
    ///     you wish.
    ///   </para>
    ///   <para>
    ///     If this method is called while one or more snapshots are waiting in
    ///     the queue, this method takes the next snapshot from the queue and makes
    ///     it the current state.
    ///   </para>
    /// </remarks>
    public override void Update() {
      ExtendedGamePadState previous = this.current;

      if (this.states.Count == 0) {
        this.current = buildState();
      } else {
        this.current = this.states.Dequeue();
      }

      generateEvents(ref previous, ref this.current);
    }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public override void TakeSnapshot() {
      this.states.Enqueue(buildState());
    }

    /// <summary>Presses the specified buttons on the game pad</summary>
    /// <param name="buttons">Buttons that will be pressed</param>
    public void Press(Buttons buttons) {
      updateButtonStates(buttons, true);
    }

    /// <summary>Presses the specified button on the game pad</summary>
    /// <param name="extendedButtonIndex">Index of the button that will be pressed</param>
    public void Press(int extendedButtonIndex) {
      if (extendedButtonIndex < 0) {
        throw new ArgumentOutOfRangeException("Invalid button index", "extendedButtonIndex");
      }

      enforceButtonCountAtLeast(extendedButtonIndex + 1);
      this.buttonStates[extendedButtonIndex] = true;
    }

    /// <summary>Releases the specified buttons on the game pad</summary>
    /// <param name="buttons">Buttons that will be released</param>
    public void Release(Buttons buttons) {
      updateButtonStates(buttons, false);
    }

    /// <summary>Releases the specified button on the game pad</summary>
    /// <param name="extendedButtonIndex">Button that will be released</param>
    public void Release(int extendedButtonIndex) {
      if (extendedButtonIndex < 0) {
        throw new ArgumentOutOfRangeException("Invalid button index", "extendedButtonIndex");
      }

      enforceButtonCountAtLeast(extendedButtonIndex + 1);
      this.buttonStates[extendedButtonIndex] = false;
    }

    /// <summary>Moves the left thumb stick to the specified position</summary>
    /// <param name="x">X coordinate of the thumb stick's position</param>
    /// <param name="y">Y coordinate of the thumb stick's position</param>
    public void MoveLeftThumbStick(float x, float y) {
      this.axisStates[0] = x; // X axis
      this.axisStates[1] = y; // Y axis
    }

    /// <summary>Moves the right thumb stick to the specified position</summary>
    /// <param name="x">X coordinate of the thumb stick's position</param>
    /// <param name="y">Y coordinate of the thumb stick's position</param>
    public void MoveRightThumbStick(float x, float y) {
      this.axisStates[12] = x; // Rx axis
      this.axisStates[13] = y; // Ry axis
    }

    /// <summary>Pushes the left analog trigger to the specified depth</summary>
    /// <param name="depth">Depth the left analog trigger will be pushed to</param>
    public void PushLeftTrigger(float depth) {
      this.sliderStates[0] = depth;
    }

    /// <summary>Pushes the right analog trigger to the specified depth</summary>
    /// <param name="depth">Depth the right analog trigger will be pushed to</param>
    public void PushRightTrigger(float depth) {
      this.sliderStates[1] = depth;
    }

    /// <summary>Moves an axis to the specified position</summary>
    /// <param name="axis">Axis that will be moved</param>
    /// <param name="position">
    ///   Position the axis will be moved to in the range from -1.0 to +1.0
    /// </param>
    public void MoveAxis(ExtendedAxes axis, float position) {
      if ((axis & this.availableAxes) != axis) {
        throw new ArgumentException(
          "Not such axis - did you forget to assign the AvailableAxes property?",
          "axis"
        );
      }

      switch (axis) {
        case ExtendedAxes.X: { this.axisStates[0] = position; break; }
        case ExtendedAxes.Y: { this.axisStates[1] = position; break; }
        case ExtendedAxes.Z: { this.axisStates[2] = position; break; }
        case ExtendedAxes.VelocityX: { this.axisStates[3] = position; break; }
        case ExtendedAxes.VelocityY: { this.axisStates[4] = position; break; }
        case ExtendedAxes.VelocityZ: { this.axisStates[5] = position; break; }
        case ExtendedAxes.AccelerationX: { this.axisStates[6] = position; break; }
        case ExtendedAxes.AccelerationY: { this.axisStates[7] = position; break; }
        case ExtendedAxes.AccelerationZ: { this.axisStates[8] = position; break; }
        case ExtendedAxes.ForceX: { this.axisStates[9] = position; break; }
        case ExtendedAxes.ForceY: { this.axisStates[10] = position; break; }
        case ExtendedAxes.ForceZ: { this.axisStates[11] = position; break; }
        case ExtendedAxes.RotationX: { this.axisStates[12] = position; break; }
        case ExtendedAxes.RotationY: { this.axisStates[13] = position; break; }
        case ExtendedAxes.RotationZ: { this.axisStates[14] = position; break; }
        case ExtendedAxes.AngularVelocityX: { this.axisStates[15] = position; break; }
        case ExtendedAxes.AngularVelocityY: { this.axisStates[16] = position; break; }
        case ExtendedAxes.AngularVelocityZ: { this.axisStates[17] = position; break; }
        case ExtendedAxes.AngularAccelerationX: { this.axisStates[18] = position; break; }
        case ExtendedAxes.AngularAccelerationY: { this.axisStates[19] = position; break; }
        case ExtendedAxes.AngularAccelerationZ: { this.axisStates[20] = position; break; }
        case ExtendedAxes.TorqueX: { this.axisStates[21] = position; break; }
        case ExtendedAxes.TorqueY: { this.axisStates[22] = position; break; }
        case ExtendedAxes.TorqueZ: { this.axisStates[23] = position; break; }
        default: {
          throw new ArgumentException("Invalid axis specified", "axis");
        }
      }
    }

    /// <summary>Moves a slider to the specified position</summary>
    /// <param name="slider">Slider that will be moved</param>
    /// <param name="position">
    ///   Position the slider will be moved to in the range from 0.0 to 1.0
    /// </param>
    public void MoveSlider(ExtendedSliders slider, float position) {
      if ((slider & this.availableSliders) != slider) {
        throw new ArgumentException(
          "Not such slider - did you forget to assign the AvailableAxes property?",
          "slider"
        );
      }

      switch (slider) {
        case ExtendedSliders.Slider1: { this.sliderStates[0] = position; break; }
        case ExtendedSliders.Slider2: { this.sliderStates[1] = position; break; }
        case ExtendedSliders.Velocity1: { this.sliderStates[2] = position; break; }
        case ExtendedSliders.Velocity2: { this.sliderStates[3] = position; break; }
        case ExtendedSliders.Acceleration1: { this.sliderStates[4] = position; break; }
        case ExtendedSliders.Acceleration2: { this.sliderStates[5] = position; break; }
        case ExtendedSliders.Force1: { this.sliderStates[6] = position; break; }
        case ExtendedSliders.Force2: { this.sliderStates[7] = position; break; }
        default: {
          throw new ArgumentException("Invalid slider specified", "slider");
        }
      }
    }

    /// <summary>Moves a PoV controller into the specified position</summary>
    /// <param name="index">Index of the PoV controller that will be moved</param>
    /// <param name="position">Position the PoV controller will be moved to</param>
    public void MovePov(int index, int position) {
      switch (index) {
        case 0: { this.povStates[0] = position; break; }
        case 1: { this.povStates[1] = position; break; }
        case 2: { this.povStates[2] = position; break; }
        case 3: { this.povStates[3] = position; break; }
        default: {
          throw new ArgumentException("Invalid PoV controller specified", "index");
        }
      }
    }

    /// <summary>Reported number of buttons on the mocked game pad</summary>
    public int ButtonCount {
      get { return this.buttonCount; }
      set { this.buttonCount = value; }
    }

    /// <summary>Reported axes on the mocked game pad</summary>
    public ExtendedAxes AvailableAxes {
      get { return this.availableAxes; }
      set { this.availableAxes = value; }
    }

    /// <summary>Reported sliders on the mocked game pad</summary>
    public ExtendedSliders AvailableSliders {
      get { return this.availableSliders; }
      set { this.availableSliders = value; }
    }

    /// <summary>Reported number of PoV controllers on the mocked game pad</summary>
    public int PovCount {
      get { return this.povCount; }
      set { this.povCount = value; }
    }

    /// <summary>Attaches (connects) the game pad</summary>
    public void Attach() {
      this.isAttached = true;
    }

    /// <summary>Detaches (disconnects) the game pad</summary>
    public void Detach() {
      this.isAttached = false;
    }

    /// <summary>Generates events for any changes to the button states</summary>
    /// <param name="previous">Previous state of the game pad to compare against</param>
    /// <param name="current">Current state of the game pad</param>
    private void generateEvents(
      ref ExtendedGamePadState previous, ref ExtendedGamePadState current
    ) {
      ulong previous1, previous2;
      previous.InternalGetButtons(out previous1, out previous2);
      ulong current1, current2;
      current.InternalGetButtons(out current1, out current2);

      // Determine which buttons have changed state since the last update
      ulong changeMask1 = previous1 ^ current1;
      ulong changeMask2 = previous2 ^ current2;

      // Report any buttons that have been pressed
      ulong pressed1 = current1 & changeMask1;
      ulong pressed2 = current2 & changeMask2;
      if ((pressed1 != 0) || (pressed2 != 0)) {
        Buttons pressed = ExtendedGamePadState.ButtonsFromExtendedButtons(pressed1);
        if (pressed != 0) {
          OnButtonPressed(pressed);
        }

        OnExtendedButtonPressed(pressed1, pressed2);
      }

      // Report any buttons that have been released
      ulong released1 = ~current1 & changeMask1;
      ulong released2 = ~current2 & changeMask2;
      if ((released1 != 0) || (released2 != 0)) {
        Buttons released = ExtendedGamePadState.ButtonsFromExtendedButtons(released1);
        if (released != 0) {
          OnButtonReleased(released);
        }

        OnExtendedButtonReleased(released1, released2);
      }
    }

    /// <summary>Constructs an extended game pad state from the current state</summary>
    /// <returns></returns>
    private ExtendedGamePadState buildState() {
      return new ExtendedGamePadState(
        this.availableAxes, this.axisStates,
        this.availableSliders, this.sliderStates,
        this.buttonCount, this.buttonStates,
        this.povCount, this.povStates
      );
    }

    /// <summary>Updates the state of all buttons in the mask</summary>
    /// <param name="buttonMask">Mask of buttons that will be updated</param>
    /// <param name="state">New state the buttons will assume</param>
    private void updateButtonStates(Buttons buttonMask, bool state) {
      if ((buttonMask & Buttons.A) == Buttons.A) {
        enforceButtonCountAtLeast(1);
        this.buttonStates[0] = state;
      }
      if ((buttonMask & Buttons.B) == Buttons.B) {
        enforceButtonCountAtLeast(2);
        this.buttonStates[1] = state;
      }
      if ((buttonMask & Buttons.X) == Buttons.X) {
        enforceButtonCountAtLeast(3);
        this.buttonStates[2] = state;
      }
      if ((buttonMask & Buttons.Y) == Buttons.Y) {
        enforceButtonCountAtLeast(4);
        this.buttonStates[3] = state;
      }
      if ((buttonMask & Buttons.LeftShoulder) == Buttons.LeftShoulder) {
        enforceButtonCountAtLeast(5);
        this.buttonStates[4] = state;
      }
      if ((buttonMask & Buttons.RightShoulder) == Buttons.RightShoulder) {
        enforceButtonCountAtLeast(6);
        this.buttonStates[5] = state;
      }
      if ((buttonMask & Buttons.Back) == Buttons.Back) {
        enforceButtonCountAtLeast(7);
        this.buttonStates[6] = state;
      }
      if ((buttonMask & Buttons.Start) == Buttons.Start) {
        enforceButtonCountAtLeast(8);
        this.buttonStates[7] = state;
      }
      if ((buttonMask & Buttons.LeftStick) == Buttons.LeftStick) {
        enforceButtonCountAtLeast(9);
        this.buttonStates[8] = state;
      }
      if ((buttonMask & Buttons.RightStick) == Buttons.RightStick) {
        enforceButtonCountAtLeast(10);
        this.buttonStates[9] = state;
      }
      if ((buttonMask & Buttons.BigButton) == Buttons.BigButton) {
        enforceButtonCountAtLeast(11);
        this.buttonStates[10] = state;
      }

      if ((buttonMask & Buttons.DPadUp) == Buttons.DPadUp) {
        GamePadDPad dpad = ExtendedGamePadState.DpadFromPov(this.povStates[0]);
        this.povStates[0] = ExtendedGamePadState.PovFromDpad(
          new GamePadDPad(ButtonState.Pressed, dpad.Down, dpad.Left, dpad.Right)
        );
      }
      if ((buttonMask & Buttons.DPadDown) == Buttons.DPadDown) {
        GamePadDPad dpad = ExtendedGamePadState.DpadFromPov(this.povStates[0]);
        this.povStates[0] = ExtendedGamePadState.PovFromDpad(
          new GamePadDPad(dpad.Up, ButtonState.Pressed, dpad.Left, dpad.Right)
        );
      }
      if ((buttonMask & Buttons.DPadLeft) == Buttons.DPadLeft) {
        GamePadDPad dpad = ExtendedGamePadState.DpadFromPov(this.povStates[0]);
        this.povStates[0] = ExtendedGamePadState.PovFromDpad(
          new GamePadDPad(dpad.Up, dpad.Down, ButtonState.Pressed, dpad.Right)
        );
      }
      if ((buttonMask & Buttons.DPadRight) == Buttons.DPadRight) {
        GamePadDPad dpad = ExtendedGamePadState.DpadFromPov(this.povStates[0]);
        this.povStates[0] = ExtendedGamePadState.PovFromDpad(
          new GamePadDPad(dpad.Up, dpad.Down, dpad.Left, ButtonState.Pressed)
        );
      }
    }

    /// <summary>
    ///   Throws an exception if less than the required number of buttons are available
    /// </summary>
    /// <param name="requiredCount">Required number of buttons</param>
    private void enforceButtonCountAtLeast(int requiredCount) {
      if (requiredCount > this.buttonCount) {
        throw new ArgumentOutOfRangeException(
          "No button by that index - did you forget to set ButtonCount?",
          "requiredCount"
        );
      }
    }

    /// <summary>Snapshots of the game pad state waiting to be processed</summary>
    private Queue<ExtendedGamePadState> states;

    /// <summary>Currently published game pad state</summary>
    private ExtendedGamePadState current;

    /// <summary>Whether the game pad is attached</summary>
    private bool isAttached;

    /// <summary>Number of buttons available on the game pad</summary>
    private int buttonCount;
    /// <summary>Current state of the buttons on the game pad</summary>
    private bool[/*128*/] buttonStates;
    /// <summary>Axes available on the game pad</summary>
    private ExtendedAxes availableAxes;
    /// <summary>Current positions of all axes on the game pad</summary>
    private float[/*24*/] axisStates;
    /// <summary>Sliders available on the game pad</summary>
    private ExtendedSliders availableSliders;
    /// <summary>Current setting of all sliders on the game pad</summary>
    private float[/*8*/] sliderStates;
    /// <summary>Number of PoV controller available on the game pad</summary>
    private int povCount;
    /// <summary>Current state of all PoV controllers on the game pad</summary>
    private int[/*4*/] povStates;

  }

} // namespace Nuclex.Input.Devices

#endif // !XBOX360

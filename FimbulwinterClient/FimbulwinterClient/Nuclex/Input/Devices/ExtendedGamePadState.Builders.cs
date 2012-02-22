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

#if !NO_DIRECTINPUT
using SlimDX.DirectInput;
#endif

namespace Nuclex.Input.Devices {

  /// <summary>Extended game pad state with additional buttons and axes</summary>
  partial struct ExtendedGamePadState {

    /// <summary>Initializes a new extended game pas state to the provided values</summary>
    /// <param name="availableAxes">Bit mask of the axes made available in the state</param>
    /// <param name="axes">
    ///   Values of all 24 axes in the order they appear in the ExtendedAxes enumeration
    /// </param>
    /// <param name="availableSliders">Bit mask of the slider provided by the state</param>
    /// <param name="sliders">
    ///   Values of all 8 sliders in the order they appear in the ExtendedSliders enumeration
    /// </param>
    /// <param name="buttonCount">Number of buttons provided by the state</param>
    /// <param name="buttons">State of all 128 buttons in the state</param>
    /// <param name="povCount">Number of Point-of-View controllers in the state</param>
    /// <param name="povs">State of all 4 Point-of-View controllers</param>
    public ExtendedGamePadState(
      ExtendedAxes availableAxes, float[/*24*/] axes,
      ExtendedSliders availableSliders, float[/*8*/] sliders,
      int buttonCount, bool[/*128*/] buttons,
      int povCount, int[/*4*/] povs
    ) {

      // Take over all axes
      this.AvailableAxes = availableAxes;
      this.X = axes[0];
      this.Y = axes[1];
      this.Z = axes[2];
      this.VelocityX = axes[3];
      this.VelocityY = axes[4];
      this.VelocityZ = axes[5];
      this.AccelerationX = axes[6];
      this.AccelerationY = axes[7];
      this.AccelerationZ = axes[8];
      this.ForceX = axes[9];
      this.ForceY = axes[10];
      this.ForceZ = axes[11];
      this.RotationX = axes[12];
      this.RotationY = axes[13];
      this.RotationZ = axes[14];
      this.AngularVelocityX = axes[15];
      this.AngularVelocityY = axes[16];
      this.AngularVelocityZ = axes[17];
      this.AngularAccelerationX = axes[18];
      this.AngularAccelerationY = axes[19];
      this.AngularAccelerationZ = axes[20];
      this.TorqueX = axes[21];
      this.TorqueY = axes[22];
      this.TorqueZ = axes[23];

      // Take over all sliders
      this.AvailableSliders = availableSliders;
      this.Slider1 = sliders[0];
      this.Slider2 = sliders[1];
      this.VelocitySlider1 = sliders[2];
      this.VelocitySlider2 = sliders[3];
      this.AccelerationSlider1 = sliders[4];
      this.AccelerationSlider2 = sliders[5];
      this.ForceSlider1 = sliders[6];
      this.ForceSlider2 = sliders[7];

      // Take over all buttons
      this.ButtonCount = buttonCount;
      this.buttonState1 = 0;
      for (int index = 0; index < Math.Min(64, buttonCount); ++index) {
        if (buttons[index]) {
          this.buttonState1 |= (1UL << index);
        }
      }
      this.buttonState2 = 0;
      for (int index = 0; index < (buttonCount - 64); ++index) {
        if (buttons[index + 64]) {
          this.buttonState2 |= (1UL << index);
        }
      }

      // Take over all PoV controllers
      this.PovCount = povCount;
      this.Pov1 = povs[0];
      this.Pov2 = povs[1];
      this.Pov3 = povs[2];
      this.Pov4 = povs[3];
    }

    /// <summary>
    ///   Initializes a new extended game pad state from a standard game pad state
    /// </summary>
    /// <param name="gamePadState">
    ///   Standard game pad state the extended game pad state is initialized from
    /// </param>
    public ExtendedGamePadState(ref GamePadState gamePadState) {
      // Axes
      {
        this.AvailableAxes =
          ExtendedAxes.X |
          ExtendedAxes.Y |
          ExtendedAxes.RotationX |
          ExtendedAxes.RotationY;

        this.X = gamePadState.ThumbSticks.Left.X;
        this.Y = gamePadState.ThumbSticks.Left.Y;
        this.Z = 0.0f;
        this.VelocityX = this.VelocityY = this.VelocityZ = 0.0f;
        this.AccelerationX = this.AccelerationY = this.AccelerationZ = 0.0f;
        this.ForceX = this.ForceY = this.ForceZ = 0.0f;

        this.RotationX = gamePadState.ThumbSticks.Right.X;
        this.RotationY = gamePadState.ThumbSticks.Right.Y;
        this.RotationZ = 0.0f;
        this.AngularVelocityX = this.AngularVelocityY = this.AngularVelocityZ = 0.0f;
        this.AngularAccelerationX = 0.0f;
        this.AngularAccelerationY = 0.0f;
        this.AngularAccelerationZ = 0.0f;
        this.TorqueX = this.TorqueY = this.TorqueZ = 0.0f;
      }

      // Buttons
      {
        this.ButtonCount = 11;
        this.buttonState1 =
          (gamePadState.IsButtonDown(Buttons.A) ? 1UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.B) ? 2UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.X) ? 4UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.Y) ? 8UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.LeftShoulder) ? 16UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.RightShoulder) ? 32UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.Back) ? 64UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.Start) ? 128UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.LeftStick) ? 256UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.RightStick) ? 512UL : 0UL) |
          (gamePadState.IsButtonDown(Buttons.BigButton) ? 1024UL : 0UL);
        this.buttonState2 = 0;
      }

      // Sliders
      {
        this.AvailableSliders =
          ExtendedSliders.Slider1 |
          ExtendedSliders.Slider2;

        this.Slider1 = gamePadState.Triggers.Left;
        this.Slider2 = gamePadState.Triggers.Right;
        this.VelocitySlider1 = this.VelocitySlider2 = 0.0f;
        this.AccelerationSlider1 = this.AccelerationSlider2 = 0.0f;
        this.ForceSlider1 = this.ForceSlider2 = 0.0f;
      }

      // PoVs
      {
        this.PovCount = 1;
        this.Pov1 = ExtendedGamePadState.PovFromDpad(gamePadState.DPad);
        this.Pov2 = -1;
        this.Pov3 = -1;
        this.Pov4 = -1;
      }
    }

#if !NO_DIRECTINPUT

    /// <summary>
    ///   Initializes a new extended game pad state from a DirectInput joystick state
    /// </summary>
    /// <param name="converter">DirectInput converter used to fill the state</param>
    /// <param name="joystickState">
    ///   Joystick state from which the extended game pad state will be built
    /// </param>
    internal ExtendedGamePadState(
      DirectInputConverter converter, ref JoystickState joystickState
    ) {

      // Take over the joystick's axes
      {
        this.AvailableAxes = converter.AvailableAxes;
        DirectInputConverter.IAxisReader[] axisReaders = converter.AxisReaders;

        if (axisReaders[0] != null) {
          this.X = axisReaders[0].GetValue(ref joystickState);
        } else {
          this.X = 0.0f;
        }
        if (axisReaders[1] != null) {
          this.Y = -axisReaders[1].GetValue(ref joystickState);
        } else {
          this.Y = 0.0f;
        }
        if (axisReaders[2] != null) {
          this.Z = axisReaders[2].GetValue(ref joystickState);
        } else {
          this.Z = 0.0f;
        }

        if (axisReaders[3] != null) {
          this.VelocityX = axisReaders[3].GetValue(ref joystickState);
        } else {
          this.VelocityX = 0.0f;
        }
        if (axisReaders[4] != null) {
          this.VelocityY = -axisReaders[4].GetValue(ref joystickState);
        } else {
          this.VelocityY = 0.0f;
        }
        if (axisReaders[5] != null) {
          this.VelocityZ = axisReaders[5].GetValue(ref joystickState);
        } else {
          this.VelocityZ = 0.0f;
        }

        if (axisReaders[6] != null) {
          this.AccelerationX = axisReaders[6].GetValue(ref joystickState);
        } else {
          this.AccelerationX = 0.0f;
        }
        if (axisReaders[7] != null) {
          this.AccelerationY = -axisReaders[7].GetValue(ref joystickState);
        } else {
          this.AccelerationY = 0.0f;
        }
        if (axisReaders[8] != null) {
          this.AccelerationZ = axisReaders[8].GetValue(ref joystickState);
        } else {
          this.AccelerationZ = 0.0f;
        }

        if (axisReaders[9] != null) {
          this.ForceX = axisReaders[9].GetValue(ref joystickState);
        } else {
          this.ForceX = 0.0f;
        }
        if (axisReaders[10] != null) {
          this.ForceY = -axisReaders[10].GetValue(ref joystickState);
        } else {
          this.ForceY = 0.0f;
        }
        if (axisReaders[11] != null) {
          this.ForceZ = axisReaders[11].GetValue(ref joystickState);
        } else {
          this.ForceZ = 0.0f;
        }

        if (axisReaders[12] != null) {
          this.RotationX = axisReaders[12].GetValue(ref joystickState);
        } else {
          this.RotationX = 0.0f;
        }
        if (axisReaders[13] != null) {
          this.RotationY = -axisReaders[13].GetValue(ref joystickState);
        } else {
          this.RotationY = 0.0f;
        }
        if (axisReaders[14] != null) {
          this.RotationZ = axisReaders[14].GetValue(ref joystickState);
        } else {
          this.RotationZ = 0.0f;
        }

        if (axisReaders[15] != null) {
          this.AngularVelocityX = axisReaders[15].GetValue(ref joystickState);
        } else {
          this.AngularVelocityX = 0.0f;
        }
        if (axisReaders[16] != null) {
          this.AngularVelocityY = -axisReaders[16].GetValue(ref joystickState);
        } else {
          this.AngularVelocityY = 0.0f;
        }
        if (axisReaders[17] != null) {
          this.AngularVelocityZ = axisReaders[17].GetValue(ref joystickState);
        } else {
          this.AngularVelocityZ = 0.0f;
        }

        if (axisReaders[18] != null) {
          this.AngularAccelerationX = axisReaders[18].GetValue(ref joystickState);
        } else {
          this.AngularAccelerationX = 0.0f;
        }
        if (axisReaders[19] != null) {
          this.AngularAccelerationY = -axisReaders[19].GetValue(ref joystickState);
        } else {
          this.AngularAccelerationY = 0.0f;
        }
        if (axisReaders[20] != null) {
          this.AngularAccelerationZ = axisReaders[20].GetValue(ref joystickState);
        } else {
          this.AngularAccelerationZ = 0.0f;
        }

        if (axisReaders[21] != null) {
          this.TorqueX = axisReaders[21].GetValue(ref joystickState);
        } else {
          this.TorqueX = 0.0f;
        }
        if (axisReaders[22] != null) {
          this.TorqueY = -axisReaders[22].GetValue(ref joystickState);
        } else {
          this.TorqueY = 0.0f;
        }
        if (axisReaders[23] != null) {
          this.TorqueZ = axisReaders[23].GetValue(ref joystickState);
        } else {
          this.TorqueZ = 0.0f;
        }
      }

      // Take over the joystick's buttons
      {
        this.ButtonCount = converter.ButtonCount;
        bool[] buttonPressed = joystickState.GetButtons();

        this.buttonState1 = 0;
        for (int index = 0; index < Math.Min(64, ButtonCount); ++index) {
          if (buttonPressed[index]) {
            this.buttonState1 |= (1UL << index);
          }
        }

        this.buttonState2 = 0;
        for (int index = 0; index < (ButtonCount - 64); ++index) {
          if (buttonPressed[index + 64]) {
            this.buttonState2 |= (1UL << index);
          }
        }
      }

      // Take over the joystick's sliders
      {
        this.AvailableSliders = converter.AvailableSliders;
        DirectInputConverter.ISliderReader[] sliderReaders = converter.SliderReaders;

        if (sliderReaders[0] != null) {
          this.Slider1 = sliderReaders[0].GetValue(ref joystickState);
        } else {
          this.Slider1 = 0.0f;
        }
        if (sliderReaders[1] != null) {
          this.Slider2 = sliderReaders[1].GetValue(ref joystickState);
        } else {
          this.Slider2 = 0.0f;
        }

        if (sliderReaders[2] != null) {
          this.VelocitySlider1 = sliderReaders[2].GetValue(ref joystickState);
        } else {
          this.VelocitySlider1 = 0.0f;
        }
        if (sliderReaders[3] != null) {
          this.VelocitySlider2 = sliderReaders[3].GetValue(ref joystickState);
        } else {
          this.VelocitySlider2 = 0.0f;
        }

        if (sliderReaders[4] != null) {
          this.AccelerationSlider1 = sliderReaders[4].GetValue(ref joystickState);
        } else {
          this.AccelerationSlider1 = 0.0f;
        }
        if (sliderReaders[5] != null) {
          this.AccelerationSlider2 = sliderReaders[5].GetValue(ref joystickState);
        } else {
          this.AccelerationSlider2 = 0.0f;
        }

        if (sliderReaders[6] != null) {
          this.ForceSlider1 = sliderReaders[6].GetValue(ref joystickState);
        } else {
          this.ForceSlider1 = 0.0f;
        }
        if (sliderReaders[7] != null) {
          this.ForceSlider2 = sliderReaders[7].GetValue(ref joystickState);
        } else {
          this.ForceSlider2 = 0.0f;
        }
      }

      // Take over the joystick's Point-of-View controllers
      {
        this.PovCount = converter.PovCount;

        int[] povs = null;
        if (this.PovCount >= 1) {
          povs = joystickState.GetPointOfViewControllers();
          this.Pov1 = povs[0];
        } else {
          this.Pov1 = -1;
        }
        if (this.PovCount >= 2) {
          this.Pov2 = povs[1];
        } else {
          this.Pov2 = -1;
        }
        if (this.PovCount >= 3) {
          this.Pov3 = povs[2];
        } else {
          this.Pov3 = -1;
        }
        if (this.PovCount >= 4) {
          this.Pov4 = povs[3];
        } else {
          this.Pov4 = -1;
        }
      }
    }

#endif

  }

} // namespace Nuclex.Input.Devices

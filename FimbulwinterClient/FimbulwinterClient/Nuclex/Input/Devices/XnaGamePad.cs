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

using MsGamePad = Microsoft.Xna.Framework.Input.GamePad;

namespace Nuclex.Input.Devices {

  /// <summary>Interfaces with an XBox 360 controller via XNA (XINPUT)</summary>
  internal class XnaGamePad : GamePad {

    /// <summary>Initializes a new XNA-based keyboard device</summary>
    public XnaGamePad(PlayerIndex playerIndex) {
      this.playerIndex = playerIndex;
      this.states = new Queue<GamePadState>();
      this.current = new GamePadState();
    }

    /// <summary>Retrieves the current state of the game pad</summary>
    /// <returns>The current state of the game pad</returns>
    public override GamePadState GetState() {
      return this.current;
    }

    /// <summary>Retrieves the current DirectInput joystick state</summary>
    /// <returns>The current state of the DirectInput joystick</returns>
    public override ExtendedGamePadState GetExtendedState() {
      return new ExtendedGamePadState(ref this.current);
    }

    /// <summary>Whether the input device is connected to the system</summary>
    public override bool IsAttached {
      get { return this.current.IsConnected; }
    }

    /// <summary>Human-readable name of the input device</summary>
    public override string Name {
      get { return "Xbox 360 game pad #" + ((int)this.playerIndex + 1).ToString(); }
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
      GamePadState previous = this.current;

      if (this.states.Count == 0) {
        this.current = MsGamePad.GetState(this.playerIndex);
      } else {
        this.current = this.states.Dequeue();
      }

      GenerateEvents(ref previous, ref this.current);
    }

    /// <summary>Takes a snapshot of the current state of the input device</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public override void TakeSnapshot() {
      this.states.Enqueue(MsGamePad.GetState(this.playerIndex));
    }

    /// <summary>Checks for state changes and triggers the corresponding events</summary>
    /// <param name="previous">Previous state of the game pad</param>
    /// <param name="current">Current state of the game pad</param>
    protected void GenerateEvents(ref GamePadState previous, ref GamePadState current) {
      if (!HaveEventSubscribers && !HaveExtendedEventSubscribers) {
        return;
      }

      Buttons pressedButtons = 0;
      Buttons releasedButtons = 0;
      ulong pressedExtendedButtons = 0;
      ulong releasedExtendedButtons = 0;

      // See if the state of the 'A' button has changed between two polls
      if (current.Buttons.A != previous.Buttons.A) {
        if (current.Buttons.A == ButtonState.Pressed) {
          pressedButtons |= Buttons.A;
          pressedExtendedButtons |= (1 << 0);
        } else {
          releasedButtons |= Buttons.A;
          releasedExtendedButtons |= (1 << 0);
        }
      }

      // See if the state of the 'B' button has changed between two polls
      if (current.Buttons.B != previous.Buttons.B) {
        if (current.Buttons.B == ButtonState.Pressed) {
          pressedButtons |= Buttons.B;
          pressedExtendedButtons |= (1 << 1);
        } else {
          releasedButtons |= Buttons.B;
          releasedExtendedButtons |= (1 << 1);
        }
      }

      // See if the state of the 'X' button has changed between two polls
      if (current.Buttons.X != previous.Buttons.X) {
        if (current.Buttons.X == ButtonState.Pressed) {
          pressedButtons |= Buttons.X;
          pressedExtendedButtons |= (1 << 2);
        } else {
          releasedButtons |= Buttons.X;
          releasedExtendedButtons |= (1 << 2);
        }
      }

      // See if the state of the 'A' button has changed between two polls
      if (current.Buttons.Y != previous.Buttons.Y) {
        if (current.Buttons.Y == ButtonState.Pressed) {
          pressedButtons |= Buttons.Y;
          pressedExtendedButtons |= (1 << 3);
        } else {
          releasedButtons |= Buttons.Y;
          releasedExtendedButtons |= (1 << 3);
        }
      }

      // See if the state of the left shoulder button has changed between two polls
      if (current.Buttons.LeftShoulder != previous.Buttons.LeftShoulder) {
        if (current.Buttons.LeftShoulder == ButtonState.Pressed) {
          pressedButtons |= Buttons.LeftShoulder;
          pressedExtendedButtons |= (1 << 4);
        } else {
          releasedButtons |= Buttons.LeftShoulder;
          releasedExtendedButtons |= (1 << 4);
        }
      }

      // See if the state of the right shoulder button has changed between two polls
      if (current.Buttons.RightShoulder != previous.Buttons.RightShoulder) {
        if (current.Buttons.RightShoulder == ButtonState.Pressed) {
          pressedButtons |= Buttons.RightShoulder;
          pressedExtendedButtons |= (1 << 5);
        } else {
          releasedButtons |= Buttons.RightShoulder;
          releasedExtendedButtons |= (1 << 5);
        }
      }

      // See if the state of the back button has changed between two polls
      if (current.Buttons.Back != previous.Buttons.Back) {
        if (current.Buttons.Back == ButtonState.Pressed) {
          pressedButtons |= Buttons.Back;
          pressedExtendedButtons |= (1 << 6);
        } else {
          releasedButtons |= Buttons.Back;
          releasedExtendedButtons |= (1 << 6);
        }
      }

      // See if the state of the start button has changed between two polls
      if (current.Buttons.Start != previous.Buttons.Start) {
        if (current.Buttons.Start == ButtonState.Pressed) {
          pressedButtons |= Buttons.Start;
          pressedExtendedButtons |= (1 << 7);
        } else {
          releasedButtons |= Buttons.Start;
          releasedExtendedButtons |= (1 << 7);
        }
      }

      // See if the state of the left stick button has changed between two polls
      if (current.Buttons.LeftStick != previous.Buttons.LeftStick) {
        if (current.Buttons.LeftStick == ButtonState.Pressed) {
          pressedButtons |= Buttons.LeftStick;
          pressedExtendedButtons |= (1 << 8);
        } else {
          releasedButtons |= Buttons.LeftStick;
          releasedExtendedButtons |= (1 << 8);
        }
      }


      // See if the state of the right stick button has changed between two polls
      if (current.Buttons.RightStick != previous.Buttons.RightStick) {
        if (current.Buttons.RightStick == ButtonState.Pressed) {
          pressedButtons |= Buttons.RightStick;
          pressedExtendedButtons |= (1 << 9);
        } else {
          releasedButtons |= Buttons.RightStick;
          releasedExtendedButtons |= (1 << 9);
        }
      }

      // See if the state of the back button has changed between two polls
      if (current.Buttons.BigButton != previous.Buttons.BigButton) {
        if (current.Buttons.BigButton == ButtonState.Pressed) {
          pressedButtons |= Buttons.BigButton;
          pressedExtendedButtons |= (1 << 10);
        } else {
          releasedButtons |= Buttons.BigButton;
          releasedExtendedButtons |= (1 << 10);
        }
      }

      if (releasedButtons != 0) {
        OnButtonReleased(releasedButtons);
      }
      if (releasedExtendedButtons != 0) {
        OnExtendedButtonReleased(releasedExtendedButtons, 0UL);
      }
      if (pressedButtons != 0) {
        OnButtonPressed(pressedButtons);
      }
      if (pressedExtendedButtons != 0) {
        OnExtendedButtonPressed(pressedExtendedButtons, 0UL);
      }
    }

    /// <summary>Index of the player this device represents</summary>
    private PlayerIndex playerIndex;

    /// <summary>Snapshots of the game pad state waiting to be processed</summary>
    private Queue<GamePadState> states;

    /// <summary>Currently published game pad state</summary>
    private GamePadState current;

  }

} // namespace Nuclex.Input.Devices

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
using System.Collections.ObjectModel;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nuclex.Input.Devices;

using XnaMouse = Microsoft.Xna.Framework.Input.Mouse;
using XnaEventHandler = System.EventHandler<System.EventArgs>;

#if WINDOWS

namespace Nuclex.Input {

  /// <summary>Manages a set of fake input devices</summary>
  public class MockInputManager : IInputService, IDisposable {

    /// <summary>Initializes a new mock input manager</summary>
    public MockInputManager() : this(null) { }

    /// <summary>Initializs a new mock input manager</summary>
    /// <param name="services">Game service container the manager registers to</param>
    public MockInputManager(GameServiceContainer services) {
      if (services != null) {
        this.gameServices = services;
        this.gameServices.AddService(typeof(IInputService), this);
      }

      setupGamePads();
      setupMouse();
      setupKeyboards();
      setupTouchPanels();
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if (this.gameServices != null) {
        object inputService = this.gameServices.GetService(typeof(IInputService));
        if (ReferenceEquals(inputService, this)) {
          this.gameServices.RemoveService(typeof(IInputService));
        }

        this.gameServices = null;
      }
    }

    /// <summary>All keyboards known to the system</summary>
    public ReadOnlyCollection<MockedKeyboard> Keyboards {
      get { return this.keyboards; }
    }

    /// <summary>All mice known to the system</summary>
    public ReadOnlyCollection<MockedMouse> Mice {
      get { return this.mice; }
    }

    /// <summary>All game pads known to the system</summary>
    public ReadOnlyCollection<MockedGamePad> GamePads {
      get { return this.gamePads; }
    }

    /// <summary>All touch panels known to the system</summary>
    public ReadOnlyCollection<MockedTouchPanel> TouchPanels {
      get { return this.touchPanels; }
    }

    /// <summary>Returns the primary mouse input device</summary>
    /// <returns>The primary mouse</returns>
    public MockedMouse GetMouse() {
      return CollectionHelper.GetIfExists(this.mice, 0);
    }

    /// <summary>Returns the keyboard on a PC</summary>
    /// <returns>The keyboard</returns>
    public MockedKeyboard GetKeyboard() {
      return CollectionHelper.GetIfExists(this.keyboards, 4);
    }

    /// <summary>Returns the chat pad for the specified player</summary>
    /// <param name="playerIndex">Player whose chat pad will be returned</param>
    /// <returns>The chat pad of the specified player</returns>
    public MockedKeyboard GetKeyboard(PlayerIndex playerIndex) {
      return this.keyboards[(int)playerIndex];
    }

    /// <summary>Returns the game pad for the specified player</summary>
    /// <param name="playerIndex">Player whose game pad will be returned</param>
    /// <returns>The game pad of the specified player</returns>
    /// <remarks>
    ///   This will only return the XINPUT devices (aka XBox 360 controllers)
    ///   attached. Any standard game pads attached to a PC can only be
    ///   returned through the ExtendedPlayerIndex overload where they will
    ///   take the places of game pads for player 5 and upwards.
    /// </remarks>
    public MockedGamePad GetGamePad(PlayerIndex playerIndex) {
      return this.gamePads[(int)playerIndex];
    }

    /// <summary>Returns the game pad for the specified player</summary>
    /// <param name="playerIndex">Player whose game pad will be returned</param>
    /// <returns>The game pad of the specified player</returns>
    public MockedGamePad GetGamePad(ExtendedPlayerIndex playerIndex) {
      return this.gamePads[(int)playerIndex];
    }

    /// <summary>Returns the touch panel on the system</summary>
    /// <returns>The system's touch panel</returns>
    public MockedTouchPanel GetTouchPanel() {
      return this.TouchPanels[0];
    }

    /// <summary>Updates the state of all input devices</summary>
    /// <remarks>
    ///   <para>
    ///     If this method is called with no snapshots in the queue, it will
    ///     query the state of all input devices immediately, raising events
    ///     for any changed states. This way, you can ignore the entire
    ///     snapshot system if you just want basic input device access.
    ///   </para>
    ///   <para>
    ///     If this method is called while one or more snapshots are waiting in
    ///     the queue, this method takes the next snapshot from the queue and makes
    ///     it the current state of all active devices.
    ///   </para>
    /// </remarks>
    public void Update() {
      if (this.snapshotCount > 0) {
        --this.snapshotCount;
      }

      for (int index = 0; index < this.gamePads.Count; ++index) {
        this.gamePads[index].Update();
      }
      for (int index = 0; index < this.mice.Count; ++index) {
        this.mice[index].Update();
      }
      for (int index = 0; index < this.keyboards.Count; ++index) {
        this.keyboards[index].Update();
      }
    }

    /// <summary>Takes a snapshot of the current state of all input devices</summary>
    /// <remarks>
    ///   This snapshot will be queued until the user calls the Update() method,
    ///   where the next polled snapshot will be taken from the queue and provided
    ///   to the user.
    /// </remarks>
    public void TakeSnapshot() {
      for (int index = 0; index < this.gamePads.Count; ++index) {
        this.gamePads[index].TakeSnapshot();
      }
      for (int index = 0; index < this.mice.Count; ++index) {
        this.mice[index].TakeSnapshot();
      }
      for (int index = 0; index < this.keyboards.Count; ++index) {
        this.keyboards[index].TakeSnapshot();
      }

      ++this.snapshotCount;
    }

    /// <summary>Number of snapshots currently in the queue</summary>
    public int SnapshotCount {
      get { return this.snapshotCount; }
    }

    /// <summary>All keyboards known to the system</summary>
    ReadOnlyCollection<IKeyboard> IInputService.Keyboards {
      get { return this.interfaceKeyboards; }
    }

    /// <summary>All mice known to the system</summary>
    ReadOnlyCollection<IMouse> IInputService.Mice {
      get { return this.interfaceMice; }
    }

    /// <summary>All game pads known to the system</summary>
    ReadOnlyCollection<IGamePad> IInputService.GamePads {
      get { return this.interfaceGamePads; }
    }

    /// <summary>All touch panels known to the system</summary>
    ReadOnlyCollection<ITouchPanel> IInputService.TouchPanels {
      get { return this.interfaceTouchPanels; }
    }

    /// <summary>Returns the primary mouse input device</summary>
    /// <returns>The primary mouse</returns>
    IMouse IInputService.GetMouse() {
      return CollectionHelper.GetIfExists(this.mice, 0);
    }

    /// <summary>Returns the keyboard on a PC</summary>
    /// <returns>The keyboard</returns>
    IKeyboard IInputService.GetKeyboard() {
      return CollectionHelper.GetIfExists(this.keyboards, 4);
    }

    /// <summary>Returns the chat pad for the specified player</summary>
    /// <param name="playerIndex">Player whose chat pad will be returned</param>
    /// <returns>The chat pad of the specified player</returns>
    IKeyboard IInputService.GetKeyboard(PlayerIndex playerIndex) {
      return this.keyboards[(int)playerIndex];
    }

    /// <summary>Returns the game pad for the specified player</summary>
    /// <param name="playerIndex">Player whose game pad will be returned</param>
    /// <returns>The game pad of the specified player</returns>
    /// <remarks>
    ///   This will only return the XINPUT devices (aka XBox 360 controllers)
    ///   attached. Any standard game pads attached to a PC can only be
    ///   returned through the ExtendedPlayerIndex overload where they will
    ///   take the places of game pads for player 5 and upwards.
    /// </remarks>
    IGamePad IInputService.GetGamePad(PlayerIndex playerIndex) {
      return this.gamePads[(int)playerIndex];
    }

    /// <summary>Returns the game pad for the specified player</summary>
    /// <param name="playerIndex">Player whose game pad will be returned</param>
    /// <returns>The game pad of the specified player</returns>
    IGamePad IInputService.GetGamePad(ExtendedPlayerIndex playerIndex) {
      return this.gamePads[(int)playerIndex];
    }

    /// <summary>Returns the touch panel on the system</summary>
    /// <returns>The system's touch panel</returns>
    ITouchPanel IInputService.GetTouchPanel() {
      return this.touchPanels[0];
    }

    /// <summary>Sets up the collection of available game pads</summary>
    private void setupGamePads() {
      var gamePads = new List<MockedGamePad>();
      var interfaceGamePads = new List<IGamePad>();

      // Add place holders for all unattached game pads
      while (gamePads.Count < 8) {
        var gamePad = new MockedGamePad();
        gamePads.Add(gamePad);
        interfaceGamePads.Add(gamePad);
      }

      this.gamePads = new ReadOnlyCollection<MockedGamePad>(gamePads);
      this.interfaceGamePads = new ReadOnlyCollection<IGamePad>(interfaceGamePads);
    }

    /// <summary>Sets up the collection of available mice</summary>
    private void setupMouse() {
      var mice = new List<MockedMouse>();
      var interfaceMice = new List<IMouse>();

      var mouse = new MockedMouse();
      mice.Add(mouse);
      interfaceMice.Add(mouse);

      this.mice = new ReadOnlyCollection<MockedMouse>(mice);
      this.interfaceMice = new ReadOnlyCollection<IMouse>(interfaceMice);
    }

    /// <summary>Sets up the collection of available keyboards</summary>
    private void setupKeyboards() {
      var keyboards = new List<MockedKeyboard>();
      var interfaceKeyboards = new List<IKeyboard>();

      for (int index = 0; index < 5; ++index) {
        var keyboard = new MockedKeyboard();

        keyboards.Add(keyboard);
        interfaceKeyboards.Add(keyboard);
      }

      this.keyboards = new ReadOnlyCollection<MockedKeyboard>(keyboards);
      this.interfaceKeyboards = new ReadOnlyCollection<IKeyboard>(interfaceKeyboards);
    }

    /// <summary>Sets up the collection of available touch panels</summary>
    private void setupTouchPanels() {
      var touchPanels = new List<MockedTouchPanel>();
      var interfaceTouchPanels = new List<ITouchPanel>();

      var touchPanel = new MockedTouchPanel();
      touchPanels.Add(touchPanel);
      interfaceTouchPanels.Add(touchPanel);

      this.touchPanels = new ReadOnlyCollection<MockedTouchPanel>(touchPanels);
      this.interfaceTouchPanels = new ReadOnlyCollection<ITouchPanel>(interfaceTouchPanels);
    }

    /// <summary>Number of state snap shots currently queued</summary>
    private int snapshotCount;

    /// <summary>Collection of all game pads known to the system</summary>
    private ReadOnlyCollection<MockedGamePad> gamePads;
    /// <summary>Collection of all mice known to the system</summary>
    private ReadOnlyCollection<MockedMouse> mice;
    /// <summary>Collection of all keyboards known to the system</summary>
    private ReadOnlyCollection<MockedKeyboard> keyboards;
    /// <summary>Collection of all touch panels known to the system</summary>
    private ReadOnlyCollection<MockedTouchPanel> touchPanels;

    /// <summary>Collection of all game pads known to the system</summary>
    private ReadOnlyCollection<IGamePad> interfaceGamePads;
    /// <summary>Collection of all mice known to the system</summary>
    private ReadOnlyCollection<IMouse> interfaceMice;
    /// <summary>Collection of all keyboards known to the system</summary>
    private ReadOnlyCollection<IKeyboard> interfaceKeyboards;
    /// <summary>Collection of all touch panels known to the system</summary>
    private ReadOnlyCollection<ITouchPanel> interfaceTouchPanels;

    /// <summary>Game service container, saved to unregister on dispose</summary>
    private GameServiceContainer gameServices;

  }

} // namespace Nuclex.Input

#endif // WINDOWS

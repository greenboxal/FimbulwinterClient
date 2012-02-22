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
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nuclex.Input.Devices;

namespace Nuclex.Input {

  /// <summary>Detects which controller the player is using</summary>
  public class ControllerDetector : IDisposable {

    /// <summary>
    ///   Called when the player pressed a button on one of the controllers
    /// </summary>
    public event EventHandler<ControllerEventArgs> ControllerDetected;

    /// <summary>Reports the index of the player who pressed a key/button</summary>
    /// <param name="playerIndex">Reported player index</param>
    public delegate void DetectionDelegate(ExtendedPlayerIndex? playerIndex);

    #region class Reporter

    /// <summary>Base class for key/button press reporters</summary>
    private class Reporter {

      /// <summary>Initializes a new reporter</summary>
      /// <param name="callback">Callback the reporter uses to report</param>
      /// <param name="playerIndex">Player index the reporter will report</param>
      public Reporter(DetectionDelegate callback, ExtendedPlayerIndex? playerIndex) {
        this.Callback = callback;
        this.PlayerIndex = playerIndex;
      }

      /// <summary>Callback the reporter invokes on a button/key press</summary>
      protected DetectionDelegate Callback;
      /// <summary>Player index the reporter will provide to the callback</summary>
      protected ExtendedPlayerIndex? PlayerIndex;

    }

    #endregion // class Reporter

    #region class KeyReporter

    /// <summary>Reports key presses on a keyboard</summary>
    private class KeyReporter : Reporter {

      /// <summary>Initializes a new keyboard reporter</summary>
      /// <param name="callback">Callback the reporter uses to report</param>
      /// <param name="playerIndex">Player index the reporter will report</param>
      public KeyReporter(
        DetectionDelegate callback, ExtendedPlayerIndex? playerIndex
      ) :
        base(callback, playerIndex) { }

      /// <summary>Subscribable callback for a key press</summary>
      /// <param name="key">Key that has been pressed</param>
      public void KeyPressed(Keys key) {
        this.Callback(this.PlayerIndex);
      }

    }

    #endregion // class KeyReporter

    #region class MouseButtonReporter

    /// <summary>Reports buttons pressed on a mouse</summary>
    private class MouseButtonReporter : Reporter {

      /// <summary>Initializes a new mouse reporter</summary>
      /// <param name="callback">Callback the reporter uses to report</param>
      /// <param name="playerIndex">Player index the reporter will report</param>
      public MouseButtonReporter(
        DetectionDelegate callback, ExtendedPlayerIndex? playerIndex
      ) :
        base(callback, playerIndex) { }

      /// <summary>Subscribable callback for a mouse button press</summary>
      /// <param name="buttons">Mouse buttons that have been pressed</param>
      public void MouseButtonPressed(MouseButtons buttons) {
        this.Callback(this.PlayerIndex);
      }

    }

    #endregion // class MouseButtonReporter

    #region class GamePadButtonReporter

    /// <summary>Reports buttons pressed on a game pad</summary>
    private class GamePadButtonReporter : Reporter {

      /// <summary>Initializes a new game pad reporter</summary>
      /// <param name="callback">Callback the reporter uses to report</param>
      /// <param name="playerIndex">Player index the reporter will report</param>
      public GamePadButtonReporter(
        DetectionDelegate callback, ExtendedPlayerIndex? playerIndex
      ) :
        base(callback, playerIndex) { }

      /// <summary>Subscribable callback for a game pad button press</summary>
      /// <param name="buttons">Game pad buttons that have been pressed</param>
      public void ButtonPressed(Buttons buttons) {
        this.Callback(this.PlayerIndex);
      }

    }

    #endregion // class GamePadButtonReporter

    /// <summary>Initializes a new controller detector</summary>
    /// <param name="inputService">
    ///   Input service the detector uses to find out the controller
    /// </param>
    public ControllerDetector(IInputService inputService) {
      this.inputService = inputService;

      this.subscribedKeyReporters = new Stack<KeyDelegate>();
      this.subscribedMouseReporters = new Stack<MouseButtonDelegate>();
      this.subscribedGamePadReporters = new Stack<GamePadButtonDelegate>();

      this.detectedDelegate = new DetectionDelegate(detected);
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      Stop();

      this.subscribedGamePadReporters = null;
      this.subscribedMouseReporters = null;
      this.subscribedKeyReporters = null;
    }

    /// <summary>Begins monitoring input devices</summary>
    public void Start() {
      if (!this.started) {
        subscribeAllEvents();
        this.started = true;
      }
    }

    /// <summary>Stops monitoring input devices</summary>
    /// <remarks>
    ///   After the detection event was triggered once, this is automatically called.
    ///   You do not need to explicitly call this unless you want to abort detection.
    /// </remarks>
    public void Stop() {
      if (this.started) {
        unsubscribeAllEvents();
        this.started = false;
      }
    }
    
    /// <summary>Fires the ControllerDetected event</summary>
    /// <param name="playerIndex">Event that will be fired</param>
    protected virtual void OnControllerDetected(ExtendedPlayerIndex? playerIndex) {
      if(ControllerDetected != null) {
        if(playerIndex.HasValue) {
          ControllerDetected(this, new ControllerEventArgs(playerIndex.Value));
        } else {
          ControllerDetected(this, new ControllerEventArgs());
        }
      }
    }

    /// <summary>Called when a key/button press has been detected</summary>
    /// <param name="playerIndex">Index of the player who pressed a key/button</param>
    private void detected(ExtendedPlayerIndex? playerIndex) {
      OnControllerDetected(playerIndex);
      // Stop(); // Causes a StackEmptyException b/c unsubscribing inside event
    }

    /// <summary>Subscribes the detector to all input devices</summary>
    private void subscribeAllEvents() {

      // Subscribe to all chat pads
      for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; ++index) {
        var reporter = new KeyReporter(this.detectedDelegate, (ExtendedPlayerIndex)index);
        var method = new KeyDelegate(reporter.KeyPressed);
        this.inputService.GetKeyboard(index).KeyPressed += method;
        this.subscribedKeyReporters.Push(method);
      }

      // Subscribe to the main PC keyboard
      {
        var reporter = new KeyReporter(this.detectedDelegate, null);
        var method = new KeyDelegate(reporter.KeyPressed);
        this.inputService.GetKeyboard().KeyPressed += method;
        this.subscribedKeyReporters.Push(method);
      }

      // Subscribe to the main PC mouse
      {
        var reporter = new MouseButtonReporter(this.detectedDelegate, null);
        var method = new MouseButtonDelegate(reporter.MouseButtonPressed);
        this.inputService.GetMouse().MouseButtonPressed += method;
        this.subscribedMouseReporters.Push(method);
      }

      // Subscribe to all game pads
      for (
        ExtendedPlayerIndex index = ExtendedPlayerIndex.One;
        index <= ExtendedPlayerIndex.Eight;
        ++index
      ) {
        var reporter = new GamePadButtonReporter(this.detectedDelegate, index);
        var method = new GamePadButtonDelegate(reporter.ButtonPressed);
        this.inputService.GetGamePad(index).ButtonPressed += method;
        this.subscribedGamePadReporters.Push(method);
      }

    }

    /// <summary>Unsubscribes the detector from all input devices</summary>
    private void unsubscribeAllEvents() {

      // Unsubscribe from all game pads
      for (
        ExtendedPlayerIndex index = ExtendedPlayerIndex.Eight;
        index >= ExtendedPlayerIndex.One;
        --index
      ) {
        var method = this.subscribedGamePadReporters.Pop();
        this.inputService.GetGamePad(index).ButtonPressed -= method;
      }

      // Unsubscribe from the main PC mouse
      {
        var method = this.subscribedMouseReporters.Pop();
        this.inputService.GetMouse().MouseButtonPressed -= method;
      }

      // Unsubscribe from the main PC keyboard
      {
        var method = this.subscribedKeyReporters.Pop();
        this.inputService.GetKeyboard().KeyPressed -= method;
      }

      // Unsubscribe from all chat pads
      for (PlayerIndex index = PlayerIndex.Four; index >= PlayerIndex.One; --index) {
        var method = this.subscribedKeyReporters.Pop();
        this.inputService.GetKeyboard().KeyPressed -= method;
      }

    }

    /// <summary>Input service the detector uses to access the controllers</summary>
    private IInputService inputService;
    /// <summary>Whether the detection is currently running</summary>
    private bool started;
    /// <summary>Currently subscribed key press reporters</summary>
    private Stack<KeyDelegate> subscribedKeyReporters;
    /// <summary>Currently subscribed mouse button press reporters</summary>
    private Stack<MouseButtonDelegate> subscribedMouseReporters;
    /// <summary>Currently subscribed game pad button press reporters</summary>
    private Stack<GamePadButtonDelegate> subscribedGamePadReporters;
    /// <summary>Delegate for the controllerDetected() method</summary>
    private DetectionDelegate detectedDelegate;

  }

} // namespace Nuclex.Input

#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

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

using Nuclex.Input;
using Nuclex.Input.Devices;

namespace Nuclex.UserInterface.Input {

  /// <summary>Default implementation of an input capturer</summary>
  public class DefaultInputCapturer : IInputCapturer, IDisposable {

    #region class DummyInputReceiver

    /// <summary>Dummy receiver for input events</summary>
    private class DummyInputReceiver : IInputReceiver {

      /// <summary>Default instance of the dummy receiver</summary>
      public static readonly DummyInputReceiver Default = new DummyInputReceiver();

      /// <summary>Injects an input command into the input receiver</summary>
      /// <param name="command">Input command to be injected</param>
      public void InjectCommand(Command command) { }

      /// <summary>Called when a button on the gamepad has been pressed</summary>
      /// <param name="button">Button that has been pressed</param>
      public void InjectButtonPress(Buttons button) { }

      /// <summary>Called when a button on the gamepad has been released</summary>
      /// <param name="button">Button that has been released</param>
      public void InjectButtonRelease(Buttons button) { }

      /// <summary>Injects a mouse position update into the receiver</summary>
      /// <param name="x">New X coordinate of the mouse cursor on the screen</param>
      /// <param name="y">New Y coordinate of the mouse cursor on the screen</param>
      public void InjectMouseMove(float x, float y) { }

      /// <summary>Called when a mouse button has been pressed down</summary>
      /// <param name="button">Index of the button that has been pressed</param>
      public void InjectMousePress(MouseButtons button) { }

      /// <summary>Called when a mouse button has been released again</summary>
      /// <param name="button">Index of the button that has been released</param>
      public void InjectMouseRelease(MouseButtons button) { }

      /// <summary>Called when the mouse wheel has been rotated</summary>
      /// <param name="ticks">Number of ticks that the mouse wheel has been rotated</param>
      public void InjectMouseWheel(float ticks) { }

      /// <summary>Called when a key on the keyboard has been pressed down</summary>
      /// <param name="keyCode">Code of the key that was pressed</param>
      public void InjectKeyPress(Keys keyCode) { }

      /// <summary>Called when a key on the keyboard has been released again</summary>
      /// <param name="keyCode">Code of the key that was released</param>
      public void InjectKeyRelease(Keys keyCode) { }

      /// <summary>Handle user text input by a physical or virtual keyboard</summary>
      /// <param name="character">Character that has been entered</param>
      public void InjectCharacter(char character) { }

    }

    #endregion // class DummyInputReceiver

    /// <summary>
    ///   Initializes a new input capturer, taking the input service from a service provider
    /// </summary>
    /// <param name="serviceProvider">
    ///   Service provider the input capturer will take the input service from
    /// </param>
    public DefaultInputCapturer(IServiceProvider serviceProvider) :
      this(getInputService(serviceProvider)) { }

    /// <summary>
    ///   Initializes a new input capturer using the specified input service
    /// </summary>
    /// <param name="inputService">
    ///   Input service the capturer will subscribe to
    /// </param>
    public DefaultInputCapturer(IInputService inputService) {
      this.inputService = inputService;
      this.inputReceiver = new DummyInputReceiver();
      this.playerIndex = ExtendedPlayerIndex.One;

      this.keyPressedDelegate = new KeyDelegate(keyPressed);
      this.keyReleasedDelegate = new KeyDelegate(keyReleased);
      this.characterEnteredDelegate = new CharacterDelegate(characterEntered);
      this.mouseButtonPressedDelegate = new MouseButtonDelegate(mouseButtonPressed);
      this.mouseButtonReleasedDelegate = new MouseButtonDelegate(mouseButtonReleased);
      this.mouseMovedDelegate = new MouseMoveDelegate(mouseMoved);
      this.mouseWheelRotatedDelegate = new MouseWheelDelegate(mouseWheelRotated);
      this.buttonPressedDelegate = new GamePadButtonDelegate(buttonPressed);
      this.buttonReleasedDelegate = new GamePadButtonDelegate(buttonReleased);

      subscribeInputDevices();
    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if (this.inputService != null) {
        unsubscribeInputDevices();
        this.inputService = null;
      }
    }

    /// <summary>Input receiver any captured input will be sent to</summary>
    public IInputReceiver InputReceiver {
      get {
        if (ReferenceEquals(this.inputReceiver, DummyInputReceiver.Default)) {
          return null;
        } else {
          return this.inputReceiver;
        }
      }
      set {
        if (value == null) {
          this.inputReceiver = DummyInputReceiver.Default;
        } else {
          this.inputReceiver = value;
        }
      }
    }

    /// <summary>Changes the controller which can interact with the GUI</summary>
    /// <param name="playerIndex">
    ///   Index of the player whose controller will be allowed to interact with the GUI
    /// </param>
    public void ChangePlayerIndex(PlayerIndex playerIndex) {
      ChangePlayerIndex((ExtendedPlayerIndex)playerIndex);
    }

    /// <summary>Changes the controller which can interact with the GUI</summary>
    /// <param name="playerIndex">
    ///   Index of the player whose controller will be allowed to interact with the GUI
    /// </param>
    public void ChangePlayerIndex(ExtendedPlayerIndex playerIndex) {
      unsubscribePlayerSpecificInputDevices();
      this.playerIndex = playerIndex;
      subscribePlayerSpecificDevices();
    }

    /// <summary>Subscribes to the events of all input devices</summary>
    private void subscribeInputDevices() {
      this.subscribedKeyboard = this.inputService.GetKeyboard();
      this.subscribedKeyboard.KeyPressed += this.keyPressedDelegate;
      this.subscribedKeyboard.KeyReleased += this.keyReleasedDelegate;
      this.subscribedKeyboard.CharacterEntered += this.characterEnteredDelegate;

      this.subscribedMouse = this.inputService.GetMouse();
      this.subscribedMouse.MouseButtonPressed += this.mouseButtonPressedDelegate;
      this.subscribedMouse.MouseButtonReleased += this.mouseButtonReleasedDelegate;
      this.subscribedMouse.MouseMoved += this.mouseMovedDelegate;
      this.subscribedMouse.MouseWheelRotated += this.mouseWheelRotatedDelegate;

      subscribePlayerSpecificDevices();
    }

    /// <summary>Subscribes to the events of all player-specific input devices</summary>
    private void subscribePlayerSpecificDevices() {
      this.subscribedGamePad = this.inputService.GetGamePad(this.playerIndex);
      this.subscribedGamePad.ButtonPressed += this.buttonPressedDelegate;
      this.subscribedGamePad.ButtonReleased += this.buttonReleasedDelegate;

      if (this.playerIndex < ExtendedPlayerIndex.Five) {
        var standardPlayerIndex = (PlayerIndex)this.playerIndex;
        this.subscribedChatPad = this.inputService.GetKeyboard(standardPlayerIndex);
        this.subscribedChatPad.KeyPressed += this.keyPressedDelegate;
        this.subscribedChatPad.KeyReleased += this.keyReleasedDelegate;
        this.subscribedChatPad.CharacterEntered += this.characterEnteredDelegate;
      }
    }

    /// <summary>Unsubscribes from the events of all input devices</summary>
    private void unsubscribeInputDevices() {
      unsubscribePlayerSpecificInputDevices();

      if (this.subscribedKeyboard != null) {
        this.subscribedKeyboard.CharacterEntered -= this.characterEnteredDelegate;
        this.subscribedKeyboard.KeyReleased -= this.keyReleasedDelegate;
        this.subscribedKeyboard.KeyPressed -= this.keyPressedDelegate;
        this.subscribedKeyboard = null;
      }
      if (this.subscribedMouse != null) {
        this.subscribedMouse.MouseWheelRotated -= this.mouseWheelRotatedDelegate;
        this.subscribedMouse.MouseMoved -= this.mouseMovedDelegate;
        this.subscribedMouse.MouseButtonReleased -= this.mouseButtonReleasedDelegate;
        this.subscribedMouse.MouseButtonPressed -= this.mouseButtonPressedDelegate;
        this.subscribedMouse = null;
      }
    }

    /// <summary>Unsubscribes from the events of all player-specific input devices</summary>
    private void unsubscribePlayerSpecificInputDevices() {
      if (this.subscribedChatPad != null) {
        this.subscribedChatPad.CharacterEntered -= this.characterEnteredDelegate;
        this.subscribedChatPad.KeyReleased -= this.keyReleasedDelegate;
        this.subscribedChatPad.KeyPressed -= this.keyPressedDelegate;
        this.subscribedChatPad = null;
      }

      if (this.subscribedGamePad != null) {
        this.subscribedGamePad.ButtonPressed -= this.buttonPressedDelegate;
        this.subscribedGamePad.ButtonReleased -= this.buttonReleasedDelegate;
        this.subscribedGamePad = null;
      }
    }

    /// <summary>Called when a button on the game pad has been released</summary>
    /// <param name="buttons">Button that has been released</param>
    private void buttonReleased(Buttons buttons) {
      this.inputReceiver.InjectButtonRelease(buttons);
    }

    /// <summary>Called when a button on the game pad has been pressed</summary>
    /// <param name="buttons">Button that has been pressed</param>
    private void buttonPressed(Buttons buttons) {
      if ((buttons & Buttons.DPadUp) != 0) {
        this.inputReceiver.InjectCommand(Command.Up);
      } else if ((buttons & Buttons.DPadDown) != 0) {
        this.inputReceiver.InjectCommand(Command.Down);
      } else if ((buttons & Buttons.DPadLeft) != 0) {
        this.inputReceiver.InjectCommand(Command.Left);
      } else if ((buttons & Buttons.DPadRight) != 0) {
        this.inputReceiver.InjectCommand(Command.Right);
      } else {
        this.inputReceiver.InjectButtonPress(buttons);
      }
    }

    /// <summary>Called when the mouse wheel has been rotated</summary>
    /// <param name="ticks">Number of ticks the wheel was rotated</param>
    private void mouseWheelRotated(float ticks) {
      this.inputReceiver.InjectMouseWheel(ticks);
    }

    /// <summary>Called when the mouse cursor has been moved</summary>
    /// <param name="x">New X coordinate of the mouse cursor</param>
    /// <param name="y">New Y coordinate of the mouse cursor</param>
    private void mouseMoved(float x, float y) {
      this.inputReceiver.InjectMouseMove(x, y);
    }

    /// <summary>Called when a mouse button has been released</summary>
    /// <param name="buttons">Mouse button that has been released</param>
    private void mouseButtonReleased(MouseButtons buttons) {
      this.inputReceiver.InjectMouseRelease(buttons);
    }

    /// <summary>Called when a mouse button has been pressed</summary>
    /// <param name="buttons">Mouse button that has been pressed</param>
    private void mouseButtonPressed(MouseButtons buttons) {
      this.inputReceiver.InjectMousePress(buttons);
    }

    /// <summary>Called when a character has been entered on the keyboard</summary>
    /// <param name="character">Character that has been entered</param>
    private void characterEntered(char character) {
      this.inputReceiver.InjectCharacter(character);
    }

    /// <summary>Called when a key has been released</summary>
    /// <param name="key">Key that was released</param>
    private void keyReleased(Keys key) {
      this.inputReceiver.InjectKeyRelease(key);
    }

    /// <summary>Called when a key has been pressed</summary>
    /// <param name="key">Key that was pressed</param>
    private void keyPressed(Keys key) {
      this.inputReceiver.InjectKeyPress(key);
    }

    /// <summary>Retrieves the input service from a service provider</summary>
    /// <param name="serviceProvider">
    ///   Service provider the service is taken from
    /// </param>
    /// <returns>The input service stored in the service provider</returns>
    private static IInputService getInputService(IServiceProvider serviceProvider) {
      var inputService = (IInputService)serviceProvider.GetService(
        typeof(IInputService)
      );
      if (inputService == null) {
        throw new InvalidOperationException(
          "Using the GUI with the DefaultInputCapturer requires the IInputService. " +
          "Please either add the IInputService to Game.Services by using the " +
          "Nuclex.Input.InputManager in your game or provide a custom IInputCapturer " +
          "implementation for the GUI and assign it before GuiManager.Initialize() " +
          "is called."
        );
      }
      return inputService;
    }

    /// <summary>Player index this input capturer is working with</summary>
    private ExtendedPlayerIndex playerIndex;

    /// <summary>Current receiver of input events</summary>
    /// <remarks>
    ///   Always valid. If no input receiver is assigned, this field will be set
    ///   to a dummy receiver.
    /// </remarks>
    private IInputReceiver inputReceiver;

    /// <summary>Input service the capturer is currently subscribed to</summary>
    private IInputService inputService;

    /// <summary>Keyboard the input capturer is subscribed to</summary>
    private IKeyboard subscribedKeyboard;
    /// <summary>Mouse the input capturer is subscribed to</summary>
    private IMouse subscribedMouse;
    /// <summary>Game pad the input capturer is subscribed to</summary>
    private IGamePad subscribedGamePad;
    /// <summary>Chat pad the input capturer is subscribed to</summary>
    private IKeyboard subscribedChatPad;

    /// <summary>Delegate for the keyPressed() method</summary>
    private KeyDelegate keyPressedDelegate;
    /// <summary>Delegate for the keyReleased() method</summary>
    private KeyDelegate keyReleasedDelegate;
    /// <summary>Delegate for the characterEntered() method</summary>
    private CharacterDelegate characterEnteredDelegate;
    /// <summary>Delegate for the mouseButtonPressed() method</summary>
    private MouseButtonDelegate mouseButtonPressedDelegate;
    /// <summary>Delegate for the mouseButtonReleased() method</summary>
    private MouseButtonDelegate mouseButtonReleasedDelegate;
    /// <summary>Delegate for the mouseMoved() method</summary>
    private MouseMoveDelegate mouseMovedDelegate;
    /// <summary>Delegate for the mouseWheelRotated() method</summary>
    private MouseWheelDelegate mouseWheelRotatedDelegate;
    /// <summary>Delegate for the buttonPressed() method</summary>
    private GamePadButtonDelegate buttonPressedDelegate;
    /// <summary>Delegate for the buttonReleased() method</summary>
    private GamePadButtonDelegate buttonReleasedDelegate;

  }

} // namespace Nuclex.UserInterface.Input

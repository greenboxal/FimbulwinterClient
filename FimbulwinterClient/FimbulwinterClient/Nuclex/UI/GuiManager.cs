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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Nuclex.Input;

using GameEventHandler = System.EventHandler<System.EventArgs>;
using Microsoft.Xna.Framework.Content;
using FimbulwinterClient.IO;

namespace Nuclex.UserInterface {

  // TODO: Ownership issue with the GUI visualizer
  //   If an instance creates its own GUI visualizer (because the user didn't assign
  //   a custom one), it belongs to the instance and should be disposed. If the
  //   use does assign a custom visualizer, it shouldn't be disposed.
  //   But what if the user stores the current visualizer, then assigns a different
  //   one and assigns our's back?

  /// <summary>Manages the state of the user interfaces and renders it</summary>
  public class GuiManager :
    IGameComponent, IUpdateable, IDrawable, IDisposable, IGuiService {

    /// <summary>Fired when the DrawOrder property changes</summary>
    public event GameEventHandler DrawOrderChanged;

    /// <summary>Fired when the Visible property changes</summary>
    public event GameEventHandler VisibleChanged;

    /// <summary>Fired when the UpdateOrder property changes</summary>
    public event GameEventHandler UpdateOrderChanged;

    /// <summary>Fired when the enabled property changes, which is never</summary>
    event GameEventHandler IUpdateable.EnabledChanged { add { } remove { } }

    ROContentManager cm;

    /// <summary>
    ///   Initializes a new GUI manager using the XNA service container
    /// </summary>
    /// <param name="gameServices">
    ///   Game service container the GuiManager will register itself in and
    ///   to take the services it consumes from.
    /// </param>
    public GuiManager(GameServiceContainer gameServices, ROContentManager cm)
    {
      this.gameServices = gameServices;

      gameServices.AddService(typeof(IGuiService), this);

      this.cm = cm;
      // Do not look for the consumed services yet. XNA uses a two-stage
      // initialization where in the first stage all managers register themselves
      // before seeking out the services they use in the second stage, marked
      // by a call to the Initialize() method.
    }

    /// <summary>
    ///   Initializes a new GUI manager without using the XNA service container
    /// </summary>
    /// <param name="graphicsDeviceService">
    ///   Graphics device service the GUI will be rendered with
    /// </param>
    /// <param name="inputService">
    ///   Input service used to read data from the input devices
    /// </param>
    /// <remarks>
    ///   This constructor is provided for users of dependency injection frameworks.
    /// </remarks>
    public GuiManager(
      IGraphicsDeviceService graphicsDeviceService,
      IInputService inputService
    ) {
      this.graphicsDeviceService = graphicsDeviceService;
      this.inputService = inputService;
    }

    /// <summary>Initializes a new GUI manager using explicit services</summary>
    /// <param name="gameServices">
    ///   Game service container the GuiManager will register itself in
    /// </param>
    /// <param name="graphicsDeviceService">
    ///   Graphics device service the GUI will be rendered with
    /// </param>
    /// <param name="inputService">
    ///   Input service used to read data from the input devices
    /// </param>
    /// <remarks>
    ///   This constructor is provided for users of dependency injection frameworks
    ///   or if you just want to be more explicit in stating which manager consumes
    ///   what services.
    /// </remarks>
    public GuiManager(
      GameServiceContainer gameServices,
      IGraphicsDeviceService graphicsDeviceService,
      IInputService inputService,
      ROContentManager cm
    ) :
      this(gameServices, cm) {

      this.graphicsDeviceService = graphicsDeviceService;
      this.inputService = inputService;
    }

    /// <summary>Immediately releases all resources used the GUI manager</summary>
    public void Dispose() {

      // Unregister the service if we have registered it before
      if (this.gameServices != null) {
        object registeredService = this.gameServices.GetService(typeof(IGuiService));
        if (ReferenceEquals(registeredService, this)) {
          this.gameServices.RemoveService(typeof(IGuiService));
        }
      }

      // Dispose the input capturer, if necessary
      if (this.inputCapturer != null) {
        IDisposable disposableInputCapturer = this.inputCapturer as IDisposable;
        if (disposableInputCapturer != null) {
          disposableInputCapturer.Dispose();
        }

        this.updateableInputCapturer = null;
        this.inputCapturer = null;
      }

      // Dispose the GUI visualizer, if necessary
      if (this.guiVisualizer != null) {
        IDisposable disposableguiVisualizer = this.guiVisualizer as IDisposable;
        if (disposableguiVisualizer != null) {
          disposableguiVisualizer.Dispose();
        }
        this.updateableGuiVisualizer = null;
        this.guiVisualizer = null;
      }

    }

    /// <summary>Handles second-stage initialization of the GUI manager</summary>
    public void Initialize() {

      // Set up a default input capturer if none was assigned by the user.
      // We only require an IInputService if the user doesn't use a custom input
      // capturer (which could be based on any other input library)
      if (this.inputCapturer == null) {
        if (this.inputService == null) {
          this.inputService = getInputService(this.gameServices);
        }

        this.inputCapturer = new Input.DefaultInputCapturer(this.inputService);

        // If a screen was assigned to the GUI before the input capturer was
        // created, then the input capturer hasn't been given the screen as its
        // input sink yet.
        if (this.screen != null) {
          this.inputCapturer.InputReceiver = this.screen;
        }
      }

      // Set up a default GUI visualizer if none was assigned by the user.
      // We only require an IGraphicsDeviceService if the user doesn't use a
      // custom visualizer (which could be using any kind of rendering)
      if (this.guiVisualizer == null) {
        if (this.graphicsDeviceService == null) {
          this.graphicsDeviceService = getGraphicsDeviceService(this.gameServices);
        }

        // Use a private service container. We know exactly what will be loaded from
        // the content manager our default GUI visualizer creates and if the user is
        // being funny, the graphics device service passed to the constructor might
        // be different from the one registered in the game service container.
        var services = new GameServiceContainer();
        services.AddService(typeof(IGraphicsDeviceService), this.graphicsDeviceService);

        Visualizer = new Visuals.Flat.FlatGuiVisualizer(cm, cm.GetStream("data/fb/skin.xml"));
      }

    }

    /// <summary>GUI that is being rendered</summary>
    /// <remarks>
    ///   The GUI manager renders one GUI full-screen onto the primary render target
    ///   (the backbuffer). This property holds the GUI that is being managed by
    ///   the GUI manager component. You can replace it at any time, for example,
    ///   if the player opens or closes your ingame menu.
    /// </remarks>
    public Screen Screen {
      get { return this.screen; }
      set {
        this.screen = value;

        // Someone could assign the screen before the component is initialized.
        // If that happens, do nothing here, the Initialize() method will take care
        // of assigning the screen to the input capturer once it is called.
        if (this.inputCapturer != null) {
          this.inputCapturer.InputReceiver = this.screen;
        }
      }
    }

    /// <summary>Input capturer that collects data from the input devices</summary>
    /// <remarks>
    ///   The GuiManager will dispose its input capturer together with itself. If you
    ///   want to keep the input capturer, unset it before disposing the GuiManager.
    ///   If you want to replace the GuiManager's input capturer after it has constructed
    ///   the default one, you should dispose the GuiManager's default input capturer
    ///   after assigning your own.
    /// </remarks>
    public Input.IInputCapturer InputCapturer {
      get { return this.inputCapturer; }
      set {
        if (!ReferenceEquals(value, this.inputCapturer)) {
          if (this.inputCapturer != null) {
            this.inputCapturer.InputReceiver = null;
          }

          this.inputCapturer = value;
          this.updateableInputCapturer = value as IUpdateable;

          if (this.inputCapturer != null) {
            this.inputCapturer.InputReceiver = this.screen;
          }
        }
      }
    }

    /// <summary>Visualizer that draws the GUI onto the screen</summary>
    /// <remarks>
    ///   The GuiManager will dispose its visualizer together with itself. If you want
    ///   to keep the visualizer, unset it before disposing the GuiManager. If you want
    ///   to replace the GuiManager's visualizer after it has constructed the default
    ///   one, you should dispose the GuiManager's default visualizer after assigning
    ///   your own.
    /// </remarks>
    public Visuals.IGuiVisualizer Visualizer {
      get { return this.guiVisualizer; }
      set {
        this.guiVisualizer = value;
        this.updateableGuiVisualizer = value as IUpdateable;
      }
    }

    /// <summary>Called when the component needs to update its state.</summary>
    /// <param name="gameTime">Provides a snapshot of the Game's timing values</param>
    public void Update(GameTime gameTime) {
      if (this.updateableInputCapturer != null) {
        this.updateableInputCapturer.Update(gameTime);
      }
      if (this.updateableGuiVisualizer != null) {
        this.updateableGuiVisualizer.Update(gameTime);
      }
    }

    /// <summary>Called when the drawable component needs to draw itself.</summary>
    /// <param name="gameTime">Provides a snapshot of the game's timing values</param>
    public void Draw(GameTime gameTime) {
      if (this.guiVisualizer != null) {
        if (this.screen != null) {
          this.guiVisualizer.Draw(this.screen);
        }
      }
    }

    /// <summary>
    ///   Indicates when the game component should be updated relative to other game
    ///   components. Lower values are updated first.
    /// </summary>
    public int UpdateOrder {
      get { return this.updateOrder; }
      set {
        if (value != this.updateOrder) {
          this.updateOrder = value;
          OnUpdateOrderChanged();
        }
      }
    }

    /// <summary>
    ///   The order in which to draw this object relative to other objects. Objects
    ///   with a lower value are drawn first.
    /// </summary>
    public int DrawOrder {
      get { return this.drawOrder; }
      set {
        if (value != this.drawOrder) {
          this.drawOrder = value;
          OnDrawOrderChanged();
        }
      }
    }

    /// <summary>Whether the GUI should be drawn during Game.Draw()</summary>
    public bool Visible {
      get { return this.visible; }
      set {
        if (value != this.visible) {
          this.visible = value;
          OnVisibleChanged();
        }
      }
    }

    /// <summary>Fires the UpdateOrderChanged event</summary>
    protected void OnUpdateOrderChanged() {
      if (UpdateOrderChanged != null) {
        UpdateOrderChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>Fires the DrawOrderChanged event</summary>
    protected void OnDrawOrderChanged() {
      if (DrawOrderChanged != null) {
        DrawOrderChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>Fires the VisibleChanged event</summary>
    protected void OnVisibleChanged() {
      if (VisibleChanged != null) {
        VisibleChanged(this, EventArgs.Empty);
      }
    }

    /// <summary>Whether the component should be updated during Game.Update()</summary>
    bool IUpdateable.Enabled {
      get { return true; }
    }

    /// <summary>Retrieves the input service from a service provider</summary>
    /// <param name="serviceProvider">
    ///   Service provider the input service is retrieved from
    /// </param>
    /// <returns>The retrieved input service</returns>
    private static IInputService getInputService(IServiceProvider serviceProvider) {
      var inputService = (IInputService)serviceProvider.GetService(
        typeof(IInputService)
      );
      if (inputService == null) {
        throw new InvalidOperationException(
          "Using the GUI with the default input capturer requires the IInputService. " +
          "Please either add the IInputService to Game.Services by using the " +
          "Nuclex.Input.InputManager in your game or provide a custom IInputCapturer " +
          "implementation for the GUI and assign it before GuiManager.Initialize() " +
          "is called."
        );
      }
      return inputService;
    }

    /// <summary>Retrieves the graphics device service from a service provider</summary>
    /// <param name="serviceProvider">
    ///   Service provider the graphics device service is retrieved from
    /// </param>
    /// <returns>The retrieved graphics device service</returns>
    private static IGraphicsDeviceService getGraphicsDeviceService(
      IServiceProvider serviceProvider
    ) {
      var graphicsDeviceService = (IGraphicsDeviceService)serviceProvider.GetService(
        typeof(IGraphicsDeviceService)
      );
      if (graphicsDeviceService == null) {
        throw new InvalidOperationException(
          "Using the GUI with the default visualizer requires the IGraphicsDeviceService. " +
          "Please either add an IGraphicsDeviceService to Game.Services by using " +
          "XNA's GraphicsDeviceManager in your game or provide a custom " +
          "IGuiVisualizer implementation for the GUI and assign it before " +
          "GuiManager.Initialize() is called."
        );
      }

      return graphicsDeviceService;
    }

    /// <summary>Game service container the GUI has registered itself in</summary>
    private GameServiceContainer gameServices;
    /// <summary>Graphics device servide the GUI uses</summary>
    private IGraphicsDeviceService graphicsDeviceService;
    /// <summary>Input service the GUI uses</summary>
    private IInputService inputService;

    /// <summary>Update order rank relative to other game components</summary>
    private int updateOrder;
    /// <summary>Draw order rank relative to other game components</summary>
    private int drawOrder;
    /// <summary>Whether the GUI should be drawn by Game.Draw()</summary>
    private bool visible = true;

    /// <summary>Captures user input for the XNA game</summary>
    private Input.IInputCapturer inputCapturer;
    /// <summary>
    ///   The IInputCapturer under its IUpdateable interface, if implemented
    /// </summary>
    private IUpdateable updateableInputCapturer;
    /// <summary>Draws the GUI</summary>
    private Visuals.IGuiVisualizer guiVisualizer;
    /// <summary>
    ///   The IGuiVisualizer under its IUpdateable interface, if implemented
    /// </summary>
    private IUpdateable updateableGuiVisualizer;

    /// <summary>The GUI screen representing the desktop</summary>
    private Screen screen;

  }

} // namespace Nuclex.UserInterface

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

#if WINDOWS

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework.Input;

using Nuclex.Input.Devices;

using Application = System.Windows.Forms.Application;
using IMessageFilter = System.Windows.Forms.IMessageFilter;
using Message = System.Windows.Forms.Message;

namespace Nuclex.Input {

  /// <summary>Filters window message before they arrive at the XNA window</summary>
  internal class WindowMessageFilter :
    IMessageFilter, IKeyboardMessageSource, IMouseMessageSource, IDisposable {

    /// <summary>Flags that will be added to the result of WM_GETDLGCODE</summary>
    private const int DlgCodeFlags =
      (UnsafeNativeMethods.DLGC_WANTALLKEYS | UnsafeNativeMethods.DLGC_WANTCHARS);

    /// <summary>Triggered when a key has been pressed down</summary>
    public event KeyboardKeyEventDelegate KeyPressed;

    /// <summary>Triggered when a key has been released again</summary>
    public event KeyboardKeyEventDelegate KeyReleased;

    /// <summary>Triggered when the user has entered a character</summary>
    public event KeyboardCharacterEventDelegate CharacterEntered;

    /// <summary>Triggered when a mouse button has been pressed</summary>
    public event MouseButtonEventDelegate MouseButtonPressed;

    /// <summary>Triggered when a mouse button has been released</summary>
    public event MouseButtonEventDelegate MouseButtonReleased;

    /// <summary>Triggered when the mouse has been moved</summary>
    public event MouseMoveEventDelegate MouseMoved;

    /// <summary>Triggered when the mouse wheel has been rotated</summary>
    public event MouseWheelEventDelegate MouseWheelRotated;

    /// <summary>Initializs the window message filter</summary>
    /// <param name="windowHandle">Window handle of XNA's game window</param>
    public WindowMessageFilter(IntPtr windowHandle) {

      // Set up the information structure for TrackMouseEvent()
      this.mouseEventTrackData = new UnsafeNativeMethods.TRACKMOUSEEVENT();
      this.mouseEventTrackData.structureSize = Marshal.SizeOf(this.mouseEventTrackData);
      this.mouseEventTrackData.flags = UnsafeNativeMethods.TME_LEAVE;
      this.mouseEventTrackData.trackWindowHandle = windowHandle;

      // Attach the message filter to the thread's message loop
      Application.AddMessageFilter(this);

    }

    /// <summary>Immediately releases all resources owned by the instance</summary>
    public void Dispose() {
      if (!this.disposed) {
        Application.RemoveMessageFilter(this);
        this.disposed = true;
      }
    }

    /// <summary>Filters out a message before it is dispatched</summary>
    /// <param name="message">
    ///   Message that will be dispatched. You must not modify this message.
    /// </param>
    /// <returns>
    ///   True if the message has been processed by the filter and should not be
    ///   dispatched or false to continue processing of the message.
    /// </returns>
    bool IMessageFilter.PreFilterMessage(ref Message message) {

      // Process the message differently based on its message id
      switch (message.Msg) {

        case (int)UnsafeNativeMethods.WindowMessages.WM_SYSKEYDOWN: {
          int virtualKeyCode = message.WParam.ToInt32();

          // Don't handle these Alt+key presses because they'd block Windows' shortcuts
          bool dontHandle =
            (virtualKeyCode == (int)Keys.F4) ||
            (virtualKeyCode == (int)Keys.Escape) ||
            (virtualKeyCode == (int)Keys.Space);

          if(dontHandle) {
            break;
          }

          goto case (int)UnsafeNativeMethods.WindowMessages.WM_KEYDOWN;
        }

        // Key on the keyboard was pressed
        case (int)UnsafeNativeMethods.WindowMessages.WM_KEYDOWN: {
          int virtualKeyCode = message.WParam.ToInt32();
          switch (virtualKeyCode) {
#if false // XNA doesn't want to distinguish Return and Enter
            case 0x0D: { // VK_ENTER
              bool extended = (message.LParam.ToInt32() & 0x01000000) != 0;
              if (extended) {
                OnKeyPressed(Keys.Enter);
              } else {
                OnKeyPressed(Keys.??);
              }

              break;
            }
#endif
            case 0x10: { // VK_SHIFT
              OnKeyPressed(
                (Keys)UnsafeNativeMethods.MapVirtualKey(
                  (uint)(message.LParam.ToInt32() & 0x00FF0000) >> 16,
                  (uint)UnsafeNativeMethods.MapType.ScanCodeToVirtualKeyEx
                )
              );
              break;
            }
            case 0x11: { // VK_CONTROL
              bool extended = (message.LParam.ToInt32() & 0x01000000) != 0;
              if (extended) {
                OnKeyPressed(Keys.RightControl);
              } else {
                OnKeyPressed(Keys.LeftControl);
              }

              break;
            }
            case 0x12: { // VK_MENU
              bool extended = (message.LParam.ToInt32() & 0x01000000) != 0;
              if (extended) {
                OnKeyPressed(Keys.RightAlt);
              } else {
                OnKeyPressed(Keys.LeftAlt);
              }
              break;
            }
            default: {
              OnKeyPressed((Keys)virtualKeyCode);
              break;
            }
          }

          UnsafeNativeMethods.TranslateMessage(ref message);

          return true; // consumed!
        }

        // Key on the keyboard was released
        case (int)UnsafeNativeMethods.WindowMessages.WM_SYSKEYUP:
        case (int)UnsafeNativeMethods.WindowMessages.WM_KEYUP: {
          int virtualKeyCode = message.WParam.ToInt32();
          switch (virtualKeyCode) {
#if false // We could, but XNA doesn't want to distinguish Return and Enter
            case 0x0D: { // VK_ENTER
              bool extended = (message.LParam.ToInt32() & 0x01000000) != 0;
              if (extended) {
                OnKeyReleased(Keys.Enter);
              } else {
                OnKeyReleased(Keys.??);
              }

              break;
            }
#endif
            case 0x10: { // VK_SHIFT
              OnKeyReleased(
                (Keys)UnsafeNativeMethods.MapVirtualKey(
                  (uint)(message.LParam.ToInt32() & 0x00FF0000) >> 16,
                  (uint)UnsafeNativeMethods.MapType.ScanCodeToVirtualKeyEx
                )
              );
              break;
            }
            case 0x11: { // VK_CONTROL
              bool isExtendedKey = (message.LParam.ToInt32() & (1 << 24)) != 0;
              if (isExtendedKey) {
                OnKeyReleased(Keys.RightControl);
              } else {
                OnKeyReleased(Keys.LeftControl);
              }

              break;
            }
            case 0x12: { // VK_MENU
              bool isExtendedKey = (message.LParam.ToInt32() & (1 << 24)) != 0;
              if (isExtendedKey) {
                OnKeyReleased(Keys.RightAlt);
              } else {
                OnKeyReleased(Keys.LeftAlt);
              }

              break;
            }
            default: {
              OnKeyReleased((Keys)virtualKeyCode);
              break;
            }
          }

          return true; // consumed!
        }

        // Character has been entered on the keyboard
        case (int)UnsafeNativeMethods.WindowMessages.WM_CHAR: {
          char character = (char)message.WParam.ToInt32();
          OnCharacterEntered(character);
          return true; // consumed!
        }


        // Mouse has been moved
        case (int)UnsafeNativeMethods.WindowMessages.WM_MOUSEMOVE: {
          if (!this.trackingMouse) {
            int result = UnsafeNativeMethods.TrackMouseEvent(ref this.mouseEventTrackData);
            Debug.Assert(
              result != 0,
              "Could not set up registration for mouse events",
              "The TrackMouseEvent() function failed, which means the game will not " +
              "detect when the mouse leaves the game window. This might result in " +
              "the assumed mouse position remaining somewhere near the window border " +
              "even though the mouse has been moved away from the game window."
            );
            this.trackingMouse = (result != 0);
          }

          short x = (short)(message.LParam.ToInt32() & 0xFFFF);
          short y = (short)(message.LParam.ToInt32() >> 16);
          OnMouseMoved((float)x, (float)y);
          break;
        }

        // Left mouse button pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_LBUTTONDOWN:
        case (int)UnsafeNativeMethods.WindowMessages.WM_LBUTTONDBLCLK: {
          OnMouseButtonPressed(MouseButtons.Left);
          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_LBUTTONUP: {
          OnMouseButtonReleased(MouseButtons.Left);
          break;
        }

        // Right mouse button pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_RBUTTONDOWN:
        case (int)UnsafeNativeMethods.WindowMessages.WM_RBUTTONDBLCLK: {
          OnMouseButtonPressed(MouseButtons.Right);
          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_RBUTTONUP: {
          OnMouseButtonReleased(MouseButtons.Right);
          break;
        }

        // Middle mouse button pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_MBUTTONDOWN:
        case (int)UnsafeNativeMethods.WindowMessages.WM_MBUTTONDBLCLK: {
          OnMouseButtonPressed(MouseButtons.Middle);
          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_MBUTTONUP: {
          OnMouseButtonReleased(MouseButtons.Middle);
          break;
        }

        // Extended mouse button pressed / released
        case (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONDOWN:
        case (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONDBLCLK: {
          short button = (short)(message.WParam.ToInt32() >> 16);
          if (button == 1)
            OnMouseButtonPressed(MouseButtons.X1);
          if (button == 2)
            OnMouseButtonPressed(MouseButtons.X2);

          break;
        }
        case (int)UnsafeNativeMethods.WindowMessages.WM_XBUTTONUP: {
          short button = (short)(message.WParam.ToInt32() >> 16);
          if (button == 1)
            OnMouseButtonReleased(MouseButtons.X1);
          if (button == 2)
            OnMouseButtonReleased(MouseButtons.X2);

          break;
        }

        // Mouse wheel rotated
        case (int)UnsafeNativeMethods.WindowMessages.WM_MOUSEHWHEEL: {
          short ticks = (short)(message.WParam.ToInt32() >> 16);
          OnMouseWheelRotated((float)ticks / 120.0f);
          break;
        }

        // Mouse has left the window's client area
        case (int)UnsafeNativeMethods.WindowMessages.WM_MOUSELEAVE: {
          OnMouseMoved(-1.0f, -1.0f);
          this.trackingMouse = false;
          break;
        }

      }

      return false;
    }

    /// <summary>Fires the KeyPressed event</summary>
    /// <param name="key">Key that has been pressed</param>
    protected void OnKeyPressed(Keys key) {
      if (KeyPressed != null) {
        KeyPressed(key);
      }
    }

    /// <summary>Fires the KeyReleased event</summary>
    /// <param name="key">Key that has been released</param>
    protected void OnKeyReleased(Keys key) {
      if (KeyReleased != null) {
        KeyReleased(key);
      }
    }

    /// <summary>Fires the CharacterEntered event</summary>
    /// <param name="character">Character that has been entered</param>
    protected void OnCharacterEntered(char character) {
      if (CharacterEntered != null) {
        CharacterEntered(character);
      }
    }

    /// <summary>Fires the MouseButtonPressed event</summary>
    /// <param name="buttons">Mouse buttons that have been pressed</param>
    protected void OnMouseButtonPressed(MouseButtons buttons) {
      if (MouseButtonPressed != null) {
        MouseButtonPressed(buttons);
      }
    }

    /// <summary>Fires the MouseButtonReleased event</summary>
    /// <param name="buttons">Mouse buttons that have been released</param>
    protected void OnMouseButtonReleased(MouseButtons buttons) {
      if (MouseButtonReleased != null) {
        MouseButtonReleased(buttons);
      }
    }

    /// <summary>Fires the MouseMoved event</summary>
    /// <param name="x">New X coordinate of the mouse</param>
    /// <param name="y">New Y coordinate of the mouse</param>
    protected void OnMouseMoved(float x, float y) {
      if (MouseMoved != null) {
        MouseMoved(x, y);
      }
    }

    /// <summary>Fires the MouseWheelRotated event</summary>
    /// <param name="ticks">Number of ticks the mouse wheel has been rotated</param>
    protected void OnMouseWheelRotated(float ticks) {
      if (MouseWheelRotated != null) {
        MouseWheelRotated(ticks);
      }
    }

    /// <summary>
    ///   Provides Informations about how the mouse cusor should be tracked on
    ///   the window to the TrackMouseEvent() function.
    /// </summary>
    private UnsafeNativeMethods.TRACKMOUSEEVENT mouseEventTrackData;

    /// <summary>True when the mouse cursor is currently being tracked</summary>
    private bool trackingMouse;

    /// <summary>True when the object has been disposed</summary>
    private bool disposed;

    /// <summary>Whether the alt key is current being held down</summary>
    private bool altKeyDown;
    /// <summary>Whether the control key is current being held down</summary>
    private bool ctrlKeyDown;

  }

} // namespace Nuclex.Input

#endif // WINDOWS
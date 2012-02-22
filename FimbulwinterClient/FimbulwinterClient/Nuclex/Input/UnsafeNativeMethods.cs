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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Nuclex.Input {

  /// <summary>Provides access to some P/Invoke-imported native methods</summary>
  internal static class UnsafeNativeMethods {

    /// <summary>
    ///   Flag returned by a window in response to WM_GETDLGCODE to indicate that it
    ///   is interested in receiving WM_CHAR messages
    /// </summary>
    public const int DLGC_WANTCHARS = 0x0080;

    /// <summary>
    ///   Flag returned by a window in response to WM_GETDLGCODE to indicate that it
    ///   wants to process all keyboard input from the user
    /// </summary>
    public const int DLGC_WANTALLKEYS = 0x0004;

    /// <summary>
    ///   The caller wants leave notification. Notification is delivered as
    ///   a WM_MOUSELEAVE message.
    /// </summary>
    public const int TME_LEAVE = 0x00000002;

    /// <summary>
    ///   Bit in lparam that indicates whether a WM_KEYDOWN is being generated due
    ///   to the keyboard's auto-repeat
    /// </summary>
    public const int WM_KEYDOWN_WASDOWN = (1 << 30);

    /// <summary>Category for generic HID devices</summary>
    public const ushort HID_USAGE_PAGE_GENERIC = 0x01;
    /// <summary>Generic HID mouse device</summary>
    public const ushort HID_USAGE_GENERIC_MOUSE = 0x02;

    /// <summary>Command ID for retrieving input data</summary>
    public const int RID_INPUT = 0x10000003;

    /// <summary>
    ///   Enables the caller to receive the input even when the caller is not in
    ///   the foreground. Note that WindowHandle must be specified.
    /// </summary>
    public const int RIDEV_INPUTSINK = 0x00000100;
    /// <summary>Mouse button click does not activate the other window</summary>
    public const int RIDEV_CAPTUREMOUSE = 0x00000200;

    /// <summary>Prevents any devices from generating legacy messages</summary>
    public const int RIDEV_NOLEGACY = 0x00000030;

    /// <summary>Indicates that the retrieve raw input came from the mouse</summary>
    public const int RIM_TYPEMOUSE = 0;

    /// <summary>Mouse movement is specified as offset to the last position</summary>
    public const int MOUSE_MOVE_RELATIVE = 0;
    /// <summary>Mouse movement is specified in absolute coordinates</summary>
    public const int MOUSE_MOVE_ABSOLUTE = 1;

    #region enum MapTypes

    /// <summary>Translation to perform</summary>
    public enum MapType : uint {
      /// <summary>
      ///   Virtual-key code is translated into a scan code. Does not distinguish
      ///   between left- and right-hand keys.
      /// </summary>
      VirtualKeyToScanCode = 0,
      /// <summary>
      ///   Scan code is translated into a virtual-key code that does not distinguish
      ///   between left- and right-hand keys.
      /// </summary>
      ScanCodeToVirtualKey = 1,
      /// <summary>
      ///   Virtual-key code is translated into an unshifted character value
      /// </summary>
      VirtualKeyToChar = 2,
      /// <summary>
      ///   Scan code is translated into a virtual-key code that distinguishes between
      ///   left- and right-hand keys.
      /// </summary>
      ScanCodeToVirtualKeyEx = 3,
      /// <summary>
      ///   Virtual-key code is translated into a scan code. Distinguishes between
      ///   left- and right-hand keys.
      /// </summary>
      VirtualKeytoScanCodeEx = 4
    }

    #endregion // enum MapTypes

    #region enum WindowMessages

    /// <summary>List of window message relevant to the input capturer</summary>
    public enum WindowMessages : int {

      /// <summary>Sent to a window to ask which types of input it processes</summary>
      WM_GETDLGCODE = 0x0087,
      /// <summary>Transmits raw input data to the window</summary>
      WM_INPUT = 0x00FF,
      /// <summary>Indicates that the user has pressed a key on the keyboard</summary>
      WM_KEYDOWN = 0x0100,
      /// <summary>Indicates that the user has released a key on the keyboard</summary>
      WM_KEYUP = 0x0101,
      /// <summary>Indicates that the user has entered text</summary>
      WM_CHAR = 0x0102,
      /// <summary>Indicates that the user has pressed a system key</summary>
      /// <remarks>
      ///   Posted to the window with the keyboard focus when the user presses the F10 key
      ///   (which activates the menu bar) or holds down the ALT key and then presses
      ///   another key.
      /// </remarks>
      WM_SYSKEYDOWN = 0x104,
      /// <summary>Indicates that the user has released a system key</summary>
      WM_SYSKEYUP = 0x105,
      /// <summary>Indicates that the user has entered text (UTF-32 variant)</summary>
      /// <remarks>
      ///   This is only required if the window is an ANSI window (created by
      ///   CreateWindowA() and not reset to unicode). In this case, windows will
      ///   send WM_CHAR with ANSI characters and WM_UNICHAR with UTF-32 characters.
      /// </remarks>
      WM_UNICHAR = 0x0109,
      /// <summary>Indicates that the mouse cursor has been moved</summary>
      WM_MOUSEMOVE = 0x0200,
      /// <summary>Indicates that the left mouse button was pressed down</summary>
      WM_LBUTTONDOWN = 0x0201,
      /// <summary>Indicates that the left mouse button was released again</summary>
      WM_LBUTTONUP = 0x0202,
      /// <summary>Indicates that the left mouse button was double-clicked</summary>
      WM_LBUTTONDBLCLK = 0x0203,
      /// <summary>Indicates that the right mouse button was pressed down</summary>
      WM_RBUTTONDOWN = 0x0204,
      /// <summary>Indicates that the right mouse button was released again</summary>
      WM_RBUTTONUP = 0x0205,
      /// <summary>Indicates that the right mouse button was double-clicked</summary>
      WM_RBUTTONDBLCLK = 0x0206,
      /// <summary>Indicates that the middle mouse button was pressed down</summary>
      WM_MBUTTONDOWN = 0x0207,
      /// <summary>Indicates that the middle mouse button was released again</summary>
      WM_MBUTTONUP = 0x0208,
      /// <summary>Indicates that the middle mouse button was double-clicked</summary>
      WM_MBUTTONDBLCLK = 0x0209,
      /// <summary>Indicates that the mouse wheel has been rotated</summary>
      WM_MOUSEHWHEEL = 0x020A,
      /// <summary>Indicates that an extended mouse button was pressed down</summary>
      WM_XBUTTONDOWN = 0x020B,
      /// <summary>Indicates that an extended mouse button was released again</summary>
      WM_XBUTTONUP = 0x020C,
      /// <summary>Indicates that an extended mouse button was double-clicked</summary>
      WM_XBUTTONDBLCLK = 0x020D,
      /// <summary>Indicates that the mouse wheel has been rotated or tilted</summary>
      /// <remarks>
      ///   This window message is only supported by Windows Vista. Mouse drivers may,
      ///   however, emulate it on Windows XP by directly communicating with
      ///   the low-level driver and injecting this message into the active window.
      /// </remarks>
      WM_MOUSEHWHEEL_TILT = 0x020E,
      /// <summary>Sent to the window when the mouse cursor has left it</summary>
      /// <remarks>
      ///   To receive this message, the window has to set up the notification using
      ///   the TrackMouseEvent() function when the mouse enters the window.
      /// </remarks>
      WM_MOUSELEAVE = 0x02A3

    }

    #endregion // enum WindowMessages

    #region struct RAWINPUTDEVICE

    /// <summary>Identifies a raw input device</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTDEVICE {

      /// <summary>Top level collection Usage page for the raw input device</summary>
      public ushort UsagePage;
      /// <summary>Top level collection Usage for the raw input device</summary>
      public ushort Usage;
      /// <summary>
      ///   Mode flag that specifies how to interpret the information provided
      ///   by UsagePage and Usage
      /// </summary>
      public int Flags;
      /// <summary>
      ///   Handle to the target window. If NULL it follows the keyboard focus
      /// </summary>
      public IntPtr WindowHandle;

    }

    #endregion // struct RAWINPUTDEVICE

    #region struct TRACKMOUSEEVENT

    /// <summary>
    ///   Used by the TrackMouseEvent function to track when the mouse pointer leaves
    ///   a window or hovers over a window
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TRACKMOUSEEVENT {

      /// <summary>The size of the TRACKMOUSEEVENT structure</summary>
      public Int32 structureSize;
      /// <summary>Specifies the services requested</summary>
      public Int32 flags;
      /// <summary>Specifies a handle to the window to track</summary>
      public IntPtr trackWindowHandle;
      /// <summary>
      ///   Specifies the hover time-out (if TME_HOVER was specified in dwFlags),
      ///   in milliseconds
      /// </summary>
      public Int32 hoverTime;

    }

    #endregion // struct TRACKMOUSEEVENT

    #region struct RAWINPUTHEADER

    /// <summary>Header information that is part of the raw input data</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTHEADER {

      /// <summary>Type of device the input is coming from.</summary>
      public int Type;
      /// <summary>Size of the packet of data.</summary>
      public int Size;
      /// <summary>Handle to the device sending the data.</summary>
      public IntPtr Device;
      /// <summary>wParam from the window message.</summary>
      public IntPtr wParam;

    }

    #endregion // sturct RAWINPUTHEADER

    #region struct RAWMOUSE

    /// <summary>
    /// Value type for raw input from a mouse.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWMOUSE {

      /// <summary>Specifies what kind of data is being transmitted</summary>
      public ushort Flags;
      /// <summary>
      ///   If the mouse wheel is rotated, this will contain the delta amount
      /// </summary>
      public ushort ButtonData;
      /// <summary>State transition of the mouse buttons</summary>
      public ushort ButtonFlags;
      /// <summary>Raw state of the mouse buttons</summary>
      public uint RawButtons;
      /// <summary>Position or offset in X direction, depending on the flags</summary>
      public int LastX;
      /// <summary>Position or offset in Y direction, depending on the flags</summary>
      public int LastY;
      /// <summary>Additional device-specific informations about the event</summary>
      public uint ExtraInformation;

    }

    #endregion // struct RAWMOUSE

    #region struct RAWHID

    /// <summary>
    ///   Describes the format of the raw input from a Human Interface Device
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWHID {

      /// <summary>Size, in bytes, of the HID data</summary>
      public int Size;
      /// <summary>Number of HID inputs contained in the data</summary>
      public int Count;
      /// <summary>Raw input data as an array of bytes</summary>
      public IntPtr Data;

    }

    #endregion // struct RAWHID

    #region stuct RAWINPUT

    /// <summary>Contains the raw input from a device</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUT {

      /// <summary>Describes the data contained in the event</summary>
      public RAWINPUTHEADER Header;
      /// <summary>Contains mouse-specific data if this is a mouse event</summary>
      public RAWMOUSE Mouse;
      /// <summary>Contains generic input data if this is an HID</summary>
      public RAWHID HID;

    }

    #endregion // struct RAWINPUT

    /// <summary>Registers a raw input device</summary>
    /// <param name="rawInputDevice">Array of raw input devices</param>
    /// <param name="deviceCount">Number of devices</param>
    /// <param name="size">Size of the RAWINPUTDEVICE structure</param>
    /// <returns>TRUE if successful, FALSE if not</returns>
    [DllImport("user32")]
    public static extern bool RegisterRawInputDevices(
      //[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
      //RAWINPUTDEVICE[] rawInputDevices,
      ref RAWINPUTDEVICE rawInputDevice,
      int deviceCount,
      int size
    );

    /// <summary>Retrieves the raw input from the specified device</summary>
    /// <param name="rawInputHandle">
    ///   Handle to the raw input structure (provided by WM_INPUT)
    /// </param>
    /// <param name="command">Command flag indicating what will be retrieved</param>
    /// <param name="data">Receives the data that comes from the RAWINPUT structure</param>
    /// <param name="size">
    ///   Size of the data buffer, receives amount of data retrieved
    /// </param>
    /// <param name="headerSize">Size, in bytes, of the RAWINPUTHEADER structure</param>
    /// <returns>
    ///   If pData is NULL and the function is successful, the return value is 0.
    ///   If pData is not NULL and the function is successful, the return value is
    ///   the number of bytes copied into pData. -1 on error.
    /// </returns>
    [DllImport("user32")]
    public static extern int GetRawInputData(
      IntPtr rawInputHandle,
      int command,
      out RAWINPUT data,
      ref int size,
      int headerSize
    );

    /// <summary>
    ///   Posts messages when the mouse pointer leaves a window or hovers over a window
    ///   for a specified amount of time
    /// </summary>
    /// <param name="eventTrack">
    ///   Pointer to a TRACKMOUSEEVENT structure that contains tracking information
    /// </param>
    /// <returns>If the function succeeds, the return value is nonzero</returns>
    [DllImport("user32")]
    public static extern int TrackMouseEvent(ref TRACKMOUSEEVENT eventTrack);

    /// <summary>
    ///   Translates (maps) a virtual-key code into a scan code or character value,
    ///   or translates a scan code into a virtual-key code
    /// </summary>
    /// <param name="code">Virtual-key code or scan code for a key</param>
    /// <param name="mapType">Translation to perform</param>
    /// <returns>
    ///   A scan code, virtual key code or character value depending on
    ///   the translation peformed. Zero if no translation could be performed.
    /// </returns>
    [DllImport("user32")]
    public static extern uint MapVirtualKey(uint code, uint mapType);

    /// <summary>
    ///   Places (posts) a message in the message queue associated with the thread that
    ///   created the specified window and returns without waiting for the thread to
    ///   process the message.
    /// </summary>
    /// <param name="windowHandle">
    ///   Handle to the window whose window procedure is to receive the message
    /// </param>
    /// <param name="messageId">Specifies the message to be posted</param>
    /// <param name="wParam">Specifies additional message-specific information</param>
    /// <param name="lParam">Specifies additional message-specific information</param>
    /// <returns></returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostMessage(
      IntPtr windowHandle, uint messageId, IntPtr wParam, IntPtr lParam
    );

    /// <summary>Sends the specified message to a window or windows</summary>
    /// <param name="windowHandle">
    ///   Handle to the window whose window procedure will receive the message
    /// </param>
    /// <param name="messageId">Specifies the message to be sent</param>
    /// <param name="wParam">Specifies additional message-specific information</param>
    /// <param name="lParam">Specifies additional message-specific information</param>
    /// <returns>Depends on the message being sent</returns>
    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(
      IntPtr windowHandle, uint messageId, IntPtr wParam, IntPtr lParam
    );

    /// <summary>Posts a message to the message queue of the specified thread</summary>
    /// <param name="threadId">
    ///   Identifier of the thread to which the message is to be posted
    /// </param>
    /// <param name="messageId">Type of message that will be posted</param>
    /// <param name="wParam">Additinal message-specified information</param>
    /// <param name="lParam">Additinal message-specified information</param>
    /// <returns>If the function succeeds, the return value is nonzero</returns>
    [DllImport("user32", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PostThreadMessage(
      int threadId, uint messageId, IntPtr wParam, IntPtr lParam
    );

    /// <summary>
    ///   Loads the specified module into the address space of the calling process
    /// </summary>
    /// <param name="filename">Name of the module that will be loaded</param>
    /// <returns>
    ///   If successful, the handle of the loaded module, otherwise NULL
    /// </returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string filename);

    /// <summary>
    ///   Frees the loaded dynamic-link library (DLL) module and, if necessary,
    ///   decrements its reference count
    /// </summary>
    /// <param name="moduleHandle">A handle to the loaded library module</param>
    /// <returns>Any non-zero value on success, otherwise zero</returns>
    [DllImport("kernel32", SetLastError = true)]
    public static extern bool FreeLibrary(IntPtr moduleHandle);

    /// <summary>Translates virtual-key messages into character messages</summary>
    /// <param name="message">Keyboard Message that will be translated</param>
    /// <returns>
    ///   True if the message has been translated into a character message that
    ///   has been posted to the thread's message queue
    /// </returns>
    /// <remarks>
    ///   The character messages are posted to the calling thread's message queue,
    ///   to be read the next time the thread calls the GetMessage() or
    ///   PeekMessage() function.
    /// </remarks>
    [DllImport("user32", SetLastError = true)]
    public extern static bool TranslateMessage(ref Message message);

  }

} // namespace Nuclex.Input

#endif // WINDOWS

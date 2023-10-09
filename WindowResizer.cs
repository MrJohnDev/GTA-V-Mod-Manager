// Decompiled with JetBrains decompiler
// Type: WindowResizer
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

internal class WindowResizer
{
  private const int WM_SYSCOMMAND = 274;
  private HwndSource hwndSource;
  private Window activeWin;
  private IntPtr retInt = IntPtr.Zero;

  public WindowResizer(Window activeW)
  {
    this.activeWin = activeW;
    this.activeWin.SourceInitialized += new EventHandler(this.InitializeWindowSource);
  }

  public void resetCursor()
  {
    if (Mouse.LeftButton == MouseButtonState.Pressed)
      return;
    this.activeWin.Cursor = Cursors.Arrow;
  }

  public void dragWindow() => this.activeWin.DragMove();

  private void InitializeWindowSource(object sender, EventArgs e)
  {
    this.hwndSource = PresentationSource.FromVisual((Visual) sender) as HwndSource;
    this.hwndSource.AddHook(new HwndSourceHook(this.WndProc));
  }

  private IntPtr WndProc(
    IntPtr hwnd,
    int msg,
    IntPtr wParam,
    IntPtr lParam,
    ref bool handled)
  {
    //Debug.WriteLine("WndProc messages: " + msg.ToString());
    //if (msg == 274)
      //Debug.WriteLine("WndProc messages: " + msg.ToString());
    return IntPtr.Zero;
  }

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  private static extern IntPtr SendMessage(
    IntPtr hWnd,
    uint Msg,
    IntPtr wParam,
    IntPtr lParam);

  private void ResizeWindow(WindowResizer.ResizeDirection direction) => WindowResizer.SendMessage(this.hwndSource.Handle, 274U, (IntPtr) (long) (61440 + direction), IntPtr.Zero);

  public void resizeWindow(object sender)
  {
    switch ((sender as Rectangle).Name)
    {
      case "top":
        this.activeWin.Cursor = Cursors.SizeNS;
        this.ResizeWindow(WindowResizer.ResizeDirection.Top);
        break;
      case "bottom":
        this.activeWin.Cursor = Cursors.SizeNS;
        this.ResizeWindow(WindowResizer.ResizeDirection.Bottom);
        break;
      case "left":
        this.activeWin.Cursor = Cursors.SizeWE;
        this.ResizeWindow(WindowResizer.ResizeDirection.Left);
        break;
      case "right":
        this.activeWin.Cursor = Cursors.SizeWE;
        this.ResizeWindow(WindowResizer.ResizeDirection.Right);
        break;
      case "topLeft":
        this.activeWin.Cursor = Cursors.SizeNWSE;
        this.ResizeWindow(WindowResizer.ResizeDirection.TopLeft);
        break;
      case "topRight":
        this.activeWin.Cursor = Cursors.SizeNESW;
        this.ResizeWindow(WindowResizer.ResizeDirection.TopRight);
        break;
      case "bottomLeft":
        this.activeWin.Cursor = Cursors.SizeNESW;
        this.ResizeWindow(WindowResizer.ResizeDirection.BottomLeft);
        break;
      case "bottomRight":
        this.activeWin.Cursor = Cursors.SizeNWSE;
        this.ResizeWindow(WindowResizer.ResizeDirection.BottomRight);
        break;
    }
  }

  public void displayResizeCursor(object sender)
  {
    switch ((sender as Rectangle).Name)
    {
      case "top":
        this.activeWin.Cursor = Cursors.SizeNS;
        break;
      case "bottom":
        this.activeWin.Cursor = Cursors.SizeNS;
        break;
      case "left":
        this.activeWin.Cursor = Cursors.SizeWE;
        break;
      case "right":
        this.activeWin.Cursor = Cursors.SizeWE;
        break;
      case "topLeft":
        this.activeWin.Cursor = Cursors.SizeNWSE;
        break;
      case "topRight":
        this.activeWin.Cursor = Cursors.SizeNESW;
        break;
      case "bottomLeft":
        this.activeWin.Cursor = Cursors.SizeNESW;
        break;
      case "bottomRight":
        this.activeWin.Cursor = Cursors.SizeNWSE;
        break;
    }
  }

  public enum ResizeDirection
  {
    Left = 1,
    Right = 2,
    Top = 3,
    TopLeft = 4,
    TopRight = 5,
    Bottom = 6,
    BottomLeft = 7,
    BottomRight = 8,
  }
}

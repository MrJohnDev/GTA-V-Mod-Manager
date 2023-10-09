// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.GetUserInput
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace GTAV_Mod_Manager
{
  public partial class GetUserInput : Window, IComponentConnector
  {
    public bool PressedAccept = false;
    public GetUserInput() => this.InitializeComponent();


    private void Accept_Click(object sender, RoutedEventArgs e)
    {
      this.PressedAccept = true;
      this.Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      this.PressedAccept = false;
      this.Close();
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      this.UserInput.Focus();
      this.UserInput.SelectAll();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Return)
      {
        this.PressedAccept = true;
        this.Close();
        e.Handled = true;
      }
      else
      {
        if (e.Key != Key.Escape)
          return;
        this.PressedAccept = false;
        this.Close();
        e.Handled = true;
      }
    }

    private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
    {
    }
  }
}

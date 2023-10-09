// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.ErrorMessage
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace GTAV_Mod_Manager
{
  public partial class ErrorMessage : Window, IComponentConnector
  {
    public ErrorMessage() => this.InitializeComponent();

    private void Button_Click(object sender, RoutedEventArgs e) => this.Close();

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Return)
        return;
      this.Close();
    }

  }
}

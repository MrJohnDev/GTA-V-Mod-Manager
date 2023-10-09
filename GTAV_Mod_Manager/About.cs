// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.About
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace GTAV_Mod_Manager
{
  public partial class About : Window, IComponentConnector
  {
 
    public About() => this.InitializeComponent();

    private void Close_Click(object sender, RoutedEventArgs e) => this.Close();

    private void Donate_Click(object sender, RoutedEventArgs e) => Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=ECQ2N3NFZQ2EN");

  }
}

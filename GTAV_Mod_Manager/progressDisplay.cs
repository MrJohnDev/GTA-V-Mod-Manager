// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.progressDisplay
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace GTAV_Mod_Manager
{
  public partial class progressDisplay : Window, IComponentConnector
  {
    public string Source;
    public string Destination;
    public progressDisplay() => this.InitializeComponent();


    public void CopyFileWithProgress(string source, string destination)
    {
      this.progress.Value = 0.0;
      WebClient webClient = new WebClient();
      webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(this.DownloadProgress);
      webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(this.webClient_DownloadFileCompleted);
      webClient.DownloadFileAsync(new Uri(source), destination);
    }

    private void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) => this.Close();

    private void DownloadProgress(object sender, DownloadProgressChangedEventArgs e) => this.progress.Value = (double) e.ProgressPercentage;

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.Source) || string.IsNullOrWhiteSpace(this.Destination))
        this.Close();
      if (System.IO.File.Exists(this.Destination))
        System.IO.File.Delete(this.Destination);
      string directoryName = Path.GetDirectoryName(this.Destination);
      if (!Directory.Exists(directoryName))
        Directory.CreateDirectory(directoryName);
      this.CopyFileWithProgress(this.Source, this.Destination);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
    }
  }
}

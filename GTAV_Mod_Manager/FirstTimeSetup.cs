// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.FirstTimeSetup
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using GTAV_Mod_Manager.Properties;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;

namespace GTAV_Mod_Manager
{
  public partial class FirstTimeSetup : Window, IComponentConnector
  {
    public bool pressedAccept = false;
    private Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

    public FirstTimeSetup()
    {
      this.InitializeComponent();
      this.ModStoragePath.Text = string.Empty;
      this.PathToGame.Text = !string.IsNullOrEmpty(Settings.Default.InstallPath) && Directory.Exists(Settings.Default.InstallPath) ? Settings.Default.InstallPath : this.InstallPath(false);
      if (string.IsNullOrWhiteSpace(this.PathToGame.Text))
        this.PathToGame.Text = "Could not detect installation!";
      this.ModStoragePath.Text = !string.IsNullOrWhiteSpace(Settings.Default.ModStoragePath) ? Settings.Default.ModStoragePath : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "GTAV Mods");
      this.DisableModsToggle.IsChecked = new bool?(Settings.Default.DisableOnExit);
      this.DetectionTimeout.Text = Settings.Default.DetectionTimeout.ToString();
      if (Settings.Default.isSteam)
        this.Steam.IsChecked = new bool?(true);
      else if (Settings.Default.isEpic)
        this.Epic.IsChecked = new bool?(true);
      else
        this.Warehouse.IsChecked = new bool?(true);
    }

    private void ChangeInstall_Click(object sender, RoutedEventArgs e)
    {
      if (Directory.Exists(this.PathToGame.Text))
        this.openFileDialog.InitialDirectory = this.PathToGame.Text;
      else
        this.openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
      this.openFileDialog.Filter = "GTA5.exe|GTA5.exe";
      if ((this.openFileDialog.ShowDialog() ?? false) == false)
        return;
      this.PathToGame.Text = Path.GetDirectoryName(this.openFileDialog.FileName);
    }

    private void ChangeModPath_Click(object sender, RoutedEventArgs e)
    {
      FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
      folderBrowserDialog.ShowNewFolderButton = true;
      if (folderBrowserDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        return;
      this.ModStoragePath.Text = folderBrowserDialog.SelectedPath.ToString();
    }

    private void Steam_Checked(object sender, RoutedEventArgs e)
    {
      if (( this.Steam.IsChecked ?? false) != false)
      {
        this.Steam.Content = (object) "Steam Selected";
        this.Warehouse.IsChecked = new bool?(false);
        this.Epic.IsChecked = new bool?(false);
       }
      else
        this.Steam.Content = (object) "Steam";
    }

    private void Epic_Checked(object sender, RoutedEventArgs e)
    {
        if ((this.Epic.IsChecked ?? false) != false)
        {
            this.Epic.Content = (object)"Epic Selected";
            this.Warehouse.IsChecked = new bool?(false);
            this.Steam.IsChecked = new bool?(false);
        }
        else
            this.Epic.Content = (object)"Epic";
    }

        private void Warehouse_Checked(object sender, RoutedEventArgs e)
    {
      if (( this.Warehouse.IsChecked ?? false) != false)
      {
        this.Warehouse.Content = (object) "R* Selected";
        this.Steam.IsChecked = new bool?(false);
        this.Epic.IsChecked = new bool?(false);
     }
      else
        this.Warehouse.Content = (object) "R* Warehouse";
    }

    private void Accept_Click(object sender, RoutedEventArgs e)
    {


      Settings.Default.isSteam = (this.Steam.IsChecked ?? false) != false;
      Settings.Default.isEpic = (this.Epic.IsChecked ?? false) != false;
      Settings.Default.isWarehouse = (this.Warehouse.IsChecked ?? false) != false;

      Settings.Default.InstallPath = this.PathToGame.Text;
      Settings.Default.ModStoragePath = this.ModStoragePath.Text;
      // ISSUE: variable of a compiler-generated type
      Settings settings = Settings.Default;
      settings.DisableOnExit = this.DisableModsToggle.IsChecked ?? false != false;
      try
      {
        int num2 = int.Parse(this.DetectionTimeout.Text);
        if (num2 != Settings.Default.DetectionTimeout)
          Settings.Default.DetectionTimeout = num2;
      }
      catch
      {
        int num3 = (int) System.Windows.MessageBox.Show("Invalid Detection Timeout. Please try again.", "invalid timeout value");
        return;
      }
      Settings.Default.Save();
      if (!Directory.Exists(this.ModStoragePath.Text))
        Directory.CreateDirectory(this.ModStoragePath.Text);
      this.pressedAccept = true;
      this.Close();
    }

    private string InstallPath(bool edit)
    {
      try
      {
        if (File.Exists(Path.Combine(Environment.CurrentDirectory, "GTA5.exe")))
        {
          this.Steam.IsChecked = new bool?(!File.Exists(Path.Combine(Environment.CurrentDirectory, "PlayGTAV.exe")));
          return Environment.CurrentDirectory;
        }
        if (!string.IsNullOrWhiteSpace(Settings.Default.InstallPath) && Directory.Exists(Settings.Default.InstallPath))
          return Settings.Default.InstallPath;
        string empty = string.Empty;
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\rockstar games\\Grand Theft Auto V", edit))
        {
          if (registryKey != null)
            empty = registryKey.GetValue("InstallFolder", (object) "").ToString();
          if (!string.IsNullOrWhiteSpace(empty))
          {
            this.Warehouse.IsChecked = new bool?(true);
            return empty;
          }
        }
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 271590", edit))
        {
          if (registryKey != null)
            empty = registryKey.GetValue("InstallLocation", (object) "").ToString();
          if (!string.IsNullOrWhiteSpace(empty))
          {
            this.Steam.IsChecked = new bool?(true);
            return empty;
          }
        }
        return string.Empty;
      }
      catch
      {
        int num = (int) System.Windows.MessageBox.Show("There was an error reading the registry. You must run this from an account with administrative privileges!");
        return string.Empty;
      }
    }

    private void DisableModsToggle_Checked(object sender, RoutedEventArgs e)
    {
      if ((this.DisableModsToggle.IsChecked ?? false) != false)
        this.DisableModsToggle.Content = (object) "Enabled";
      else
        this.DisableModsToggle.Content = (object) "Disabled";
    }

    private void DetectionTimeout_PreviewKeyDown(object sender, TextCompositionEventArgs e)
    {
      Regex regex = new Regex("[^0-9]+");
      e.Handled = regex.IsMatch(e.Text);
    }
  }
}

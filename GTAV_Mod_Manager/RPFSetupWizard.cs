// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.RPFSetupWizard
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using GTAV_Mod_Manager.Properties;
using Microsoft.Win32;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace GTAV_Mod_Manager
{
  public partial class RPFSetupWizard : Window, IComponentConnector
  {
    public string RPFInstallLocation;
    private OpenFileDialog openFile = new OpenFileDialog();

    public RPFSetupWizard()
    {
      try
      {
        this.InitializeComponent();
        this.openFile.Filter = "RPF Files (*.rpf)|*.rpf";
      }
      catch (Exception ex)
      {
        this.ErrorMessage("RPF Import Crash", ex.ToString());
      }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => this.Close();

    private void Accept_Click(object sender, RoutedEventArgs e)
    {
      this.InstallRPF();
      this.Close();
    }

    private void InstallRPF()
    {
      try
      {
        List<char> invalid = ((IEnumerable<char>) Path.GetInvalidFileNameChars()).ToList<char>();
        invalid.AddRange((IEnumerable<char>) new char[10]
        {
          '!',
          '@',
          '#',
          '$',
          '%',
          '^',
          '&',
          '*',
          '/',
          '~'
        });
        string str = new string(this.FriendlyName.Text.Where<char>((Func<char, bool>) (x => !invalid.Contains(x))).ToArray<char>());
        string path2_1 = string.Format("{0}.rpf", (object) str);
        string path2_2 = string.Format("{0}.config", (object) str);
        string destination = Path.Combine(this.RPFInstallLocation, path2_1);
        string path = Path.Combine(this.RPFInstallLocation, path2_2);
        string text1 = this.PathToModdedRPF.Text;
        string text2 = this.PathToVanillaRPF.Text;
        this.copyFile(text1, destination);
        File.WriteAllText(path, text2);
      }
      catch (Exception ex)
      {
        this.ErrorMessage("RPF Import failed!", ex.ToString());
      }
    }

    private void BrowseModded_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        this.openFile.InitialDirectory = Environment.CurrentDirectory;
        if ((this.openFile.ShowDialog() ?? false) == false)
          return;
        this.PathToModdedRPF.Text = this.openFile.FileName;
        this.BrowseVanilla.IsEnabled = true;
      }
      catch (Exception ex)
      {
        this.ErrorMessage("(Step 1) RPF Import failed!", ex.ToString());
      }
    }

    private void BrowseVanilla_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        this.openFile.InitialDirectory = Settings.Default.InstallPath;
        if (( this.openFile.ShowDialog() ?? false) == false)
          return;
        if (!this.openFile.FileName.ToLower().Contains(Settings.Default.InstallPath.ToLower()))
          this.ErrorMessage("Warning!", "It looks like you chose an RPF that isn't in your game directory. Linking will not work if you choose the wrong location.");
        this.PathToVanillaRPF.Text = this.openFile.FileName;
        this.EnterName.IsEnabled = true;
      }
      catch (Exception ex)
      {
        this.ErrorMessage("(Step 2) RPF Import failed!", ex.ToString());
      }
    }

    private void EnterName_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        GetUserInput getUserInput = new GetUserInput();
        getUserInput.TitleMessage.Text = "Select a friendly name for your RPF file";
        getUserInput.UserInput.Text = "Enter Name Here";
        getUserInput.Owner = this.Owner;
        getUserInput.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        getUserInput.ShowDialog();
        if (!getUserInput.PressedAccept || string.IsNullOrWhiteSpace(getUserInput.UserInput.Text))
          return;
        this.FriendlyName.Text = getUserInput.UserInput.Text;
        this.Accept.IsEnabled = true;
        this.DoYouAccept.Visibility = Visibility.Visible;
      }
      catch (Exception ex)
      {
        this.ErrorMessage("(Step 3) RPF Import failed!", ex.ToString());
      }
    }

    private void ErrorMessage(string title, string message)
    {
      GTAV_Mod_Manager.ErrorMessage errorMessage = new GTAV_Mod_Manager.ErrorMessage();
      try
      {
        errorMessage.Owner = (Window) this;
        errorMessage.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }
      catch
      {
        errorMessage.WindowStartupLocation = WindowStartupLocation.CenterScreen;
      }
      errorMessage.TitleMessage.Text = title;
      errorMessage.Message.Text = message;
      errorMessage.ShowDialog();
    }

    private void copyFile(string source, string destination)
    {
      progressDisplay progressDisplay = new progressDisplay();
      progressDisplay.TitleMessage.Text = "Transfering file...";
      progressDisplay.Message.Text = string.Format("Copying {0} to {1}", (object) source, (object) destination);
      progressDisplay.Source = source;
      progressDisplay.Destination = destination;
      progressDisplay.Owner = (Window) this;
      progressDisplay.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      progressDisplay.ShowDialog();
    }
  }
}

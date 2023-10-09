// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.Options
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace GTAV_Mod_Manager
{
  public partial class Options : Window, IComponentConnector
  {
    public bool PerformedMigration = false;
    private bool firstOpen = true;
    private bool firstTime = true;

    public Options()
    {
      this.InitializeComponent();
      this.RegFix.IsChecked = new bool?(this.isRegFixed());
      this.firstOpen = false;
      this.UseFirewall.IsChecked = new bool?(Settings.Default.UseFirewall);
      this.NvidiaStreamer.IsEnabled = false;
      this.TransferMode.IsChecked = new bool?(Settings.Default.CopyOnly);
      this.ForceSocialClub.IsChecked = new bool?(Settings.Default.SocialClubOffline);
      this.CustomCommandline.IsChecked = new bool?(Settings.Default.CustomCMDEnabled);
      this.CustomCommands.Text = Settings.Default.CustomCMDText;
      this.BypassVerification.IsChecked = new bool?(Settings.Default.SocialClubInTarget);
      ToggleButton bypassVerification = this.BypassVerification;
      bool? isChecked = this.ForceSocialClub.IsChecked;
      bypassVerification.IsEnabled = (isChecked ?? false) != false;
      this.Mod_Detection.IsChecked = new bool?(Settings.Default.FileDetectionMode);
      this.NanyMode.IsChecked = new bool?(Settings.Default.NannyMode);
      this.LaunchExeToggle.IsChecked = new bool?(Settings.Default.LaunchPlayGTAV);
      ToggleButton launchExeToggle = this.LaunchExeToggle;
      isChecked = this.LaunchExeToggle.IsChecked;
      string str = (isChecked ?? false) != false ? "Launching PlayGTAV.exe" : "Launching GTA5.exe";
      launchExeToggle.Content = (object) str;
      this.setTooltips();
      this.firstTime = false;
    }

    private void setTooltips()
    {
      this.RegFix.ToolTip = (object) "Enabling this feature will set GTA5.exe to high priority and the launcher to low priority";
      this.UseFirewall.ToolTip = (object) "Enabling this feature will use Windows Firewall rules to block GTA5 from reaching the internet";
      this.TransferMode.ToolTip = (object) "Changing this from Symlink will use native windows File copy mode. This mode takes longer but will work on systems that cannot get symlink working properly.";
      this.ForceSocialClub.ToolTip = (object) "Enabling this feature will add -scOfflineOnly to commandline.txt, stopping you from logging in to social club.";
      this.CustomCommandline.ToolTip = (object) "Enabling this feature will allow you to add additional commands to the commandline.txt file";
      this.BypassVerification.ToolTip = (object) "Enabling this feature will put -scOfflineOnly in the targetpath of the launcher, and may help if the game keeps trying to redownload files you have modified.";
      this.Mod_Detection.ToolTip = (object) "File Mod detection will let you place all your mods loosely in your mods folder. Folder Mod detection requires you to put each mod in a unique folder, similar to nexus mod manager";
      this.NanyMode.ToolTip = (object) "Too many users using this tool incorrectly so I have to hold their hands and yell at them with error messages when they install mods incorrectly. Untick this if you intentinally are modding vanilla files.";
    }

    private bool isRegFixed()
    {
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\GTA5.exe\\PerfOptions", false))
        return registryKey != null && registryKey.GetValue("CpuPriorityClass", (object) null) != null && registryKey.GetValue("CpuPriorityClass", (object) "").ToString().EndsWith("3");
    }

    private void ToggleButton_Checked(object sender, RoutedEventArgs e)
    {
    }

    private void RegFix_Checked(object sender, RoutedEventArgs e)
    {
      if ((this.RegFix.IsChecked ?? false) != false)
      {
        if (this.firstOpen)
          this.firstOpen = false;
        else if (!this.setPriorities())
          return;
        this.RegFix.Content = (object) "High Priority Tweak Enabled";
      }
      else
      {
        if (this.firstOpen)
          this.firstOpen = false;
        else if (!this.revertPriorities())
          return;
        this.RegFix.Content = (object) "High Priority Tweak Disabled";
      }
    }

    private bool setPriorities()
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      dictionary.Add("GTA5.exe", "00000003");
      dictionary.Add("GTAVLauncher.exe", "00000005");
      dictionary.Add("subprocess.exe", "00000005");
      try
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options", true))
        {
          foreach (KeyValuePair<string, string> keyValuePair in dictionary)
          {
            if (registryKey == null)
              return false;
            if (registryKey.OpenSubKey(keyValuePair.Key) == null)
              registryKey.CreateSubKey(keyValuePair.Key);
            if (registryKey.OpenSubKey(keyValuePair.Key + "\\PerfOptions") == null)
              registryKey.CreateSubKey(keyValuePair.Key + "\\PerfOptions");
            registryKey.OpenSubKey(keyValuePair.Key + "\\PerfOptions", true).SetValue("CpuPriorityClass", (object) keyValuePair.Value, RegistryValueKind.DWord);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Problem Writing to the Registry", ex.Message);
        return false;
      }
    }

    private bool revertPriorities()
    {
      string[] strArray = new string[3]
      {
        "GTA5.exe",
        "GTAVLauncher.exe",
        "SubProcess.exe"
      };
      try
      {
        using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options", true))
        {
          foreach (string subkey in strArray)
            registryKey.DeleteSubKeyTree(subkey, false);
        }
        return true;
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Hey Sanka you dead? No mon I'm not dead", "Looks like you tried accessing the registry without proper permissions. Shame on you!\n" + ex.Message);
        return false;
      }
    }

    private void NvidiaStreamer_Checked(object sender, RoutedEventArgs e)
    {
      if ((this.NvidiaStreamer.IsChecked ?? false) != false)
        this.NvidiaStreamer.Content = (object) "Enable Nvidia Streaming Service (Nvidia Only)";
      else
        this.NvidiaStreamer.Content = (object) "Disable Nvidia Streaming Service (Nvidia Only)";
    }

    private void Button_Click(object sender, RoutedEventArgs e) => this.Close();

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
      About about = new About();
      about.Owner = this.Owner;
      about.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      about.ShowDialog();
    }

    private void ErrorMessage(string title, string message)
    {
      GTAV_Mod_Manager.ErrorMessage errorMessage = new GTAV_Mod_Manager.ErrorMessage();
      errorMessage.TitleMessage.Text = title;
      errorMessage.Message.Text = message;
      errorMessage.Owner = this.Owner;
      errorMessage.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      errorMessage.ShowDialog();
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
      FirstTimeSetup firstTimeSetup = new FirstTimeSetup();
      firstTimeSetup.Owner = this.Owner;
      firstTimeSetup.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      firstTimeSetup.ShowDialog();
    }

    private void UseFirewall_Unchecked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = this.UseFirewall.IsChecked;
      bool useFirewall = Settings.Default.UseFirewall;
      if ((isChecked.GetValueOrDefault() != useFirewall ? 1 : (!isChecked.HasValue ? 1 : 0)) != 0)
      {
        // ISSUE: variable of a compiler-generated type
        Settings settings = Settings.Default;
        isChecked = this.UseFirewall.IsChecked;
        settings.UseFirewall = (isChecked ?? false) != false;
        Settings.Default.Save();
      }
      isChecked = this.UseFirewall.IsChecked;
      if ((isChecked ?? false) != false)
        this.UseFirewall.Content = (object) "Firewall Rule Creation Enabled";
      else
        this.UseFirewall.Content = (object) "Firewall Rule Creation Disabled";
    }

    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
      try
      {
        Process.Start("http://www.reddit.com/r/GrandTheftAutoV_PC/comments/35u4k1/release_version_gtav_mod_manager_by_bilago/");
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.ToString());
      }
    }


    private void ModImporter_Click(object sender, RoutedEventArgs e)
    {
      this.PerformedMigration = true;
      this.PerformMigration();
      this.ErrorMessage("Mod Migration complete!", "Migration is now complete. Please make check your GTAV installation directory to ensure all mods have been moved. This migration tool makes NO Guarentee that all mods have been migrated.");
    }

    public void PerformMigration() => this.ErrorMessage("Featured disabled", "This feature is too powerful and causes too many support requests, please migrate your mods on your own");

    private bool isSymbolic(string path, bool isDirectory) => isDirectory ? new DirectoryInfo(path).Attributes.HasFlag((Enum) FileAttributes.ReparsePoint) : new FileInfo(path).Attributes.HasFlag((Enum) FileAttributes.ReparsePoint);

    private static void moveFile(string source, string destination)
    {
      if (File.Exists(destination))
        File.Delete(destination);
      File.Move(source, destination);
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key != Key.Escape)
        return;
      this.Close();
    }

    private void TransferMode_Checked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = this.TransferMode.IsChecked;
      bool copyOnly = Settings.Default.CopyOnly;
      if ((isChecked.GetValueOrDefault() != copyOnly ? 1 : (!isChecked.HasValue ? 1 : 0)) != 0)
      {
        // ISSUE: variable of a compiler-generated type
        Settings settings = Settings.Default;
        isChecked = this.TransferMode.IsChecked;
        settings.CopyOnly = (isChecked ?? false) != false;
        Settings.Default.Save();
      }
      isChecked = this.TransferMode.IsChecked;
      if ((isChecked ?? false) != false)
        this.TransferMode.Content = (object) "Copy Files Mode Enabled";
      else
        this.TransferMode.Content = (object) "Symlink Files Mode Enabled";
    }

    private void CustomCommandline_Checked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = this.CustomCommandline.IsChecked;
      if ((isChecked ?? false) != false)
      {
        this.CustomCommandline.Content = (object) "Custom Commandline Text Enabled";
        if (!this.firstTime)
        {
          GetUserInput getUserInput = new GetUserInput();
          getUserInput.Owner = (Window) this;
          getUserInput.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          getUserInput.UserInput.Text = Settings.Default.CustomCMDText;
          getUserInput.TitleMessage.Text = "Input new commandlines below";
          getUserInput.ShowDialog();
          if (!getUserInput.PressedAccept)
          {
            e.Handled = true;
            return;
          }
          if (Settings.Default.CustomCMDText != getUserInput.UserInput.Text)
          {
            Settings.Default.CustomCMDText = getUserInput.UserInput.Text;
            Settings.Default.Save();
            this.CustomCommands.Text = Settings.Default.CustomCMDText;
          }
        }
        else
          this.firstTime = false;
      }
      else
        this.CustomCommandline.Content = (object) "Custom Commandline Text Disabled";
      isChecked = this.CustomCommandline.IsChecked;
      bool customCmdEnabled = Settings.Default.CustomCMDEnabled;
      if ((isChecked.GetValueOrDefault() != customCmdEnabled ? 1 : (!isChecked.HasValue ? 1 : 0)) == 0)
        return;
      // ISSUE: variable of a compiler-generated type
      Settings settings = Settings.Default;
      isChecked = this.CustomCommandline.IsChecked;
      settings.CustomCMDEnabled = (isChecked ?? false) != false;
      Settings.Default.Save();
    }

    private void ForceSocialClub_Checked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = this.ForceSocialClub.IsChecked;
      bool socialClubOffline = Settings.Default.SocialClubOffline;
      if ((isChecked.GetValueOrDefault() != socialClubOffline ? 1 : (!isChecked.HasValue ? 1 : 0)) != 0)
      {
        // ISSUE: variable of a compiler-generated type
        Settings settings = Settings.Default;
        isChecked = this.ForceSocialClub.IsChecked;
        settings.SocialClubOffline = (isChecked ?? false) != false;
        Settings.Default.Save();
      }
      isChecked = this.ForceSocialClub.IsChecked;
      if ((isChecked ?? false) != false)
        this.ForceSocialClub.Content = (object) "Force Social Club Offline Enabled";
      else
        this.ForceSocialClub.Content = (object) "Force Social Club Offline Disabled";
      ToggleButton bypassVerification = this.BypassVerification;
      isChecked = this.ForceSocialClub.IsChecked;
      bypassVerification.IsEnabled = (isChecked ?? false) != false;
    }

    private void close_Click(object sender, RoutedEventArgs e) => this.Close();

    private void BypassVerification_Checked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = this.BypassVerification.IsChecked;
      bool socialClubInTarget = Settings.Default.SocialClubInTarget;
      if ((isChecked.GetValueOrDefault() != socialClubInTarget ? 1 : (!isChecked.HasValue ? 1 : 0)) != 0)
      {
        // ISSUE: variable of a compiler-generated type
        Settings settings = Settings.Default;
        isChecked = this.BypassVerification.IsChecked;
        settings.SocialClubInTarget = (isChecked ?? false) != false;
        Settings.Default.Save();
      }
      isChecked = this.BypassVerification.IsChecked;
      if ((isChecked ?? false) != false)
        this.BypassVerification.Content = (object) "SC Bypass File Verification Enabled";
      else
        this.BypassVerification.Content = (object) "SC Bypass File Verification Disabled";
    }

    private void Mod_Detection_Checked(object sender, RoutedEventArgs e)
    {
      bool? isChecked = this.Mod_Detection.IsChecked;
      bool fileDetectionMode = Settings.Default.FileDetectionMode;
      if ((isChecked.GetValueOrDefault() != fileDetectionMode ? 1 : (!isChecked.HasValue ? 1 : 0)) != 0)
      {
        // ISSUE: variable of a compiler-generated type
        Settings settings = Settings.Default;
        isChecked = this.Mod_Detection.IsChecked;
        settings.FileDetectionMode = (isChecked ?? false) != false;
        Settings.Default.Save();
      }
      isChecked = this.Mod_Detection.IsChecked;
      if ((isChecked ?? false) != false)
        this.Mod_Detection.Content = (object) "File Mod Detection Enabled";
      else
        this.Mod_Detection.Content = (object) "Folder Mod Detection Enabled";
    }



    private void NanyMode_Checked(object sender, RoutedEventArgs e)
    {
      Settings.Default.NannyMode = (this.NanyMode.IsChecked ?? false) != false;
      Settings.Default.Save();
    }

    private void LaunchExeToggle_Checked(object sender, RoutedEventArgs e)
    {
      // ISSUE: variable of a compiler-generated type
      Settings settings = Settings.Default;
      bool? isChecked = this.LaunchExeToggle.IsChecked;
      settings.LaunchPlayGTAV = (isChecked ?? false) != false;
      ToggleButton launchExeToggle = this.LaunchExeToggle;
      isChecked = this.LaunchExeToggle.IsChecked;
      string str = (isChecked ?? false) != false ? "Launching PlayGTAV.exe" : "Launching GTA5.exe";
      launchExeToggle.Content = (object) str;
      Settings.Default.Save();
    }
  }
}

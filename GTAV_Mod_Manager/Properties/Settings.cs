// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.Properties.Settings
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GTAV_Mod_Manager.Properties
{
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
  [CompilerGenerated]
  internal sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        Settings defaultInstance = Settings.defaultInstance;
        return defaultInstance;
      }
    }

    [DebuggerNonUserCode]
    [DefaultSettingValue("True")]
    [UserScopedSetting]
    public bool FirstTime
    {
      get => (bool) this[nameof (FirstTime)];
      set => this[nameof (FirstTime)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("")]
    [DebuggerNonUserCode]
    public string InstallPath
    {
      get => (string) this[nameof (InstallPath)];
      set => this[nameof (InstallPath)] = (object) value;
    }

    [DefaultSettingValue("")]
    [UserScopedSetting]
    [DebuggerNonUserCode]
    public string ModStoragePath
    {
      get => (string) this[nameof (ModStoragePath)];
      set => this[nameof (ModStoragePath)] = (object) value;
    }

    [DefaultSettingValue("False")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public bool isSteam
    {
      get => (bool) this[nameof (isSteam)];
      set => this[nameof (isSteam)] = (object) value;
    }
   [DefaultSettingValue("False")]
   [DebuggerNonUserCode]
   [UserScopedSetting]
   public bool isWarehouse
   {
       get => (bool)this[nameof(isWarehouse)];
       set => this[nameof(isWarehouse)] = (object)value;
   }

   [DefaultSettingValue("False")]
   [DebuggerNonUserCode]
   [UserScopedSetting]
   public bool isEpic
   {
       get => (bool)this[nameof(isEpic)];
       set => this[nameof(isEpic)] = (object)value;
   }

    [UserScopedSetting]
    [DefaultSettingValue("True")]
    [DebuggerNonUserCode]
    public bool UpdateSettings
    {
      get => (bool) this[nameof (UpdateSettings)];
      set => this[nameof (UpdateSettings)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("True")]
    public bool UseFirewall
    {
      get => (bool) this[nameof (UseFirewall)];
      set => this[nameof (UseFirewall)] = (object) value;
    }

    [DefaultSettingValue("False")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public bool DisableOnExit
    {
      get => (bool) this[nameof (DisableOnExit)];
      set => this[nameof (DisableOnExit)] = (object) value;
    }

    [DefaultSettingValue("")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public string TheList
    {
      get => (string) this[nameof (TheList)];
      set => this[nameof (TheList)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    public string CustomRPF
    {
      get => (string) this[nameof (CustomRPF)];
      set => this[nameof (CustomRPF)] = (object) value;
    }

    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    [UserScopedSetting]
    public bool BypassGame
    {
      get => (bool) this[nameof (BypassGame)];
      set => this[nameof (BypassGame)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("True")]
    [DebuggerNonUserCode]
    public bool RPFWarning
    {
      get => (bool) this[nameof (RPFWarning)];
      set => this[nameof (RPFWarning)] = (object) value;
    }

    [DebuggerNonUserCode]
    [UserScopedSetting]
    [DefaultSettingValue("False")]
    public bool CopyOnly
    {
      get => (bool) this[nameof (CopyOnly)];
      set => this[nameof (CopyOnly)] = (object) value;
    }

    [DefaultSettingValue("False")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public bool CustomCMDEnabled
    {
      get => (bool) this[nameof (CustomCMDEnabled)];
      set => this[nameof (CustomCMDEnabled)] = (object) value;
    }

    [DefaultSettingValue("")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public string CustomCMDText
    {
      get => (string) this[nameof (CustomCMDText)];
      set => this[nameof (CustomCMDText)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("True")]
    [DebuggerNonUserCode]
    public bool SocialClubOffline
    {
      get => (bool) this[nameof (SocialClubOffline)];
      set => this[nameof (SocialClubOffline)] = (object) value;
    }

    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    [UserScopedSetting]
    public bool SocialClubInTarget
    {
      get => (bool) this[nameof (SocialClubInTarget)];
      set => this[nameof (SocialClubInTarget)] = (object) value;
    }

    [DebuggerNonUserCode]
    [UserScopedSetting]
    [DefaultSettingValue("550")]
    public double Height
    {
      get => (double) this[nameof (Height)];
      set => this[nameof (Height)] = (object) value;
    }

    [DefaultSettingValue("757")]
    [UserScopedSetting]
    [DebuggerNonUserCode]
    public double Width
    {
      get => (double) this[nameof (Width)];
      set => this[nameof (Width)] = (object) value;
    }

    [DefaultSettingValue("True")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public bool FileDetectionMode
    {
      get => (bool) this[nameof (FileDetectionMode)];
      set => this[nameof (FileDetectionMode)] = (object) value;
    }

    [DefaultSettingValue("False")]
    [UserScopedSetting]
    [DebuggerNonUserCode]
    public bool DownloadBeta
    {
      get => (bool) this[nameof (DownloadBeta)];
      set => this[nameof (DownloadBeta)] = (object) value;
    }

    [DefaultSettingValue("75")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public int DetectionTimeout
    {
      get => (int) this[nameof (DetectionTimeout)];
      set => this[nameof (DetectionTimeout)] = (object) value;
    }

    [UserScopedSetting]
    [DebuggerNonUserCode]
    [DefaultSettingValue("False")]
    public bool DisableUpdates
    {
      get => (bool) this[nameof (DisableUpdates)];
      set => this[nameof (DisableUpdates)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("True")]
    [DebuggerNonUserCode]
    public bool NannyMode
    {
      get => (bool) this[nameof (NannyMode)];
      set => this[nameof (NannyMode)] = (object) value;
    }

    [DefaultSettingValue("True")]
    [DebuggerNonUserCode]
    [UserScopedSetting]
    public bool UndoAllNannyPrompt
    {
      get => (bool) this[nameof (UndoAllNannyPrompt)];
      set => this[nameof (UndoAllNannyPrompt)] = (object) value;
    }

    [UserScopedSetting]
    [DefaultSettingValue("True")]
    [DebuggerNonUserCode]
    public bool LaunchPlayGTAV
    {
      get => (bool) this[nameof (LaunchPlayGTAV)];
      set => this[nameof (LaunchPlayGTAV)] = (object) value;
    }
  }
}

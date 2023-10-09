// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.MainWindow
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using GTAV_Mod_Manager.Properties;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using Monitor.Core.Utilities;
using NetFwTypeLib;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GTAV_Mod_Manager
{
  public partial class MainWindow : Window, IComponentConnector
  {
    private WindowResizer ob;
    private OpenFileDialog openFileDialog = new OpenFileDialog();
    public bool ForceQuit = false;
    private string installedModsPath;
    private string InstalledRPFPath;
    private string InstalledGTALuaPath;
    private string GameRPFPath;
    private string GameUpdateFolder;
    private string GameX64Folder;
    public static bool DisplayNoUpdates = true;
    private bool isSteam;
    private bool isEpic;
    private string gameInstallationPath;
    private string gta5FilePath;
    private string gtaLauncherPath;
    private string commandLineFilePath;
    private List<string> configs = new List<string>();
    private bool tryLoadLua = false;
    private bool tryLoadNetScripts = false;
    private bool tryLoadSweetFX = false;
    private bool tryLoadRPFScripts = false;
    private bool tryLoadPythonScripts = false;
    private bool tryLoadlspdfr = false;
    private string cfgFileLoaded = string.Empty;
    private int enabledModCount = 0;
    private List<string> injectorNames = new List<string>()
    {
      "scripthookv",
      "scripthookvdotnet",
      "dinput8",
      "dsound",
      "d3d9",
      "d3d11",
      "dxgi"
    };
    public CollectionView Colview;
    private List<string> injectors = new List<string>();

    [DllImport("kernel32.dll")]
    private static extern bool CreateSymbolicLink(
      string lpSymlinkFileName,
      string lpTargetFileName,
      MainWindow.SymbolicLink dwFlags);

    [DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool CreateHardLink(
      string lpFileName,
      string lpExistingFileName,
      IntPtr lpSecurityAttributes);

    private void symlinkDirectory(string source, bool Install)
    {
      try
      {
        if (!Directory.Exists(source))
          return;
        string str = this.joinPaths(this.GameInstallationPath, new DirectoryInfo(source).Name);
        if (!Install)
        {
          try
          {
            this.DeleteDirectory(str, true);
          }
          catch
          {
          }
        }
        else
        {
          if (Directory.Exists(str))
            return;
          if (Settings.Default.CopyOnly)
            this.copyDirectory(source, str);
          else
            JunctionPoint.Create(str, source, true);
        }
      }
      catch (Exception ex)
      {
        File.AppendAllText("errors.log", ex.ToString());
      }
    }

    private void symlinkDirectory(string source, string dest, bool Install)
    {
      try
      {
        if (!Directory.Exists(source))
          return;
        string str = dest;
        if (!Install)
        {
          try
          {
            this.DeleteDirectory(str, true);
          }
          catch
          {
          }
        }
        else
        {
          if (Directory.Exists(str))
            return;
          if (Settings.Default.CopyOnly)
            this.copyDirectory(source, str);
          else
            JunctionPoint.Create(str, source, true);
        }
      }
      catch (Exception ex)
      {
        File.AppendAllText("errors.log", ex.ToString());
      }
    }

    private bool DeleteFile(string file)
    {
      if (!File.Exists(file))
        return true;
      if (Settings.Default.NannyMode)
      {
        string[] source = new string[2]
        {
          this.GameUpdateFolder.ToLower(),
          this.GameX64Folder.ToLower()
        };
        string p = file.ToLower();
        if (((IEnumerable<string>) source).Any<string>((Func<string, bool>) (x => p.StartsWith(x))))
        {
          this.ErrorMessage("[NANNY MODE] You installed some mods incorrectly...", string.Format("You installed a mod that is trying to delete {0}. This means you installed that mod incorrectly as it shouldn't try to replace or delete vanilla files.", (object) file));
          return false;
        }
      }
      File.Delete(file);
      return true;
    }

    private bool DeleteDirectory(string dir, bool recursive)
    {
      if (!Directory.Exists(dir))
        return true;
      if (Settings.Default.NannyMode)
      {
        string[] source = new string[2]
        {
          this.GameUpdateFolder.ToLower(),
          this.GameX64Folder.ToLower()
        };
        string p = dir.ToLower();
        if (((IEnumerable<string>) source).Any<string>((Func<string, bool>) (x => p.StartsWith(x))))
        {
          this.ErrorMessage("[NANNY MODE] You installed some mods incorrectly...", string.Format("You installed a mod that is trying to delete {0}. This means you installed that mod incorrectly as it shouldn't try to replace or delete vanilla folders.", (object) dir));
          return false;
        }
      }
      Directory.Delete(dir, recursive);
      return true;
    }

    private void symlinkFile(string source, string altDestination, bool Install)
    {
      try
      {
        string empty = string.Empty;
        string str = string.IsNullOrEmpty(altDestination) ? this.joinPaths(this.GameInstallationPath, this.getFileName(source, new bool?())) : altDestination;
        this.DeleteFile(str);
        if (!Install)
          return;
        if (Settings.Default.CopyOnly)
          this.copyFile(source, str);
        else
          MainWindow.CreateSymbolicLink(str, source, MainWindow.SymbolicLink.File);
      }
      catch (Exception ex)
      {
        File.AppendAllText("errors.log", ex.ToString());
      }
    }

    private bool hardlinkFile(string source, string altDestination, bool Install)
    {
      try
      {
        bool flag1 = Settings.Default.CopyOnly;
        if (!File.Exists(source))
          throw new Exception("Source file does not exist:\n" + source);
        if (!flag1 && System.IO.Path.GetPathRoot(source) != System.IO.Path.GetPathRoot(this.GameInstallationPath))
          flag1 = true;
        string empty = string.Empty;
        bool flag2 = !string.IsNullOrWhiteSpace(altDestination);
        string str = !flag2 ? this.joinPaths(this.GameInstallationPath, this.getFileName(source, new bool?())) : altDestination;
        if (!Install)
          return this.DeleteFile(str);
        if (Install)
        {
          if (File.Exists(str))
            return true;
          if (flag1)
            this.copyFile(source, str);
          else if (!MainWindow.CreateHardLink(str, source, IntPtr.Zero))
            this.copyFile(source, str);
        }
        else if (flag2 && File.Exists(str + ".original"))
          File.Move(str + ".original", str);
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Error creating Hardlink", ex.Message);
      }
      return true;
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

    public ObservableCollection<MainWindow.ModInfoClass> TheList { get; set; }

    public List<MainWindow.ModInfoClass> TheBakList { get; set; }

    public string InstalledPythonPath { get; set; }

    public string InstalledScriptsPath { get; set; }

    public string InstalledLuaPath { get; set; }

    public string InstalledModsPath
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(this.installedModsPath))
          return this.installedModsPath;
        this.ErrorMessage("Whooops!", "It appears your Mod Installation folder is not set up correctly. Please visit the setup wizard to configure.");
        return string.Empty;
      }
      set
      {
        if (!(this.installedModsPath != value))
          return;
        if (!Directory.Exists(value))
          Directory.CreateDirectory(value);
        this.InstalledRPFPath = this.joinPaths(value, "RPF");
        this.InstalledScriptsPath = this.joinPaths(value, "scripts");
        this.InstalledPythonPath = this.joinPaths(value, "python\\scripts");
        this.InstalledLuaPath = this.joinPaths(this.InstalledScriptsPath, "addins");
        this.InstalledGTALuaPath = this.joinPaths(value, "GTALua");
        this.installedModsPath = value;
        if (Settings.Default.FileDetectionMode && !Directory.Exists(this.InstalledLuaPath))
          Directory.CreateDirectory(this.InstalledLuaPath);
        if (!Directory.Exists(this.InstalledRPFPath))
          Directory.CreateDirectory(this.InstalledRPFPath);
      }
    }

    public string GameInstallationPath
    {
      get => this.gameInstallationPath;
      set
      {
        this.gta5FilePath = this.joinPaths(value, "GTA5.exe");
        this.gameInstallationPath = value;
        this.GameRPFPath = this.joinPaths(value, "mods");
        this.GameUpdateFolder = this.joinPaths(value, "update");
        this.GameX64Folder = this.joinPaths(value, "x64");
        if (!Directory.Exists(value))
          return;
        if (!File.Exists(this.gta5FilePath))
        {
          if (File.Exists(this.gta5FilePath + ".original"))
            File.Move(this.gta5FilePath + ".original", this.gta5FilePath);
          else
            this.ErrorMessage("GTA5.exe is missing.", "GTA5.exe is not present in\n" + value + "\nPlease make sure the correct location is selected before continuing.");
        }
        this.gtaLauncherPath = this.joinPaths(value, "GTAVLauncher.exe");
        this.commandLineFilePath = this.joinPaths(value, "commandline.txt");
      }
    }

    private string joinPaths(string part1, string part2)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(part1))
          throw new Exception("The first part of the string is null! This is a configuration error!");
        return !string.IsNullOrWhiteSpace(part2) ? System.IO.Path.Combine(part1, part2) : throw new Exception("First part is good but this second half of the path is null! Why did that happen??");
      }
      catch (Exception ex)
      {
        throw new Exception("Looks like something isn't configured correctly.\n" + ex.ToString());
      }
    }

    private void SaveCollection()
    {
      this.TheBakList = this.TheList.ToList<MainWindow.ModInfoClass>();
      using (MemoryStream serializationStream = new MemoryStream())
      {
        new BinaryFormatter().Serialize((Stream) serializationStream, (object) this.TheBakList);
        serializationStream.Position = 0L;
        byte[] numArray = new byte[(int) serializationStream.Length];
        serializationStream.Read(numArray, 0, numArray.Length);
        Settings.Default.TheList = Convert.ToBase64String(numArray);
        Settings.Default.Save();
      }
    }

    private List<MainWindow.ModInfoClass> LoadCollection()
    {
      if (string.IsNullOrWhiteSpace(Settings.Default.TheList))
        return new List<MainWindow.ModInfoClass>();
      try
      {
        using (MemoryStream serializationStream = new MemoryStream(Convert.FromBase64String(Settings.Default.TheList)))
          return (List<MainWindow.ModInfoClass>) new BinaryFormatter().Deserialize((Stream) serializationStream);
      }
      catch
      {
        return new List<MainWindow.ModInfoClass>();
      }
    }

    public MainWindow()
    {
      try
      {
        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
        if (Settings.Default.UpdateSettings)
        {
          Settings.Default.Upgrade();
          Settings.Default.UpdateSettings = false;
          Settings.Default.Save();
        }
        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        EmbeddedAssembly.initialize("GTAV_Mod_Manager", new List<string>(){});
        if (Settings.Default.FirstTime)
        {
          TOS tos = new TOS();
          tos.WindowStartupLocation = WindowStartupLocation.CenterScreen;
          tos.ShowDialog();
          if (!tos.accept)
            Environment.Exit(0);
          FirstTimeSetup firstTimeSetup = new FirstTimeSetup();
          firstTimeSetup.WindowStartupLocation = WindowStartupLocation.CenterScreen;
          firstTimeSetup.ShowDialog();
          if (!firstTimeSetup.pressedAccept)
          {
            Settings.Default.Reset();
            Settings.Default.Save();
            Environment.Exit(0);
          }
          Settings.Default.FileDetectionMode = false;
          Settings.Default.FirstTime = false;
          Settings.Default.Save();
        }
        this.InitializeComponent();
        this.Height = Settings.Default.Height;
        this.Width = Settings.Default.Width;
        // if (this.ModList != null)
        this.ModList.Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
        this.ob = new WindowResizer((Window) this);
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message);
        File.WriteAllText("crash.txt", ex.ToString());
      }
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      int num = (int) MessageBox.Show(e.ExceptionObject.ToString(), "Crash!!");
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
      try
      {
        this.MainVersion.Text = "v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        this.GameInstallationPath = Settings.Default.InstallPath;
        if (string.IsNullOrWhiteSpace(Settings.Default.ModStoragePath))
        {
          Settings.Default.ModStoragePath = this.joinPaths(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "GTAV Mods");
          Settings.Default.Save();
        }
        this.InstalledModsPath = Settings.Default.ModStoragePath;
        this.isSteam = Settings.Default.isSteam;
        this.isEpic = Settings.Default.isEpic;
        this.TheBakList = this.LoadCollection();
        this.DetectMods();
        this.Colview = (CollectionView) CollectionViewSource.GetDefaultView((object) this.ModList.ItemsSource);
        this.Colview.Filter = new Predicate<object>(this.UserFilter);
        this.SaveCollection();
        if (File.Exists("uk12.txt"))
        {
          Settings.Default.BypassGame = true;
          this.Bypass.Visibility = Visibility.Visible;
        }
        else
        {
          Settings.Default.BypassGame = false;
          this.Bypass.Visibility = Visibility.Hidden;
        }
        try
        {
          if (Environment.GetCommandLineArgs().Length <= 1)
            return;
          this.Dispatcher.BeginInvoke((Action)delegate ()
          {
            if (Environment.GetCommandLineArgs()[1].ToLower() == "sp")
            {
              this.ForceQuit = true;
              Settings.Default.DisableOnExit = true;
              this.EnableSelectedMods();
            }
            else
              this.startOnline(true);
          });
        }
        catch (Exception ex)
        {
          int num = (int) MessageBox.Show(ex.ToString());
        }
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.Message);
        File.WriteAllText("crash.txt", ex.ToString());
      }
    }

    private static bool CloseApplicationAlreadyRunning()
    {
      Process[] processesByName = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
      if (processesByName.Length < 2)
        return false;
      foreach (Process process in processesByName)
      {
        if (process.Id != Process.GetCurrentProcess().Id)
        {
          process.Kill();
          return !process.WaitForExit(6000);
        }
      }
      return false;
    }

    private void FWRule(
      string path,
      NET_FW_RULE_DIRECTION_ d,
      NET_FW_ACTION_ fwaction,
      string action)
    {
      try
      {
        // ISSUE: variable of a compiler-generated type
        INetFwRule instance1 = (INetFwRule) Activator.CreateInstance(System.Type.GetTypeFromProgID("HNetCfg.FWRule"));
        instance1.Action = fwaction;
        instance1.Enabled = true;
        instance1.InterfaceTypes = "All";
        instance1.ApplicationName = path;
        instance1.Name = this.getFileName(path, new bool?());
        // ISSUE: variable of a compiler-generated type
        INetFwPolicy2 instance2 = (INetFwPolicy2) Activator.CreateInstance(System.Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        instance1.Direction = d;
        bool flag;
        try
        {
          // ISSUE: reference to a compiler-generated method
          instance2.Rules.Item(instance1.Name);
          flag = true;
        }
        catch
        {
          flag = false;
        }
        if (!Settings.Default.UseFirewall)
        {
          if (!flag)
            return;
          // ISSUE: reference to a compiler-generated method
          instance2.Rules.Remove(instance1.Name);
        }
        else if (action == "1")
        {
          if (!flag)
          {
            // ISSUE: reference to a compiler-generated method
            instance2.Rules.Add(instance1);
          }
        }
        else if (flag)
        {
          // ISSUE: reference to a compiler-generated method
          instance2.Rules.Remove(instance1.Name);
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Error setting firewall rule.", ex.Message);
        File.WriteAllText("crash.txt", ex.ToString());
      }
    }

    private void CheckBoxZone_Checked(object sender, RoutedEventArgs e)
    {
    }

    private void DragMover_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      try
      {
        if (this.WindowState != WindowState.Normal)
          this.WindowState = WindowState.Normal;
        this.DragMove();
      }
      catch
      {
      }
    }

    private async void listBoxZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (this.ModList.SelectedItem == null)
        return;
      try
      {
        MainWindow.ModInfoClass selectedItem = (MainWindow.ModInfoClass) this.ModList.SelectedItem;
        if (selectedItem == null)
          return;
        this.ModName.Text = selectedItem.Title;
        this.Author.Text = selectedItem.Author;
        this.Version.Text = selectedItem.Version;
        this.ModType.Text = selectedItem.fileType.ToString();
        this.FilePath.Text = selectedItem.filePath;
        if (this.Author.Text == "N/A" || this.Version.Text == "N/A")
          this.getAuthorVersionFromDir(selectedItem.filePath);
        int selectedIndex = this.ModList.SelectedIndex;
        if (!File.Exists(selectedItem.CfgPath))
          selectedItem.CfgPath = this.findConfigFile(this.getFileName(this.FilePath.Text, new bool?(false)), selectedItem.filePath, false);
        if (File.Exists(selectedItem.CfgPath))
          this.FillLogWindow(selectedItem.CfgPath);
        else
          this.CfgText.Text = "No Configuration file found. Press 'Attach Config' to attach a setup.\nPlease note, not all plugins come with configurations. This message is not an error.";
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Uh Oh!", ex.ToString());
      }
    }

    public async void FillLogWindow(string file)
    {
      this.SaveCFG.IsEnabled = false;
      this.AttachCFG.IsEnabled = false;
      int thisIndex = this.ModList.SelectedIndex;
      this.CfgText.IsEnabled = false;
      this.CfgText.Text = string.Empty;
      this.ConfigLoading.Visibility = Visibility.Visible;
      List<string> s = await Task.Run<List<string>>((Func<List<string>>) (() => ((IEnumerable<string>) File.ReadAllLines(file)).ToList<string>()));
      for (int index = 0; index < s.Count && thisIndex == this.ModList.SelectedIndex; ++index)
      {
        TextBox cfgText = this.CfgText;
        cfgText.Text = cfgText.Text + s[index] + Environment.NewLine;
        this.ConfigLoading.Text = string.Format("Configuration Text loading line {0}/{1}... Please wait...", (object) index, (object) s.Count);
                MainWindow.DoEvents();
            }
      
      this.AttachCFG.IsEnabled = true;
      this.CfgText.IsEnabled = true;
      this.SaveCFG.IsEnabled = true;
      this.ConfigLoading.Visibility = Visibility.Hidden;
      this.cfgFileLoaded = file;
    }

    public static void SetRtf(RichTextBox rtb, string document, string contents = null)
    {
      if (string.IsNullOrEmpty(contents))
        contents = File.Exists(document) ? File.ReadAllText(document) : "No Configuration file found. Press 'Attach Config' to attach a setup.\nPlease note, not all plugins come with configurations. This message is not an error.";
      using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
      {
        memoryStream.Position = 0L;
        rtb.SelectAll();
        rtb.Selection.Load((Stream) memoryStream, DataFormats.Rtf);
      }
    }

    public static void SaveRtf(RichTextBox rtb, string document)
    {
      using (FileStream fileStream = new FileStream(document, FileMode.Create))
        new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd).Save((Stream) fileStream, DataFormats.Text);
    }

    private void getAuthorVersionFromDir(string p)
    {
      if (!Directory.Exists(p))
        return;
      string str1 = "N/A";
      string str2 = "N/A";
      string[] strArray = new string[2]{ "*.asi", "*.dll" };
      foreach (string searchPattern in strArray)
      {
        foreach (string file in Directory.GetFiles(p, searchPattern, System.IO.SearchOption.AllDirectories))
        {
          try
          {
            str2 = FileVersionInfo.GetVersionInfo(file).FileVersion.ToString();
            str1 = FileVersionInfo.GetVersionInfo(file).CompanyName;
          }
          catch (Exception)
          {
          }
        }
      }
      int index = this.TheList.IndexOf(this.TheList.Where<MainWindow.ModInfoClass>((Func<MainWindow.ModInfoClass, bool>) (x => x.filePath == p)).FirstOrDefault<MainWindow.ModInfoClass>());
      if (index == -1 || !(this.TheList[index].Author != str1) && !(this.TheList[index].Version != str2))
        return;
      this.TheList[index].Author = str1;
      this.Author.Text = str1;
      this.TheList[index].Version = str2;
      this.Version.Text = str2;
      this.SaveCollection();
    }

    private void close_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        this.SaveCollection();
        this.Close();
        Environment.Exit(0);
      }
      catch
      {
        Environment.Exit(0);
      }
    }

    private void minimize_Click(object sender, RoutedEventArgs e) => this.WindowState = WindowState.Minimized;

    private void OptionsButton_Click(object sender, RoutedEventArgs e)
    {
      bool fileDetectionMode = Settings.Default.FileDetectionMode;
      this.Dimmer.Visibility = Visibility.Visible;
      Options options = new Options();
      options.Owner = (Window) this;
      options.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      options.ShowDialog();
      if (this.GameInstallationPath != Settings.Default.InstallPath)
        this.GameInstallationPath = Settings.Default.InstallPath;
      if (this.InstalledModsPath != Settings.Default.ModStoragePath)
      {
        this.InstalledModsPath = Settings.Default.ModStoragePath;
        this.DetectMods();
      }
      if (fileDetectionMode != Settings.Default.FileDetectionMode)
      {
        Settings.Default.FileDetectionMode = fileDetectionMode;
        this.undoAll();
        Settings.Default.FileDetectionMode = !fileDetectionMode;
        this.DetectMods();
      }
      this.isSteam = Settings.Default.isSteam;
      this.isEpic = Settings.Default.isEpic;
      this.Dimmer.Visibility = Visibility.Hidden;
      if (!options.PerformedMigration)
        return;
      this.DetectMods();
    }

    private void EnableAll_Click(object sender, RoutedEventArgs e)
    {
      foreach (MainWindow.ModInfoClass modInfoClass in this.ModList.ItemsSource)
        modInfoClass.IsChecked = true;
    }

    private void DisableAll_Click(object sender, RoutedEventArgs e) => this.DisableAllMods();

    private void StartOnline_Click(object sender, RoutedEventArgs e)
    {
      if (!this.undoAll())
        this.ErrorMessage("[NANNY MODE] Mods installed incorrectly", "You have mods installed that tried to delete vanilla files (or you aborted mod cleanup)... check out the 'incorrectlyInstalledMods.txt file for more information. Cannot continue.");
      else
        this.start(string.Empty, true);
    }

    private void startOnline(bool launchGame)
    {
      try
      {
        if (!this.undoAll())
        {
          this.ErrorMessage("[NANNY MODE] Mods installed incorrectly", "You have mods installed that tried to delete vanilla files... check out the 'incorrectlyInstalledMods.txt file for more information. Cannot continue.");
        }
        else
        {
          if (!launchGame)
            return;
          this.start(string.Empty, true);
        }
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.ToString());
      }
    }

    private void StartSinglePlayer_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        this.EnableSelectedMods();
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.ToString());
      }
    }

    private void AddMod_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        this.openFileDialog.Filter = "mod files|*.lua;*.asi;*.dll;*.cs;*.vb;*.fx;*.py|All Files|*.*";
        if ((this.openFileDialog.ShowDialog() ?? false) != false)
        {
          if (string.IsNullOrWhiteSpace(this.openFileDialog.FileName))
            this.ErrorMessage("There was a problem with your selection.", "It seems you tried to import something but your attempts returned back fruitless. Please try to make your file selection again.");
          else if (!File.Exists(this.openFileDialog.FileName))
            this.ErrorMessage("ERROR!!!", "The file you selected does not exist!");
          else if (!Settings.Default.FileDetectionMode)
            this.ImportAMod_Beta(this.openFileDialog.FileName);
          else
            this.ImportAMod(this.openFileDialog.FileName);
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Houston, we have a problem!", ex.ToString());
      }
    }

    private void ImportAMod_Beta(string p)
    {
      string fileName = System.IO.Path.GetFileName(p);
      string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(p);
      string extension = System.IO.Path.GetExtension(p);
      string part1_1 = this.joinPaths(this.InstalledModsPath, withoutExtension);
      string str = this.joinPaths(part1_1, fileName);
      string part1_2 = this.joinPaths(part1_1, "scripts");
      string part1_3 = this.joinPaths(part1_2, "addins");
      string part1_4 = this.joinPaths(part1_1, "python\\scripts");
      string part1_5 = this.joinPaths(part1_1, "Plugins");
      switch (extension)
      {
        case ".asi":
        case ".fx":
          if (p.ToLower() != str.ToLower())
            this.copyFile(p, str);
          string configFile = this.findConfigFile(withoutExtension, System.IO.Path.GetDirectoryName(p), true);
          if (string.IsNullOrWhiteSpace(configFile) || !File.Exists(configFile))
            break;
          File.Copy(configFile, this.joinPaths(System.IO.Path.GetDirectoryName(str), this.getFileName(configFile, new bool?())), true);
          break;
        case ".cs":
        case ".vb":
          str = this.joinPaths(part1_2, fileName);
          goto case ".asi";
        case ".dll":
          foreach (string injectorName in this.injectorNames)
          {
            if (fileName.ToLower().Contains(injectorName))
              break;
          }
          str = !this.askAQuestion("DLL mod detected.", "Is this a RAGEHook DLL plugin?") ? this.joinPaths(part1_2, fileName) : this.joinPaths(part1_5, fileName);
          goto case ".asi";
        case ".lua":
          str = this.joinPaths(part1_3, fileName);
          goto case ".asi";
        case ".py":
          str = this.joinPaths(part1_4, fileName);
          goto case ".asi";
      }
    }

    private bool askAQuestion(string title, string question)
    {
      AskQuestion askQuestion = new AskQuestion();
      askQuestion.Question.Text = question;
      askQuestion.TitleMessage.Text = title;
      askQuestion.Owner = (Window) this;
      askQuestion.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      askQuestion.ShowDialog();
      return askQuestion.PressedYes;
    }

    private void ImportAMod(string p)
    {
      string fileName1 = this.getFileName(p, new bool?());
      string fileName2 = this.getFileName(p, new bool?(false));
      string fileName3 = this.getFileName(p, new bool?(true));
      string str = string.Empty;
      string empty = string.Empty;
      switch (fileName3)
      {
        case ".asi":
        case ".fx":
          str = this.joinPaths(this.InstalledModsPath, fileName1);
          break;
        case ".cs":
        case ".vb":
          str = this.joinPaths(this.InstalledScriptsPath, fileName1);
          break;
        case ".dll":
          foreach (string injectorName in this.injectorNames)
          {
            if (fileName2.ToLower().Contains(injectorName))
            {
              str = this.joinPaths(this.InstalledModsPath, fileName1);
              break;
            }
          }
          if (string.IsNullOrWhiteSpace(str))
          {
            str = this.joinPaths(this.InstalledScriptsPath, fileName1);
            break;
          }
          break;
        case ".lua":
          str = this.joinPaths(this.InstalledLuaPath, fileName1);
          break;
        case null:
          return;
        default:
          return;
      }
      string configFile = this.findConfigFile(fileName2, System.IO.Path.GetDirectoryName(p), false);
      if (!string.IsNullOrWhiteSpace(configFile) && File.Exists(configFile))
        File.Copy(configFile, this.joinPaths(System.IO.Path.GetDirectoryName(str), this.getFileName(configFile, new bool?())), true);
      if (p.ToLower() != str.ToLower())
        this.copyFile(p, str);
      this.DetectMods();
      this.ErrorMessage("Mod Import complete!", string.Format("{0} has been imported sucessfully!", (object) fileName1));
    }

    private void DeleteMod_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (this.ModList.SelectedIndex == -1)
          return;
        if (!this.undoAll())
        {
          this.ErrorMessage("[NANNY MODE] Mods installed incorrectly", "You have mods installed that tried to delete vanilla files... check out the 'incorrectlyInstalledMods.txt file for more information. Cannot continue.");
        }
        else
        {
          int num = this.ModList.SelectedIndex - 1;
          List<MainWindow.ModInfoClass> modInfoClassList = new List<MainWindow.ModInfoClass>();
          foreach (object selectedItem in (IEnumerable) this.ModList.SelectedItems)
            modInfoClassList.Add((MainWindow.ModInfoClass) selectedItem);
          foreach (MainWindow.ModInfoClass modInfoClass in modInfoClassList)
          {
            if (File.Exists(modInfoClass.filePath))
              FileSystem.DeleteFile(modInfoClass.filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
            else if (Directory.Exists(modInfoClass.filePath))
              FileSystem.DeleteDirectory(modInfoClass.filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
            if (!string.IsNullOrWhiteSpace(modInfoClass.CfgPath) && File.Exists(modInfoClass.CfgPath))
              File.Delete(modInfoClass.CfgPath);
            this.TheList.Remove(modInfoClass);
          }
          modInfoClassList.Clear();
          if (this.ModList.Items.Count > 0)
            this.ModList.SelectedIndex = num;
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Take a nap GTAV Mod Manager!", ex.ToString());
      }
    }

    private void AttachCFG_Click(object sender, RoutedEventArgs e)
    {
      if (Directory.Exists(this.FilePath.Text))
        this.openFileDialog.InitialDirectory = this.FilePath.Text;
      else
        this.openFileDialog.InitialDirectory = this.installedModsPath;
      this.openFileDialog.Filter = "config files|*.txt;*.cfg;*.ini;*.xml;*.log;*.config|All Files|*.*";
      if ((this.openFileDialog.ShowDialog() ?? false) == false)
        return;
      try
      {
        this.cfgFileLoaded = this.openFileDialog.FileName;
        this.FillLogWindow(this.cfgFileLoaded);
        this.cfgFileLoaded = this.openFileDialog.FileName;
        MainWindow.ModInfoClass selectedMod = (MainWindow.ModInfoClass) this.ModList.SelectedItem;
        if (selectedMod == null)
          return;
        int index = this.TheList.IndexOf(this.TheList.Where<MainWindow.ModInfoClass>((Func<MainWindow.ModInfoClass, bool>) (x => x.filePath == selectedMod.filePath)).FirstOrDefault<MainWindow.ModInfoClass>());
        if (index == -1)
          return;
        this.TheList[index].CfgPath = this.cfgFileLoaded;
        this.SaveCollection();
      }
      catch
      {
        this.ErrorMessage("Unable to load config file", "Config file is either corrupt or you tried loading a non-text config file. Please try again.");
        this.cfgFileLoaded = string.Empty;
      }
    }

    private void SaveCFG_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(this.cfgFileLoaded) || !File.Exists(this.cfgFileLoaded))
          return;
        File.WriteAllText(this.cfgFileLoaded, this.CfgText.Text);
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Uh oh!", ex.Message);
      }
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
      if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        return;
      string[] data = (string[]) e.Data.GetData(DataFormats.FileDrop);
      if (data.Length > 0)
        this.ManuallyAddMod(data[0]);
    }

    private void Window_DragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(DataFormats.FileDrop) && sender != e.Source)
        return;
      e.Effects = DragDropEffects.None;
    }

    private void ErrorMessage(string title, string message)
    {
      try
      {
        this.Dispatcher.BeginInvoke((Action)delegate ()
        {
          try
          {
            if (this.Dimmer != null)
              this.Dimmer.Visibility = Visibility.Visible;
          }
          catch
          {
          }
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
          errorMessage.TitleMessage.Text = title ?? "This Error is Missing a Title!";
          errorMessage.Message.Text = message ?? "This Error is missing a message... thats not good.";
          errorMessage.ShowDialog();
          try
          {
            if (this.Dimmer == null)
              return;
            this.Dimmer.Visibility = Visibility.Hidden;
          }
          catch
          {
          }
        });
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(message + Environment.NewLine + ex.ToString(), "Error showing an error!");
      }
    }

    private void start(string mode, bool online)
    {
      try
      {
        if (string.IsNullOrWhiteSpace(mode))
          this.FWRule(this.gta5FilePath, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, "0");
        else
          this.FWRule(this.gta5FilePath, NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT, NET_FW_ACTION_.NET_FW_ACTION_BLOCK, "1");
        if (!Settings.Default.SocialClubOffline)
          mode = string.Empty;
        if (!online && Settings.Default.CustomCMDEnabled)
          mode = !string.IsNullOrWhiteSpace(mode) ? string.Format("{0} {1}", (object) mode, (object) Settings.Default.CustomCMDText) : Settings.Default.CustomCMDText;
        File.WriteAllText(this.commandLineFilePath, mode);
        string arguments = string.Empty;
        if (!Settings.Default.BypassGame)
        {
                    if (File.Exists(this.joinPaths(this.GameInstallationPath, "RAGEPluginHook.exe")))
                        this.gtaLauncherPath = this.gtaLauncherPath.Replace("GTAVLauncher", "RAGEPluginHook").Replace("PlayGTAV", "RAGEPluginHook");
                    else if (!Settings.Default.LaunchPlayGTAV && !online)
                        this.gtaLauncherPath = this.gtaLauncherPath.Replace("GTAVLauncher", "GTA5");
                    else if(Settings.Default.isSteam)
                        this.gtaLauncherPath = "steam://rungameid/271590";
                    else if (Settings.Default.isEpic)
                        this.gtaLauncherPath = "com.epicgames.launcher://apps/0584d2013f0149a791e7b9bad0eec102%3A6e563a2c0f5f46e3b4e88b5f4ed50cca%3A9d2d0eb64d5c44529cece33fe2a46482?action=launch&silent=true";
                    else
                    {
                        if (Settings.Default.SocialClubInTarget && Settings.Default.SocialClubOffline)
                        {
                            this.gtaLauncherPath = this.gtaLauncherPath.Replace("GTAVLauncher", "PlayGTAV");
                            arguments = mode;
                        }
                        else if (File.Exists(this.joinPaths(this.GameInstallationPath, "PlayGTAV.exe")))
                            this.gtaLauncherPath = this.gtaLauncherPath.Replace("GTAVLauncher", "PlayGTAV");
                        else
                            this.gtaLauncherPath = this.gtaLauncherPath.Replace("PlayGTAV", "GTAVLauncher");
                    }
                        
          try
          {
                        Process.Start(new ProcessStartInfo(this.gtaLauncherPath, arguments)
            {
              WorkingDirectory = this.GameInstallationPath
            });
          }
          catch (System.IO.FileNotFoundException)
          {
            this.ErrorMessage("FILE NOT FOUND", this.gtaLauncherPath);
          }
          catch (System.ComponentModel.Win32Exception)
          {
              this.ErrorMessage("FILE NOT FOUND", this.gtaLauncherPath);
          }
          catch (Exception ex)
          {
            this.ErrorMessage("CRASH", ex.ToString());
          }
        }
        if (!Settings.Default.DisableOnExit || online)
        {
          this.Close();
          Application.Current.Shutdown();
        }
        else
        {
          this.Dimmer.Visibility = Visibility.Visible;
          this.DimmedText.Text = "Waiting for GTA5.exe...";
          MainWindow.DoEvents();
          this.DimmedText.Visibility = Visibility.Visible;
          this.OptionsButton.IsEnabled = false;
          Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
          this.WaitForProcess("gta5");
        }
      }
      catch (Exception ex1)
      {
        try
        {
          string str = ex1.InnerException != null ? ex1.InnerException.Message : "";
          this.ErrorMessage("Could not start The game", ex1.Message + Environment.NewLine + str);
        }
        catch (Exception ex2)
        {
          this.ErrorMessage("Starting the game critical error", ex1.Message + Environment.NewLine + ex2.ToString());
        }
      }
    }

    private void WaitForProcess(string proc) => new Thread((ThreadStart) (() =>
    {
      Process[] pArray = new Process[0];
      int count = Settings.Default.DetectionTimeout;
      for (; pArray.Length == 0; pArray = Process.GetProcessesByName(proc))
      {
        Thread.Sleep(1000);
        if (count == 0)
        {
          this.shutDownDimmer(false);
          return;
        }
        --count;
        this.Dispatcher.BeginInvoke((Action)delegate ()
        {
          if (this.WindowState == WindowState.Minimized)
            return;
          this.DimmedText.Text = string.Format("Waiting for {0}.exe...Timeout in {1}", (object) proc, (object) count);
        });
      }
      this.Dispatcher.BeginInvoke((Action)delegate ()
      {
        this.DimmedText.Text = string.Format("{0}.exe found... waiting for it to exit.", (object) proc.ToUpper());
        foreach (Process process in pArray)
        {
          process.EnableRaisingEvents = true;
          process.Exited += new EventHandler(this.p_Exited);
        }
        this.WindowState = WindowState.Minimized;
      });
    }))
    {
      IsBackground = true
    }.Start();

    private void p_Exited(object sender, EventArgs e)
    {
      this.Dispatcher.BeginInvoke((Action)delegate ()
      {
        this.WindowState = WindowState.Normal;
        this.DimmedText.Text = "GTA5.exe has quit. Cleaning up mods...";
        MainWindow.DoEvents();
      });
      Thread.Sleep(2000);
      this.Dispatcher.BeginInvoke((Action)delegate()
      {
        try
        {
          this.shutDownDimmer(this.ForceQuit);
        }
        catch (Exception ex)
        {
          this.ErrorMessage("P_EXIT Error", ex.ToString());
        }
      });
    }

    private void shutDownDimmer(bool shutdownApp)
    {
      try
      {
        this.Dispatcher.BeginInvoke((Action)delegate ()
        {
          this.undoAll();
          try
          {
            this.Dimmer.Visibility = Visibility.Hidden;
            this.DimmedText.Visibility = Visibility.Hidden;
            this.OptionsButton.IsEnabled = true;
            if (!shutdownApp)
              return;
            Application.Current.Shutdown();
          }
          catch (Exception ex)
          {
            this.ErrorMessage("Crashed!", ex.ToString());
          }
        });
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Crashed!", ex.ToString());
      }
    }

    private void DetectMods()
    {
      try
      {
        if (this.TheList != null)
          this.TheList.Clear();
        else
          this.TheList = new ObservableCollection<MainWindow.ModInfoClass>();
        if (!Settings.Default.FileDetectionMode)
          this.DetectModsBeta();
        else
          this.DetectModsLegacy();
        try
        {
          this.ModList.ItemsSource = (IEnumerable) this.TheList;
          this.AutoSizeColumns();
          if (this.ModList.Items.Count > 0)
          {
            this.ProgramTitle.Text = "Mod Manager";
            this.MainVersion.Visibility = Visibility.Visible;
            if (this.ModList.SelectedIndex != -1)
              return;
            this.ModList.SelectedIndex = 0;
          }
          else
          {
            this.ProgramTitle.Text = "No Mods installed";
            this.MainVersion.Visibility = Visibility.Hidden;
            this.CfgText.Text = "Looks like you need to install some mods. Go ahead and click on the gear on the top right to Migrate mods you installed in the GTAV folder. If you have not downloaded any mods, please visit www.gta5-mods.com to get started.";
            this.FilePath.Text = "";
          }
        }
        catch (Exception ex)
        {
          this.ErrorMessage("Game over man, game over!", ex.ToString());
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Game over man, game over!", ex.ToString());
      }
    }

    private void AutoSizeColumns()
    {
      if (!(this.ModList.View is GridView view))
        return;
      foreach (GridViewColumn column in (Collection<GridViewColumn>) view.Columns)
      {
        if (double.IsNaN(column.Width))
          column.Width = column.ActualWidth;
        column.Width = double.NaN;
      }
    }

    private void DetectModsLegacy()
    {
      try
      {
        this.injectors.Clear();
        if (string.IsNullOrWhiteSpace(this.InstalledModsPath) || string.IsNullOrWhiteSpace(this.InstalledLuaPath) || string.IsNullOrWhiteSpace(this.InstalledScriptsPath) || string.IsNullOrWhiteSpace(this.InstalledRPFPath))
        {
          this.ErrorMessage("One of your mods folder is not configured!", "Your program is not configured correctly. Please visit the setup wizard (in options) and configure your mods location again.");
        }
        else
        {
          List<string> stringList = new List<string>();
          stringList.Add(this.InstalledModsPath);
          this.configs.Clear();
          this.tryLoadLua = File.Exists(this.joinPaths(this.InstalledModsPath, "lua.asi")) || File.Exists(this.joinPaths(this.InstalledModsPath, "lua_sdk.asi"));
          this.tryLoadNetScripts = File.Exists(this.joinPaths(this.InstalledModsPath, "scripthookvdotnet.dll")) || File.Exists(this.joinPaths(this.InstalledModsPath, "scripthookvdotnet.asi"));
          this.tryLoadPythonScripts = File.Exists(this.joinPaths(this.InstalledModsPath, "scripthookvpy3k.asi"));
          this.tryLoadlspdfr = Directory.Exists(this.joinPaths(this.InstalledModsPath, "lspdfr"));
          if (!File.Exists(this.joinPaths(this.InstalledModsPath, "OpenIV.asi")) && File.Exists(this.joinPaths(this.GameInstallationPath, "OpenIV.asi")))
            File.Move(this.joinPaths(this.GameInstallationPath, "OpenIV.asi"), this.joinPaths(this.InstalledModsPath, "OpenIV.asi"));
          this.tryLoadRPFScripts = File.Exists(this.joinPaths(this.InstalledModsPath, "OpenIV.asi"));
          if (this.tryLoadLua)
            stringList.Add(this.InstalledLuaPath);
          if (this.tryLoadNetScripts)
            stringList.Add(this.InstalledScriptsPath);
          if (Directory.Exists(this.InstalledRPFPath))
            stringList.Add(this.InstalledRPFPath);
          if (this.tryLoadPythonScripts)
            stringList.Add(this.InstalledPythonPath);
          if (this.tryLoadlspdfr)
            stringList.Add(this.joinPaths(this.InstalledModsPath, "lspdfr\\Plugins"));
          foreach (string path in stringList)
          {
            string searchPattern = "*.*";
            if (path.Contains("addins"))
              searchPattern = "*.*lua";
            else if (path.ToLower().Contains("\\rpf"))
              searchPattern = "*.rpf";
            else if (path.ToLower().Contains("\\python\\scripts"))
              searchPattern = "*.*py";
            else if (path.ToLower().Contains("lspdfr"))
              searchPattern = "LSPD First Response.dll";
            if (Directory.Exists(path))
            {
              foreach (string file in Directory.GetFiles(path, searchPattern, System.IO.SearchOption.TopDirectoryOnly))
                this.AddAModLegacy(file);
            }
            else
              this.ErrorMessage("Say Whaaaaaaaat", string.Format("the directory {0} does not exist on your machine!", (object) path));
          }
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Game over man, game over!", ex.ToString());
      }
    }

    private void DetectModsBeta()
    {
      try
      {
        bool flag = false;
        foreach (string directory in Directory.GetDirectories(this.InstalledModsPath, "*", System.IO.SearchOption.TopDirectoryOnly))
        {
          if (System.IO.Path.GetFileName(directory).ToLower() == "rpf")
            flag = true;
          else
            this.AddAModBeta(directory);
        }
        if (!flag)
          return;
        foreach (string file in Directory.GetFiles(this.InstalledRPFPath, "*.rpf", System.IO.SearchOption.TopDirectoryOnly))
          this.AddAModLegacy(file);
      }
      catch (Exception ex)
      {
        this.ErrorMessage(nameof (DetectModsBeta), ex.ToString());
      }
    }

    private void AddAModBeta(string dir)
    {
      MainWindow.ModInfoClass modInfoClass = this.TheBakList.Find((Predicate<MainWindow.ModInfoClass>) (x => x.filePath == dir));
      if (modInfoClass == null)
      {
        modInfoClass = new MainWindow.ModInfoClass()
        {
          Author = "N/A",
          Version = "N/A"
        };
        modInfoClass.Title = System.IO.Path.GetFileName(dir);
        modInfoClass.filePath = dir;
        modInfoClass.IsChecked = false;
        modInfoClass.fileType = MainWindow.ModInfoClass.Type.Folder;
        modInfoClass.CfgPath = this.findConfigFile(modInfoClass.Title, dir, false);
      }
      this.TheList.Add(modInfoClass);
    }

    private void AddAModLegacy(string file)
    {
      try
      {
        string lower1 = System.IO.Path.GetExtension(file).ToLower();
        string installedModsPath = this.InstalledModsPath;
        bool flag = false;
        switch (lower1)
        {
          case ".txt":
          case ".ini":
          case ".cfg":
          case ".xml":
          case ".log":
            if (!file.ToLower().Contains("\\fov.ini") && !file.ToLower().Contains("\\version.txt"))
            {
              this.configs.Add(file);
              break;
            }
            goto case ".dll";
          case ".disabled_lua":
          case ".disabled_dll":
          case ".disabled_py":
          case ".disabled_cs":
          case ".disabled_vb":
            this.disableEnableMod(true, file.Replace("disabled_", ""), file);
            file = file.Replace("disabled_", "");
            flag = true;
            goto case ".dll";
          case ".disabled":
            this.disableEnableMod(true, file.Replace(".disabled", ""), file);
            file = file.Replace(".disabled", "");
            flag = true;
            goto case ".dll";
          case ".asi":
          case ".fx":
            if (file.Contains("\\scripts\\"))
              break;
            goto case ".dll";
          case ".dll":
            MainWindow.ModInfoClass modInfoClass = this.TheBakList.Find((Predicate<MainWindow.ModInfoClass>) (x => x.filePath == file));
            if (modInfoClass == null)
            {
              modInfoClass = new MainWindow.ModInfoClass()
              {
                Author = "N/A",
                Version = "N/A"
              };
              modInfoClass.Title = this.getFileName(file, new bool?(false));
              modInfoClass.filePath = file;
              modInfoClass.fileExt = lower1;
            }
            else
              flag = !modInfoClass.IsChecked;
            string part1;
            switch (modInfoClass.fileExt)
            {
              case ".dll":
                part1 = this.InstalledScriptsPath;
                string lower2 = this.getFileName(file, new bool?(false)).ToLower();
                switch (lower2)
                {
                  case "scripthookv":
                  case "scripthookvdotnet":
                  case "dinput8":
                  case "dsound":
                    if (lower2.Contains("\\scripts\\"))
                      return;
                    this.injectors.Add(file);
                    return;
                  case "d3d11":
                    if (lower2.Contains("\\scripts\\"))
                      return;
                    this.tryLoadSweetFX = true;
                    this.injectors.Add(file);
                    return;
                  case "dxgi":
                    modInfoClass.fileType = MainWindow.ModInfoClass.Type.Injector;
                    part1 = this.InstalledModsPath;
                    break;
                  case "lspd first response":
                    modInfoClass.fileType = MainWindow.ModInfoClass.Type.Injector;
                    part1 = this.joinPaths(this.InstalledModsPath, "lspdfr\\Plugins");
                    break;
                  default:
                    modInfoClass.fileType = MainWindow.ModInfoClass.Type.DotNet_Mod;
                    break;
                }
                try
                {
                  modInfoClass.Version = FileVersionInfo.GetVersionInfo(file).FileVersion.ToString();
                  modInfoClass.Author = FileVersionInfo.GetVersionInfo(file).CompanyName;
                  break;
                }
                catch
                {
                  break;
                }
              case ".cs":
              case ".vb":
                part1 = this.InstalledScriptsPath;
                modInfoClass.fileType = MainWindow.ModInfoClass.Type.DotNet_Mod;
                break;
              case ".py":
                if (!this.tryLoadPythonScripts)
                  return;
                part1 = this.InstalledPythonPath;
                modInfoClass.fileType = MainWindow.ModInfoClass.Type.Python_Mod;
                break;
              case ".asi":
                part1 = this.InstalledModsPath;
                modInfoClass.fileType = MainWindow.ModInfoClass.Type.ASI_Mod;
                if (file.ToLower().Contains("lua"))
                  this.tryLoadLua = true;
                else if (file.ToLower().Contains("scripthookvdotnet"))
                  this.tryLoadNetScripts = true;
                else if (file.ToLower().Contains("openiv.asi"))
                  this.tryLoadRPFScripts = true;
                else if (file.ToLower().Contains("scripthookvpy3k.asi"))
                  this.tryLoadPythonScripts = true;
                try
                {
                  modInfoClass.Version = FileVersionInfo.GetVersionInfo(file).FileVersion.ToString();
                  modInfoClass.Author = FileVersionInfo.GetVersionInfo(file).CompanyName;
                  break;
                }
                catch
                {
                  break;
                }
              case ".fx":
                part1 = this.InstalledModsPath;
                modInfoClass.fileType = MainWindow.ModInfoClass.Type.SweetFX;
                break;
              case ".rpf":
                if (!this.tryLoadRPFScripts && Settings.Default.FileDetectionMode)
                  return;
                part1 = this.InstalledRPFPath;
                modInfoClass.fileType = MainWindow.ModInfoClass.Type.RPF_Mod;
                break;
              case ".lua":
                if (!this.tryLoadLua)
                  return;
                part1 = this.InstalledLuaPath;
                modInfoClass.fileType = MainWindow.ModInfoClass.Type.LUA_Mod;
                break;
              case ".ini":
                if (!file.ToLower().Contains("\\fov.ini"))
                  return;
                part1 = this.InstalledModsPath;
                modInfoClass.fileType = MainWindow.ModInfoClass.Type.ASI_Mod;
                break;
              case ".exe":
                if (!file.ToLower().Contains("\\gta5.exe"))
                  return;
                part1 = this.InstalledModsPath;
                modInfoClass.fileType = MainWindow.ModInfoClass.Type.Game_Binary;
                break;
              case null:
                return;
              default:
                return;
            }
            modInfoClass.IsChecked = !flag && File.Exists(this.joinPaths(part1, this.getFileName(modInfoClass.filePath, new bool?())));
            if (!File.Exists(modInfoClass.CfgPath))
              modInfoClass.CfgPath = !Settings.Default.FileDetectionMode ? this.findConfigFile(modInfoClass.Title, modInfoClass.filePath, false) : this.findConfigFile(modInfoClass.Title, System.IO.Path.GetDirectoryName(modInfoClass.filePath), false);
            this.TheList.Add(modInfoClass);
            break;
          case ".cs":
          case ".vb":
            if (!file.Contains("\\scripts\\"))
              break;
            goto case ".dll";
          case ".py":
            if (!file.Contains("\\python\\scripts"))
              break;
            goto case ".dll";
          case ".rpf":
            if (!file.Contains("\\RPF\\"))
              break;
            goto case ".dll";
          case ".lua":
            if (!file.Contains("addins"))
              break;
            goto case ".dll";
          case ".exe":
            if (!file.ToLower().Contains("gta5.exe"))
              break;
            goto case ".dll";
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Ruh Roh George!!", file + Environment.NewLine + ex.Message);
      }
    }

    private string findConfigFile(string modName, string directory, bool checkNameOnly)
    {
      try
      {
        bool flag = directory.ToLower().EndsWith(".rpf");
        string path = directory;
        if (File.Exists(path))
          path = System.IO.Path.GetDirectoryName(path);
        string searchPattern = modName + "*";
        if (!checkNameOnly)
        {
          if (flag)
            searchPattern = modName + ".*";
          else if (!Settings.Default.FileDetectionMode)
            searchPattern = "*.*";
        }
        System.IO.SearchOption searchOption = System.IO.SearchOption.AllDirectories;
        if (Settings.Default.FileDetectionMode)
          searchOption = System.IO.SearchOption.TopDirectoryOnly;
        if (!directory.ToLower().Contains(this.InstalledModsPath.ToLower()))
          searchOption = System.IO.SearchOption.TopDirectoryOnly;
        foreach (string file in Directory.GetFiles(path, searchPattern, searchOption))
        {
          switch (System.IO.Path.GetExtension(file).ToLower())
          {
            case ".txt":
            case ".cfg":
            case ".xml":
            case ".ini":
            case ".lua":
            case ".config":
            case ".log":
            case ".py":
            case ".cs":
            case ".vb":
              if (!file.Contains("\\version.txt"))
                return file;
              break;
          }
        }
      }
      catch
      {
      }
      return string.Empty;
    }

    private void ManuallyAddMod(string location)
    {
      try
      {
        string str = string.Empty;
        switch (this.getFileName(location, new bool?(true)))
        {
          case ".lua":
            str = this.joinPaths(this.InstalledLuaPath, this.getFileName(location, new bool?()));
            break;
          case ".asi":
          case ".fx":
          case ".exe":
            str = this.joinPaths(this.InstalledModsPath, this.getFileName(location, new bool?()));
            break;
          case ".dll":
            foreach (string injectorName in this.injectorNames)
            {
              if (location.ToLower().Contains(injectorName))
              {
                str = this.joinPaths(this.InstalledModsPath, this.getFileName(location, new bool?()));
                break;
              }
            }
            if (string.IsNullOrWhiteSpace(str))
            {
              str = this.joinPaths(this.InstalledScriptsPath, this.getFileName(location, new bool?()));
              break;
            }
            break;
          default:
            this.ErrorMessage("This file is not a supported mod.", "This isn't a supported mod, please try your selection again.");
            return;
        }
        if (File.Exists(str))
        {
          this.ErrorMessage("Mod already is installed!", "Mod already exists. Is this an udpated version? Delete the mod first before adding it again");
        }
        else
        {
          this.copyFile(location, str);
          MainWindow.ModInfoClass modInfoClass = new MainWindow.ModInfoClass();
          modInfoClass.filePath = str;
          modInfoClass.Title = this.getFileName(str, new bool?(false));
          modInfoClass.IsChecked = true;
          modInfoClass.fileExt = this.getFileName(location, new bool?(true));
          try
          {
            modInfoClass.Version = FileVersionInfo.GetVersionInfo(location).FileVersion.ToString();
            modInfoClass.Author = FileVersionInfo.GetVersionInfo(location).CompanyName;
          }
          catch
          {
          }
          this.TheList.Add(modInfoClass);
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("You had one job Mod Manager, ONE JOB!!!", ex.ToString());
      }
    }

    private void DisableAllMods()
    {
      foreach (MainWindow.ModInfoClass modInfoClass in this.ModList.ItemsSource)
        modInfoClass.IsChecked = false;
    }

    private string getFileName(string path, bool? ext)
    {
      if (!ext.HasValue)
        return System.IO.Path.GetFileName(path);
      return (ext ?? false) != false ? System.IO.Path.GetExtension(path).ToLower() : System.IO.Path.GetFileNameWithoutExtension(path);
    }

    private bool undoAll()
    {
      try
      {
        if (Settings.Default.UndoAllNannyPrompt)
        {
          if (!this.askAQuestion("Mod removal Nanny Mod Warning", "Since people cannot read, I have to hold your hand and ask you: Pressing YES will clear your GTAV Game installation folder of all installed mods, INCLUDING ONES THAT YOU PLACED THERE MANUALLY. By Pressing YES you will never be warned about this again."))
            return false;
          Settings.Default.UndoAllNannyPrompt = false;
          Settings.Default.Save();
        }
        foreach (string part2 in new List<string>()
        {
          "SweetFX",
          "scripts",
          "python",
          "lspdfr",
          "mods",
          "Reshade",
          "Plugins",
          "Licenses",
          "addins"
        })
        {
          string dir = this.joinPaths(this.GameInstallationPath, part2);
          try
          {
            this.DeleteDirectory(dir, true);
          }
          catch
          {
            try
            {
              Thread.Sleep(1000);
              this.DeleteDirectory(dir, true);
            }
            catch (Exception ex)
            {
              this.ErrorMessage("We Crashed!", string.Format("Error trying to delete the folder {0} from your game directory\n{1}", (object) part2, (object) ex.ToString()));
            }
          }
        }
        foreach (MainWindow.ModInfoClass modInfoClass in this.ModList.ItemsSource)
        {
          try
          {
            if (modInfoClass.fileType == MainWindow.ModInfoClass.Type.Folder)
            {
              if (!this.loadFilesFromDirectory(modInfoClass.filePath, System.IO.Path.GetFileName(modInfoClass.filePath), false))
                return false;
            }
          }
          catch (Exception ex)
          {
            this.ErrorMessage("I Crashed!", ex.ToString());
          }
        }
        foreach (string file in Directory.GetFiles(this.GameInstallationPath, "*.*"))
        {
          try
          {
            string fileName1 = this.getFileName(file, new bool?(true));
            string fileName2 = this.getFileName(file, new bool?());
            switch (fileName1)
            {
              case ".dll":
                using (List<string>.Enumerator enumerator = this.injectorNames.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    string current = enumerator.Current;
                    if (file.ToLower().Contains("\\" + current + ".dll"))
                    {
                      int num = 0;
                      while (File.Exists(file))
                      {
                        try
                        {
                          File.Delete(file);
                          break;
                        }
                        catch
                        {
                          if (num > 20)
                          {
                            File.Delete(file);
                            break;
                          }
                          Thread.Sleep(1000);
                          ++num;
                        }
                      }
                      try
                      {
                        File.Delete(file);
                        break;
                      }
                      catch
                      {
                        Thread.Sleep(5000);
                        File.Delete(file);
                        break;
                      }
                    }
                  }
                  continue;
                }
              case ".asi":
              case ".fx":
              case ".mp3":
              case ".png":
              case ".metagen":
                File.Delete(file);
                try
                {
                  string possibleLocation = this.joinPaths(this.InstalledModsPath, fileName2);
                  if (this.TheList.First<MainWindow.ModInfoClass>((Func<MainWindow.ModInfoClass, bool>) (x => x.filePath == possibleLocation)) != null)
                  {
                    this.DeleteFile(file);
                    break;
                  }
                  break;
                }
                catch
                {
                  break;
                }
              case ".exe":
                if (fileName2.ToLower().Contains("gta5.exe"))
                {
                  if (File.Exists(this.gta5FilePath + ".original"))
                  {
                    File.Delete(this.gta5FilePath);
                    File.Move(this.gta5FilePath + ".original", this.gta5FilePath);
                    continue;
                  }
                  continue;
                }
                continue;
              case ".log":
              case ".txt":
              case ".cfg":
              case ".xml":
              case ".ini":
              case ".rtf":
                if (!(fileName2 == "version.txt") && !(fileName2 == "commandline.txt"))
                {
                  File.Delete(file);
                  continue;
                }
                continue;
              default:
                continue;
            }
          }
          catch (Exception ex)
          {
            this.ErrorMessage("Fatality!!!!", "One of your mods failed to undo...\n" + ex.Message);
            File.AppendAllText("error.log", ex.ToString());
          }
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("That's a Crash!", ex.ToString());
      }
      return true;
    }

    private void EnableDisableMod(MainWindow.ModInfoClass m)
    {
      try
      {
        switch (m.fileType)
        {
          case MainWindow.ModInfoClass.Type.Injector:
          case MainWindow.ModInfoClass.Type.ASI_Mod:
            if (m.filePath.ToLower().Contains("lspdfr"))
            {
              this.tryLoadlspdfr = m.IsChecked;
              break;
            }
            this.symlinkFile(m.filePath, string.Empty, m.IsChecked);
            break;
          case MainWindow.ModInfoClass.Type.DotNet_Mod:
            if (!m.filePath.Contains("\\scripts"))
              break;
            string backupFile1 = m.filePath.Replace(m.fileExt, ".disabled_" + m.fileExt.Trim('.'));
            this.disableEnableMod(m.IsChecked, m.filePath, backupFile1);
            break;
          case MainWindow.ModInfoClass.Type.LUA_Mod:
            if (!m.filePath.Contains("\\addins"))
              break;
            string backupFile2 = m.filePath.Replace(m.fileExt, ".disabled_" + m.fileExt.Trim('.'));
            this.disableEnableMod(m.IsChecked, m.filePath, backupFile2);
            break;
          case MainWindow.ModInfoClass.Type.SweetFX:
            this.hardlinkFile(m.filePath, string.Empty, m.IsChecked);
            break;
          case MainWindow.ModInfoClass.Type.RPF_Mod:
            this.RPFHandler(m.filePath, m.CfgPath, m.IsChecked);
            break;
          case MainWindow.ModInfoClass.Type.Python_Mod:
            if (!m.filePath.Contains("\\python\\scripts"))
              break;
            string backupFile3 = m.filePath.Replace(m.fileExt, ".disabled_" + m.fileExt.Trim('.'));
            this.disableEnableMod(m.IsChecked, m.filePath, backupFile3);
            break;
          case MainWindow.ModInfoClass.Type.Game_Binary:
            if (m.filePath.ToLower().Contains("\\gta5.exe") && !File.Exists(this.gta5FilePath + ".original"))
              File.Move(this.gta5FilePath, this.gta5FilePath + ".original");
            if (File.Exists(this.gta5FilePath + ".original"))
            {
              if (File.Exists(this.gta5FilePath))
                File.Delete(this.gta5FilePath);
              if (m.IsChecked)
              {
                this.symlinkFile(m.filePath, string.Empty, true);
                break;
              }
              File.Move(this.gta5FilePath + ".original", this.gta5FilePath);
              break;
            }
            this.ErrorMessage("GTA5.exe backup not found.", "You're trying to load a modified GTA5.exe but no backup was found. This file will not be loaded.");
            break;
          case MainWindow.ModInfoClass.Type.Unknown:
            --this.enabledModCount;
            break;
          case MainWindow.ModInfoClass.Type.Folder:
            this.disableEnableFolderMod(m);
            break;
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Hey Niko, want to go bowling?", "Looks like your mod has an issue.\n" + ex.Message);
      }
    }

    private void disableEnableFolderMod(MainWindow.ModInfoClass m)
    {
      try
      {
        this.loadFilesFromDirectory(m.filePath, System.IO.Path.GetFileName(m.filePath), m.IsChecked);
      }
      catch (Exception ex)
      {
        this.ErrorMessage("DisableENableFolderMod Crash", ex.ToString());
      }
    }

    private void EnableSelectedMods()
    {
      try
      {
        this.SaveCollection();
        this.enabledModCount = 0;
        foreach (MainWindow.ModInfoClass m in (IEnumerable<MainWindow.ModInfoClass>) this.TheList.OrderBy<MainWindow.ModInfoClass, int>((Func<MainWindow.ModInfoClass, int>) (x => x.IsChecked ? 1 : 0)))
        {
          try
          {
            if (m.IsChecked)
              ++this.enabledModCount;
            this.EnableDisableMod(m);
          }
          catch (Exception ex)
          {
            this.ErrorMessage("Oh boy, something went wrong with this mod", m.Title + Environment.NewLine + ex.ToString());
          }
        }
        if (this.enabledModCount == 0)
        {
          if (!this.undoAll())
            this.ErrorMessage("[NANNY MODE] Mod Installed incorrectly...", "You have a mod installed that will delete vanilla files (or you aborted mod removal)... install your mods properly and try again.");
          this.start(string.Empty, false);
        }
        else
        {
          if (Settings.Default.FileDetectionMode)
            this.EnableSelectedModsLegacy();
          this.start("-scOfflineOnly", false);
        }
      }
      catch (Exception ex)
      {
        int num = (int) MessageBox.Show(ex.ToString());
      }
    }

    private void EnableSelectedModsLegacy()
    {
      try
      {
        bool flag1 = false;
        bool flag2 = false;
        foreach (string injector in this.injectors)
        {
          string lower = injector.ToLower();
          if (lower.Contains("\\scripthookv.dll") || lower.Contains("\\scripthookvdotnet.dll"))
          {
            if (File.Exists(injector))
              flag1 = true;
          }
          else if (lower.Contains("\\dinput8.dll") && File.Exists(injector))
            flag2 = true;
          this.symlinkFile(injector, string.Empty, true);
        }
        if (this.tryLoadLua || this.tryLoadNetScripts)
          this.symlinkDirectory(this.InstalledScriptsPath, true);
        if (this.tryLoadPythonScripts)
          this.symlinkDirectory(this.joinPaths(this.installedModsPath, "python"), true);
        if (this.tryLoadSweetFX)
          this.copyDirectory(this.joinPaths(this.InstalledModsPath, "SweetFX"), this.joinPaths(this.GameInstallationPath, "SweetFX"));
        if (this.tryLoadlspdfr)
          this.loadFilesFromDirectory(this.joinPaths(this.InstalledModsPath, "lspdfr"), "lspdfr", true);
        else
          this.loadFilesFromDirectory(this.joinPaths(this.InstalledModsPath, "lspdfr"), "lspdfr", false);
        foreach (string config in this.configs)
        {
          if (!config.Contains("\\version.txt"))
            this.symlinkFile(config, string.Empty, true);
        }
        List<string> values = new List<string>();
        if (!flag1)
          values.Add("ScripthookV.dll");
        if (!flag2)
          values.Add("dinput8.dll");
        if (values.Count > 0)
          this.ErrorMessage("Missing vital files!", "You're trying to start the game modded but you're missing these files:\n" + string.Join("  ", (IEnumerable<string>) values) + "\nAborting Launch.");
      }
      catch (Exception ex)
      {
        this.ErrorMessage("EnableSelectedModsLegacy Error", ex.ToString());
      }
    }

    private bool loadFilesFromDirectory(string p, string parentDir, bool install)
    {
      try
      {
        if (!Directory.Exists(p))
          return true;
        foreach (string directory in Directory.GetDirectories(p, "*", System.IO.SearchOption.AllDirectories))
        {
          string str = directory.Replace(this.joinPaths(this.InstalledModsPath, parentDir), this.GameInstallationPath);
          if (install)
          {
            if (!Directory.Exists(str))
            {
              switch (new DirectoryInfo(str).Name.ToLower())
              {
                case "singleplayergarage":
                case "singleplayerapartment":
                case "superyacht":
                case "MyRides":
                  this.symlinkDirectory(directory, str, true);
                  break;
                default:
                  Directory.CreateDirectory(str);
                  break;
              }
            }
          }
          else if (!this.DeleteDirectory(str, true))
          {
            File.AppendAllLines("incorrectlyInstalledMod.log", (IEnumerable<string>) new string[4]
            {
              p,
              "\t=>\t",
              str,
              "\n"
            });
            return false;
          }
        }
        string[] files = Directory.GetFiles(p, "*.*", System.IO.SearchOption.AllDirectories);
        string lower = this.joinPaths(this.GameInstallationPath, "version.txt").ToLower();
        foreach (string str in files)
        {
          this.getFileName(str, new bool?());
          string altDestination = str.Replace(this.joinPaths(this.InstalledModsPath, parentDir), this.GameInstallationPath);
          if (!(altDestination.ToLower() == lower) && !this.hardlinkFile(str, altDestination, install))
          {
            File.AppendAllLines("incorrectlyInstalledMod.log", (IEnumerable<string>) new string[4]
            {
              p,
              "\t=>\t",
              altDestination,
              "\n"
            });
            return false;
          }
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("Unable to load/unload mod!", string.Format("There was an error when trying to load one of your mods: {0} {1}", (object) p, (object) parentDir));
        this.ErrorMessage("More Details", ex.ToString());
      }
      return true;
    }

    private void RPFHandler(string rpfLocation, string configPath, bool enableMod)
    {
      try
      {
        string newValue = this.GameRPFPath.ToLower();
        if (!newValue.EndsWith("\\mods"))
          newValue = this.GameInstallationPath + "\\mods";
        if (!File.Exists(configPath))
        {
          this.ErrorMessage("RPF loading failed.", "You have an RPF mod selected but it's missing it's configuration file. Please re-import this mod using the import wizard.");
        }
        else
        {
          string str1 = File.ReadAllText(configPath);
          string str2 = str1;
          string str3 = str1 + ".original";
          string str4 = str1.ToLower().Replace(this.GameInstallationPath.ToLower(), newValue);
          if (!str4.Contains(newValue))
          {
            this.ErrorMessage("RPF Error has occured", "please see the error log for more information. Send this to Bilago@gmail.com");
            this.crashReport(new Exception("RPF Error. Destination isn't to a mod folder."), "ModRPFPath: " + newValue);
          }
          else
          {
            string directoryName = System.IO.Path.GetDirectoryName(str4);
            if (enableMod)
            {
              if (!Directory.Exists(directoryName))
              {
                try
                {
                  Directory.CreateDirectory(directoryName);
                  if (!Directory.Exists(directoryName))
                    throw new Exception("Couldn't create directory!");
                }
                catch
                {
                  this.ErrorMessage("RPF Failure!", "Unable to create directory:\n" + directoryName);
                  return;
                }
              }
            }
            if (File.Exists(str3))
            {
              File.Delete(str2);
              File.Move(str3, str2);
            }
            this.hardlinkFile(rpfLocation, str4, enableMod);
          }
        }
      }
      catch (Exception ex)
      {
        string str = ex.InnerException != null ? ex.InnerException.Message : "";
        this.ErrorMessage("Whoop, there it is.", string.Format("There was an error!\n{0}\n{1}", (object) ex.Message, (object) str));
        this.crashReport(ex, "RPF Handler");
      }
    }

    private void disableEnableMod(bool enable, string filePath, string backupFile)
    {
      try
      {
        string fileName = this.getFileName(filePath, new bool?(true));
        if (enable)
        {
          if (filePath.Contains(fileName + fileName))
            filePath = filePath.Replace(fileName + fileName, fileName);
          if (!File.Exists(backupFile))
            return;
          if (!File.Exists(filePath))
            File.Move(backupFile, filePath);
          else
            File.Delete(backupFile);
        }
        else
        {
          if (backupFile.Contains(fileName + fileName))
            backupFile = backupFile.Replace(fileName + fileName, fileName);
          if (File.Exists(filePath))
          {
            File.Delete(backupFile);
            File.Move(filePath, backupFile);
          }
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("No no no no no no!", "Error disabling mod!!\n" + ex.ToString());
      }
    }

    private void ModList_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      try
      {
        System.Type type = VisualTreeHelper.GetParent((DependencyObject) e.OriginalSource).GetType();
        if (type != typeof (ContentPresenter) && type != typeof (ListViewItem) || this.ModList.SelectedItem == null)
          return;
        MainWindow.ModInfoClass selectedMod = (MainWindow.ModInfoClass) this.ModList.SelectedItem;
        int index = this.TheList.IndexOf(this.TheList.Where<MainWindow.ModInfoClass>((Func<MainWindow.ModInfoClass, bool>) (x => x.Title == selectedMod.Title)).FirstOrDefault<MainWindow.ModInfoClass>());
        if (index == -1)
          return;
        if (e.ChangedButton == MouseButton.Right)
        {
          if (!Directory.Exists(selectedMod.filePath))
            return;
          Process.Start(selectedMod.filePath);
        }
        else
        {
          GetUserInput getUserInput = new GetUserInput();
          getUserInput.UserInput.Text = selectedMod.Title;
          getUserInput.Owner = (Window) this;
          getUserInput.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          getUserInput.TitleMessage.Text = string.Format("Choose A New Name for {0}", (object) selectedMod.Title);
          getUserInput.ShowDialog();
          if (getUserInput.PressedAccept && !string.IsNullOrWhiteSpace(getUserInput.UserInput.Text) && !(getUserInput.UserInput.Text == selectedMod.Title))
          {
            this.TheList[index].Title = getUserInput.UserInput.Text;
            this.ModList.Items.SortDescriptions.Clear();
            this.ModList.Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending));
            this.SaveCollection();
          }
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage("No Soup for you!!!", ex.ToString());
      }
    }

    private void ReloadButton_Click(object sender, RoutedEventArgs e) => this.DetectMods();

    private void OpenModFolder_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Process.Start(this.InstalledModsPath);
      }
      catch
      {
        this.ErrorMessage("Hasta la vista, baby", "Looks like your installed mods path isn't set to a valid location...");
      }
    }

    private void ModdedRPF_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => this.openFileDialog.InitialDirectory = this.InstalledRPFPath;

    private void ImportModdedRPF_Click(object sender, RoutedEventArgs e)
    {
      if (Settings.Default.FileDetectionMode && !this.tryLoadRPFScripts)
      {
        this.ErrorMessage("OpenIV.asi not found!", "You are attempting to import an RPF file but you don't have OpenIV.asi installed yet. Please install this plugin and try again.");
      }
      else
      {
        if (Settings.Default.RPFWarning)
        {
          this.ErrorMessage("Please read this.", "By Continuing, you are agreeing that you know what you are doing and accept all responsibility. It is highly recommended that you:\n1. Create a personal backup for any RPF files you modify\n2. Modify a copy of the original RPF OUTSIDE of your Game directory.");
          Settings.Default.RPFWarning = false;
          Settings.Default.Save();
        }
        RPFSetupWizard rpfSetupWizard = new RPFSetupWizard();
        rpfSetupWizard.Owner = (Window) this;
        rpfSetupWizard.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        rpfSetupWizard.RPFInstallLocation = this.InstalledRPFPath;
        rpfSetupWizard.ShowDialog();
        this.DetectMods();
      }
    }

    private void copyDirectory(string strSource, string strDestination)
    {
      try
      {
        if (!Directory.Exists(strSource))
          return;
        Directory.CreateDirectory(strDestination);
        DirectoryInfo directoryInfo = new DirectoryInfo(strSource);
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
          if (!File.Exists(System.IO.Path.Combine(strDestination, file.Name)))
            file.CopyTo(System.IO.Path.Combine(strDestination, file.Name), true);
        }
        foreach (DirectoryInfo directory in directoryInfo.GetDirectories())
          this.copyDirectory(System.IO.Path.Combine(strSource, directory.Name), System.IO.Path.Combine(strDestination, directory.Name));
      }
      catch (Exception ex)
      {
        this.ErrorMessage("CopyDirectory crash!", ex.ToString());
      }
    }

    public static void DoEvents()
    {
      try
      {
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, (Action)delegate() { });
      }
      catch
      {
      }
    }


    private void crashReport(Exception ex, string additionalInfo)
    {
      using (StreamWriter streamWriter = new StreamWriter("CrashReport.log"))
      {
        streamWriter.WriteLine(ex.Message);
        streamWriter.WriteLine(ex.InnerException != null ? ex.InnerException.Message : "");
        streamWriter.WriteLine();
        streamWriter.WriteLine(additionalInfo);
        streamWriter.WriteLine();
        streamWriter.WriteLine("IsSteam: " + (this.isSteam.ToString() != null ? this.isSteam.ToString() : "null"));
        streamWriter.WriteLine("GameInstallationPath: " + (this.GameInstallationPath != null ? this.GameInstallationPath : "null"));
        streamWriter.WriteLine("InstalledModsPath: " + (this.InstalledModsPath != null ? this.installedModsPath : "null"));
        streamWriter.WriteLine("InstalledLuaPath: " + (this.InstalledLuaPath != null ? this.InstalledLuaPath : "null"));
        streamWriter.WriteLine("InstalledScriptsPath " + (this.InstalledScriptsPath != null ? this.InstalledScriptsPath : "null"));
        streamWriter.WriteLine("InstalledRPFPath: " + (this.InstalledRPFPath != null ? this.InstalledRPFPath : "null"));
        streamWriter.WriteLine("GameRPFPath: " + (this.GameRPFPath != null ? this.GameRPFPath : "null"));
        streamWriter.WriteLine("Gta5FilePath: " + (this.gta5FilePath != null ? this.gta5FilePath : "null"));
        streamWriter.WriteLine("CommandlineFilePath: " + (this.commandLineFilePath != null ? this.commandLineFilePath : "null"));
        streamWriter.WriteLine("TryLoadLua: " + (this.tryLoadLua.ToString() != null ? this.tryLoadLua.ToString() : "null"));
        streamWriter.WriteLine("TryLoadSweetFX: " + (this.tryLoadSweetFX.ToString() != null ? this.tryLoadSweetFX.ToString() : "null"));
        streamWriter.WriteLine("TryLoadNetScripts: " + (this.tryLoadNetScripts.ToString() != null ? this.tryLoadNetScripts.ToString() : "null"));
        streamWriter.WriteLine("FileCopyMode: " + (object) Settings.Default.CopyOnly);
        streamWriter.WriteLine("File Detection Mode: " + (object) Settings.Default.FileDetectionMode);
        streamWriter.WriteLine();
        streamWriter.WriteLine("Config files detected: ");
        foreach (string config in this.configs)
          streamWriter.WriteLine(config);
        streamWriter.WriteLine();
        streamWriter.WriteLine("Installed Mod Info");
        foreach (MainWindow.ModInfoClass the in (Collection<MainWindow.ModInfoClass>) this.TheList)
        {
          if (the.CfgPath == null)
            the.CfgPath = string.Empty;
          string str = string.Format("Name: {0}\tFile: {1}\tType:{2}\tEnabled: {4}\tConfig: {3}", (object) the.Title, (object) the.filePath, (object) the.fileType, (object) the.CfgPath, (object) the.IsChecked);
          if (the.fileType == MainWindow.ModInfoClass.Type.RPF_Mod)
            str = !File.Exists(the.CfgPath) ? str + " Destination: Config File Doesn't exist!" : str + " Destination: " + File.ReadAllText(the.CfgPath);
          streamWriter.WriteLine(str);
        }
      }
    }

    private void OptionsButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.crashReport(new Exception("Manually created"), "");
      this.ErrorMessage("Complete!", "Crash Report created!");
    }

    private void ReloadButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.SaveCollection();
      File.WriteAllText("ModInfo.dat", Settings.Default.TheList);
      this.ErrorMessage("Mod List Backup complete!", "Backup has been saved to ModInfo.dat. Right click \"Add Mod...\" to restore from this file.");
    }

    private void AddMod_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      if (!File.Exists("ModInfo.dat"))
        return;
      Settings.Default.TheList = File.ReadAllText("ModInfo.dat");
      if (string.IsNullOrWhiteSpace(Settings.Default.TheList))
      {
        this.ErrorMessage("Backup is corrupt!", "Backup mod file is empty.");
      }
      else
      {
        this.TheBakList = this.LoadCollection();
        if (this.TheBakList.Count == 0)
        {
          this.ErrorMessage("Backup is corrupt!", "The backup didn't return anything useful. Possibly corrupt or just no data!");
        }
        else
        {
          Settings.Default.Save();
          this.ErrorMessage("Mod List Restoration Complete!", "You have successfully restored your mod info from backup!");
          this.DetectMods();
        }
      }
    }

    private void DisplayResizeCursor(object sender, MouseEventArgs e)
    {
      ((UIElement) sender).Opacity = 100.0;
      this.ob.displayResizeCursor(sender);
    }

    private void ResetCursor(object sender, MouseEventArgs e)
    {
      ((UIElement) sender).Opacity = 0.0;
      this.ob.resetCursor();
    }

    private void Resize(object sender, MouseButtonEventArgs e)
    {
      this.ob.resizeWindow(sender);
      this.saveSize();
    }

    private void saveSize()
    {
      if (Settings.Default.Height != this.Height)
        Settings.Default.Height = this.Height;
      if (Settings.Default.Width != this.Width)
        Settings.Default.Width = this.Width;
      Settings.Default.Save();
    }

    private void Drag(object sender, MouseButtonEventArgs e) => this.ob.dragWindow();

    private void ModList_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        System.Type type = VisualTreeHelper.GetParent((DependencyObject) e.OriginalSource).GetType();
        if (type != typeof (ContentPresenter) && type != typeof (ListViewItem) || this.ModList.SelectedItem == null)
          return;
        MainWindow.ModInfoClass selectedItem = (MainWindow.ModInfoClass) this.ModList.SelectedItem;
        if (Directory.Exists(selectedItem.filePath))
          Process.Start(selectedItem.filePath);
      }
      catch
      {
      }
    }

    private void ModList_PreviewKeyUp(object sender, KeyEventArgs e)
    {
      MainWindow.ModInfoClass modInfoClass = this.TheList.FirstOrDefault<MainWindow.ModInfoClass>((Func<MainWindow.ModInfoClass, bool>) (item => item.Title.StartsWith(e.Key.ToString())));
      if (modInfoClass == null)
        return;
      this.ModList.SelectedItem = (object) modInfoClass;
      this.ModList.ScrollIntoView(this.ModList.SelectedItem);
    }

    private void ClearFilter_PreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
      this.FilterBox.Text = string.Empty;
      this.SortCheckBox.IsChecked = new bool?(false);
    }

    private void FilterBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      this.SortCheckBox.IsChecked = new bool?(false);
      if (this.ModList.ItemsSource == null)
        return;
      CollectionViewSource.GetDefaultView((object) this.ModList.ItemsSource).Refresh();
    }

    private bool UserFilter(object item)
    {
      if ((this.SortCheckBox.IsChecked ?? false) != false)
        return (item as MainWindow.ModInfoClass).IsChecked;
      return string.IsNullOrWhiteSpace(this.FilterBox.Text) || (item as MainWindow.ModInfoClass).Title.IndexOf(this.FilterBox.Text, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void SortCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      this.FilterBox.Text = string.Empty;
      if (this.ModList.ItemsSource == null)
        return;
      CollectionViewSource.GetDefaultView((object) this.ModList.ItemsSource).Refresh();
    }


    private enum SymbolicLink
    {
      File,
      Directory,
    }

    [Serializable]
    public class ModInfoClass : INotifyPropertyChanged
    {
      private bool isChecked;
      private string title;

      public string Title
      {
        get => this.title;
        set
        {
          if (this.Title == value)
            return;
          this.title = value;
          this.RaisePropertyChanged(nameof (Title));
        }
      }

      public string Author { get; set; }

      public string Version { get; set; }

      public string CfgPath { get; set; }

      public string filePath { get; set; }

      public string fileExt { get; set; }

      public MainWindow.ModInfoClass.Type fileType { get; set; }

      public bool IsChecked
      {
        get => this.isChecked;
        set
        {
          if (this.isChecked == value)
            return;
          this.isChecked = value;
          this.RaisePropertyChanged(nameof (IsChecked));
        }
      }

      [field: NonSerialized]
      public event PropertyChangedEventHandler PropertyChanged;

      private void RaisePropertyChanged(string propName)
      {
        PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
        if (propertyChanged == null)
          return;
        propertyChanged((object) this, new PropertyChangedEventArgs(propName));
      }

      public enum Type
      {
        Injector,
        Hook,
        DotNet_Mod,
        ASI_Mod,
        LUA_Mod,
        SweetFX,
        RPF_Mod,
        Python_Mod,
        Game_Binary,
        Unknown,
        Folder,
      }
    }
  }
}

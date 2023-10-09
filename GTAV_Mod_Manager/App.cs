// Decompiled with JetBrains decompiler
// Type: GTAV_Mod_Manager.App
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;

namespace GTAV_Mod_Manager
{
    public partial class App : Application
    {
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            this.StartupUri = new System.Uri("/GTAV Mod Manager;component/mainwindow.xaml", System.UriKind.Relative);
        }
    }
}

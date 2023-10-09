// Decompiled with JetBrains decompiler
// Type: NetFwTypeLib.INetFwRule
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib
{
  [CompilerGenerated]
  [Guid("AF230D27-BABA-4E42-ACED-F524F22CFCE2")]
  [TypeIdentifier]
  [ComImport]
  public interface INetFwRule
  {
    string Name { [DispId(1)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(1)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [SpecialName]
    sealed extern void _VtblGap1_2();

    string ApplicationName { [DispId(3)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(3)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    [SpecialName]
    sealed extern void _VtblGap2_14();

    NET_FW_RULE_DIRECTION_ Direction { [DispId(11)] get; [DispId(11)] [param: In] set; }

    [SpecialName]
    sealed extern void _VtblGap3_2();

    string InterfaceTypes { [DispId(13)] [return: MarshalAs(UnmanagedType.BStr)] get; [DispId(13)] [param: MarshalAs(UnmanagedType.BStr), In] set; }

    bool Enabled { [DispId(14)] get; [DispId(14)] [param: In] set; }

    [SpecialName]
    sealed extern void _VtblGap4_6();

    NET_FW_ACTION_ Action { [DispId(18)] get; [DispId(18)] [param: In] set; }
  }
}

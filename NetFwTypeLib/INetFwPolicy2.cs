// Decompiled with JetBrains decompiler
// Type: NetFwTypeLib.INetFwPolicy2
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib
{
  [TypeIdentifier]
  [CompilerGenerated]
  [Guid("98325047-C671-4174-8D81-DEFCD3F03186")]
  [ComImport]
  public interface INetFwPolicy2
  {
    [SpecialName]
    sealed extern void _VtblGap1_11();

    INetFwRules Rules { [DispId(7)] [return: MarshalAs(UnmanagedType.Interface)] get; }
  }
}

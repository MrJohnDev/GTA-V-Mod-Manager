// Decompiled with JetBrains decompiler
// Type: NetFwTypeLib.INetFwRules
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NetFwTypeLib
{
  [Guid("9C4C6277-5027-441E-AFAE-CA1F542DA009")]
  [TypeIdentifier]
  [CompilerGenerated]
  [ComImport]
  public interface INetFwRules : IEnumerable
  {
    [SpecialName]
    sealed extern void _VtblGap1_1();

    [DispId(2)]
    void Add([MarshalAs(UnmanagedType.Interface), In] INetFwRule rule);

    [DispId(3)]
    void Remove([MarshalAs(UnmanagedType.BStr), In] string Name);

    [DispId(4)]
    [return: MarshalAs(UnmanagedType.Interface)]
    INetFwRule Item([MarshalAs(UnmanagedType.BStr), In] string Name);
  }
}

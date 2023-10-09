// Decompiled with JetBrains decompiler
// Type: CustomExtensions.StringExtension
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomExtensions
{
  public static class StringExtension
  {
    private static char[] invalid = new char[4]
    {
      ' ',
      '-',
      '_',
      '.'
    };

    public static string CleanString(this string y) => new string(y.Where<char>((Func<char, bool>) (x => !((IEnumerable<char>) StringExtension.invalid).Contains<char>(x))).ToArray<char>()).ToLower();
  }
}

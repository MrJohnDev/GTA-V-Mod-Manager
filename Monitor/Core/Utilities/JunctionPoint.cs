// Decompiled with JetBrains decompiler
// Type: Monitor.Core.Utilities.JunctionPoint
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Monitor.Core.Utilities
{
  public static class JunctionPoint
  {
    private const int ERROR_NOT_A_REPARSE_POINT = 4390;
    private const int ERROR_REPARSE_ATTRIBUTE_CONFLICT = 4391;
    private const int ERROR_INVALID_REPARSE_DATA = 4392;
    private const int ERROR_REPARSE_TAG_INVALID = 4393;
    private const int ERROR_REPARSE_TAG_MISMATCH = 4394;
    private const int FSCTL_SET_REPARSE_POINT = 589988;
    private const int FSCTL_GET_REPARSE_POINT = 589992;
    private const int FSCTL_DELETE_REPARSE_POINT = 589996;
    private const uint IO_REPARSE_TAG_MOUNT_POINT = 2684354563;
    private const string NonInterpretedPathPrefix = "\\??\\";

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DeviceIoControl(
      IntPtr hDevice,
      uint dwIoControlCode,
      IntPtr InBuffer,
      int nInBufferSize,
      IntPtr OutBuffer,
      int nOutBufferSize,
      out int pBytesReturned,
      IntPtr lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr CreateFile(
      string lpFileName,
      JunctionPoint.EFileAccess dwDesiredAccess,
      JunctionPoint.EFileShare dwShareMode,
      IntPtr lpSecurityAttributes,
      JunctionPoint.ECreationDisposition dwCreationDisposition,
      JunctionPoint.EFileAttributes dwFlagsAndAttributes,
      IntPtr hTemplateFile);

    public static void Create(string junctionPoint, string targetDir, bool overwrite)
    {
      targetDir = Path.GetFullPath(targetDir);
      if (!Directory.Exists(targetDir))
        throw new IOException("Target path does not exist or is not a directory.");
      if (Directory.Exists(junctionPoint))
      {
        if (!overwrite)
          throw new IOException("Directory already exists and overwrite parameter is false.");
      }
      else
        Directory.CreateDirectory(junctionPoint);
      using (SafeFileHandle safeFileHandle = JunctionPoint.OpenReparsePoint(junctionPoint, JunctionPoint.EFileAccess.GenericWrite))
      {
        byte[] bytes = Encoding.Unicode.GetBytes("\\??\\" + Path.GetFullPath(targetDir));
        JunctionPoint.REPARSE_DATA_BUFFER structure = new JunctionPoint.REPARSE_DATA_BUFFER();
        structure.ReparseTag = 2684354563U;
        structure.ReparseDataLength = (ushort) (bytes.Length + 12);
        structure.SubstituteNameOffset = (ushort) 0;
        structure.SubstituteNameLength = (ushort) bytes.Length;
        structure.PrintNameOffset = (ushort) (bytes.Length + 2);
        structure.PrintNameLength = (ushort) 0;
        structure.PathBuffer = new byte[16368];
        Array.Copy((Array) bytes, (Array) structure.PathBuffer, bytes.Length);
        IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf((object) structure));
        try
        {
          Marshal.StructureToPtr((object) structure, num, false);
          if (JunctionPoint.DeviceIoControl(safeFileHandle.DangerousGetHandle(), 589988U, num, bytes.Length + 20, IntPtr.Zero, 0, out int _, IntPtr.Zero))
            return;
          JunctionPoint.ThrowLastWin32Error("Unable to create junction point.");
        }
        finally
        {
          Marshal.FreeHGlobal(num);
        }
      }
    }

    public static void Delete(string junctionPoint)
    {
      if (!Directory.Exists(junctionPoint))
      {
        if (File.Exists(junctionPoint))
          throw new IOException("Path is not a junction point.");
      }
      else
      {
        using (SafeFileHandle safeFileHandle = JunctionPoint.OpenReparsePoint(junctionPoint, JunctionPoint.EFileAccess.GenericWrite))
        {
          JunctionPoint.REPARSE_DATA_BUFFER structure = new JunctionPoint.REPARSE_DATA_BUFFER();
          structure.ReparseTag = 2684354563U;
          structure.ReparseDataLength = (ushort) 0;
          structure.PathBuffer = new byte[16368];
          IntPtr num = Marshal.AllocHGlobal(Marshal.SizeOf((object) structure));
          try
          {
            Marshal.StructureToPtr((object) structure, num, false);
            if (!JunctionPoint.DeviceIoControl(safeFileHandle.DangerousGetHandle(), 589996U, num, 8, IntPtr.Zero, 0, out int _, IntPtr.Zero))
              JunctionPoint.ThrowLastWin32Error("Unable to delete junction point.");
          }
          finally
          {
            Marshal.FreeHGlobal(num);
          }
          try
          {
            Directory.Delete(junctionPoint);
          }
          catch (IOException ex)
          {
            throw new IOException("Unable to delete junction point.", (Exception) ex);
          }
        }
      }
    }

    public static bool Exists(string path)
    {
      if (!Directory.Exists(path))
        return false;
      using (SafeFileHandle handle = JunctionPoint.OpenReparsePoint(path, JunctionPoint.EFileAccess.GenericRead))
        return JunctionPoint.InternalGetTarget(handle) != null;
    }

    public static string GetTarget(string junctionPoint)
    {
      using (SafeFileHandle handle = JunctionPoint.OpenReparsePoint(junctionPoint, JunctionPoint.EFileAccess.GenericRead))
        return JunctionPoint.InternalGetTarget(handle) ?? throw new IOException("Path is not a junction point.");
    }

    private static string InternalGetTarget(SafeFileHandle handle)
    {
      int num1 = Marshal.SizeOf(typeof (JunctionPoint.REPARSE_DATA_BUFFER));
      IntPtr num2 = Marshal.AllocHGlobal(num1);
      try
      {
        if (!JunctionPoint.DeviceIoControl(handle.DangerousGetHandle(), 589992U, IntPtr.Zero, 0, num2, num1, out int _, IntPtr.Zero))
        {
          if (Marshal.GetLastWin32Error() == 4390)
            return (string) null;
          JunctionPoint.ThrowLastWin32Error("Unable to get information about junction point.");
        }
        JunctionPoint.REPARSE_DATA_BUFFER structure = (JunctionPoint.REPARSE_DATA_BUFFER) Marshal.PtrToStructure(num2, typeof (JunctionPoint.REPARSE_DATA_BUFFER));
        if (structure.ReparseTag != 2684354563U)
          return (string) null;
        string target = Encoding.Unicode.GetString(structure.PathBuffer, (int) structure.SubstituteNameOffset, (int) structure.SubstituteNameLength);
        if (target.StartsWith("\\??\\"))
          target = target.Substring("\\??\\".Length);
        return target;
      }
      finally
      {
        Marshal.FreeHGlobal(num2);
      }
    }

    private static SafeFileHandle OpenReparsePoint(
      string reparsePoint,
      JunctionPoint.EFileAccess accessMode)
    {
      SafeFileHandle safeFileHandle = new SafeFileHandle(JunctionPoint.CreateFile(reparsePoint, accessMode, JunctionPoint.EFileShare.Read | JunctionPoint.EFileShare.Write | JunctionPoint.EFileShare.Delete, IntPtr.Zero, JunctionPoint.ECreationDisposition.OpenExisting, JunctionPoint.EFileAttributes.BackupSemantics | JunctionPoint.EFileAttributes.OpenReparsePoint, IntPtr.Zero), true);
      if (Marshal.GetLastWin32Error() != 0)
        JunctionPoint.ThrowLastWin32Error("Unable to open reparse point.");
      return safeFileHandle;
    }

    private static void ThrowLastWin32Error(string message) => throw new IOException(message, Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));

    [Flags]
    private enum EFileAccess : uint
    {
      GenericRead = 2147483648, // 0x80000000
      GenericWrite = 1073741824, // 0x40000000
      GenericExecute = 536870912, // 0x20000000
      GenericAll = 268435456, // 0x10000000
    }

    [Flags]
    private enum EFileShare : uint
    {
      None = 0,
      Read = 1,
      Write = 2,
      Delete = 4,
    }

    private enum ECreationDisposition : uint
    {
      New = 1,
      CreateAlways = 2,
      OpenExisting = 3,
      OpenAlways = 4,
      TruncateExisting = 5,
    }

    [Flags]
    private enum EFileAttributes : uint
    {
      Readonly = 1,
      Hidden = 2,
      System = 4,
      Directory = 16, // 0x00000010
      Archive = 32, // 0x00000020
      Device = 64, // 0x00000040
      Normal = 128, // 0x00000080
      Temporary = 256, // 0x00000100
      SparseFile = 512, // 0x00000200
      ReparsePoint = 1024, // 0x00000400
      Compressed = 2048, // 0x00000800
      Offline = 4096, // 0x00001000
      NotContentIndexed = 8192, // 0x00002000
      Encrypted = 16384, // 0x00004000
      Write_Through = 2147483648, // 0x80000000
      Overlapped = 1073741824, // 0x40000000
      NoBuffering = 536870912, // 0x20000000
      RandomAccess = 268435456, // 0x10000000
      SequentialScan = 134217728, // 0x08000000
      DeleteOnClose = 67108864, // 0x04000000
      BackupSemantics = 33554432, // 0x02000000
      PosixSemantics = 16777216, // 0x01000000
      OpenReparsePoint = 2097152, // 0x00200000
      OpenNoRecall = 1048576, // 0x00100000
      FirstPipeInstance = 524288, // 0x00080000
    }

    private struct REPARSE_DATA_BUFFER
    {
      public uint ReparseTag;
      public ushort ReparseDataLength;
      public ushort Reserved;
      public ushort SubstituteNameOffset;
      public ushort SubstituteNameLength;
      public ushort PrintNameOffset;
      public ushort PrintNameLength;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16368)]
      public byte[] PathBuffer;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: EmbeddedAssembly
// Assembly: GTAV Mod Manager, Version=1.0.6379.16959, Culture=neutral, PublicKeyToken=null
// MVID: 4020FBC2-BCD0-401F-AC8F-734276BE45A6
// Assembly location: C:\Users\User\Desktop\GTAV Mod Manager.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

public class EmbeddedAssembly
{
  private static Dictionary<string, Assembly> dic = (Dictionary<string, Assembly>) null;

  public static void Load(string embeddedResource, string fileName)
  {
    if (EmbeddedAssembly.dic == null)
      EmbeddedAssembly.dic = new Dictionary<string, Assembly>();
    byte[] numArray = (byte[]) null;
    using (Stream manifestResourceStream = Assembly.GetEntryAssembly().GetManifestResourceStream(embeddedResource))
    {
      numArray = manifestResourceStream != null ? new byte[(int) manifestResourceStream.Length] : throw new Exception(embeddedResource + " is not found in Embedded Resources.");
      manifestResourceStream.Read(numArray, 0, (int) manifestResourceStream.Length);
      try
      {
        Assembly assembly = Assembly.Load(numArray);
        EmbeddedAssembly.dic.Add(assembly.FullName, assembly);
        return;
      }
      catch
      {
      }
    }
    bool flag = false;
    string path = "";
    using (SHA1CryptoServiceProvider cryptoServiceProvider = new SHA1CryptoServiceProvider())
    {
      string str1 = BitConverter.ToString(cryptoServiceProvider.ComputeHash(numArray)).Replace("-", string.Empty);
      path = Path.GetTempPath() + fileName;
      if (File.Exists(path))
      {
        byte[] buffer = File.ReadAllBytes(path);
        string str2 = BitConverter.ToString(cryptoServiceProvider.ComputeHash(buffer)).Replace("-", string.Empty);
        flag = str1 == str2;
      }
      else
        flag = false;
    }
    if (!flag)
      File.WriteAllBytes(path, numArray);
    Assembly assembly1 = Assembly.LoadFile(path);
    EmbeddedAssembly.dic.Add(assembly1.FullName, assembly1);
  }

  public static Assembly Get(string assemblyFullName) => EmbeddedAssembly.dic == null || EmbeddedAssembly.dic.Count == 0 || !EmbeddedAssembly.dic.ContainsKey(assemblyFullName) ? (Assembly) null : EmbeddedAssembly.dic[assemblyFullName];

  public static void initialize(string nameSpace, List<string> assemblies)
  {
    foreach (string assembly in assemblies)
      EmbeddedAssembly.Load(string.Format("{0}.{1}", (object) nameSpace, (object) assembly), assembly);
    AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(EmbeddedAssembly.CurrentDomain_AssemblyResolve);
  }

  private static Assembly CurrentDomain_AssemblyResolve(
    object sender,
    ResolveEventArgs args)
  {
    return EmbeddedAssembly.Get(args.Name);
  }
}

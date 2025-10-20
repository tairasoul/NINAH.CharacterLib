using System.Reflection;
using UnityEngine;

namespace tairasoul.ninah.characterlib.utils;

public class AssemblyUtils {
  public static byte[] GetResourceBytes(Assembly assembly, string resource) {
    using Stream? stream = assembly.GetManifestResourceStream(resource);
    if (stream == null) return [];
    using MemoryStream ms = new();
    stream.CopyTo(ms);
    return ms.ToArray();
  }
  
  public static byte[] GetResourceBytes(string resource) {
    return GetResourceBytes(Assembly.GetCallingAssembly(), resource);
  }

  public static Texture2D? GetTexture(Assembly assembly, string resource) {
    byte[] bytes = GetResourceBytes(assembly, resource);
    if (bytes.Length == 0) return null;
    Texture2D texture = new(2, 2);
    texture.LoadImage(bytes);
    texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
    return texture;
  }

  public static Texture2D? GetTexture(string resource) {
    return GetTexture(Assembly.GetCallingAssembly(), resource);
  }
}
using System.Reflection;
using UnityEngine;

namespace tairasoul.ninah.characterlib.utils;

public class AssemblyUtils {
  /// <summary>
  /// Get bytes of a resource from an assembly.
  /// </summary>
  /// <param name="assembly">The assembly the resource is in.</param>
  /// <param name="resource">The name of the resource you want to get.</param>
  /// <returns>The bytes of the resource, or an empty array if it doesn't exist.</returns>
  public static byte[] GetResourceBytes(Assembly assembly, string resource) {
    using Stream? stream = assembly.GetManifestResourceStream(resource);
    if (stream == null) return [];
    using MemoryStream ms = new();
    stream.CopyTo(ms);
    return ms.ToArray();
  }

  /// <summary>
  /// Get bytes of a resource from the current assembly.
  /// </summary>
  /// <param name="resource">The name of the resource you want to get.</param>
  /// <returns>The bytes of the resource, or an empty array if it doesn't exist.</returns>
  public static byte[] GetResourceBytes(string resource) {
    return GetResourceBytes(Assembly.GetCallingAssembly(), resource);
  }

  /// <summary>
  /// Get a Texture2D from a resource from an assembly.
  /// </summary>
  /// <param name="assembly">The assembly the resource is in.</param>
  /// <param name="resource">The name of the resource you want to get.</param>
  /// <returns>The Texture2D from the resource, or null if it doesn't exist.</returns>
  public static Texture2D? GetTexture(Assembly assembly, string resource) {
    byte[] bytes = GetResourceBytes(assembly, resource);
    if (bytes.Length == 0) return null;
    Texture2D texture = new(2, 2);
    texture.LoadImage(bytes);
    texture.hideFlags = HideFlags.DontUnloadUnusedAsset;
    return texture;
  }

  /// <summary>
  /// Get a Texture2D from a resource in the current assembly.
  /// </summary>
  /// <param name="resource">The name of the resource you want to get.</param>
  /// <returns>The Texture2D from the resource, or null if it doesn't exist.</returns>

  public static Texture2D? GetTexture(string resource) {
    return GetTexture(Assembly.GetCallingAssembly(), resource);
  }
}
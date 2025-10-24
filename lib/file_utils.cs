using UnityEngine;

namespace tairasoul.ninah.characterlib.utils;

public class FileUtils {
  /// <summary>
  /// Load a Texture2D from a file.
  /// </summary>
  /// <param name="file">The file to load.</param>
  /// <returns>The Texture2D from the file, or null if the file doesn't exist.</returns>
  public static Texture2D? ReadTextureFromFile(string file) {
    if (!File.Exists(file)) return null;

    byte[] fileBytes = File.ReadAllBytes(file);
    Texture2D texture = new(2, 2);
    texture.LoadImage(fileBytes);
    return texture;
  } 
}
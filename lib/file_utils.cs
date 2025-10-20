using UnityEngine;

namespace tairasoul.ninah.characterlib.utils;

public class FileUtils {
  public static Texture2D? ReadTextureFromFile(string file) {
    if (!File.Exists(file)) return null;

    byte[] fileBytes = File.ReadAllBytes(file);
    Texture2D texture = new(2, 2);
    texture.LoadImage(fileBytes);
    return texture;
  } 
}
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace tairasoul.ninah.characterlib;

[BepInPlugin("tairasoul.ninah.characterlib", "CharacterLib", "1.0.0")]
class Plugin : BasePlugin {
  internal static ManualLogSource Logger;
  public override void Load() {
    Logger = Log;
    ClassInjector.RegisterTypeInIl2Cpp<CustomCharacter>();
    ClassInjector.RegisterTypeInIl2Cpp<CharacterLibBehaviour>();
    CharacterLib.RegisterYarnAssemblies();
    CharacterLib.CreateRegisteredList();
    GameObject logic = new("CharacterLibLogic")
    {
      hideFlags = HideFlags.DontUnloadUnusedAsset | HideFlags.HideAndDontSave
    };
    GameObject.DontDestroyOnLoad(logic);
    logic.AddComponent<CharacterLibBehaviour>();
    Harmony harmony = new("tairasoul.ninah.characterlib");
    harmony.PatchAll();
  }
}
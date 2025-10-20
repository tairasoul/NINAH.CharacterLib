using System.Reflection;
using _Code.DialogSystem.Commands;
using HarmonyLib;

namespace tairasoul.ninah.characterlib.patches;

[HarmonyPatch(typeof(DialogCommandsInstance))]
static class DialogCommandsInstanceCharacterRedirects {
  static IEnumerable<MethodBase> TargetMethods() {
    List<MethodBase> methods = [
      typeof(DialogCommandsInstance).GetMethod("LetCharacterIn"),
      typeof(DialogCommandsInstance).GetMethod("RefuseOldLetInNew"),
      typeof(DialogCommandsInstance).GetMethod("AggressiveLetCharacterIn"),
      typeof(DialogCommandsInstance).GetMethod("KillCharacter"),
      typeof(DialogCommandsInstance).GetMethod("KillCharacterWithNoGun"),
      typeof(DialogCommandsInstance).GetMethod("KillTomorrow"),
      typeof(DialogCommandsInstance).GetMethod("ShowSign"),
      typeof(DialogCommandsInstance).GetMethod("RefuseCharacter"),
      typeof(DialogCommandsInstance).GetMethod("ExileCharacter"),
      typeof(DialogCommandsInstance).GetMethod("ExileAfterTomorrow"),
      typeof(DialogCommandsInstance).GetMethod("ChangePose"),
      typeof(DialogCommandsInstance).GetMethod("ChangePoseTomorrow"),
      typeof(DialogCommandsInstance).GetMethod("IsImposter"),
      typeof(DialogCommandsInstance).GetMethod("IsCharacterPlaceEmpty"),
      typeof(DialogCommandsInstance).GetMethod("GetCharacterOnPlace"),
      typeof(DialogCommandsInstance).GetMethod("IsAlive"),
    ];
    return methods;
  }

  [HarmonyPrefix]
  static void CharacterMethodPrefix(ref string __0) {
    foreach (CustomCharacter character in CharacterLib.characters) {
      if (character.name == __0) {
        Plugin.Logger.LogDebug($"Redirecting {__0} to {character._characterType} int");
        __0 = ((int)character._characterType).ToString();
        return;
      }
    }
  }
}

[HarmonyPatch(typeof(DialogCommandsInstance))]
static class DialogCOmmandsInstanceMiscRedirects {
  [HarmonyPatch(nameof(DialogCommandsInstance.SetEmotion))]
  [HarmonyPrefix]
  static void SetEmotionPrefix(ref string __0) {
    foreach (CustomCharacter character in CharacterLib.characters) {
      if (character.EmotionRedirects.ContainsKey(__0)) {
        __0 = character.EmotionRedirects[__0];
        return;
      }
    }
  }
}
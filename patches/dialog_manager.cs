using _Code.Characters;
using _Code.DialogSystem;
using _Code.Infrastructure.Consumables;
using HarmonyLib;

namespace tairasoul.ninah.characterlib.patches;

[HarmonyPatch(typeof(DialogManager))]
class DialogManagerPatches {
  [HarmonyPatch(nameof(DialogManager.RunDialog))]
  [HarmonyPrefix]
  static void RunDialogPatch(CharacterSOData character, ref string nodeName) {
    if (CharacterLib.characterTypes.Contains(character._characterType)) {
      Dictionary<string, string> replacements = CharacterLib.references[character._characterType].NodeReplacements;
      if (replacements.ContainsKey(nodeName))
        nodeName = replacements[nodeName];
    }
  }
  [HarmonyPatch(nameof(DialogManager.IsNodeVisited))]
  [HarmonyPrefix]
  static void IsNodeVisitedPatch(ref string nodeName) {
    foreach (CustomCharacter character in CharacterLib.characters) {
      Dictionary<string, string> replacements = CharacterLib.references[character._characterType].NodeReplacements;
      if (replacements.ContainsKey(nodeName))
        nodeName = replacements[nodeName];
    }
    /*if (CharacterLib.characterTypes.Contains(character._characterType)) {
      Dictionary<string, string> replacements = CharacterLib.references[character._characterType].NodeReplacements;
      if (replacements.ContainsKey(nodeName))
        nodeName = replacements[nodeName];
    }*/
  }
}
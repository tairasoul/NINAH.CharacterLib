using System.Reflection;
using _Code.Characters;
using _Code.Infrastructure.Rooms;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Dynamic.Utils;

namespace CharacterLib.Patches;

[HarmonyPatch(typeof(ARoom))]
class ARoomInitPatch  {
  [HarmonyPatch(nameof(ARoom.Init))]
  [HarmonyPrefix]
  public static void InitPrefix(ARoom __instance, ARoomView view) {
    CharactersManager cm = __instance._charactersManager.Cast<CharactersManager>();
    foreach (CustomCharacter character in CharacterLib.characters) {
      if (character._room == __instance.RoomType)
      {
        if (!cm._characterData.Any((v) => v._characterType == character._characterType))
        {
          cm._characterData = (Il2CppReferenceArray<CharacterSOData>)cm._characterData.AddLast(character);
        }
        if (!view._CharacterViews_k__BackingField.Any((v) => v._Data_k__BackingField._characterType == character._characterType))
        {
          CharacterLib.AddObjectView(character, view);
        }
      }
    }
  }
}
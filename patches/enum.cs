using System.Reflection;
using System.Runtime.CompilerServices;
using _Code.Characters;
using _Code.Infrastructure.Sound;
using ECM2;
using HarmonyLib;
using Il2CppInterop.Runtime;

namespace CharacterLib.Patches;

[HarmonyPatch(typeof(Enum))]
class EnumPatches {
  static Dictionary<int, string> cache = [];
  internal static bool Bypass = false;

  // truly sinister code
  // necessary so we can inject our custom characters into the enum, since il2cpp doesnt allow for traditional enum modification through Cessil

  [HarmonyPatch(nameof(Enum.GetValues), [typeof(Type)])]
  [HarmonyPostfix]
  static void GetValuesPostfix(Enum __instance, Type enumType, ref Array __result) {
    if (enumType == null) return;
    if (enumType.FullName == null) return;
    string typeName = enumType.FullName;
    if (typeName == "_Code.Characters.ECharacterType") {
      List<ECharacterType> arr = [.. __result.Cast<ECharacterType>()];
      arr.AddRange(CharacterLibrary.characterTypes);
      __result = arr.ToArray();
    }
  }

  // unsure if these actually get used, but just in case
  [HarmonyPatch(nameof(Enum.GetName), [typeof(Type), typeof(object)])]
  [HarmonyPrefix]
  static bool GetNamePrefix(Enum __instance, Type enumType, object value, ref string __result) {
    if (enumType == null) return true;
    if (enumType.FullName == null) return true;
    string typeName = enumType.FullName;
    if (typeName == "_Code.Characters.ECharacterType")
    {
      foreach (CustomCharacter character in CharacterLibrary.characters) {
        if (value.ToString() == character.CharacterType.ToString()) {
          __result = character.CharacterType.ToString();
          return false;
        }
      }
    }
    return true;
  }

  [HarmonyPatch(nameof(Enum.IsDefined), [typeof(Type), typeof(object)])]
  [HarmonyPrefix]
  static bool IsDefinedPrefix(Enum __instance, Type enumType, object value, ref bool __result) {
    if (enumType == null) return true;
    if (enumType.FullName == null) return true;
    string typeName = enumType.FullName;
    if (typeName == "_Code.Characters.ECharacterType")
    {
      foreach (CustomCharacter character in CharacterLibrary.characters) {
        if (value.ToString() == character.CharacterType.ToString()) {
          __result = true;
          return false;
        }
      }
    }
    return true;
  }

  // only really used for UnityExplorer
  /*[HarmonyPatch(nameof(Enum.ToString), [])]
  [HarmonyPostfix]
  static void ToStringPostfix(Enum __instance, ref string __result) {
    string typeName = __instance.GetType().FullName;
    if (int.TryParse(__result, out int res))
      if (!cache.TryGetValue(res, out __result))
        if (typeName == "_Code.Characters.ECharacterType")
          foreach (CustomCharacter custom in CharacterLib.characters)
            if (custom._characterType == (ECharacterType)res)
            {
              cache[res] = custom.name;
              __result = custom.name;
              return;
            }
  }*/
}
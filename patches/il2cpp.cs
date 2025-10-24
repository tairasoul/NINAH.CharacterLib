using System.Reflection;
using _Code.Events;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using CharacterLib.Helpers;

namespace CharacterLib.Patches;

// mainly to fix UnityExplorer but also to make doing this shit manually unnecessary

[HarmonyPatch]
class Il2cppValueTuplePatchesConstructor {
  static IEnumerable<MethodBase> TargetMethods() {
    List<MethodBase> methods = [];
    Type[] typesToCheck = [typeof(Il2CppSystem.ValueTuple<int, int>), typeof(Il2CppSystem.ValueTuple<CharacterDingDongEvent, int>)];
    foreach (Type type in typesToCheck) {
      List<ConstructorInfo> infos = AccessTools.GetDeclaredConstructors(type);
      foreach (ConstructorInfo info in infos) {
        if (info.GetParameters().Length > 0) {
          methods.Add(info);
        }
      }
    }
    return methods;
  }

  [HarmonyPostfix]
  static void Constructor(Il2CppObjectBase __instance, object[] __args) {
    int argCount = 0;
    foreach (object arg in __args) {
      argCount++;
      if (arg.GetType() == typeof(int)) {
        __instance.WriteInt32($"Item{argCount}", (int)arg);
      }
    }
  }
}

[HarmonyPatch]
class Il2cppValueTuplePatchesGetter {
  static IEnumerable<MethodBase> TargetMethods() {
    List<MethodBase> methods = [];
    Type[] typesToCheck = [typeof(Il2CppSystem.ValueTuple<int, int>), typeof(Il2CppSystem.ValueTuple<CharacterDingDongEvent, int>)];
    foreach (Type type in typesToCheck) {
      foreach (MethodInfo method in type.GetMethods()) {
        if (method.Name.StartsWith("get_") && method.ReturnType == typeof(int)) {
          methods.Add(method);
        }
      }
    }
    return methods;
  }

  [HarmonyPrefix]
  static bool Getter(Il2CppObjectBase __instance, MethodBase __originalMethod, ref int __result) {
    string propertyName = __originalMethod.Name[4..];
    __result = __instance.ReadInt32(propertyName);
    return false;
  }
}

[HarmonyPatch]
class Il2cppValueTuplePatchesSetter {
  static IEnumerable<MethodBase> TargetMethods() {
    List<MethodBase> methods = [];
    Type[] typesToCheck = [typeof(Il2CppSystem.ValueTuple<int, int>), typeof(Il2CppSystem.ValueTuple<CharacterDingDongEvent, int>)];
    foreach (Type type in typesToCheck) {
      foreach (MethodInfo method in type.GetMethods()) {
        if (method.Name.StartsWith("set_") && method.GetParameters()[0].ParameterType == typeof(int)) {
          methods.Add(method);
        }
      }
    }
    return methods;
  }

  [HarmonyPrefix]
  static bool Setter(Il2CppObjectBase __instance, MethodBase __originalMethod, int __0) {
    string propertyName = __originalMethod.Name[4..];
    __instance.WriteInt32(propertyName, __0);
    return false;
  }
}
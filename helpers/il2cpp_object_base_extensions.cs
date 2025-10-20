using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;

namespace tairasoul.ninah.characterlib.helpers;

public static class Il2cppObjectBaseExtensions {
  // because generated il2cpp interop code does some weird unboxing shit which doesnt work for ints on a valuetuple for whatever reason
  // and only on a valuetuple
  public static unsafe int ReadInt32(this Il2CppObjectBase obj, string field) {
    var klass = obj.ObjectClass;
    var fieldN = IL2CPP.il2cpp_class_get_field_from_name(klass, field);
    if (fieldN == IntPtr.Zero) throw new MissingFieldException($"Field '{field}' not found in {obj}");

    int offset = (int)IL2CPP.il2cpp_field_get_offset(fieldN);
    IntPtr objectPtr = IL2CPP.Il2CppObjectBaseToPtrNotNull(obj);
    IntPtr fieldPtr = new(objectPtr.ToInt64() + offset);
    return Unsafe.Read<int>((void*)fieldPtr);
  }
  public static unsafe void WriteInt32(this Il2CppObjectBase obj, string field, int value) {
    var klass = obj.ObjectClass;
    var fieldN = IL2CPP.il2cpp_class_get_field_from_name(klass, field);
    if (fieldN == IntPtr.Zero) throw new MissingFieldException($"Field '{field}' not found in {obj}");

    int offset = (int)IL2CPP.il2cpp_field_get_offset(fieldN);
    IntPtr objectPtr = IL2CPP.Il2CppObjectBaseToPtrNotNull(obj);
    IntPtr fieldPtr = new(objectPtr.ToInt64() + offset);
    Unsafe.Write((void*)fieldPtr, value);
  }
}
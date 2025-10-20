namespace tairasoul.ninah.characterlib.helpers;

public static class Il2cppListExtensions {
  public static void AddItem<T>(this Il2CppSystem.Collections.Generic.List<T> list, T value) {
    int idxAdd = list._size;
    list._size++;
    list._items[idxAdd] = value;
  }
}
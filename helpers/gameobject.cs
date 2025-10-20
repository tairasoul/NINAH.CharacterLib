using UnityEngine;

namespace tairasoul.ninah.characterlib.helpers;

public static class GameObjectExtensions
{
  public static GameObject Find(this GameObject @object, string name)
  {
    return @object.transform.Find(name).gameObject;
  }

  public static void SetParent(this GameObject @object, GameObject parent, bool worldPositionStays)
  {
    @object.transform.SetParent(parent.transform, worldPositionStays);
  }

  public static GameObject AddObject(this GameObject @object, string name)
  {
    GameObject obj = new(name);
    obj.SetParent(@object, false);
    return obj;
  }

  public static GameObject Instantiate(this GameObject @object)
  {
    return GameObject.Instantiate(@object);
  }

  public static GameObject[] GetChildren(this GameObject @object)
  {
    int childCount = @object.transform.childCount;
    GameObject[] children = new GameObject[childCount];
    for (int i = 0; i < childCount; i++)
    {
      children[i] = @object.transform.GetChild(i).gameObject;
    }
    return children;
  }
}
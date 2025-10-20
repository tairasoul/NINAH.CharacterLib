namespace tairasoul.ninah.characterlib.helpers;

public static class ObjectExtensions {
  public static T GetField<T>(this object obj, string field)
  {
    return obj.GetType().GetF<T>(field, obj);
  }

  public static void SetField(this object obj, string field, object value) 
  {
    obj.GetType().SetF(field, value, obj);
  }

  public static T GetProperty<T>(this object obj, string property) 
  {
    return obj.GetType().GetP<T>(property, obj);
  }

  public static void SetProperty(this object obj, string property, object value) {
    obj.GetType().SetP(property, value, obj);
  }

  public static T CallMethod<T>(this object obj, string method, object?[]? param = null) {
    return (T)obj.GetType().Call(method, obj, param);
  }

  public static void CallMethod(this object obj, string method, object?[]? param = null) {
    obj.GetType().Call(method, obj, param);
  }
}
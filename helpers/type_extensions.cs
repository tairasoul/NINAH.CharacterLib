using System.Reflection;
using System;

namespace tairasoul.ninah.characterlib.helpers;

public static class TypeExtensions
{
	internal static T GetF<T>(this Type obj, string field, object instance) {
		return (T)obj.GetField(field, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
	}

	internal static void SetF(this Type obj, string field, object value, object instance) {
		obj.GetField(field, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
	}

	internal static T GetP<T>(this Type obj, string property, object? instance = null) {
		return (T)obj.GetProperty(property, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).GetValue(instance);
	}

	internal static void SetP(this Type obj, string property, object value, object? instance = null) {
		obj.GetProperty(property, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).SetValue(instance, value);
	}

	internal static object Call(this Type obj, string method, object instance, object?[]? param) {
		return obj.GetMethod(method, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic).Invoke(instance, param);
	}
}
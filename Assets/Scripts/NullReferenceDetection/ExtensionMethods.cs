using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ExtensionMethods
{
    public static IEnumerable<Component> AllComponents(this GameObject gameObject)
    {
        return gameObject.GetComponents<Component>();
    }

    public static IEnumerable<FieldInfo> GetInspectableFields(this Component component)
    {
        var type = component.GetType();
        var inspectableFields = new List<FieldInfo>();

        var publicFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
        var privateFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

        //Private fields can be inspected if they are explicitly serialized
        privateFields = privateFields.Where(f => f.HasAttribute<SerializeField>()).ToList();
        //Add remaining private and public fields to the list of all inspectable fields
        inspectableFields.AddRange(publicFields);
        inspectableFields.AddRange(privateFields);
        //Remove fields that should be hidden in the inspector
        inspectableFields = inspectableFields.Where(f => !f.HasAttribute<HideInInspector>()).ToList();

        return inspectableFields;
    }

    public static bool HasAttribute<T>(this FieldInfo field)
    {
        return field.GetCustomAttributes(typeof(T), false).Any();
    }

    public static bool IsNull(this FieldInfo field, object obj)
    {
        var value = field.GetValue(obj);
        return value == null || value.ToString() == "null";
    }
}

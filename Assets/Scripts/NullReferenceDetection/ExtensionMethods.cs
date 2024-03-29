﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NullReferenceDetection
{
    public static class ExtensionMethods
    {
        public static IEnumerable<Component> AllComponents(this GameObject gameObject)
        {
            return gameObject.GetComponents<Component>();
        }

        public static IEnumerable<FieldInfo> GetInspectableFields(this Component component)
        {
            var componentType = component.GetType();
            var inspectableFields = new List<FieldInfo>();

            var publicFields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance).ToList();
            var privateFields = componentType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToList();

            // Private fields can be inspected if they are explicitly serialized
            privateFields = privateFields.Where(f => f.HasAttribute<SerializeField>()).ToList();
            
            // Add remaining private and public fields to the list of all inspectable fields
            inspectableFields.AddRange(publicFields);
            inspectableFields.AddRange(privateFields);
            
            // Remove fields that should be hidden in the inspector
            inspectableFields = inspectableFields.Where(f => !f.HasAttribute<HideInInspector>()).ToList();

            return inspectableFields;
        }

        public static bool HasAttribute<T>(this FieldInfo field)
        {
            return field.GetCustomAttributes(typeof(T), true).Any();
        }

        public static T GetAttribute<T>(this FieldInfo field)
        {
            return (T)field.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }

        public static bool IsNull(this FieldInfo field, object obj)
        {
            var value = field.GetValue(obj);
            return value == null || value.ToString() == "null";
        }

        public static IEnumerable<Type> GetDescendantTypes(this Type type)
        {
            var allTypes = Assembly.GetExecutingAssembly().GetTypes();
            return allTypes.Where(t => t != type && type.IsAssignableFrom(t)).ToList();
        }
    }
}
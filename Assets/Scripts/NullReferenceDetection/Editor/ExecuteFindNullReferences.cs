using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullReferenceDetection.Editor
{
    public static class ExecuteFindNullReferences
    {
        [MenuItem("Tools/Find Null References")]
        public static void Execute()
        {
            CheckForNullReferences(IsVisible);
        }

        public static bool CheckForNullReferences(Func<NullReference, bool> filter)
        {
            var detector = new NullReferenceDetector();
            var nullReferences = detector.FindAllNullReferences(
                PreferencesSerialization.LoadPrefabList(),
                filter,
                PreferencesSerialization.LoadIgnoreList())
                .ToList();

            foreach (var nullReference in nullReferences)
            {
                var fieldName = ObjectNames.NicifyVariableName(nullReference.FieldName);
                var color = ColorFor(nullReference);

                var message = string.Format("Null reference found in <b>{0}</b> > <b>{1}</b> > <color={2}><b>{3}</b></color>\n",
                    nullReference.GameObjectName,
                    nullReference.ComponentName,
                    color,
                    fieldName);

                Debug.Log(message, nullReference.GameObject);
            }

            return nullReferences.Any();
        }

        public static bool IsNotUnity(NullReference nullReference)
        {
            var namespaceName = nullReference.FieldInfo.FieldType.Namespace;
            return namespaceName != null && !namespaceName
                .Split('.')
                .First()
                .Equals("UnityEngine");
        }

        public static bool IsVisible(NullReference nullReference)
        {
            return PreferencesStorage.IsVisible(nullReference.AttributeIdentifier);
        }

        public static string ColorFor(NullReference nullReference)
        {
            var color = PreferencesStorage.ColorFor(nullReference.AttributeIdentifier);
            return string.Format("#{0}", ColorUtility.ToHtmlStringRGB(color));
        }
    }
}

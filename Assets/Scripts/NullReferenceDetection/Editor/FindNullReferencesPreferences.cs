using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NullReferenceDetection
{
    public class FindNullReferencesPreferences : MonoBehaviour
    {
        [PreferenceItem("Null finder")]
        public static void PreferencesGUI()
        {            
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            var attributes = PreferencesStorage.PersistableAttributes;

            foreach(var attribute in attributes)
            {
                HandleAttributeUI(attribute);
            }
        }

        private static void HandleAttributeUI(PersistableAttribute attribute)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - 6, rect.width, rect.height + 12);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            attribute.IsEnabled = EditorGUILayout.Toggle(attribute.IsEnabled, GUILayout.Width(15));
            EditorGUILayout.LabelField(attribute.Identifier, GUILayout.Width(150));
            attribute.Color = EditorGUILayout.ColorField(attribute.Color);            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);            
        }
    }
}
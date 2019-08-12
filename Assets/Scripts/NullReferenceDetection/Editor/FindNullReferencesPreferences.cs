using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

namespace NullReferenceDetection
{
    public class FindNullReferencesPreferences : MonoBehaviour
    {
        private const float CellMargin = 6;
        
        private static List<Tuple<String, bool>> ignoreList;
        private static bool dirtyIgnoreList;//keeps track of whether the local List has had any changes made to it
        
        private static List<GameObject> prefabsList;
        private static bool dirtyPrefabsList;//keeps track of whether the local list has had any changes made to it

        [PreferenceItem("Null Finder")]
        public static void PreferencesGUI()
        {
            GUILayout.Space(15);

            HandleAttributePreferences();
            
            GUILayout.Space(15);

            HandleIgnoreListPreferences();
       
            GUILayout.Space(15);

            HandlePrefabPreferences();
        }

        #region attribute preferences
        
        /*do the UI elements for attribute tags*/
        private static void HandleAttributePreferences()
        {
            EditorGUILayout.LabelField("Attribute tag preferences");
            GUILayout.Space(5);   
            
            var attributes = PreferencesStorage.PersistableAttributes;

            foreach(var attribute in attributes)
            {
                HandleIndividualAttribute(attribute);
            }
        }

        private static void HandleIndividualAttribute(PersistableAttribute attribute)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, rect.height + CellMargin * 2f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            attribute.IsEnabled = EditorGUILayout.Toggle(attribute.IsEnabled, GUILayout.Width(15));
            EditorGUILayout.LabelField(attribute.Identifier, GUILayout.Width(150));
            attribute.Color = EditorGUILayout.ColorField(attribute.Color, GUILayout.Width(500));            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);            
        }
        
        #endregion
        
        #region ignore-items preferences

        /*do the UI elements for the ignore list*/
        private static void HandleIgnoreListPreferences()
        {
           
            if (ignoreList == null)
                ignoreList = ExtensionMethods.LoadIgnoreList();
                         
            EditorGUILayout.LabelField("Ignore list - enter the name of a GameObject to ignore when scanning (case sensitive)");
            GUILayout.Space(5);
             
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, 10 + CellMargin);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 55 + CellMargin * 2f), new Color(0.5f, 0.5f, 0.5f, 0.3f));//the drawn rectangle is bigger than the actual rectangle to include the buttons
          
            EditorGUI.LabelField(rect, "Use +/- buttons to increase or decrease number of items in ignore list: (" + ignoreList.Count.ToString() + ")");
                         
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
             
            if (GUILayout.Button("+", GUILayout.Width(40))) 
                ignoreList.Add(new Tuple<string, bool>("", false));
                         
            if (GUILayout.Button("-", GUILayout.Width(40)))
                ignoreList.RemoveAt(ignoreList.Count - 1);
                         
            GUILayout.Space(18);
             
            if (ignoreList.Count == 0)
                return;//nothing more to do here
             
            //handle UI and save changes to local ignoreList
            for (int i = 0; i < ignoreList.Count; i++)
                ignoreList[i] = HandleIndividualIgnoreItem(ignoreList[i]);
             
            //save the inputs, if anything changed
            if (dirtyIgnoreList)
            {
                ExtensionMethods.SaveIgnoreList(ignoreList);
                dirtyIgnoreList = false;
            }
        }
        
        //Draws the UI element for each ignore item, and saves the values into the list for use
        private static Tuple<String, bool> HandleIndividualIgnoreItem(Tuple<String, bool> t)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, rect.height + CellMargin * 2f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            
            EditorGUILayout.LabelField("GameObject name: ", GUILayout.Width(112));
            var name = EditorGUILayout.TextField(t.Item1, GUILayout.Width(250));
            EditorGUILayout.LabelField("     Also ignore children: ", GUILayout.Width(137));
            var ignoreChildren = EditorGUILayout.Toggle(t.Item2, GUILayout.Width(15));
            //EditorGUILayout.LabelField(attribute.Identifier, GUILayout.Width(150));
            //attribute.Color = EditorGUILayout.ColorField(attribute.Color);            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);

            if (name != t.Item1 || ignoreChildren != t.Item2)
                dirtyIgnoreList = true;
            
            
            return new Tuple<string, bool>(name.Trim(), ignoreChildren);
        }
        
        #endregion

        #region prefab-item preferences

        private static void HandlePrefabPreferences()
        {
            
            if (prefabsList == null)
                prefabsList = ExtensionMethods.LoadPrefabList();
            
            EditorGUILayout.LabelField("Additional Prefabs to scan (only objects in the scene will be scanned, plus any prefabs which you define here)");
            GUILayout.Space(5);

            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, 10 + CellMargin);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 55 + CellMargin * 2f), new Color(0.5f, 0.5f, 0.5f, 0.3f));//the drawn rectangle is bigger than the actual rectangle to include the buttons
            
            EditorGUI.LabelField(rect, "Use +/- buttons to increase or decrease number of items in external file list: (" + prefabsList.Count.ToString() + ")");
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("+", GUILayout.Width(40))) 
                prefabsList.Add(null);
            
            if (GUILayout.Button("-", GUILayout.Width(40)))
                prefabsList.RemoveAt(prefabsList.Count - 1);
            
            GUILayout.Space(18);

            if (prefabsList.Count == 0)
                return;//nothing more to do here

            //handle UI and save changes to local ignoreList
            for (int i = 0; i < prefabsList.Count; i++)
                prefabsList[i] = HandleIndividualPrefabItems(prefabsList[i]);

            //save the inputs, if anything changed
            if (dirtyPrefabsList)
            {
                ExtensionMethods.SavePrefabList(prefabsList);
                dirtyPrefabsList = false;
            }
        }
        
        private static GameObject HandleIndividualPrefabItems(GameObject previousValue)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, rect.height + CellMargin * 2f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));

            GameObject newValue = null;
            newValue = (GameObject)EditorGUI.ObjectField(new Rect(rect.x, rect.y + 7, 400, 15), "Select a prefab:", previousValue, typeof(GameObject), false);
            
            EditorGUILayout.LabelField("", GUILayout.Width(500));
            
            if (newValue != null)
                EditorGUILayout.LabelField(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(newValue));
          
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);

            if (newValue != previousValue)
            {
                dirtyPrefabsList = true;
                return newValue;
            }
            else
            {
                return previousValue;
            }
        }
        
        #endregion
    }
}
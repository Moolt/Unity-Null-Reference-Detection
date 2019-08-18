using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullReferenceDetection
{
    public class FindNullReferencesPreferences : MonoBehaviour
    {
        private const float CellMargin = 6;

        private static List<BlacklistItem> _ignoreList;
        private static List<GameObject> _prefabsList;

        // Keeps track of whether the local List has had any changes made to it.
        private static bool _dirtyIgnoreList;

        // Keeps track of whether the local list has had any changes made to it
        private static bool _dirtyPrefabsList;

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

        /// <summary>
        /// Do the UI elements for attribute tags
        /// </summary>
        private static void HandleAttributePreferences()
        {
            EditorGUILayout.LabelField("Attribute tag preferences");
            GUILayout.Space(5);

            var attributes = PreferencesStorage.PersistableAttributes;

            foreach (var attribute in attributes)
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

        /// <summary>
        /// Do the UI elements for the ignore list
        /// </summary>
        private static void HandleIgnoreListPreferences()
        {
            if (_ignoreList == null)
            {
                _ignoreList = PreferencesSerialization
                .LoadIgnoreList()
                .ToList();
            }

            EditorGUILayout.LabelField("Ignore list - enter the name of a GameObject to ignore when scanning (case sensitive)");
            GUILayout.Space(5);

            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, 10 + CellMargin);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 55 + CellMargin * 2f), new Color(0.5f, 0.5f, 0.5f, 0.3f));//the drawn rectangle is bigger than the actual rectangle to include the buttons

            EditorGUI.LabelField(rect, "Use +/- buttons to increase or decrease number of items in ignore list: (" + _ignoreList.Count.ToString() + ")");

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                var blacklistItem = new BlacklistItem(string.Empty, ignoreChildren: false);
                _ignoreList.Add(blacklistItem);
            }

            if (GUILayout.Button("-", GUILayout.Width(40)) && _ignoreList.Count > 0)
            {
                _ignoreList.RemoveAt(_ignoreList.Count - 1);
                _dirtyIgnoreList = true;
            }

            GUILayout.Space(18);

            if (_ignoreList.Count != 0)
            {
                // Handle UI and save changes to local ignoreList
                for (int i = 0; i < _ignoreList.Count; i++)
                {
                    _ignoreList[i] = HandleIndividualIgnoreItem(_ignoreList[i]);
                }
            }

            // Save the inputs, if anything changed
            if (_dirtyIgnoreList)
            {
                PreferencesSerialization.SaveIgnoreList(_ignoreList);
                _dirtyIgnoreList = false;
            }
        }

        /// <summary>
        /// Draws the UI element for each ignore item, and saves the values into the list for use
        /// </summary>
        private static BlacklistItem HandleIndividualIgnoreItem(BlacklistItem blacklistItem)
        {
            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, rect.height + CellMargin * 2f);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));

            EditorGUILayout.LabelField("GameObject name: ", GUILayout.Width(112));
            var name = EditorGUILayout.TextField(blacklistItem.Name, GUILayout.Width(250));
            EditorGUILayout.LabelField("     Also ignore children: ", GUILayout.Width(137));
            var ignoreChildren = EditorGUILayout.Toggle(blacklistItem.IgnoreChildren, GUILayout.Width(15));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);

            if (name != blacklistItem.Name || ignoreChildren != blacklistItem.IgnoreChildren)
            {
                _dirtyIgnoreList = true;
            }

            return new BlacklistItem(name.Trim(), ignoreChildren);
        }

        #endregion

        #region prefab-item preferences

        private static void HandlePrefabPreferences()
        {
            if (_prefabsList == null)
            {
                _prefabsList = PreferencesSerialization.LoadPrefabList().ToList();
            }

            EditorGUILayout.LabelField("Additional Prefabs to scan (only objects in the scene will be scanned, plus any prefabs which you define here)");
            GUILayout.Space(5);

            var rect = EditorGUILayout.BeginHorizontal();
            rect = new Rect(rect.x, rect.y - CellMargin, rect.width, 10 + CellMargin);

            // The drawn rectangle is bigger than the actual rectangle to include the buttons
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 55 + CellMargin * 2f), new Color(0.5f, 0.5f, 0.5f, 0.3f));

            EditorGUI.LabelField(rect, "Use +/- buttons to increase or decrease number of items in external file list: (" + _prefabsList.Count.ToString() + ")");

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("+", GUILayout.Width(40)))
            {
                _prefabsList.Add(null);
            }

            if (GUILayout.Button("-", GUILayout.Width(40)) && _prefabsList.Count > 0)
            {
                _prefabsList.RemoveAt(_prefabsList.Count - 1);
                _dirtyPrefabsList = true;
            }

            GUILayout.Space(18);

            // Handle UI and save changes to local ignoreList
            if (_prefabsList.Count > 0)
            {
                for (int i = 0; i < _prefabsList.Count; i++)
                {
                    _prefabsList[i] = HandleIndividualPrefabItems(_prefabsList[i]);
                }
            }

            // Save the inputs if anything changed
            if (_dirtyPrefabsList)
            {
                PreferencesSerialization.SavePrefabList(_prefabsList);
                _dirtyPrefabsList = false;
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
            {
                EditorGUILayout.LabelField(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(newValue));
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(14);

            if (newValue != previousValue)
            {
                _dirtyPrefabsList = true;
                return newValue;
            }

            return previousValue;
        }

        #endregion
    }
}
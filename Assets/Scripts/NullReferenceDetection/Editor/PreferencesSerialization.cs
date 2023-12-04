using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullReferenceDetection.Editor
{
    public static class PreferencesSerialization
    {
        #region save/load functions

        // TODO: Where would we like to save the files?
        private static readonly string FolderPath = string.Empty;
        private static readonly string IgnoreListFile = FolderPath + "NullReferenceDetector.IgnoreList";
        private static readonly string PrefabListFile = FolderPath + "NullReferenceDetector.PrefabList";

        /// <summary>
        /// Applies the values entered in the GUI and saves them for future sessions.
        /// </summary>
        public static void SaveIgnoreList(IEnumerable<BlacklistItem> ignoreList)
        {
            // Remove empty entries
            ignoreList = ignoreList.Where(item => !item.NameEmpty).ToList();

            try
            {
                var writer = new StreamWriter(IgnoreListFile, false);

                foreach (var itemToIgnore in ignoreList)
                {
                    var children = itemToIgnore.IgnoreChildren ? "1" : "0";
                    writer.WriteLine($"{itemToIgnore.Name} {children}");
                }

                writer.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Reads values from file and returns list.
        /// </summary>
        public static IEnumerable<BlacklistItem> LoadIgnoreList()
        {
            var ignoreList = new List<BlacklistItem>();

            if (!File.Exists(IgnoreListFile))
            {
                File.Create(IgnoreListFile);
                Debug.LogWarning("Null reference checker found no previously saved preferences file, so it created a new one");
                return ignoreList;
            }

            try
            {
                var reader = new StreamReader(IgnoreListFile);

                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();

                    if (line == null)
                    {
                        continue;
                    }
                    
                    if (line.Trim().Length == 0)
                    {
                        continue;
                    }

                    var name = line.Substring(0, line.Length - 2);

                    if (line[^1] == '1')
                    {
                        ignoreList.Add(new BlacklistItem(name, ignoreChildren: true));
                    }
                    if (line[^1] == '0')
                    {
                        ignoreList.Add(new BlacklistItem(name, ignoreChildren: false));
                    }
                    else
                    {
                        throw new IOException("Null Reference Checker encountered a problem when loading saved file.");
                    }
                }

                reader.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }

            return ignoreList;
        }

        /// <summary>
        /// Applies the values entered in the GUI and saves them for future sessions
        /// </summary>
        public static void SavePrefabList(IEnumerable<GameObject> prefabList)
        {
            try
            {
                var writer = new StreamWriter(PrefabListFile, false);

                foreach (var itemToSave in prefabList)
                {
                    if (itemToSave == null)
                    {
                        continue;
                    }

                    writer.WriteLine(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(itemToSave));
                }

                writer.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// Reads values from file and returns list.
        /// When saved to file the paths of the prefabs are saved, not the prefabs themselves.
        /// </summary>
        public static IEnumerable<GameObject> LoadPrefabList()
        {
            var results = new List<GameObject>();

            if (!File.Exists(PrefabListFile))
            {
                File.Create(PrefabListFile);
                Debug.LogWarning("Null reference checker found no previously saved preferences file, so it created a new one");
                return new List<GameObject>();
            }

            try
            {
                var reader = new StreamReader(PrefabListFile);

                while (reader.Peek() != -1)
                {
                    var line = reader.ReadLine();

                    if (line == null)
                    {
                        continue;
                    }
                    
                    if (line.Trim().Length == 0)
                    {
                        continue;
                    }

                    var loadedAsset = AssetDatabase.LoadAssetAtPath<GameObject>(line);

                    if (loadedAsset != null)
                    {
                        results.Add(loadedAsset);
                    }
                    else
                    {
                        Debug.LogWarning("Null reference checker found no prefab at path " +
                        line +
                        " perhaps the path or filename changed. this path will be deleted from the list of prefabs and needs to be re-added manually");
                    }
                }

                reader.Dispose();
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
            return results;
        }

        #endregion
    }
}
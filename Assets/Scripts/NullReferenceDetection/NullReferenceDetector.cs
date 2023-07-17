using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
using UnityObject = UnityEngine.Object;

namespace NullReferenceDetection
{
    public class NullReferenceDetector
    {
        public IEnumerable<NullReference> FindAllNullReferences(
            IEnumerable<GameObject> prefabList,
            IEnumerable<BlacklistItem> ignoreList = null)
        {
            var sceneObjects = UnityObject.FindObjectsOfType<GameObject>();
            var prefabObjects = FindObjectsInPrefabs(prefabList);
            var allObjects = sceneObjects.Concat(prefabObjects);

            if (ignoreList != null)
            {
                allObjects = RemoveIgnoredItems(allObjects, ignoreList);
            }

            return allObjects.SelectMany(FindNullReferencesIn).ToList();
        }

        public IEnumerable<NullReference> FindAllNullReferences(
            IEnumerable<GameObject> prefabList,
            Func<NullReference, bool> predicate,
            IEnumerable<BlacklistItem> ignoreList = null)
        {
            return FindAllNullReferences(prefabList, ignoreList).Where(predicate).ToList();
        }

        private IEnumerable<NullReference> FindNullReferencesIn(GameObject gameObject)
        {
            var components = gameObject.AllComponents();
            return components.Where(n => n != null).SelectMany(FindNullReferencesIn);
        }

        private IEnumerable<NullReference> FindNullReferencesIn(Component component)
        {
            var inspectableFields = component.GetInspectableFields();
            var nullFields = inspectableFields.Where(f => f.IsNull(component));

            return nullFields.Select(f =>
                new NullReference
                {
                    Source = component,
                    FieldInfo = f,
                    Attribute = AttributeFor(f)
                }).ToList();
        }

        private Type AttributeFor(FieldInfo field)
        {
            if (field.HasAttribute<BaseAttribute>())
            {
                return field.GetAttribute<BaseAttribute>().GetType();
            }

            return null;
        }

        private IEnumerable<GameObject> RemoveIgnoredItems(IEnumerable<GameObject> inputObjects, IEnumerable<BlacklistItem> ignoreList)
        {
            var objectsToBeRemoved = new List<GameObject>();

            // Make a list of gameobjects to be filtered out (not really efficient but it doesnt need to be 👿)
            foreach (var blacklistItem in ignoreList)
            {
                foreach (var targetObject in inputObjects)
                {
                    if (targetObject.name != blacklistItem.Name)
                    {
                        continue;
                    }
                    objectsToBeRemoved.Add(targetObject);

                    if (!blacklistItem.IgnoreChildren)
                    {
                        continue;
                    }

                    foreach (Transform child in targetObject.transform)
                    {
                        objectsToBeRemoved.Add(child.gameObject);
                    }
                }
            }

            // Now remove those items we found
            var cleanedList = inputObjects.ToList();

            foreach (var targetObject in objectsToBeRemoved)
            {
                cleanedList.Remove(targetObject);
            }

            return cleanedList;
        }

        private IEnumerable<GameObject> FindObjectsInPrefabs(IEnumerable<GameObject> prefabList)
        {
            var foundObjects = new List<GameObject>();

            foreach (var gameObject in prefabList)
            {
                foundObjects.Add(gameObject);
                foundObjects.AddRange(gameObject.GetComponentsInChildren<Transform>().Select(t => t.gameObject));
            }

            return foundObjects;
        }
    }
}

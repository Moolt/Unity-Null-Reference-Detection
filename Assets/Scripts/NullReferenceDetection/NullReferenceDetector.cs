using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityObject = UnityEngine.Object;
using System;

namespace NullReferenceDetection
{
    public class NullReferenceDetector
    {
        public IEnumerable<NullReference> FindAllNullReferences(List<Tuple<string, bool>> ignoreList, List<GameObject> prefabList)
        {        
            var sceneObjects = UnityObject.FindObjectsOfType<GameObject>();

            var prefabObjects = FindObjectsInPrefabs(prefabList); 
            
            var allObjects = sceneObjects.Concat(prefabObjects).ToArray();
                
            allObjects = RemoveIgnoredItems(allObjects, ignoreList);
            
            return allObjects.SelectMany(o => FindNullReferencesIn(o));
        }

        public IEnumerable<NullReference> FindAllNullReferences(Func<NullReference,bool> filter, List<Tuple<string, bool>> ignoreList, List<GameObject> prefabList)
        {
            return FindAllNullReferences(ignoreList, prefabList).Where(filter).ToList();
        }

        private IEnumerable<NullReference> FindNullReferencesIn(GameObject gameObject)
        {
            var components = gameObject.AllComponents();
            return components.Where(n=>n!=null).SelectMany(c => FindNullReferencesIn(c));
        }

        private IEnumerable<NullReference> FindNullReferencesIn(Component component)
        {
            var inspectableFields = component.GetInspectableFields();
            var nullFields = inspectableFields.Where(f => f.IsNull(component));

            return nullFields.Select(f => new NullReference { Source = component, FieldInfo = f, Attribute = AttributeFor(f) });
        }

        private Type AttributeFor(FieldInfo field)
        {
            if (field.HasAttribute<BaseAttribute>())
            {
                return field.GetAttribute<BaseAttribute>().GetType();
            }

            return null;
        }

        private GameObject[] RemoveIgnoredItems(GameObject[] objects, List<Tuple<string, bool>> ignoreList)
        {            
            List<GameObject> objectsToBeRemoved = new List<GameObject>();
            
            ////1. make a list of gameobjects to be filtered out (not really efficient but it doesnt need to be 👿)
            foreach (var tuple in ignoreList)
            {
                foreach (GameObject g in objects)
                {
                    if (g.name == tuple.Item1)
                    {
                        objectsToBeRemoved.Add(g);

                        if (tuple.Item2)
                        {
                            foreach (Transform t in g.transform)
                            {
                                objectsToBeRemoved.Add(t.gameObject);
                            }
                        }
                    }
                }
            }

            ////2. now remove those items we found
            
            List<GameObject> cleanedList = objects.ToList();

            foreach (GameObject g in objectsToBeRemoved)
            {
                cleanedList.Remove(g);
            }
            
            return cleanedList.ToArray();
        }

        private GameObject[] FindObjectsInPrefabs(List<GameObject> prefabList)
        {
            List<GameObject> foundObjects = new List<GameObject>();
            
            foreach (var gameObject in prefabList)
            {
                foundObjects.Add(gameObject);
                foundObjects.AddRange(gameObject.GetComponentsInChildren<Transform>().Select(t => t.gameObject));
            }

            return foundObjects.ToArray();
        }
    }
}
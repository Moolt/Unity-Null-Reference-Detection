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
        public IEnumerable<NullReference> FindAllNullReferences()
        {
            var objects = UnityObject.FindObjectsOfType<GameObject>();
            return objects.SelectMany(o => FindNullReferencesIn(o));
        }

        public IEnumerable<NullReference> FindAllNullReferences(Func<NullReference,bool> filter)
        {
            return FindAllNullReferences().Where(filter).ToList();
        }

        private IEnumerable<NullReference> FindNullReferencesIn(GameObject gameObject)
        {
            var components = gameObject.AllComponents();
            return components.SelectMany(c => FindNullReferencesIn(c));
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
    }
}
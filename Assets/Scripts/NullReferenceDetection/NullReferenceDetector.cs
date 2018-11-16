using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityObject = UnityEngine.Object;

namespace NullReferenceDetection
{
    public class NullReferenceDetector
    {
        public IEnumerable<NullReference> FindAllNullReferences()
        {
            var objects = UnityObject.FindObjectsOfType<GameObject>();
            return objects.SelectMany(o => FindNullReferencesIn(o));
        }

        public IEnumerable<NullReference> FindAllNullReferences(bool removeOptionalValues)
        {
            if (removeOptionalValues)
            {
                return FindAllNullReferences().Where(r => r.Severity != NullReferenceSeverity.Ignore).ToList();
            }
            return FindAllNullReferences();
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

            return nullFields.Select(f => new NullReference { Source = component, FieldInfo = f, Severity = SeverityFor(f) });
        }

        private NullReferenceSeverity SeverityFor(FieldInfo field)
        {
            if (field.HasAttribute<ValueRequired>())
            {
                return NullReferenceSeverity.Severe;
            }

            if (field.HasAttribute<ValueOptional>())
            {
                return NullReferenceSeverity.Ignore;
            }

            return NullReferenceSeverity.Normal;
        }
    }
}
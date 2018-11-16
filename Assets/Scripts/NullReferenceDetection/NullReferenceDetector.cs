using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public enum NullReferenceSeverity
{
    Ignore,
    Normal,
    Severe
}

public struct NullReference
{
    public NullReferenceSeverity Severity;
    public FieldInfo FieldInfo;
    public Component Source;

    public GameObject GameObject
    {
        get
        {
            return Source.gameObject;
        }
    }

    public string GameObjectName
    {
        get
        {
            return Source.gameObject.name;
        }
    }

    public string ComponentName
    {
        get
        {
            return Source.GetType().ToString();
        }
    }

    public string FieldName
    {
        get
        {
            return FieldInfo.Name;
        }
    }
}

public class NullReferenceDetector {

    public IEnumerable<NullReference> FindAllNullReferences()
    {
        var objects = Object.FindObjectsOfType<GameObject>();
        var result = objects.SelectMany(o => FindNullReferencesIn(o)).ToList();
        return objects.SelectMany(o => FindNullReferencesIn(o));
    }

    private IEnumerable<NullReference> FindNullReferencesIn(GameObject gameObject)
    {
        var components = gameObject.AllComponents();
        var result = components.SelectMany(c => FindNullReferencesIn(c)).ToList();
        return components.SelectMany(c => FindNullReferencesIn(c));
    }

    private IEnumerable<NullReference> FindNullReferencesIn(Component component)
    {        
        var inspectableFields = component.GetInspectableFields();
        var nullFields = inspectableFields.Where(f => f.IsNull(component)).ToList();

        var result = nullFields.Select(f => new NullReference { Source = component, FieldInfo = f, Severity = NullReferenceSeverity.Normal }).ToList();
        return nullFields.Select(f => new NullReference { Source = component, FieldInfo = f, Severity = NullReferenceSeverity.Normal });        
    }
}

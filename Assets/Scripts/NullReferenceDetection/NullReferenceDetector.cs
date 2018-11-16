using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
using UnityObject = UnityEngine.Object;

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
        var objects = UnityObject.FindObjectsOfType<GameObject>();
        return objects.SelectMany(o => FindNullReferencesIn(o));
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

        return nullFields.Select(f => new NullReference { Source = component, FieldInfo = f, Severity = NullReferenceSeverity.Normal });        
    }
}

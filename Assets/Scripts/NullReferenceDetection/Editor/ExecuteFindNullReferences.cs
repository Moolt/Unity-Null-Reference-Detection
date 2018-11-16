using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public static class ExecuteFindNullReferences {

	[MenuItem("Tools/Find null references")]
    public static void Execute()
    {
        var detector = new NullReferenceDetector();
        var nullReferences = detector.FindAllNullReferences().ToList();

        foreach(var nullReference in nullReferences)
        {
            var fieldName = ObjectNames.NicifyVariableName(nullReference.FieldInfo.Name);
            var message = string.Format("Null reference found in <b>{0}</b> > <b>{1}</b> > <color=#FF0000><b>{2}</b></color>", nullReference.GameObject.name, nullReference.Source.GetType().ToString(), fieldName);
            Debug.Log(message, nullReference.GameObject);
        }
    }
}
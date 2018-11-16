using UnityEditor;
using UnityEngine;
using System.Linq;

public static class ExecuteFindNullReferences
{
    [MenuItem("Tools/Find null references")]
    public static void Execute()
    {
        var detector = new NullReferenceDetector();
        var nullReferences = detector.FindAllNullReferences(removeOptionalValues: true).ToList();

        foreach (var nullReference in nullReferences)
        {
            var fieldName = ObjectNames.NicifyVariableName(nullReference.FieldName);
            var color = ColorForSeverity(nullReference.Severity);

            var message = string.Format("Null reference found in <b>{0}</b> > <b>{1}</b> > <color={2}><b>{3}</b></color>\n",
                nullReference.GameObjectName,
                nullReference.ComponentName,
                color,
                fieldName);

            Debug.Log(message, nullReference.GameObject);
        }
    }

    private static string ColorForSeverity(NullReferenceSeverity severity)
    {
        return severity == NullReferenceSeverity.Normal ? "#b29400" : "#ff0000";
    }
}
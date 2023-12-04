using UnityEditor;
using UnityEditor.Callbacks;

namespace NullReferenceDetection.Editor
{
    public static class CompileTimeCheck
    {
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            if (!PersistableBoolean.CheckOnCompile)
            {
                return;
            }

            EditorApplication.delayCall += CheckForNullReferences;
        }

        private static void CheckForNullReferences()
        {
            EditorApplication.delayCall -= CheckForNullReferences;
            ExecuteFindNullReferences.CheckForNullReferences();
        }
    }
}
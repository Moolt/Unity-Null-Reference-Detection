using UnityEditor;
using UnityEngine;

namespace NullReferenceDetection.Editor
{
    [InitializeOnLoad]
    public static class PlayModeCheck
    {
        static PlayModeCheck()
        {
            EditorApplication.playModeStateChanged += CheckForNullReferences;
        }

        private static void CheckForNullReferences(PlayModeStateChange state)
        {
            if (!PersistableBoolean.CheckOnPlay)
            {
                return;
            }

            if (!PersistableBoolean.CheckOnPlayIntercept && state == PlayModeStateChange.EnteredPlayMode)
            {
                ExecuteFindNullReferences.CheckForNullReferences();
                return;
            }

            if (PersistableBoolean.CheckOnPlayIntercept && state == PlayModeStateChange.ExitingEditMode)
            {
                var hasNullReferences = ExecuteFindNullReferences.CheckForNullReferences();

                if (hasNullReferences)
                {
                    EditorApplication.isPlaying = false;
                    Debug.LogError(
                        "Entering play mode has been stopped because null references have been found on one or more objects.\n Fix these errors or disable the null reference check in the settings.");
                }
            }
        }
    }
}
using System.Reflection;
using UnityEngine;

namespace NullReferenceDetection
{
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
}
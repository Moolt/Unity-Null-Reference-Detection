using System;
using System.Reflection;
using UnityEngine;

namespace NullReferenceDetection
{
    public struct NullReference
    {
        public static readonly string UnattributedIdentifier = "Unattributed";

        public Type Attribute { get; set; }

        public FieldInfo FieldInfo { get; set; }

        public Component Source { get; set; }

        public GameObject GameObject => Source.gameObject;

        public string GameObjectName => Source.gameObject.name;

        public string ComponentName => Source.GetType().ToString();

        public string FieldName => FieldInfo.Name;

        public bool IsAttributed => Attribute != null;

        public string AttributeIdentifier
        {
            get
            {
                return Attribute != null ? Attribute.Name : UnattributedIdentifier;
            }
        }
    }
}
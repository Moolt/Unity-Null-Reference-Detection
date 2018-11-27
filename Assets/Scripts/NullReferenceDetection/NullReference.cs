using System;
using System.Reflection;
using UnityEngine;

namespace NullReferenceDetection
{
    public struct NullReference
    {
        public static readonly string UnattributedIdentifier = "Unattributed";

        public Type Attribute;
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

        public bool IsAttributed
        {
            get
            {
                return Attribute != null;
            }
        }

        public string AttributeIdentifier
        {
            get
            {
                return Attribute != null ? Attribute.Name : UnattributedIdentifier;
            }
        }
    }
}
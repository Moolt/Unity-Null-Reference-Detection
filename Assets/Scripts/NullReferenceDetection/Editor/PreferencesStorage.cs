using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NullReferenceDetection
{
    public static class PreferencesStorage
    {
        private static readonly Dictionary<string, PersistableAttribute> mapping = new Dictionary<string, PersistableAttribute>();

        static PreferencesStorage()
        {
            InitializeMapping();
        }

        private static void InitializeMapping()
        {
            var attributes = typeof(BaseAttribute).GetDescendantTypes();

            // Initialize settings for unattributes fields
            var unattributedWrapper = new PersistableAttribute(NullReference.UnattributedIdentifier, true, new Color(.717f, .647f, .125f));
            mapping.Add(NullReference.UnattributedIdentifier, unattributedWrapper);

            // Initialize Attributes
            foreach (var attribute in attributes)
            {
                PersistableAttribute persistenceWrapper;

                if (attribute == typeof(ValueOptional))
                {
                    persistenceWrapper = new PersistableAttribute(attribute.Name, false, Color.black);
                }
                else if (attribute == typeof(ValueRequired))
                {
                    persistenceWrapper = new PersistableAttribute(attribute.Name, true, new Color(.718f, .129f, .176f));
                }
                else
                {
                    persistenceWrapper = new PersistableAttribute(attribute.Name);
                }

                mapping.Add(attribute.Name, persistenceWrapper);
            }
        }

        public static PersistableAttribute PersistableAttributeFor(string attributeName)
        {
            return mapping[attributeName];
        }

        public static PersistableAttribute PersistableAttributeFor(Type attributeType)
        {
            return PersistableAttributeFor(attributeType.Name);
        }

        public static bool IsVisible(string attributeName)
        {
            return mapping[attributeName].IsEnabled;
        }

        public static Color ColorFor(string attributeName)
        {
            return mapping[attributeName].Color;
        }

        public static IEnumerable<PersistableAttribute> PersistableAttributes
        {
            get
            {
                return mapping.Values.ToList();
            }
        }
    }
}
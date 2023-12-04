using UnityEditor;
using UnityEngine;

namespace NullReferenceDetection.Editor
{
    public class PersistableAttribute
    {
        private const string EnabledPostfix = "enabled";
        private const string ColorPostfix = "color";

        private readonly bool _defaultEnabledState;
        private readonly Color _defaultColor = Color.black;

        private readonly string _identifier;
        
        private bool? _enabled;
        private Color? _color;

        public PersistableAttribute(string identifier)
        {
            _identifier = identifier;
        }

        public PersistableAttribute(string identifier, bool enabled, Color color) : this(identifier)
        {
            _defaultEnabledState = enabled;
            _defaultColor = color;

            if (!IsEntryExistingFor(EnabledPostfix))
            {
                IsEnabled = enabled;
            }

            if (!IsEntryExistingFor(ColorPostfix))
            {
                Color = color;
            }
        }

        public string Identifier => _identifier;

        public bool IsEnabled
        {
            get
            {
                if (_enabled == null)
                {
                    if (IsEntryExistingFor(EnabledPostfix))
                    {
                        _enabled = EditorPrefs.GetBool(KeyForPostfix(EnabledPostfix));
                    }
                    else
                    {
                        IsEnabled = true;
                    }
                }
                return _enabled ?? _defaultEnabledState;
            }
            set
            {
                _enabled = value;
                EditorPrefs.SetBool(KeyForPostfix(EnabledPostfix), value);
            }
        }

        public Color Color
        {
            get
            {
                if (_color == null)
                {
                    if (IsEntryExistingFor(ColorPostfix))
                    {
                        var colorString = EditorPrefs.GetString(KeyForPostfix(ColorPostfix));

                        Color localColor;
                        ColorUtility.TryParseHtmlString(colorString, out localColor);
                        _color = localColor;
                    }
                    else
                    {
                        Color = _defaultColor;
                    }
                }
                return _color ?? _defaultColor;
            }
            set
            {
                _color = value;
                var colorInstance = _color ?? _defaultColor;
                var hexColor = $"#{ColorUtility.ToHtmlStringRGB(colorInstance)}";
                EditorPrefs.SetString(KeyForPostfix(ColorPostfix), hexColor);
            }
        }

        private string KeyForPostfix(string postfix)
        {
            return $"null_helper_{_identifier}_{postfix}";
        }

        private bool IsEntryExistingFor(string postfix)
        {
            return EditorPrefs.HasKey(KeyForPostfix(postfix));
        }
    }
}

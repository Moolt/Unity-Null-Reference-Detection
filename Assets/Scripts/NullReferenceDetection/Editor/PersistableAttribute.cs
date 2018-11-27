using UnityEditor;
using UnityEngine;

namespace NullReferenceDetection
{
    public class PersistableAttribute
    {
        private const string EnabledPostfix = "enabled";
        private const string ColorPostfix = "color";

        private string _identifier;
        private bool? _enabled = null;
        private Color? _color = null;

        public PersistableAttribute(string identifier)
        {
            _identifier = identifier;
        }

        public PersistableAttribute(string identifier, bool enabled, Color color) : this(identifier)
        {
            if (!IsEntryExistingFor(EnabledPostfix))
            {
                IsEnabled = enabled;
            }

            if (!IsEntryExistingFor(ColorPostfix))
            {
                Color = color;
            }
        }

        public string Identifier
        {
            get
            {
                return _identifier;
            }
        }

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
                return _enabled ?? false;
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
                        Color = Color.black;
                    }
                }
                return _color ?? Color.black;
            }
            set
            {
                _color = value;
                var colorInstance = _color ?? Color.black;
                var hexColor = string.Format("#{0}", ColorUtility.ToHtmlStringRGB(colorInstance));
                EditorPrefs.SetString(KeyForPostfix(ColorPostfix), hexColor);
            }
        }

        private string KeyForPostfix(string postix)
        {
            return string.Format("null_helper_{0}_{1}", _identifier, postix);
        }

        private bool IsEntryExistingFor(string postfix)
        {
            return EditorPrefs.HasKey(KeyForPostfix(postfix));
        }
    }
}

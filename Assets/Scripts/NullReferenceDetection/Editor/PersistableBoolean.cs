using UnityEditor;

namespace NullReferenceDetection.Editor
{
    public class PersistableBoolean
    {
        private readonly string _identifier;
        private readonly bool _defaultValue;

        public PersistableBoolean(string identifier, bool defaultValue)
        {
            _identifier = identifier;
            _defaultValue = defaultValue;
        }
        
        public static implicit operator bool(PersistableBoolean persistableBoolean) => persistableBoolean.Value;

        public string Identifier => _identifier;

        public bool Value
        {
            get
            {
                if (!EditorPrefs.HasKey(_identifier))
                {
                    EditorPrefs.SetBool(_identifier, _defaultValue);
                }

                return EditorPrefs.GetBool(_identifier);
            }
            set => EditorPrefs.SetBool(_identifier, value);
        }

        public bool DefaultValue => _defaultValue;

        public static PersistableBoolean CheckOnPlay = new PersistableBoolean("null_helper_check_play", false);
        public static PersistableBoolean CheckOnPlayIntercept = new PersistableBoolean("null_helper_check_play_intercept", false);
        public static PersistableBoolean CheckOnCompile = new PersistableBoolean("null_helper_check_compile", false);
    }
}
namespace NullReferenceDetection
{
    public class BlacklistItem
    {
        public BlacklistItem(string name, bool ignoreChildren)
        {
            Name = name;
            IgnoreChildren = ignoreChildren;
        }

        public string Name { get; }

        public bool IgnoreChildren { get; }

        public bool NameEmpty => string.IsNullOrEmpty(Name);
    }
}
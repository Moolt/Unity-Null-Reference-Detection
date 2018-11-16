using System;

namespace NullReferenceDetection
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ValueRequired : Attribute
    {
    }
}
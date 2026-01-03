namespace BetterLadders.Config;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class SourceGenAttribute(Type targetFieldType, Type targetClassType) : Attribute
{
}
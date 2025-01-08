using System;
using System.Reflection;

namespace AoC.Solutions;

/// <summary>
///     Apply to a field or property a value that should replace the current value when the Test Cases are running. A
///     field with this Attribute cannot be const nor static readonly and an auto-property must have a setter.
/// </summary>
/// <param name="args">Arguments to match the Type of the field/property or to be used in a constructor for that Type</param>
/// <remarks>
///     It's up to the developer to ensure the Attribute's constructor is used and the arguments match the Type of the
///     field/property if primitive or can be used in a constructor for one if not primitive. This is not enforced and
///     will throw.
/// </remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class TestValueAttribute(params object[] args) : Attribute
{
    public readonly object[] Args = args;
}

public static class SolverModifier
{
    public static void ApplyTestValues<T>(this T instance) where T : ISolver
    {
        var type = instance.GetType();
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                          BindingFlags.Instance | BindingFlags.Static;

        // override field value
        foreach (var field in type.GetFields(bindingFlags))
        {
            var attribute = field.GetCustomAttribute<TestValueAttribute>();
            if (attribute == null) continue;
            var value = GetValueFromAttribute(field.FieldType, attribute);
            field.SetValue(instance, value);
        }

        // override property value
        foreach (var property in type.GetProperties(bindingFlags))
        {
            var attribute = property.GetCustomAttribute<TestValueAttribute>();
            if (attribute == null) continue;
            var value = GetValueFromAttribute(property.PropertyType, attribute);
            property.SetValue(instance, value);
        }
    }

    private static object? GetValueFromAttribute(Type type, TestValueAttribute attribute)
    {
        var args = attribute.Args;

        if (type.IsPrimitive || type == typeof(string))
            return args.Length == 1
                ? Convert.ChangeType(args[0], type)
                : throw new ArgumentException(
                    $"Cannot initialize primitive or string type '{type}' with {args.Length} arguments.");

        return Activator.CreateInstance(type, args);
    }
}
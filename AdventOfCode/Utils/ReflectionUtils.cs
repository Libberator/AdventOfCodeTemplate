using System;
using System.Linq;

namespace AoC;

public static partial class Utils
{
    public static T GetClassOfType<T>(string className, params object[] args)
    {
        var genericType = typeof(T).Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(T)))
            .FirstOrDefault(t => t.Name == className);

        if (genericType is null)
            throw new Exception($"There is no class named {className}");

        if (Activator.CreateInstance(genericType, args) is not T instance)
            throw new Exception($"Somehow the class {className} does not implement {nameof(T)}... which should be impossible");

        return instance;
    }
}
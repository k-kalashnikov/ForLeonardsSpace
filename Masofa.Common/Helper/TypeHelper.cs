namespace Masofa.Common.Helper
{
    public static class TypeHelper
    {
        public static Type? GetTypeFromAllAssemblies(string typeName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType(typeName);
                if (type != null)
                    return type;
            }

            return null;
        }
    }
}

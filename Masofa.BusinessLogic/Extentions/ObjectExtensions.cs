using System.Reflection;

namespace Masofa.BusinessLogic.Extentions
{
    public static class ObjectExtensions
    {
        public static void CopyFrom<T>(this T target, object source) where T : class
        {
            var targetType = target.GetType();
            var sourceType = source.GetType();

            var props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                  .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var targetProp in props)
            {
                var sourceProp = sourceType.GetProperty(targetProp.Name);
                if (sourceProp?.CanRead == true && sourceProp.PropertyType == targetProp.PropertyType)
                {
                    var value = sourceProp.GetValue(source);
                    targetProp.SetValue(target, value);
                }
            }
        }
    }
}

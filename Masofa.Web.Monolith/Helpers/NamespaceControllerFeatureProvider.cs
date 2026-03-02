using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Masofa.Web.Monolith.Helpers
{
    public class NamespaceControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly string _namespacePrefix;

        public NamespaceControllerFeatureProvider(string namespacePrefix)
        {
            _namespacePrefix = namespacePrefix;
        }

        protected override bool IsController(TypeInfo typeInfo)
        {
            var isController = base.IsController(typeInfo);
            return isController && typeInfo.Namespace?.StartsWith(_namespacePrefix) == true;
        }
    }

    public class ConfigurableControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly string _allowedNamespace;

        public ConfigurableControllerFeatureProvider(string allowedNamespace)
        {
            _allowedNamespace = allowedNamespace;
        }

        protected override bool IsController(TypeInfo typeInfo)
        {
            var isController = base.IsController(typeInfo);

            if (!isController || string.IsNullOrEmpty(_allowedNamespace))
            {
                return false;
            }

            return typeInfo.Namespace.Contains(_allowedNamespace);
        }
    }
}

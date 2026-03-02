namespace Masofa.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, Inherited = false, AllowMultiple = false)]
    public class SystemFieldNotForImport : Attribute { }
}

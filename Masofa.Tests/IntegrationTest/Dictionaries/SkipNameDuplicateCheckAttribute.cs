using System;

namespace Masofa.Tests.IntegrationTest.Dictionaries.Attributes
{
    /// <summary>
    /// Атрибут, указывающий, что для данной модели
    /// не требуется проверка уникальности по полю Name/Names в тестах.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SkipNameDuplicateCheckAttribute : Attribute
    {
    }
}
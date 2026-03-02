using NetTopologySuite.Geometries;

using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Tiles
{
    /// <summary>
    /// Базовая сущность слоя с пространственными данными.
    /// </summary>
    public class BaseLayer : BaseEntity
    {
        /// <summary>
        /// Координата X левого-верхнего угла ограничивающего прямоугольника
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Координата Y левого-верхнего угла ограничивающего прямоугольника
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Геометрия полигона и/или дополнительные данные для отрисовки слоя.
        /// </summary>
        public Geometry? PoligonData { get; set; }

        /// <summary>
        /// Тип слоя, сохраняемый в БД
        /// </summary>
        public LayerType LayerType { get; set; }

        /// <summary>
        /// Строковое представление <see cref="LayerType"/>.  
        /// Используется для сериализации/десериализации, не хранится в БД.
        /// </summary>
        [NotMapped]
        public string LayerTypeName
        {
            get => LayerType.ToString();
            set
            {
                if (Enum.TryParse<LayerType>(value, true, out var parsed))
                    LayerType = parsed;
                else
                    LayerType = LayerType.Default;
            }
        }
    }

    /// <summary>
    /// Перечисление возможных типов слоя данных
    /// </summary>
    public enum LayerType
    {
        /// <summary>
        /// Тип по умолчанию
        /// </summary>
        Default = 0,

        /// <summary>
        /// Слой с климатическими/погодными данными.
        /// </summary>
        Weather = 1
    }
}

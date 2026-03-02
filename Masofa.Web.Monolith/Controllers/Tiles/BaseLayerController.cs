using Masofa.BusinessLogic.Services.BusinessLogicLogger;
using Masofa.Common.Models.Tiles;
using Masofa.Common.Services.FileStorage;
using Masofa.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using SkiaSharp;

namespace Masofa.Web.Monolith.Controllers.Tiles
{
    /// <summary>
    /// Базовый контроллер для всех слоёв данных
    /// </summary>
    /// <typeparam name="TModel">Конкретный наследник <see cref="BaseLayer"/>.</typeparam>
    /// <typeparam name="TDbContext">Контекст EF Core</typeparam>
    [Route("tiles/[controller]")]
    [ApiExplorerSettings(GroupName = "Tiles")]
    public abstract class BaseLayerController<TModel, TDbContext> : BaseCrudController<TModel, TDbContext>
        where TModel : BaseLayer
        where TDbContext : MasofaTileDbContext
    {
        protected const int TileSize = 256;
        protected const int EarthRadius = 6378137;

        #region конструктор
        protected BaseLayerController(
            IFileStorageProvider fileStorageProvider,
            TDbContext dbContext,
            ILogger logger,
            IConfiguration configuration,
            IMediator mediator,
            IBusinessLogicLogger businessLogicLogger,
            IHttpContextAccessor httpContextAccessor) : base(
                fileStorageProvider,
                dbContext,
                logger,
                configuration,
                mediator,
                businessLogicLogger,
                httpContextAccessor)
        { }
        #endregion

        #region отрисовка тайлов — единая виртуальная реализация
        /// <summary>
        /// Отрисовывает тайл слоя без фильтрации по дате
        /// </summary>
        /// <param name="zoom">Уровень масштабирования (zoom level)</param>
        /// <param name="x">X координата тайла</param>
        /// <param name="y">Y координата тайла</param>
        /// <returns>PNG изображение тайла</returns>
        /// <response code="200">Тайл успешно отрисован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet("draw/{zoom}/{x}/{y}.png")]
        public virtual byte[] DrawLayer(int zoom, int x, int y)
            => DrawLayerInternal(zoom, x, y, null);

        /// <summary>
        /// Отрисовывает тайл слоя с фильтрацией по дате
        /// </summary>
        /// <param name="zoom">Уровень масштабирования (zoom level)</param>
        /// <param name="x">X координата тайла</param>
        /// <param name="y">Y координата тайла</param>
        /// <param name="date">Дата для фильтрации данных</param>
        /// <returns>PNG изображение тайла</returns>
        /// <response code="200">Тайл успешно отрисован</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet("draw/{zoom}/{x}/{y}/{date}.png")]
        public virtual byte[] DrawLayer(int zoom, int x, int y, DateOnly date)
            => DrawLayerInternal(zoom, x, y, date);
        #endregion

        #region private helpers
        private byte[] DrawLayerInternal(int zoom, int tileX, int tileY, DateOnly? date)
        {
            // 1. Ограничивающий прямоугольник тайла в метрах Web-Mercator
            var envelope = TileToEnvelope(zoom, tileX, tileY);
            var tilePolygon = EnvelopeToPolygon(envelope);

            // 2. Выборка пересекающихся полигонов
            var query = DbContext.Set<TModel>()
                                 .Where(b => b.PoligonData != null &&
                                             b.PoligonData.Intersects(tilePolygon));

            if (date.HasValue)
            {
                var dt = date.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(b => b.PoligonData != null);
            }

            var layers = query.ToList();
            if (layers.Count == 0)
            {
                return EmptyPng();
            }

            // 3. Отрисовка
            using var surface = SKSurface.Create(new SKImageInfo(TileSize, TileSize, SKColorType.Rgba8888, SKAlphaType.Premul));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);

            // вычисляем пиксельный масштаб
            double res = Resolution(zoom);
            double minX = envelope.MinX;
            double maxY = envelope.MaxY;

            foreach (var layer in layers)
            {
                // используем поле PoligonData (Geometry)
                DrawGeometry(canvas, layer.PoligonData, minX, maxY, res);
            }

            using var image = surface.Snapshot();
            using var png = image.Encode(SKEncodedImageFormat.Png, 100);
            return png.ToArray();
        }

        private static Envelope TileToEnvelope(int zoom, int x, int y)
        {
            double res = Resolution(zoom);
            double minX = x * TileSize * res - OriginShift;
            double maxY = OriginShift - y * TileSize * res;
            double maxX = minX + TileSize * res;
            double minY = maxY - TileSize * res;
            return new Envelope(minX, maxX, minY, maxY);
        }

        private static Polygon? EnvelopeToPolygon(Envelope env)
        {
            if (env == null || env.IsNull) return null;

            var coords = new[]
            {
                new Coordinate(env.MinX, env.MinY),
                new Coordinate(env.MaxX, env.MinY),
                new Coordinate(env.MaxX, env.MaxY),
                new Coordinate(env.MinX, env.MaxY),
                new Coordinate(env.MinX, env.MinY)
            };

            var ring = new LinearRing(coords);
            return new Polygon(ring);
        }

        private static double Resolution(int zoom) =>
            2 * Math.PI * EarthRadius / (TileSize * Math.Pow(2, zoom));

        private const double OriginShift = Math.PI * EarthRadius;

        private static void DrawGeometry(SKCanvas canvas, Geometry? geo, double originX, double originY, double res)
        {
            if (geo == null) return;

            using var paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = new SKColor(0, 128, 255, 120),
                IsAntialias = true
            };

            if (geo is Polygon p)
            {
                var exterior = p.ExteriorRing;
                var path = new SKPath();
                for (int i = 0; i < exterior.NumPoints; i++)
                {
                    var pt = exterior.GetPointN(i);
                    float px = (float)((pt.X - originX) / res);
                    float py = (float)((originY - pt.Y) / res); // Y инвертируем
                    if (i == 0) path.MoveTo(px, py);
                    else path.LineTo(px, py);
                }
                path.Close();
                canvas.DrawPath(path, paint);
            }
        }

        private static byte[] EmptyPng()
        {
            using var s = SKSurface.Create(new SKImageInfo(1, 1, SKColorType.Rgba8888, SKAlphaType.Premul));
            s.Canvas.Clear(SKColors.Transparent);
            using var img = s.Snapshot();
            return img.Encode(SKEncodedImageFormat.Png, 100).ToArray();
        }
        #endregion
    }
}
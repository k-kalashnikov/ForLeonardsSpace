using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using OSGeo.OSR;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.IO;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace Masofa.Common.Helper
{
    /// <summary>
    /// Provides helper methods for working with satellite image data and generating various vegetation indices.
    /// </summary>
    /// <remarks>This static class includes methods for locating image data directories, matching granules,
    /// and generating TIFF files for vegetation indices such as ARVI, EVI, GNDVI, NDVI, and others. It also provides
    /// methods for processing raster data and extracting relevant information for specific fields and
    /// seasons.</remarks>
    public static class IndicesHelper
    {
        public const double EPS = 1e-8;
        public static string FindImgDataPath(string extractRoot)
        {
            var granuleDir = Directory.GetDirectories(extractRoot, "GRANULE", SearchOption.AllDirectories).FirstOrDefault();
            if (granuleDir == null)
            {
                return null;
            }

            var l1cDirs = Directory.GetDirectories(granuleDir, "L1C_T*", SearchOption.AllDirectories);
            if (l1cDirs.Length == 0)
            {
                return null;
            }

            var l1cDir = l1cDirs[0];

            var imgDataDir = Path.Combine(l1cDir, "IMG_DATA");
            if (!Directory.Exists(imgDataDir))
            {
                return null;
            }

            return imgDataDir;
        }
        public static List<(string B02Path, string B04Path, string B08Path)> MatchGranulesARVI(List<string> b02Files, List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B02Path, string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB02 = basePart + "_B02.jp2";
                string expectedB08 = basePart + "_B08.jp2";

                string b02Match = b02Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB02);
                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b02Match != null && b08Match != null)
                {
                    granules.Add((b02Match, b04, b08Match));
                }
            }

            return granules;
        }
        public static string CreateColorfulARVITiff(string b02Path, string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_ARVI_vis") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b02Dataset = Gdal.Open(b02Path, Access.GA_ReadOnly); // BLUE
            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b02Path}, {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
            {
                throw new Exception("B02, B04 and B08 must have the same dimensions!");
            }

            var b02Band = b02Dataset.GetRasterBand(1); // BLUE
            var b04Band = b04Dataset.GetRasterBand(1); // RED
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b02Scale, b02Offset;
            int b02HasScale, b02HasOffset;
            b02Band.GetScale(out b02Scale, out b02HasScale);
            b02Band.GetOffset(out b02Offset, out b02HasOffset);

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b02HasScale == 0)
            {
                b02Scale = 0.0001;
            }
            if (b04HasScale == 0 || b04Scale == 0)
            {
                b04Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }


            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b02Buffer.Length; i++)
            {
                b02Buffer[i] = (float)(b02Offset + b02Buffer[i] * b02Scale);
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                // Защита от NaN/Infinity
                if (float.IsNaN(b02Buffer[i]) || float.IsInfinity(b02Buffer[i])) b02Buffer[i] = 0f;
                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var arviBuffer = new float[width * height];
            int noDataCount = 0;

            const float G = 2.5f;
            const float C1 = 6.0f;
            const float C2 = 7.5f;
            const float L = 1.0f;
            const float EPS = 1e-9f;

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                float blue = b02Buffer[i];
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                // Фильтруем нереальные или нулевые значения
                if (blue <= 0 || red <= 0 || nir <= 0)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + (2 * red) + blue;
                if (Math.Abs(denominator) < EPS)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float arvi = (nir - (2 * red) + blue) / denominator;

                // Защита от NaN/Infinity
                if (float.IsNaN(arvi) || float.IsInfinity(arvi))
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                // Опционально: обрезаем по физически возможному диапазону EVI
                // MODIS EVI обычно от -0.2 до 1.0, но в Sentinel-2 может быть немного шире
                if (arvi < -1.0f || arvi > 1.5f)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                arviBuffer[i] = arvi;
            }


            // 📊 Находим реальный min/max среди валидных данных
            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                if (arviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (arviBuffer[i] < actualMin) actualMin = arviBuffer[i];
                    if (arviBuffer[i] > actualMax) actualMax = arviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.2f;
                actualMax = 1.0f;
            }


            var byteBuffer = new byte[width * height];

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                if (arviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → прозрачный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (arviBuffer[i] - actualMin) / (actualMax - actualMin);
                    // Clamp к [0, 1] на случай выбросов
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var arviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            arviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            arviDataset.SetProjection(projection);

            var arviBand = arviDataset.GetRasterBand(1);
            arviBand.SetNoDataValue(255);

            // 🎨 Цветовая палитра для ARVI: от коричневого → жёлтого → светло-зелёного → тёмно-зелёного
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);
            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета (подобраны для высокого контраста)
            var waterLo = (r: (byte)70, g: (byte)85, b: (byte)115); // глубокая вода/тени
            var waterHi = (r: (byte)95, g: (byte)110, b: (byte)140); // мелкая вода/асфальт/низкий ARVI<0
            var soilLo = (r: (byte)150, g: (byte)90, b: (byte)40);  // коричневый (почва)
            var soilHi = (r: (byte)205, g: (byte)140, b: (byte)60);  // охра
            var yellow = (r: (byte)255, g: (byte)230, b: (byte)40);  // яркий жёлтый (стресс)
            var greenLo = (r: (byte)170, g: (byte)235, b: (byte)100); // светло-зелёный
            var greenMd = (r: (byte)70, g: (byte)190, b: (byte)85);  // средний зелёный
            var greenHi = (r: (byte)10, g: (byte)110, b: (byte)35);  // тёмно-зелёный (густая растительность)

            // Пороговые значения ARVI (можешь подправить при желании)
            double t0 = 0.0;  // вода/асфальт ниже 0
            double t1 = 0.2;  // почва/оч. слабая
            double t2 = 0.4;  // стресс → начало вегетации
            double t3 = 0.6;  // нормальная растительность
            double t4 = 0.8;  // хорошая растительность

            // NoData — прозрачный
            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            // 1..254 — валидные значения; 0 мы не используем (индекс всегда >=1 при твоём маппинге)
            for (int idx = 1; idx <= 254; idx++)
            {
                // восстановим приблизительный ARVI из индекса (линейная обратная нормализация)
                double arvi = actualMin + (idx / 254.0) * (actualMax - actualMin);

                ColorEntry ce;

                if (arvi < t0)
                {
                    // < 0.0: вода/асфальт — сине-серые, чтобы резко отличались от почвы
                    double t = Math.Clamp((arvi - (t0 - 0.2)) / (0.2), 0, 1); // плавный переход -0.2..0.0
                    var c = Lerp3(waterLo, waterHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (arvi < t1)
                {
                    // 0.0–0.2: почва — от тёмной к охре (контрастно относительно воды)
                    double t = (arvi - t0) / (t1 - t0);
                    var c = Lerp3(soilLo, soilHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (arvi < t2)
                {
                    // 0.2–0.4: охра → яркий жёлтый (стресс выделяется)
                    double t = (arvi - t1) / (t2 - t1);
                    var c = Lerp3(soilHi, yellow, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (arvi < t3)
                {
                    // 0.4–0.6: жёлтый → светло-зелёный (резкий поворот в «зелень»)
                    double t = (arvi - t2) / (t3 - t2);
                    var c = Lerp3(yellow, greenLo, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (arvi < t4)
                {
                    // 0.6–0.8: светло-зелёный → зелёный
                    double t = (arvi - t3) / (t4 - t3);
                    var c = Lerp3(greenLo, greenMd, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    // ≥ 0.8: зелёный → тёмно-зелёный (густая листовая масса)
                    // небольшой «подзавал» в тёмные тона для визуальной доминанты
                    double t = Math.Clamp((arvi - t4) / 0.3, 0, 1); // до ~1.1
                    var c = Lerp3(greenMd, greenHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            arviBand.SetRasterColorTable(colorTable);
            arviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            arviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static string CreateGrayscaleARVITiff(string b02Path, string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_ARVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b02Dataset = Gdal.Open(b02Path, Access.GA_ReadOnly); // BLUE
            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b02Path}, {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
            {
                throw new Exception("B02, B04 and B08 must have the same dimensions!");
            }

            var b02Band = b02Dataset.GetRasterBand(1);
            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b02Scale, b02Offset;
            int b02HasScale, b02HasOffset;
            b02Band.GetScale(out b02Scale, out b02HasScale);
            b02Band.GetOffset(out b02Offset, out b02HasOffset);

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b02HasScale == 0 || b02Scale == 0) b02Scale = 0.0001;
            if (b04HasScale == 0 || b04Scale == 0) b04Scale = 0.0001;
            if (b08HasScale == 0 || b08Scale == 0) b08Scale = 0.0001;

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b02Buffer.Length; i++)
            {
                b02Buffer[i] = (float)(b02Offset + b02Buffer[i] * b02Scale);
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b02Buffer[i]) || float.IsInfinity(b02Buffer[i])) b02Buffer[i] = 0f;
                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var arviBuffer = new float[width * height];
            int noDataCount = 0;

            const float G = 2.5f;
            const float C1 = 6.0f;
            const float C2 = 7.5f;
            const float L = 1.0f;
            const float EPS = 1e-9f;

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                float blue = b02Buffer[i];
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (blue <= 0 || red <= 0 || nir <= 0)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + C1 * red - C2 * blue + L;
                if (Math.Abs(denominator) < EPS)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float arvi = G * (nir - red) / denominator;

                if (float.IsNaN(arvi) || float.IsInfinity(arvi))
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                if (arvi < -1.0f || arvi > 1.5f)
                {
                    arviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                arviBuffer[i] = arvi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                if (arviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (arviBuffer[i] < actualMin) actualMin = arviBuffer[i];
                    if (arviBuffer[i] > actualMax) actualMax = arviBuffer[i];
                }
            }

            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.2f;
                actualMax = 1.0f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < arviBuffer.Length; i++)
            {
                if (arviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → белый или прозрачный
                }
                else
                {
                    // Вариант A: Чёрное = низкий ARVI, Белое = высокий ARVI (рекомендуется)
                    float normalized = (arviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)scaled;

                    /*
                    // Вариант B: ИНВЕРТИРОВАННЫЙ — Белое = низкий ARVI, Чёрное = высокий ARVI
                    float normalized = (arviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round((1 - normalized) * 254);
                    byteBuffer[i] = (byte)scaled;
                    */
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var arviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            arviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            arviDataset.SetProjection(projection);

            var arviBand = arviDataset.GetRasterBand(1);
            arviBand.SetNoDataValue(255);

            arviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static List<(string B02Path, string B04Path, string B08Path)> MatchGranulesEVI(List<string> b02Files, List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B02Path, string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB02 = basePart + "_B02.jp2";
                string expectedB08 = basePart + "_B08.jp2";

                string b02Match = b02Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB02);
                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b02Match != null && b08Match != null)
                {
                    granules.Add((b02Match, b04, b08Match));
                }
            }

            return granules;
        }
        public static string CreateColorfulEVITiff(string b02Path, string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_EVI_vis3") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b02Dataset = Gdal.Open(b02Path, Access.GA_ReadOnly); // BLUE
            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b02Path}, {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
            {
                throw new Exception("B02, B04 and B08 must have the same dimensions!");
            }

            var b02Band = b02Dataset.GetRasterBand(1); // BLUE
            var b04Band = b04Dataset.GetRasterBand(1); // RED
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b02Scale, b02Offset;
            int b02HasScale, b02HasOffset;
            b02Band.GetScale(out b02Scale, out b02HasScale);
            b02Band.GetOffset(out b02Offset, out b02HasOffset);

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b02HasScale == 0)
            {
                b02Scale = 0.0001;
            }
            if (b04HasScale == 0 || b04Scale == 0)
            {
                b04Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }


            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b02Buffer.Length; i++)
            {
                b02Buffer[i] = (float)(b02Offset + b02Buffer[i] * b02Scale);
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                // Защита от NaN/Infinity
                if (float.IsNaN(b02Buffer[i]) || float.IsInfinity(b02Buffer[i])) b02Buffer[i] = 0f;
                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var eviBuffer = new float[width * height];
            int noDataCount = 0;

            const float G = 2.5f;
            const float C1 = 6.0f;
            const float C2 = 7.5f;
            const float L = 1.0f;
            const float EPS = 1e-9f;

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                float blue = b02Buffer[i];
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                // Фильтруем нереальные или нулевые значения
                if (blue <= 0 || red <= 0 || nir <= 0)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + C1 * red - C2 * blue + L;
                if (Math.Abs(denominator) < EPS)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float evi = G * (nir - red) / denominator;

                // Защита от NaN/Infinity
                if (float.IsNaN(evi) || float.IsInfinity(evi))
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                // Опционально: обрезаем по физически возможному диапазону EVI
                // MODIS EVI обычно от -0.2 до 1.0, но в Sentinel-2 может быть немного шире
                if (evi < -1.0f || evi > 1.5f)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                eviBuffer[i] = evi;
            }


            // 📊 Находим реальный min/max среди валидных данных
            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                if (eviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (eviBuffer[i] < actualMin) actualMin = eviBuffer[i];
                    if (eviBuffer[i] > actualMax) actualMax = eviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.2f;
                actualMax = 1.0f;
            }


            var byteBuffer = new byte[width * height];

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                if (eviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → прозрачный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (eviBuffer[i] - actualMin) / (actualMax - actualMin);
                    // Clamp к [0, 1] на случай выбросов
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var eviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            eviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            eviDataset.SetProjection(projection);

            var eviBand = eviDataset.GetRasterBand(1);
            eviBand.SetNoDataValue(255);

            // 🎨 Цветовая палитра для EVI: от коричневого → жёлтого → светло-зелёного → тёмно-зелёного
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);
            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета (подобраны для высокого контраста)
            var waterLo = (r: (byte)70, g: (byte)85, b: (byte)115); // глубокая вода/тени
            var waterHi = (r: (byte)95, g: (byte)110, b: (byte)140); // мелкая вода/асфальт/низкий EVI<0
            var soilLo = (r: (byte)150, g: (byte)90, b: (byte)40);  // коричневый (почва)
            var soilHi = (r: (byte)205, g: (byte)140, b: (byte)60);  // охра
            var yellow = (r: (byte)255, g: (byte)230, b: (byte)40);  // яркий жёлтый (стресс)
            var greenLo = (r: (byte)170, g: (byte)235, b: (byte)100); // светло-зелёный
            var greenMd = (r: (byte)70, g: (byte)190, b: (byte)85);  // средний зелёный
            var greenHi = (r: (byte)10, g: (byte)110, b: (byte)35);  // тёмно-зелёный (густая растительность)

            // Пороговые значения EVI (можешь подправить при желании)
            double t0 = 0.0;  // вода/асфальт ниже 0
            double t1 = 0.2;  // почва/оч. слабая
            double t2 = 0.4;  // стресс → начало вегетации
            double t3 = 0.6;  // нормальная растительность
            double t4 = 0.8;  // хорошая растительность

            // NoData — прозрачный
            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            // 1..254 — валидные значения; 0 мы не используем (индекс всегда >=1 при твоём маппинге)
            for (int idx = 1; idx <= 254; idx++)
            {
                // восстановим приблизительный EVI из индекса (линейная обратная нормализация)
                double evi = actualMin + (idx / 254.0) * (actualMax - actualMin);

                ColorEntry ce;

                if (evi < t0)
                {
                    // < 0.0: вода/асфальт — сине-серые, чтобы резко отличались от почвы
                    double t = Math.Clamp((evi - (t0 - 0.2)) / (0.2), 0, 1); // плавный переход -0.2..0.0
                    var c = Lerp3(waterLo, waterHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (evi < t1)
                {
                    // 0.0–0.2: почва — от тёмной к охре (контрастно относительно воды)
                    double t = (evi - t0) / (t1 - t0);
                    var c = Lerp3(soilLo, soilHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (evi < t2)
                {
                    // 0.2–0.4: охра → яркий жёлтый (стресс выделяется)
                    double t = (evi - t1) / (t2 - t1);
                    var c = Lerp3(soilHi, yellow, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (evi < t3)
                {
                    // 0.4–0.6: жёлтый → светло-зелёный (резкий поворот в «зелень»)
                    double t = (evi - t2) / (t3 - t2);
                    var c = Lerp3(yellow, greenLo, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (evi < t4)
                {
                    // 0.6–0.8: светло-зелёный → зелёный
                    double t = (evi - t3) / (t4 - t3);
                    var c = Lerp3(greenLo, greenMd, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    // ≥ 0.8: зелёный → тёмно-зелёный (густая листовая масса)
                    // небольшой «подзавал» в тёмные тона для визуальной доминанты
                    double t = Math.Clamp((evi - t4) / 0.3, 0, 1); // до ~1.1
                    var c = Lerp3(greenMd, greenHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            eviBand.SetRasterColorTable(colorTable);
            eviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            eviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static string CreateGrayscaleEVITiff(string b02Path, string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_EVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b02Dataset = Gdal.Open(b02Path, Access.GA_ReadOnly); // BLUE
            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b02Path}, {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
            {
                throw new Exception("B02, B04 and B08 must have the same dimensions!");
            }

            var b02Band = b02Dataset.GetRasterBand(1);
            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b02Scale, b02Offset;
            int b02HasScale, b02HasOffset;
            b02Band.GetScale(out b02Scale, out b02HasScale);
            b02Band.GetOffset(out b02Offset, out b02HasOffset);

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b02HasScale == 0 || b02Scale == 0) b02Scale = 0.0001;
            if (b04HasScale == 0 || b04Scale == 0) b04Scale = 0.0001;
            if (b08HasScale == 0 || b08Scale == 0) b08Scale = 0.0001;

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b02Buffer.Length; i++)
            {
                b02Buffer[i] = (float)(b02Offset + b02Buffer[i] * b02Scale);
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b02Buffer[i]) || float.IsInfinity(b02Buffer[i])) b02Buffer[i] = 0f;
                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var eviBuffer = new float[width * height];
            int noDataCount = 0;

            const float G = 2.5f;
            const float C1 = 6.0f;
            const float C2 = 7.5f;
            const float L = 1.0f;
            const float EPS = 1e-9f;

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                float blue = b02Buffer[i];
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (blue <= 0 || red <= 0 || nir <= 0)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + C1 * red - C2 * blue + L;
                if (Math.Abs(denominator) < EPS)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float evi = G * (nir - red) / denominator;

                if (float.IsNaN(evi) || float.IsInfinity(evi))
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                if (evi < -1.0f || evi > 1.5f)
                {
                    eviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                eviBuffer[i] = evi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                if (eviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (eviBuffer[i] < actualMin) actualMin = eviBuffer[i];
                    if (eviBuffer[i] > actualMax) actualMax = eviBuffer[i];
                }
            }

            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.2f;
                actualMax = 1.0f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < eviBuffer.Length; i++)
            {
                if (eviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → белый или прозрачный
                }
                else
                {
                    // Вариант A: Чёрное = низкий EVI, Белое = высокий EVI (рекомендуется)
                    float normalized = (eviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)scaled;

                    /*
                    // Вариант B: ИНВЕРТИРОВАННЫЙ — Белое = низкий EVI, Чёрное = высокий EVI
                    float normalized = (eviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round((1 - normalized) * 254);
                    byteBuffer[i] = (byte)scaled;
                    */
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var eviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            eviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            eviDataset.SetProjection(projection);

            var eviBand = eviDataset.GetRasterBand(1);
            eviBand.SetNoDataValue(255);

            eviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static List<(string B03Path, string B08Path)> MatchGranulesGNDVI(List<string> b03Files, List<string> b08Files)
        {
            var granules = new List<(string B03Path, string B08Path)>();

            foreach (var b03 in b03Files)
            {
                string fileName = Path.GetFileName(b03);
                if (!fileName.EndsWith("_B03.jp2")) continue;

                string basePart = fileName.Replace("_B03.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b03, b08Match));
                }
            }

            return granules;
        }
        public static string CreateColorfulGNDVITiff(string b03Path, string b08Path)
        {
            var datetime = DateTime.Now;
            string outputFileName = Path.GetFileNameWithoutExtension(b03Path).Replace("_B03", "_GNDVI_vis1") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b03Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b08Path}");
            }

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
            {
                throw new Exception("B03 and B08 must have the same dimensions!");
            }

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            // Получаем масштаб и смещение для корректного преобразования DN → реальные значения
            double b03Scale, b03Offset;
            int b03HasScale, b03HasOffset;
            b03Band.GetScale(out b03Scale, out b03HasScale);
            b03Band.GetOffset(out b03Offset, out b03HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b03HasScale == 0 || b03Scale == 0)
            {
                b03Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b03Buffer.Length; i++)
            {
                b03Buffer[i] = (float)(b03Offset + b03Buffer[i] * b03Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b03Buffer[i]) || float.IsInfinity(b03Buffer[i])) b03Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var gndviBuffer = new float[width * height];
            int noDataCount = 0;

            const float EPS = 1e-9f;

            for (int i = 0; i < gndviBuffer.Length; i++)
            {
                float green = b03Buffer[i];
                float nir = b08Buffer[i];

                if (green <= 0 || nir <= 0)
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + green;
                if (Math.Abs(denominator) < EPS)
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float gndvi = (nir - green) / denominator;

                if (float.IsNaN(gndvi) || float.IsInfinity(gndvi))
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                // Обрезаем значения по диапазону [-0.1, 1.0]
                gndvi = Math.Max(-0.1f, Math.Min(1.0f, gndvi));
                gndviBuffer[i] = gndvi;
            }


            // Создаём байтовый буфер для индексов палитры (0–254)
            var byteBuffer = new byte[width * height];

            // Фиксированные пороги и цвета
            var thresholds = new[] { -0.1f, 0.0f, 0.25f, 0.5f, 0.75f, 1.0f };
            var colors = new[]
            {
                new ColorEntry { c1 = 255, c2 = 255, c3 = 255, c4 = 255 }, // #FFFFFF — Open land
                new ColorEntry { c1 = 152, c2 = 251, c3 = 152, c4 = 255 }, // #98FB98 — Very bad
                new ColorEntry { c1 = 59, c2 = 179, c3 = 113, c4 = 255 },  // #3CB371 — Stress
                new ColorEntry { c1 = 46, c2 = 139, c3 = 87, c4 = 255 },   // #2E8B57 — Good
                new ColorEntry { c1 = 0, c2 = 100, c3 = 0, c4 = 255 }      // #006400 — Very good
            };

            // Каждое значение GNDVI привязываем к цвету по порогам
            for (int i = 0; i < gndviBuffer.Length; i++)
            {
                if (gndviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData
                }
                else
                {
                    // Найдём диапазон
                    int idx = 0;
                    for (int j = 0; j < thresholds.Length - 1; j++)
                    {
                        if (gndviBuffer[i] >= thresholds[j] && gndviBuffer[i] < thresholds[j + 1])
                        {
                            idx = j;
                            break;
                        }
                    }

                    // Устанавливаем индекс палитры (0–4)
                    byteBuffer[i] = (byte)idx;
                }
            }

            // Создание TIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var gndviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);
            gndviDataset.SetGeoTransform(geoTransform);

            string projection = b03Dataset.GetProjection();
            gndviDataset.SetProjection(projection);

            var gndviBand = gndviDataset.GetRasterBand(1);
            gndviBand.SetNoDataValue(255);

            // Устанавливаем цветовую таблицу
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Добавляем цвета для каждого уровня
            for (int i = 0; i < colors.Length; i++)
            {
                colorTable.SetColorEntry(i, colors[i]);
            }

            // NoData — чёрный (прозрачный)
            var noDataEntry = new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 };
            colorTable.SetColorEntry(255, noDataEntry);

            gndviBand.SetRasterColorTable(colorTable);
            gndviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            gndviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static string CreateGrayscaleGNDVITiff(string b03Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b03Path).Replace("_B03", "_GNDVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b03Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b08Path}");
            }

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
            {
                throw new Exception("B03 and B08 must have the same dimensions!");
            }

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            // Получаем масштаб и смещение
            double b03Scale, b03Offset;
            int b03HasScale, b03HasOffset;
            b03Band.GetScale(out b03Scale, out b03HasScale);
            b03Band.GetOffset(out b03Offset, out b03HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b03HasScale == 0 || b03Scale == 0)
            {
                b03Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b03Buffer.Length; i++)
            {
                b03Buffer[i] = (float)(b03Offset + b03Buffer[i] * b03Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b03Buffer[i]) || float.IsInfinity(b03Buffer[i])) b03Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var gndviBuffer = new float[width * height];
            int noDataCount = 0;

            const float EPS = 1e-9f;

            for (int i = 0; i < gndviBuffer.Length; i++)
            {
                float green = b03Buffer[i];
                float nir = b08Buffer[i];

                if (green <= 0 || nir <= 0)
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + green;
                if (Math.Abs(denominator) < EPS)
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float gndvi = (nir - green) / denominator;

                if (float.IsNaN(gndvi) || float.IsInfinity(gndvi))
                {
                    gndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                // Обрезаем по диапазону [-0.1, 1.0]
                gndvi = Math.Max(-0.1f, Math.Min(1.0f, gndvi));
                gndviBuffer[i] = gndvi;
            }

            // Нормализация в [0, 255] для 8-битного серого
            var byteBuffer = new byte[width * height];
            float minVal = -0.1f;
            float maxVal = 1.0f;

            for (int i = 0; i < gndviBuffer.Length; i++)
            {
                if (gndviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 0; // NoData = 0 (чёрный)
                }
                else
                {
                    float normalized = (gndviBuffer[i] - minVal) / (maxVal - minVal);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    byteBuffer[i] = (byte)Math.Round(normalized * 255);
                }
            }

            // Создание TIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var gndviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);
            gndviDataset.SetGeoTransform(geoTransform);

            string projection = b03Dataset.GetProjection();
            gndviDataset.SetProjection(projection);

            var gndviBand = gndviDataset.GetRasterBand(1);
            gndviBand.SetNoDataValue(0); // 0 = NoData

            // Убираем палитру — это просто серый канал
            gndviBand.SetRasterColorInterpretation(ColorInterp.GCI_GrayIndex);

            gndviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static List<(string B03Path, string B11Path)> MatchGranulesMNDWI(List<string> B03Files, List<string> B11Files)
        {
            var granules = new List<(string B03Path, string B11Path)>();

            foreach (var B03 in B03Files)
            {
                string fileName = Path.GetFileName(B03);
                if (!fileName.EndsWith("_B03.jp2")) continue;

                string basePart = fileName.Replace("_B03.jp2", "");
                string expectedB11 = basePart + "_B11.jp2";

                string B11Match = B11Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB11);

                if (B11Match != null)
                {
                    granules.Add((B03, B11Match));
                }
            }

            return granules;
        }
        public static string CreateColorfulMNDWITiff(string b03Path, string b11Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b03Path).Replace("_B03", "_MNDWI_vis2") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b03Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(b11Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b11Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b11Path}");
            }

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b11Band = b11Dataset.GetRasterBand(1);

            // Буферы для double
            var greenDouble = new double[width * height];
            var swirDouble = new double[width * height];

            // Читаем B03 как double
            b03Band.ReadRaster(0, 0, width, height, greenDouble, width, height, 0, 0);

            // Ресемплируем B11 до размеров B03
            var extraArgs = new RasterIOExtraArg();
            extraArgs.nVersion = 1;
            extraArgs.eResampleAlg = (RIOResampleAlg)ResampleAlg.GRA_Bilinear;

            b11Band.ReadRaster(
                0, 0,
                b11Dataset.RasterXSize, b11Dataset.RasterYSize,
                swirDouble,
                width, height,
                0, 0,
                extraArgs);

            // Расчёт MNDWI
            var mndwiBuffer = new double[width * height];
            int noDataCount = 0;

            for (int i = 0; i < greenDouble.Length; i++)
            {
                double green = greenDouble[i];
                double swir = swirDouble[i];

                // Фильтр NoData
                if (green <= 0 || swir <= 0)
                {
                    mndwiBuffer[i] = -9999.0;
                    noDataCount++;
                    continue;
                }

                double denominator = green + swir + EPS;
                if (Math.Abs(denominator) < 1e-9)
                {
                    mndwiBuffer[i] = -9999.0;
                    noDataCount++;
                    continue;
                }

                double mndwi = (green - swir) / denominator;
                mndwiBuffer[i] = mndwi;
            }


            // Находим реальный диапазон значений
            double actualMin = double.MaxValue;
            double actualMax = double.MinValue;

            for (int i = 0; i < mndwiBuffer.Length; i++)
            {
                if (mndwiBuffer[i] > -1000)
                {
                    if (mndwiBuffer[i] < actualMin) actualMin = mndwiBuffer[i];
                    if (mndwiBuffer[i] > actualMax) actualMax = mndwiBuffer[i];
                }
            }

            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001;
            }


            // Нормализация к [0, 254]
            var byteBuffer = new byte[width * height];

            for (int i = 0; i < mndwiBuffer.Length; i++)
            {
                if (mndwiBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData
                }
                else
                {
                    double normalized = (mndwiBuffer[i] - actualMin) / (actualMax - actualMin);
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            // Создание GeoTIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var mndwiDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);
            mndwiDataset.SetGeoTransform(geoTransform);

            string projection = b03Dataset.GetProjection();
            mndwiDataset.SetProjection(projection);

            var mndwiBand = mndwiDataset.GetRasterBand(1);
            mndwiBand.SetNoDataValue(255);

            // 🎨 Цветовая палитра
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            for (int i = 0; i < 255; i++)
            {
                byte r, g, b;

                if (i < 85)
                {
                    // Коричневый → оранжевый
                    r = (byte)(139 + i * 1.4);
                    g = (byte)(69 + i * 0.8);
                    b = (byte)(19 + i * 0.3);
                }
                else if (i < 170)
                {
                    // Оранжевый → белый
                    r = (byte)(255 - (i - 85) * 1.5);
                    g = (byte)(137 + (i - 85) * 1.3);
                    b = (byte)(44 + (i - 85) * 2.4);
                }
                else
                {
                    // Белый → синий (вода)
                    r = (byte)(255 - (i - 170) * 1.5);
                    g = (byte)(255 - (i - 170) * 1.5);
                    b = 255;
                }
                //if (i < 85)
                //{
                //    // Коричневый → оранжевый
                //    r = (byte)(139 + i * 1.4);
                //    g = (byte)(69 + i * 0.8);
                //    b = (byte)(19 + i * 0.3);
                //}
                //else if (i < 170)
                //{
                //    // Оранжевый → белый
                //    r = (byte)(255 - (i - 85) * 1.5);
                //    g = (byte)(137 + (i - 85) * 1.3);
                //    b = (byte)(44 + (i - 85) * 2.4);
                //}
                //else
                //{
                //    // Белый → ярко-синий (вода)
                //    r = 0;
                //    g = (byte)(255 - (i - 170) * 2); // 255 → 0
                //    b = 255;
                //}

                var colorEntry = new ColorEntry();
                colorEntry.c1 = r;
                colorEntry.c2 = g;
                colorEntry.c3 = b;
                colorEntry.c4 = 255;
                colorTable.SetColorEntry(i, colorEntry);
            }

            var noDataEntry = new ColorEntry();
            noDataEntry.c1 = 0;
            noDataEntry.c2 = 0;
            noDataEntry.c3 = 0;
            noDataEntry.c4 = 0;
            colorTable.SetColorEntry(255, noDataEntry);

            mndwiBand.SetRasterColorTable(colorTable);
            mndwiBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            mndwiBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static string CreateGrayscaleMNDWITiff(string b03Path, string b11Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b03Path).Replace("_B03", "_MNDWI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b03Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(b11Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b11Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b11Path}");
            }

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b11Band = b11Dataset.GetRasterBand(1);

            // Буферы для double
            var greenDouble = new double[width * height];
            var swirDouble = new double[width * height];

            // Читаем B03 как double
            b03Band.ReadRaster(0, 0, width, height, greenDouble, width, height, 0, 0);

            // Ресемплируем B11 до размеров B03
            var extraArgs = new RasterIOExtraArg();
            extraArgs.nVersion = 1;
            extraArgs.eResampleAlg = (RIOResampleAlg)ResampleAlg.GRA_Bilinear;

            b11Band.ReadRaster(
                0, 0,
                b11Dataset.RasterXSize, b11Dataset.RasterYSize,
                swirDouble,
                width, height,
                0, 0,
                extraArgs);

            // Расчёт MNDWI
            var mndwiBuffer = new double[width * height];
            int noDataCount = 0;

            for (int i = 0; i < greenDouble.Length; i++)
            {
                double green = greenDouble[i];
                double swir = swirDouble[i];

                // Фильтр NoData
                if (green <= 0 || swir <= 0)
                {
                    mndwiBuffer[i] = -9999.0;
                    noDataCount++;
                    continue;
                }

                double denominator = green + swir + EPS;
                if (Math.Abs(denominator) < 1e-9)
                {
                    mndwiBuffer[i] = -9999.0;
                    noDataCount++;
                    continue;
                }

                double mndwi = (green - swir) / denominator;
                mndwiBuffer[i] = mndwi;
            }

            // Находим реальный диапазон значений
            double actualMin = double.MaxValue;
            double actualMax = double.MinValue;

            for (int i = 0; i < mndwiBuffer.Length; i++)
            {
                if (mndwiBuffer[i] > -1000)
                {
                    if (mndwiBuffer[i] < actualMin) actualMin = mndwiBuffer[i];
                    if (mndwiBuffer[i] > actualMax) actualMax = mndwiBuffer[i];
                }
            }

            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001;
            }

            // Нормализация к [0, 254] — градации серого
            var byteBuffer = new byte[width * height];

            for (int i = 0; i < mndwiBuffer.Length; i++)
            {
                if (mndwiBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData
                }
                else
                {
                    double normalized = (mndwiBuffer[i] - actualMin) / (actualMax - actualMin);
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            // Создание GeoTIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var mndwiDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);
            mndwiDataset.SetGeoTransform(geoTransform);

            string projection = b03Dataset.GetProjection();
            mndwiDataset.SetProjection(projection);

            var mndwiBand = mndwiDataset.GetRasterBand(1);
            mndwiBand.SetNoDataValue(255);

            mndwiBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static List<(string B08Path, string B11Path)> MatchGranulesNDMI(List<string> b08Files, List<string> b11Files)
        {
            var granules = new List<(string B08Path, string B11Path)>();

            foreach (var b08 in b08Files)
            {
                string fileName = Path.GetFileName(b08);
                if (!fileName.EndsWith("_B08.jp2")) continue;

                string basePart = fileName.Replace("_B08.jp2", "");
                string expectedB11 = basePart + "_B11.jp2";

                string b11Match = b11Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB11);

                if (b11Match != null)
                {
                    granules.Add((b08, b11Match));
                }
            }

            return granules;
        }
        public static string CreateColorfulNDMITiff(string b08Path, string b11Path)
        {
            var datetime = DateTime.Now;
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_NDMI_vis7") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(b11Path, Access.GA_ReadOnly);

            if (b08Dataset == null || b11Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b08Path}, {b11Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            var b08Band = b08Dataset.GetRasterBand(1);
            var b11Band = b11Dataset.GetRasterBand(1);

            var nirDouble = new double[width * height];
            var swirDouble = new double[width * height];

            b08Band.ReadRaster(0, 0, width, height, nirDouble, width, height, 0, 0);

            var extraArgs = new RasterIOExtraArg();
            extraArgs.nVersion = 1;
            extraArgs.eResampleAlg = (RIOResampleAlg)ResampleAlg.GRA_Bilinear;

            b11Band.ReadRaster(
                0, 0,
                b11Dataset.RasterXSize, b11Dataset.RasterYSize,
                swirDouble,
                width, height,
                0, 0,
                extraArgs);

            var ndmiBuffer = new float[width * height];
            int noDataCount = 0;

            for (int i = 0; i < nirDouble.Length; i++)
            {
                double nir = nirDouble[i];
                double swir = swirDouble[i];

                if (nir <= 0 || swir <= 0)
                {
                    ndmiBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                double denominator = nir + swir;
                if (Math.Abs(denominator) < 1e-9)
                {
                    ndmiBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float ndmi = (float)((nir - swir) / denominator);
                ndmiBuffer[i] = ndmi;
            }

            // Найти действительные мин/макс
            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;

            for (int i = 0; i < ndmiBuffer.Length; i++)
            {
                if (ndmiBuffer[i] > -1000)
                {
                    if (ndmiBuffer[i] < actualMin) actualMin = ndmiBuffer[i];
                    if (ndmiBuffer[i] > actualMax) actualMax = ndmiBuffer[i];
                }
            }

            // Если все значения одинаковые — добавляем небольшой разброс
            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001f;
            }

            // Преобразуем в byte [0..254] (NoData = 255)
            var byteBuffer = new byte[width * height];

            for (int i = 0; i < ndmiBuffer.Length; i++)
            {
                if (ndmiBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → чёрный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (ndmiBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized)); // Clamp
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            // Создаем выходной файл
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var ndmiDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            ndmiDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            ndmiDataset.SetProjection(projection);

            var ndmiBand = ndmiDataset.GetRasterBand(1);
            ndmiBand.SetNoDataValue(255);

            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы для интерполяции
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);

            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета (по HEX)
            var lightBlue = (r: (byte)173, g: (byte)216, b: (byte)230);  // #ADD8E6
            var skyBlue = (r: (byte)0, g: (byte)187, b: (byte)255);     // #00BFFF
            var blue = (r: (byte)0, g: (byte)0, b: (byte)255);          // #0000FF
            var darkBlue = (r: (byte)0, g: (byte)0, b: (byte)139);      // #00008B

            // Диапазоны NDMI
            double minNDMI = -0.1;
            double maxNDMI = 1.0;

            // Задаем пороги
            double t0 = 0.0;   // (-0.1 → 0)
            double t1 = 0.3;   // (0 → 0.3)
            double t2 = 0.7;   // (0.3 → 0.7)
            double t3 = 1.0;   // (0.7 → 1)

            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            for (int idx = 0; idx <= 254; idx++)
            {
                double normalizedNDMI = minNDMI + (idx / 254.0) * (maxNDMI - minNDMI);
                ColorEntry ce;

                if (normalizedNDMI < t0)
                {
                    // Интерполируем от #ADD8E6 к #00BFFF
                    double t = (normalizedNDMI - minNDMI) / (t0 - minNDMI);
                    var c = Lerp3(lightBlue, skyBlue, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (normalizedNDMI < t1)
                {
                    // #00BFFF → #0000FF
                    double t = (normalizedNDMI - t0) / (t1 - t0);
                    var c = Lerp3(skyBlue, blue, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (normalizedNDMI < t2)
                {
                    // #0000FF → #00008B
                    double t = (normalizedNDMI - t1) / (t2 - t1);
                    var c = Lerp3(blue, darkBlue, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    // #00008B → #00008B (константа)
                    ce = new ColorEntry { c1 = darkBlue.r, c2 = darkBlue.g, c3 = darkBlue.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            ndmiBand.SetRasterColorTable(colorTable);
            ndmiBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            ndmiBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static string CreateGrayscaleNDMITiff(string b08Path, string b11Path)
        {
            var datetime = DateTime.Now;
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_NDMI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(b11Path, Access.GA_ReadOnly);

            if (b08Dataset == null || b11Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b08Path}, {b11Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            var b08Band = b08Dataset.GetRasterBand(1);
            var b11Band = b11Dataset.GetRasterBand(1);

            var nirDouble = new double[width * height];
            var swirDouble = new double[width * height];

            b08Band.ReadRaster(0, 0, width, height, nirDouble, width, height, 0, 0);

            var extraArgs = new RasterIOExtraArg();
            extraArgs.nVersion = 1;
            extraArgs.eResampleAlg = (RIOResampleAlg)ResampleAlg.GRA_Bilinear;

            b11Band.ReadRaster(
                0, 0,
                b11Dataset.RasterXSize, b11Dataset.RasterYSize,
                swirDouble,
                width, height,
                0, 0,
                extraArgs);

            var ndmiBuffer = new float[width * height];
            int noDataCount = 0;

            for (int i = 0; i < nirDouble.Length; i++)
            {
                double nir = nirDouble[i];
                double swir = swirDouble[i];

                if (nir <= 0 || swir <= 0)
                {
                    ndmiBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                double denominator = nir + swir;
                if (Math.Abs(denominator) < 1e-9)
                {
                    ndmiBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float ndmi = (float)((nir - swir) / denominator);
                ndmiBuffer[i] = ndmi;
            }

            // Найти реальные min/max
            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;

            for (int i = 0; i < ndmiBuffer.Length; i++)
            {
                if (ndmiBuffer[i] > -1000)
                {
                    if (ndmiBuffer[i] < actualMin) actualMin = ndmiBuffer[i];
                    if (ndmiBuffer[i] > actualMax) actualMax = ndmiBuffer[i];
                }
            }

            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001f;
            }

            // Преобразуем в byte [0..255], NoData → 0
            var byteBuffer = new byte[width * height];

            for (int i = 0; i < ndmiBuffer.Length; i++)
            {
                if (ndmiBuffer[i] < -1000)
                {
                    byteBuffer[i] = 0; // NoData → черный (0)
                }
                else
                {
                    // Нормализация: [actualMin, actualMax] → [0, 255]
                    float normalized = (ndmiBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized)); // Clamp
                    int scaled = (int)Math.Round(normalized * 255);
                    byteBuffer[i] = (byte)scaled;
                }
            }

            // Создаём GeoTIFF
            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var ndmiDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            ndmiDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            ndmiDataset.SetProjection(projection);

            var ndmiBand = ndmiDataset.GetRasterBand(1);
            ndmiBand.SetNoDataValue(0); // NoData = 0 (черный)

            // Устанавливаем интерпретацию как "gray"
            ndmiBand.SetRasterColorInterpretation(ColorInterp.GCI_GrayIndex);

            ndmiBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static List<(string B04Path, string B08Path)> MatchGranulesNDVI(List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b04, b08Match));
                }
            }

            return granules;
        }
        public static string CreateColorfulNDVITiff(string b04Path, string b08Path)
        {
            var datetime = DateTime.Now;
            string outputFileName = Path.GetFileNameWithoutExtension(b04Path).Replace("_B04", "_NDVI_vis") + 8 + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b04Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);

            if (b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b04Path}, {b08Path}");
            }

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
            {
                throw new Exception("B04 and B08 must have the same dimensions!");
            }

            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            var ndviBuffer = new float[width * height];
            int noDataCount = 0;

            for (int i = 0; i < b04Buffer.Length; i++)
            {
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (red <= 0 || nir <= 0)
                {
                    ndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float denominator = nir + red;
                if (Math.Abs(denominator) < 1e-9f)
                {
                    ndviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float ndvi = (nir - red) / denominator;
                ndviBuffer[i] = ndvi;
            }


            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;

            for (int i = 0; i < ndviBuffer.Length; i++)
            {
                if (ndviBuffer[i] > -1000)
                {
                    if (ndviBuffer[i] < actualMin) actualMin = ndviBuffer[i];
                    if (ndviBuffer[i] > actualMax) actualMax = ndviBuffer[i];
                }
            }

            // Защита от деления на ноль
            if (actualMin == actualMax)
            {
                actualMax = actualMin + 0.001f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < ndviBuffer.Length; i++)
            {
                if (ndviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData
                }
                else
                {
                    float normalized = (ndviBuffer[i] - actualMin) / (actualMax - actualMin);
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var ndviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);
            ndviDataset.SetGeoTransform(geoTransform);

            string projection = b04Dataset.GetProjection();
            ndviDataset.SetProjection(projection);

            var ndviBand = ndviDataset.GetRasterBand(1);
            ndviBand.SetNoDataValue(255);

            // Создаём цветовую таблицу: красный → жёлтый → зелёный
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Палитра: красный → жёлтый → зелёный
            for (int i = 0; i < 255; i++)
            {
                byte r, g, b;

                if (i < 60)
                {
                    // Красный → оранжевый
                    r = 255;
                    g = (byte)(i * 4); // 0 → 240
                    b = 0;
                }
                else if (i < 120)
                {
                    // Оранжевый → жёлтый
                    r = (byte)(255 - (i - 60) * 2); // 255 → 195
                    g = 255;
                    b = 0;
                }
                else if (i < 180)
                {
                    // Жёлтый → светло-зелёный
                    r = 0;
                    g = 255;
                    b = (byte)((i - 120) * 2); // 0 → 120
                }
                else
                {
                    // Светло-зелёный → тёмно-зелёный
                    r = 0;
                    g = (byte)(255 - (i - 180) * 2); // 255 → 90
                    b = 255;
                }

                var colorEntry = new ColorEntry();
                colorEntry.c1 = r;
                colorEntry.c2 = g;
                colorEntry.c3 = b;
                colorEntry.c4 = 255;

                colorTable.SetColorEntry(i, colorEntry);
            }

            // NoData — чёрный
            var noDataEntry = new ColorEntry();
            noDataEntry.c1 = 0;
            noDataEntry.c2 = 0;
            noDataEntry.c3 = 0;
            noDataEntry.c4 = 0;

            colorTable.SetColorEntry(255, noDataEntry);

            ndviBand.SetRasterColorTable(colorTable);
            ndviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            ndviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static string CreateGrayscaleNDVITiff(string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b04Path).Replace("_B04", "_NDVI") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b04Path), outputFileName);

            if (File.Exists(outputPath))
            {
                //logger.LogInformation("skip");
                return outputPath; // или логируй, что пропускаем
            }

            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly);

            if (b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b04Path} or {b08Path}");
            }

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
            {
                throw new Exception("B04 and B08 must have the same dimensions!");
            }

            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b04Buffer = new double[width * height];
            var b08Buffer = new double[width * height];

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            var ndviBuffer = new double[width * height];
            int noDataCount = 0;

            for (int i = 0; i < b04Buffer.Length; i++)
            {
                double b04 = b04Buffer[i];
                double b08 = b08Buffer[i];

                if (b04 <= 0 || b08 <= 0 || b04 + b08 == 0)
                {
                    ndviBuffer[i] = -9999f;
                    noDataCount++;
                }
                else
                {
                    ndviBuffer[i] = (b08 - b04) / (b08 + b04 == 0 ? b08 + b04 + EPS : b08 + b04);
                }
            }

            string[] creationOptions =
            {
                "COMPRESS=LZW",
                "TILED=YES",
                "BIGTIFF=IF_SAFER",
                "PREDICTOR=2"
            };

            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var ndviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Float32, creationOptions);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);
            ndviDataset.SetGeoTransform(geoTransform);

            string projection = b04Dataset.GetProjection();
            ndviDataset.SetProjection(projection);

            var ndviBand = ndviDataset.GetRasterBand(1);
            ndviBand.SetNoDataValue(-9999.0);
            ndviBand.WriteRaster(0, 0, width, height, ndviBuffer, width, height, 0, 0);

            //ndviDataset.SetMetadataItem("CREATED_BY", "Masofa NDVI Processor");
            //ndviDataset.SetMetadataItem("SOURCE_B04", Path.GetFileName(b04Path));
            //ndviDataset.SetMetadataItem("SOURCE_B08", Path.GetFileName(b08Path));

            return outputPath;
        }
        public static List<(string B03Path, string B08Path)> MatchGranulesORVI(List<string> b03Files, List<string> b08Files)
        {
            var granules = new List<(string B03Path, string B08Path)>();

            foreach (var b03 in b03Files)
            {
                string fileName = Path.GetFileName(b03);
                if (!fileName.EndsWith("_B03.jp2")) continue;

                string basePart = fileName.Replace("_B03.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b03, b08Match));
                }
            }

            return granules;
        }
        public static string CreateColorfulORVITiff(string b03Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_ORVI_vis3") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly); // GREEN
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b03Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b03Dataset.RasterXSize || height != b03Dataset.RasterYSize)
            {
                throw new Exception("B03 and B08 must have the same dimensions!");
            }

            var b03Band = b03Dataset.GetRasterBand(1); // GREEN
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b03Scale, b03Offset;
            int b03HasScale, b03HasOffset;
            b03Band.GetScale(out b03Scale, out b03HasScale);
            b03Band.GetOffset(out b03Offset, out b03HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b03HasScale == 0 || b03Scale == 0)
            {
                b03Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b03Buffer.Length; i++)
            {
                b03Buffer[i] = (float)(b03Offset + b03Buffer[i] * b03Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b03Buffer[i]) || float.IsInfinity(b03Buffer[i])) b03Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var orviBuffer = new float[width * height];
            int noDataCount = 0;

            const float EPS = 1e-9f;

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                float green = b03Buffer[i];
                float nir = b08Buffer[i];

                if (green <= 0 || nir <= 0)
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float orvi = nir / (green + EPS);

                if (float.IsNaN(orvi) || float.IsInfinity(orvi))
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                if (orvi < 0.0f || orvi > 10.0f)
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                orviBuffer[i] = orvi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                if (orviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (orviBuffer[i] < actualMin) actualMin = orviBuffer[i];
                    if (orviBuffer[i] > actualMax) actualMax = orviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = 0.5f;
                actualMax = 3.0f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                if (orviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → прозрачный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (orviBuffer[i] - actualMin) / (actualMax - actualMin);
                    // Clamp к [0, 1] на случай выбросов
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var orviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            orviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            orviDataset.SetProjection(projection);

            var orviBand = orviDataset.GetRasterBand(1);
            orviBand.SetNoDataValue(255);

            // Цветовая палитра для ORVI (NIR/Green): от тёмно-коричневого → жёлтого → зелёного → тёмно-зелёного
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);
            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета
            var water = (r: (byte)80, g: (byte)100, b: (byte)130);   // вода/тени
            var soilLo = (r: (byte)140, g: (byte)80, b: (byte)40);   // сухая почва
            var soilHi = (r: (byte)200, g: (byte)130, b: (byte)50);  // влажная почва/начало роста
            var yellow = (r: (byte)255, g: (byte)220, b: (byte)40);  // стресс/начало вегетации
            var greenLo = (r: (byte)160, g: (byte)220, b: (byte)100); // слабая растительность
            var greenMd = (r: (byte)80, g: (byte)180, b: (byte)70);  // хорошая растительность
            var greenHi = (r: (byte)20, g: (byte)100, b: (byte)30);  // густая растительность

            // Пороговые значения ORVI (NIR/Green)
            double t0 = 0.8;  // вода/почва
            double t1 = 1.2;  // сухая → влажная почва
            double t2 = 1.6;  // начало вегетации
            double t3 = 2.0;  // нормальная растительность
            double t4 = 2.5;  // густая растительность

            // NoData — прозрачный
            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            // Заполняем палитру
            for (int idx = 1; idx <= 254; idx++)
            {
                double orvi = actualMin + (idx / 254.0) * (actualMax - actualMin);
                ColorEntry ce;

                if (orvi < t0)
                {
                    double t = Math.Clamp((orvi - (t0 - 0.5)) / 0.5, 0, 1);
                    var c = Lerp3(water, soilLo, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (orvi < t1)
                {
                    double t = (orvi - t0) / (t1 - t0);
                    var c = Lerp3(soilLo, soilHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (orvi < t2)
                {
                    double t = (orvi - t1) / (t2 - t1);
                    var c = Lerp3(soilHi, yellow, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (orvi < t3)
                {
                    double t = (orvi - t2) / (t3 - t2);
                    var c = Lerp3(yellow, greenLo, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (orvi < t4)
                {
                    double t = (orvi - t3) / (t4 - t3);
                    var c = Lerp3(greenLo, greenMd, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    double t = Math.Clamp((orvi - t4) / 1.0, 0, 1);
                    var c = Lerp3(greenMd, greenHi, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            orviBand.SetRasterColorTable(colorTable);
            orviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            orviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static string CreateGrayscaleORVITiff(string b03Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_ORVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b03Dataset = Gdal.Open(b03Path, Access.GA_ReadOnly); // GREEN
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b03Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b03Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b03Dataset.RasterXSize || height != b03Dataset.RasterYSize)
            {
                throw new Exception("B03 and B08 must have the same dimensions!");
            }

            var b03Band = b03Dataset.GetRasterBand(1); // GREEN
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b03Scale, b03Offset;
            int b03HasScale, b03HasOffset;
            b03Band.GetScale(out b03Scale, out b03HasScale);
            b03Band.GetOffset(out b03Offset, out b03HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b03HasScale == 0 || b03Scale == 0)
            {
                b03Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b03Buffer.Length; i++)
            {
                b03Buffer[i] = (float)(b03Offset + b03Buffer[i] * b03Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b03Buffer[i]) || float.IsInfinity(b03Buffer[i])) b03Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var orviBuffer = new float[width * height];
            int noDataCount = 0;

            const float EPS = 1e-9f;

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                float green = b03Buffer[i];
                float nir = b08Buffer[i];

                if (green <= 0 || nir <= 0)
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float orvi = nir / (green + EPS);

                if (float.IsNaN(orvi) || float.IsInfinity(orvi))
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                if (orvi < 0.0f || orvi > 10.0f)
                {
                    orviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                orviBuffer[i] = orvi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                if (orviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (orviBuffer[i] < actualMin) actualMin = orviBuffer[i];
                    if (orviBuffer[i] > actualMax) actualMax = orviBuffer[i];
                }
            }

            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = 0.5f;
                actualMax = 3.0f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < orviBuffer.Length; i++)
            {
                if (orviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255;
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (orviBuffer[i] - actualMin) / (actualMax - actualMin);
                    // Clamp к [0, 1] на случай выбросов
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var orviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            orviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            orviDataset.SetProjection(projection);

            var orviBand = orviDataset.GetRasterBand(1);
            orviBand.SetNoDataValue(255);

            orviBand.SetRasterColorInterpretation(ColorInterp.GCI_GrayIndex);

            orviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static List<(string B04Path, string B08Path)> MatchGranulesOSAVI(List<string> b04Files, List<string> b08Files)
        {
            var granules = new List<(string B04Path, string B08Path)>();

            foreach (var b04 in b04Files)
            {
                string fileName = Path.GetFileName(b04);
                if (!fileName.EndsWith("_B04.jp2")) continue;

                string basePart = fileName.Replace("_B04.jp2", "");
                string expectedB08 = basePart + "_B08.jp2";

                string b08Match = b08Files.FirstOrDefault(f => Path.GetFileName(f) == expectedB08);

                if (b08Match != null)
                {
                    granules.Add((b04, b08Match));
                }
            }

            return granules;
        }
        public static string CreateColorfulOSAVITiff(string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_OSAVI_vis3") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize)
            {
                throw new Exception("B04 and B08 must have the same dimensions!");
            }

            var b04Band = b04Dataset.GetRasterBand(1); // RED
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b04HasScale == 0 || b04Scale == 0)
            {
                b04Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b04Buffer.Length; i++)
            {
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var osaviBuffer = new float[width * height];
            int noDataCount = 0;

            const float OSAVI_X = 0.16f; // Почвенная коррекция
            const float EPS = 1e-9f;

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (red <= 0 || nir <= 0)
                {
                    osaviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float osavi = (nir - red) / (nir + red + OSAVI_X + EPS);

                if (float.IsNaN(osavi) || float.IsInfinity(osavi) || osavi < -1.0f || osavi > 1.0f)
                {
                    osaviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                osaviBuffer[i] = osavi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                if (osaviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (osaviBuffer[i] < actualMin) actualMin = osaviBuffer[i];
                    if (osaviBuffer[i] > actualMax) actualMax = osaviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.1f;
                actualMax = 0.8f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                if (osaviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → прозрачный
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (osaviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var osaviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            osaviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            osaviDataset.SetProjection(projection);

            var osaviBand = osaviDataset.GetRasterBand(1);
            osaviBand.SetNoDataValue(255);

            // Цветовая палитра для OSAVI: от коричневого (почва) → жёлтого → зелёного → тёмно-зелёного (густая растительность)
            ColorTable colorTable = new ColorTable(PaletteInterp.GPI_RGB);

            // Хелперы
            static byte Lerp(byte a, byte b, double t) =>
                (byte)Math.Clamp(Math.Round(a + (b - a) * t), 0, 255);
            static (byte r, byte g, byte b) Lerp3((byte r, byte g, byte b) c1, (byte r, byte g, byte b) c2, double t) =>
                (Lerp(c1.r, c2.r, t), Lerp(c1.g, c2.g, t), Lerp(c1.b, c2.b, t));

            // Узловые цвета для OSAVI (диапазон ~ -0.1 до 0.8)
            var bareSoil = (r: (byte)139, g: (byte)69, b: (byte)19);   // Коричневый — голая почва
            var dryVegetation = (r: (byte)210, g: (byte)180, b: (byte)140); // Сухая/начальная растительность
            var stressed = (r: (byte)255, g: (byte)220, b: (byte)40);  // Жёлтый — стресс/начало роста
            var moderate = (r: (byte)160, g: (byte)220, b: (byte)100); // Светло-зелёный — умеренная растительность
            var healthy = (r: (byte)80, g: (byte)180, b: (byte)70);    // Зелёный — здоровая растительность
            var dense = (r: (byte)20, g: (byte)100, b: (byte)30);      // Тёмно-зелёный — густая растительность

            // Пороговые значения OSAVI (адаптивные, но типичные диапазоны)
            double t0 = 0.0;   // Почва → начальная растительность
            double t1 = 0.2;   // Начало вегетации
            double t2 = 0.35;  // Стресс/умеренный рост
            double t3 = 0.5;   // Хорошая растительность
            double t4 = 0.65;  // Густая растительность

            // NoData — прозрачный
            colorTable.SetColorEntry(255, new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 });

            // Заполняем палитру
            for (int idx = 1; idx <= 254; idx++)
            {
                double osavi = actualMin + (idx / 254.0) * (actualMax - actualMin);
                ColorEntry ce;

                if (osavi < 0.0)
                {
                    // Очень низкие значения — почва, тени, вода
                    double t = Math.Clamp((osavi - (actualMin)) / (0.0 - actualMin), 0, 1);
                    var c = Lerp3(bareSoil, dryVegetation, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (osavi < t0)
                {
                    double t = osavi / t0;
                    var c = Lerp3(dryVegetation, stressed, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (osavi < t1)
                {
                    double t = (osavi - t0) / (t1 - t0);
                    var c = Lerp3(stressed, moderate, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (osavi < t3)
                {
                    double t = (osavi - t1) / (t3 - t1);
                    var c = Lerp3(moderate, healthy, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else if (osavi < t4)
                {
                    double t = (osavi - t3) / (t4 - t3);
                    var c = Lerp3(healthy, dense, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }
                else
                {
                    double t = Math.Clamp((osavi - t4) / (actualMax - t4), 0, 1);
                    var c = Lerp3(dense, dense, t);
                    ce = new ColorEntry { c1 = c.r, c2 = c.g, c3 = c.b, c4 = 255 };
                }

                colorTable.SetColorEntry(idx, ce);
            }

            osaviBand.SetRasterColorTable(colorTable);
            osaviBand.SetRasterColorInterpretation(ColorInterp.GCI_PaletteIndex);

            osaviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }
        public static string CreateGrayscaleOSAVITiff(string b04Path, string b08Path)
        {
            string outputFileName = Path.GetFileNameWithoutExtension(b08Path).Replace("_B08", "_OSAVI_gray") + ".tif";
            string outputPath = Path.Combine(Path.GetDirectoryName(b08Path), outputFileName);

            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            using var b04Dataset = Gdal.Open(b04Path, Access.GA_ReadOnly); // RED
            using var b08Dataset = Gdal.Open(b08Path, Access.GA_ReadOnly); // NIR

            if (b04Dataset == null || b08Dataset == null)
            {
                throw new Exception($"Failed to open one of the files: {b04Path}, {b08Path}");
            }

            int width = b08Dataset.RasterXSize;
            int height = b08Dataset.RasterYSize;

            if (width != b04Dataset.RasterXSize || height != b04Dataset.RasterYSize)
            {
                throw new Exception("B04 and B08 must have the same dimensions!");
            }

            var b04Band = b04Dataset.GetRasterBand(1); // RED
            var b08Band = b08Dataset.GetRasterBand(1); // NIR

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            double b04Scale, b04Offset;
            int b04HasScale, b04HasOffset;
            b04Band.GetScale(out b04Scale, out b04HasScale);
            b04Band.GetOffset(out b04Offset, out b04HasOffset);

            double b08Scale, b08Offset;
            int b08HasScale, b08HasOffset;
            b08Band.GetScale(out b08Scale, out b08HasScale);
            b08Band.GetOffset(out b08Offset, out b08HasOffset);

            if (b04HasScale == 0 || b04Scale == 0)
            {
                b04Scale = 0.0001;
            }
            if (b08HasScale == 0 || b08Scale == 0)
            {
                b08Scale = 0.0001;
            }

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            for (int i = 0; i < b04Buffer.Length; i++)
            {
                b04Buffer[i] = (float)(b04Offset + b04Buffer[i] * b04Scale);
                b08Buffer[i] = (float)(b08Offset + b08Buffer[i] * b08Scale);

                if (float.IsNaN(b04Buffer[i]) || float.IsInfinity(b04Buffer[i])) b04Buffer[i] = 0f;
                if (float.IsNaN(b08Buffer[i]) || float.IsInfinity(b08Buffer[i])) b08Buffer[i] = 0f;
            }

            var osaviBuffer = new float[width * height];
            int noDataCount = 0;

            const float OSAVI_X = 0.16f; // Почвенная коррекция
            const float EPS = 1e-9f;

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                float red = b04Buffer[i];
                float nir = b08Buffer[i];

                if (red <= 0 || nir <= 0)
                {
                    osaviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                float osavi = (nir - red) / (nir + red + OSAVI_X + EPS);

                if (float.IsNaN(osavi) || float.IsInfinity(osavi) || osavi < -1.0f || osavi > 1.0f)
                {
                    osaviBuffer[i] = -9999f;
                    noDataCount++;
                    continue;
                }

                osaviBuffer[i] = osavi;
            }

            float actualMin = float.MaxValue;
            float actualMax = float.MinValue;
            int validPixelCount = 0;

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                if (osaviBuffer[i] > -1000)
                {
                    validPixelCount++;
                    if (osaviBuffer[i] < actualMin) actualMin = osaviBuffer[i];
                    if (osaviBuffer[i] > actualMax) actualMax = osaviBuffer[i];
                }
            }

            // Защита от случая, когда все значения одинаковы
            if (actualMin == actualMax || validPixelCount == 0)
            {
                actualMin = -0.1f;
                actualMax = 0.8f;
            }

            var byteBuffer = new byte[width * height];

            for (int i = 0; i < osaviBuffer.Length; i++)
            {
                if (osaviBuffer[i] < -1000)
                {
                    byteBuffer[i] = 255; // NoData → белый или прозрачный? Решаем ниже
                }
                else
                {
                    // Нормализация в [0, 1]
                    float normalized = (osaviBuffer[i] - actualMin) / (actualMax - actualMin);
                    normalized = Math.Max(0, Math.Min(1, normalized));
                    int scaled = (int)Math.Round(normalized * 254);
                    byteBuffer[i] = (byte)Math.Max(0, Math.Min(254, scaled));
                }
            }

            string[] creationOptions = { "COMPRESS=LZW", "TILED=YES", "BIGTIFF=IF_SAFER" };
            Driver driver = Gdal.GetDriverByName("GTiff");
            if (driver == null)
            {
                throw new Exception("GTiff driver not found!");
            }

            using var osaviDataset = driver.Create(outputPath, width, height, 1, DataType.GDT_Byte, creationOptions);

            double[] geoTransform = new double[6];
            b08Dataset.GetGeoTransform(geoTransform);
            osaviDataset.SetGeoTransform(geoTransform);

            string projection = b08Dataset.GetProjection();
            osaviDataset.SetProjection(projection);

            var osaviBand = osaviDataset.GetRasterBand(1);
            osaviBand.SetNoDataValue(255);

            // 🔥 ВАЖНО: Убираем палитру, интерпретируем как градации серого
            osaviBand.SetRasterColorInterpretation(ColorInterp.GCI_GrayIndex);

            // Пишем данные
            osaviBand.WriteRaster(0, 0, width, height, byteBuffer, width, height, 0, 0);

            return outputPath;
        }

        #region WorkTiff
        public static TiffGenerationResult ArchiveFolderWorkARVI(string imgDataPath, string productId)
        {
            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b02Files.Any() || !b04Files.Any() || !b08Files.Any())
            {
                return null;
            }

            var granules = IndicesHelper.MatchGranulesARVI(b02Files, b04Files, b08Files);
            if (granules.Count == 0)
            {
                return null;
            }
            string localArviPath = IndicesHelper.CreateGrayscaleARVITiff(granules[0].B02Path, granules[0].B04Path, granules[0].B08Path);
            string localArviColoredPath = IndicesHelper.CreateColorfulARVITiff(granules[0].B02Path, granules[0].B04Path, granules[0].B08Path);

            return new TiffGenerationResult()
            {
                GrayTiffBucket = "sentinelarvi",
                GrayTiffFileName = $"{productId}_ARVI.tif",
                GrayTiffLocalPath = localArviPath,
                ColorTiffBucket = "sentinelarvicolore",
                ColorTiffFileName = $"{productId}_ARVI_vis.tif",
                ColorTiffLocalPath = localArviColoredPath
            };
        }
        public static TiffGenerationResult ArchiveFolderWorkEVI(string imgDataPath, string productId)
        {
            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b02Files.Any() || !b04Files.Any() || !b08Files.Any())
            {
                return null;
            }

            var granules = IndicesHelper.MatchGranulesEVI(b02Files, b04Files, b08Files);
            if (granules.Count == 0)
            {
                return null;
            }
            string localGrayPath = IndicesHelper.CreateGrayscaleEVITiff(granules[0].B02Path, granules[0].B04Path, granules[0].B08Path);
            string localColoredPath = IndicesHelper.CreateColorfulEVITiff(granules[0].B02Path, granules[0].B04Path, granules[0].B08Path);

            return new TiffGenerationResult()
            {
                GrayTiffBucket = "sentinelevi",
                GrayTiffFileName = $"{productId}_EVI.tif",
                GrayTiffLocalPath = localGrayPath,
                ColorTiffBucket = "sentinelevicolore",
                ColorTiffFileName = $"{productId}_EVI_vis.tif",
                ColorTiffLocalPath = localColoredPath
            };
        }
        public static TiffGenerationResult ArchiveFolderWorkGNDVI(string imgDataPath, string productId)
        {
            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
            {
                return null;
            }

            var granules = IndicesHelper.MatchGranulesGNDVI(b03Files, b08Files);
            if (granules.Count == 0)
            {
                return null;
            }
            string localPath = IndicesHelper.CreateGrayscaleGNDVITiff(granules[0].B03Path, granules[0].B08Path);
            string localColoredPath = IndicesHelper.CreateColorfulGNDVITiff(granules[0].B03Path, granules[0].B08Path);

            return new TiffGenerationResult()
            {
                GrayTiffBucket = "sentinelgndvi",
                GrayTiffFileName = $"{productId}_GNDVI.tif",
                GrayTiffLocalPath = localPath,
                ColorTiffBucket = "sentinelgndvicolore",
                ColorTiffFileName = $"{productId}_GNDVI_vis.tif",
                ColorTiffLocalPath = localColoredPath
            };
        }
        public static TiffGenerationResult ArchiveFolderWorkMNDWI(string imgDataPath, string productId)
        {
            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b11Files.Any())
            {
                return null;
            }

            var granules = IndicesHelper.MatchGranulesMNDWI(b03Files, b11Files);
            if (granules.Count == 0)
            {
                return null;
            }
            string localPath = IndicesHelper.CreateGrayscaleMNDWITiff(granules[0].B03Path, granules[0].B11Path);
            string localColoredPath = IndicesHelper.CreateColorfulMNDWITiff(granules[0].B03Path, granules[0].B11Path);

            return new TiffGenerationResult()
            {
                GrayTiffBucket = "sentinelmndwi",
                GrayTiffFileName = $"{productId}_MNDWI.tif",
                GrayTiffLocalPath = localPath,
                ColorTiffBucket = "sentinelmndwicolore",
                ColorTiffFileName = $"{productId}_MNDWI_vis.tif",
                ColorTiffLocalPath = localColoredPath
            };
        }
        public static TiffGenerationResult ArchiveFolderWorkNDMI(string imgDataPath, string productId)
        {
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b08Files.Any() || !b11Files.Any())
            {
                return null;
            }

            var granules = IndicesHelper.MatchGranulesNDMI(b08Files, b11Files);
            if (granules.Count == 0)
            {
                return null;
            }
            string localPath = IndicesHelper.CreateGrayscaleNDMITiff(granules[0].B08Path, granules[0].B11Path);
            string localColoredPath = IndicesHelper.CreateColorfulNDMITiff(granules[0].B08Path, granules[0].B11Path);

            return new TiffGenerationResult()
            {
                GrayTiffBucket = "sentinelndmi",
                GrayTiffFileName = $"{productId}_NDMI.tif",
                GrayTiffLocalPath = localPath,
                ColorTiffBucket = "sentinelndmicolore",
                ColorTiffFileName = $"{productId}_NDMI_vis.tif",
                ColorTiffLocalPath = localColoredPath
            };
        }
        public static TiffGenerationResult ArchiveFolderWorkNDVI(string imgDataPath, string productId)
        {
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
            {
                return null;
            }

            var granules = IndicesHelper.MatchGranulesNDVI(b04Files, b08Files);
            if (granules.Count == 0)
            {
                return null;
            }
            string localPath = IndicesHelper.CreateGrayscaleNDVITiff(granules[0].B04Path, granules[0].B08Path);
            string localColoredPath = IndicesHelper.CreateColorfulNDVITiff(granules[0].B04Path, granules[0].B08Path);

            return new TiffGenerationResult()
            {
                GrayTiffBucket = "sentinelndvi",
                GrayTiffFileName = $"{productId}_NDVI.tif",
                GrayTiffLocalPath = localPath,
                ColorTiffBucket = "sentinelndvicolore",
                ColorTiffFileName = $"{productId}_NDVI_vis.tif",
                ColorTiffLocalPath = localColoredPath
            };
        }
        public static TiffGenerationResult ArchiveFolderWorkORVI(string imgDataPath, string productId)
        {
            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
            {
                return null;
            }

            var granules = IndicesHelper.MatchGranulesORVI(b03Files, b08Files);
            if (granules.Count == 0)
            {
                return null;
            }
            string localPath = IndicesHelper.CreateGrayscaleORVITiff(granules[0].B03Path, granules[0].B08Path);
            string localColoredPath = IndicesHelper.CreateColorfulORVITiff(granules[0].B03Path, granules[0].B08Path);

            return new TiffGenerationResult()
            {
                GrayTiffBucket = "sentinelorvi",
                GrayTiffFileName = $"{productId}_ORVI.tif",
                GrayTiffLocalPath = localPath,
                ColorTiffBucket = "sentinelorvicolore",
                ColorTiffFileName = $"{productId}_ORVI_vis.tif",
                ColorTiffLocalPath = localColoredPath
            };
        }
        public static TiffGenerationResult ArchiveFolderWorkOSAVI(string imgDataPath, string productId)
        {
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
            {
                return null;
            }

            var granules = IndicesHelper.MatchGranulesOSAVI(b04Files, b08Files);
            if (granules.Count == 0)
            {
                return null;
            }
            string localPath = IndicesHelper.CreateGrayscaleOSAVITiff(granules[0].B04Path, granules[0].B08Path);
            string localColoredPath = IndicesHelper.CreateColorfulOSAVITiff(granules[0].B04Path, granules[0].B08Path);

            return new TiffGenerationResult()
            {
                GrayTiffBucket = "sentinelosavi",
                GrayTiffFileName = $"{productId}_OSAVI.tif",
                GrayTiffLocalPath = localPath,
                ColorTiffBucket = "sentinelosavicolore",
                ColorTiffFileName = $"{productId}_OSAVI_vis.tif",
                ColorTiffLocalPath = localColoredPath
            };
        }
        #endregion

        #region WorkDbForSeason
        public static List<ArviPoint> ArchiveFolderWorkDbARVI(
            string imgDataPath,
            string productId,
            List<Masofa.Common.Models.CropMonitoring.Field> fields,
            List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<ArviPoint>();

            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b02Files.Any() || !b04Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesARVI(b02Files, b04Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b02Dataset = Gdal.Open(granules[0].B02Path, Access.GA_ReadOnly);
            using var b04Dataset = Gdal.Open(granules[0].B04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
                return result;

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
                return result;

            var b02Band = b02Dataset.GetRasterBand(1);
            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b04Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B04Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                            var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                            var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float blue = b02Buffer[i];
                        float red = b04Buffer[i];
                        float nir = b08Buffer[i];

                        if (blue <= 0 || red <= 0 || nir <= 0)
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new ArviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero2 = blue,
                            BZero4 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static List<EviPoint> ArchiveFolderWorkDbEVI(
            string imgDataPath,
            string productId,
            List<Masofa.Common.Models.CropMonitoring.Field> fields,
            List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<EviPoint>();

            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b02Files.Any() || !b04Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesEVI(b02Files, b04Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b02Dataset = Gdal.Open(granules[0].B02Path, Access.GA_ReadOnly);
            using var b04Dataset = Gdal.Open(granules[0].B04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
                return result;

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
                return result;

            var b02Band = b02Dataset.GetRasterBand(1);
            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b04Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B04Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float blue = b02Buffer[i];
                        float red = b04Buffer[i];
                        float nir = b08Buffer[i];

                        if (blue <= 0 || red <= 0 || nir <= 0)
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new EviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero2 = blue,
                            BZero4 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static List<GndviPoint> ArchiveFolderWorkDbGNDVI(
            string imgDataPath,
            string productId,
            List<Masofa.Common.Models.CropMonitoring.Field> fields,
            List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<GndviPoint>();

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesGNDVI(b03Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b03Dataset = Gdal.Open(granules[0].B03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
                return result;

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b03Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B03Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b03Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new GndviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero3 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static List<MndwiPoint> ArchiveFolderWorkDbMNDWI(
            string imgDataPath,
            string productId,
            List<Masofa.Common.Models.CropMonitoring.Field> fields,
            List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<MndwiPoint>();

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b11Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesMNDWI(b03Files, b11Files);
            if (granules.Count == 0)
                return result;

            using var b03Dataset = Gdal.Open(granules[0].B03Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(granules[0].B11Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b11Dataset == null)
                return result;

            int targetWidth = b03Dataset.RasterXSize;
            int targetHeight = b03Dataset.RasterYSize;
            double[] targetGeoTransform = new double[6];
            b03Dataset.GetGeoTransform(targetGeoTransform);
            string targetProjection = b03Dataset.GetProjection();

            using var memDriver = Gdal.GetDriverByName("MEM");
            using var b11Resampled = memDriver.Create("", targetWidth, targetHeight, 1, DataType.GDT_Float32, null);

            b11Resampled.SetGeoTransform(targetGeoTransform);
            b11Resampled.SetProjection(targetProjection);

            Gdal.ReprojectImage(b11Dataset, b11Resampled, null, null, ResampleAlg.GRA_Bilinear, 0, 0.0, null, null, null);

            var b03Band = b03Dataset.GetRasterBand(1);
            var b11Band = b11Resampled.GetRasterBand(1);

            var b03Buffer = new float[targetWidth * targetHeight];
            var b11Buffer = new float[targetWidth * targetHeight];

            b03Band.ReadRaster(0, 0, targetWidth, targetHeight, b03Buffer, targetWidth, targetHeight, 0, 0);
            b11Band.ReadRaster(0, 0, targetWidth, targetHeight, b11Buffer, targetWidth, targetHeight, 0, 0);

            string wkt = targetProjection;
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = targetGeoTransform;
            int width = targetWidth;
            int height = targetHeight;

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B03Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b03Buffer[i];
                        float nir = b11Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new MndwiPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero3 = red,
                            B11 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static List<NdmiPoint> ArchiveFolderWorkDbNDMI(
            string imgDataPath,
            string productId,
            List<Masofa.Common.Models.CropMonitoring.Field> fields,
            List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<NdmiPoint>();

            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b11Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesNDMI(b08Files, b11Files);
            if (granules.Count == 0)
                return result;

            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(granules[0].B11Path, Access.GA_ReadOnly);

            if (b11Dataset == null || b08Dataset == null) 
                return result;

            int targetWidth = b08Dataset.RasterXSize;
            int targetHeight = b08Dataset.RasterYSize;
            double[] targetGeoTransform = new double[6];
            b08Dataset.GetGeoTransform(targetGeoTransform);
            string targetProjection = b08Dataset.GetProjection();

            using var memDriver = Gdal.GetDriverByName("MEM");
            using var b11Resampled = memDriver.Create("", targetWidth, targetHeight, 1, DataType.GDT_Float32, null);

            b11Resampled.SetGeoTransform(targetGeoTransform);
            b11Resampled.SetProjection(targetProjection);

            Gdal.ReprojectImage(b11Dataset, b11Resampled, null, null, ResampleAlg.GRA_Bilinear, 0, 0.0, null, null, null);

            var b08Band = b08Dataset.GetRasterBand(1);
            var b11Band = b11Resampled.GetRasterBand(1);

            var b08Buffer = new float[targetWidth * targetHeight];
            var b11Buffer = new float[targetWidth * targetHeight];

            b08Band.ReadRaster(0, 0, targetWidth, targetHeight, b08Buffer, targetWidth, targetHeight, 0, 0);
            b11Band.ReadRaster(0, 0, targetWidth, targetHeight, b11Buffer, targetWidth, targetHeight, 0, 0);

            string wkt = targetProjection;
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = targetGeoTransform;
            int width = targetWidth;
            int height = targetHeight;

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B08Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;
                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b11Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0 || float.IsNaN(red) || float.IsNaN(nir))
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new NdmiPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero8 = nir,
                            B11 = red,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static List<NdviPoint> ArchiveFolderWorkDbNDVI(
        string imgDataPath,
        string productId,
        List<Masofa.Common.Models.CropMonitoring.Field> fields,
        List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<NdviPoint>();

            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesNDVI(b04Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b04Dataset = Gdal.Open(granules[0].B04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b04Dataset == null || b08Dataset == null)
                return result;

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b04Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B04Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b04Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new NdviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero4 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static List<NdwiPoint> ArchiveFolderWorkDbNDWI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields,
    List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<NdwiPoint>();

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesGNDVI(b03Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b03Dataset = Gdal.Open(granules[0].B03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
                return result;

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b03Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B03Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b03Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new NdwiPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero3 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static List<OrviPoint> ArchiveFolderWorkDbORVI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields,
    List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<OrviPoint>();

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesORVI(b03Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b03Dataset = Gdal.Open(granules[0].B03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
                return result;

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b03Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B03Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float green = b03Buffer[i];
                        float nir = b08Buffer[i];

                        if (green <= 0 || nir <= 0)
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new OrviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero3 = green,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static List<OsaviPoint> ArchiveFolderWorkDbOSAVI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields,
    List<Masofa.Common.Models.CropMonitoring.Season> seasons)
        {
            var result = new List<OsaviPoint>();

            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesOSAVI(b04Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b04Dataset = Gdal.Open(granules[0].B04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b04Dataset == null || b08Dataset == null)
                return result;

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b04Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {seasons.Count} seasons for {Path.GetFileName(granules[0].B04Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var season in seasons)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {seasons.Count}");

                if (season.Polygon == null) continue;

                var envWgs84 = season.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!season.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b04Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        Masofa.Common.Models.CropMonitoring.Field field = null;
                        if (season.FieldId.HasValue && fieldById.TryGetValue(season.FieldId.Value, out var f))
                        {
                            if (f.Polygon?.Contains(pointGeom) == true)
                                field = f;
                        }

                        result.Add(new OsaviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero4 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                            SeasonId = season.Id
                        });
                    }
                }
            }

            return result;
        }

        public static string ExportTiffWithPolygonToSvg(string tiffPath, Polygon polygon)
        {
            using var dataset = Gdal.Open(tiffPath, Access.GA_ReadOnly);
            if (dataset == null)
                throw new ArgumentException("Не удалось открыть TIFF-файл.", nameof(tiffPath));


            int dataSetWidth = dataset.RasterXSize;
            int dataSetHeight = dataset.RasterYSize;






            string wkt = dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            var dataSetGeoTransform = new double[6];
            dataset.GetGeoTransform(dataSetGeoTransform);

            // Получаем bounding box полигона
            var env = polygon.EnvelopeInternal;
            var envWgs84 = polygon.EnvelopeInternal;

            var cornersWgs84 = new (double X, double Y)[]
            {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
            };

            var cornersRaster = cornersWgs84
                .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                .ToArray();

            double rasterMinX = cornersRaster.Min(p => p.X);
            double rasterMaxX = cornersRaster.Max(p => p.X);
            double rasterMinY = cornersRaster.Min(p => p.Y);
            double rasterMaxY = cornersRaster.Max(p => p.Y);

            int xStart = (int)Math.Floor((rasterMinX - dataSetGeoTransform[0]) / dataSetGeoTransform[1]);
            int xEnd = (int)Math.Ceiling((rasterMaxX - dataSetGeoTransform[0]) / dataSetGeoTransform[1]);
            int yStart = (int)Math.Floor((rasterMaxY - dataSetGeoTransform[3]) / dataSetGeoTransform[5]);
            int yEnd = (int)Math.Ceiling((rasterMinY - dataSetGeoTransform[3]) / dataSetGeoTransform[5]);

            xStart = Math.Max(0, xStart);
            xEnd = Math.Min(dataSetWidth, xEnd);
            yStart = Math.Max(0, yStart);
            yEnd = Math.Min(dataSetHeight, yEnd);

            if (xStart >= xEnd || yStart >= yEnd)
            {
                return string.Empty;
            }

            // Читаем данные
            const int bandCount = 3; // RGB
            var buffer = new float[bandCount, yEnd - yStart, xEnd - xStart];
            var datasetBand = dataset.GetRasterBand(1);
            var datasetBuffer = new float[dataSetWidth * dataSetHeight];
            datasetBand.ReadRaster(0, 0, dataSetWidth, dataSetHeight, datasetBuffer, dataSetWidth, dataSetHeight, 0, 0);
            var colorTable = datasetBand.GetRasterColorTable();
            for (int y = 0; y < yEnd - yStart; y++)
            {
                for (int x = 0; x < xEnd - xStart; x++)
                {
                    try
                    {
                        double pixelCenterX = dataSetGeoTransform[0] + (x + 0.5) * dataSetGeoTransform[1];
                        double pixelCenterY = dataSetGeoTransform[3] + (y + 0.5) * dataSetGeoTransform[5];
                        int i = ((y + yStart) * dataSetWidth) + (x + xStart);

                        // Получаем цвет из палитры
                        ColorEntry ce;
                        if (i == 255) // NoData — прозрачный
                        {
                            ce = new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 0 };
                        }
                        else
                        {
                            ce = colorTable.GetColorEntry(i);
                            // Если вдруг цвет не задан — чёрный
                            if (ce == null)
                                ce = new ColorEntry { c1 = 0, c2 = 0, c3 = 0, c4 = 255 };
                        }

                        buffer[0, y, x] = ce.c1; // R
                        buffer[1, y, x] = ce.c2; // G
                        buffer[2, y, x] = ce.c3; // B
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

            //var pixelShell = new List<PointF>();

            //for (int y = 0; y < yEnd - yStart; y++)
            //{
            //    for (int x = 0; x < xEnd - xStart; x++)
            //    {
            //        pixelShell.Add(new PointF(x, y));
            //    }
            //}


            // Создаём ImageSharp изображение
            using var image = new SixLabors.ImageSharp.Image<SixLabors.ImageSharp.PixelFormats.Rgba32>(xEnd - xStart, yEnd - yStart);
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<SixLabors.ImageSharp.PixelFormats.Rgba32> row = accessor.GetRowSpan(y);
                    for (int x = 0; x < accessor.Width; x++)
                    {
                        byte r = (byte)Math.Clamp(buffer[0, y, x], 0, 255);
                        byte g = (byte)Math.Clamp(buffer[1, y, x], 0, 255);
                        byte b = (byte)Math.Clamp(buffer[2, y, x], 0, 255);
                        row[x] = new SixLabors.ImageSharp.PixelFormats.Rgba32(r, g, b, 255);
                    }
                }
            });


            //// Рисуем полигон поверх изображения
            //var pen = new SixLabors.ImageSharp.Drawing.Processing.SolidPen(SixLabors.ImageSharp.Color.Magenta, 2); // фиолетовая линия (Magenta ≈ фиолетовая)
            //image.Mutate(ctx =>
            //{
            //    var pathBuilder = new SixLabors.ImageSharp.Drawing.PathBuilder();
            //    for (int i = 1; i < pixelShell.Count; i++)
            //    {
            //        pathBuilder.AddLine(pixelShell[i - 1], pixelShell[i]);
            //    }
            //    ctx.Draw(pen, new SixLabors.ImageSharp.Drawing.PathCollection(pathBuilder.Build()));
            //});

            // Конвертируем в base64 PNG
            using var ms = new MemoryStream();
            image.SaveAsPng(ms);
            string base64Image = Convert.ToBase64String(ms.ToArray());

            // Генерируем SVG
            var svg = $"<svg xmlns='http://www.w3.org/2000/svg' width='{xEnd - xStart}' height='{yEnd - yStart}' viewBox='0 0 {xEnd - xStart} {yEnd - yStart}'>" +
                    $"<image href='data:image/png;base64,{base64Image}' width='{xEnd - xStart}' height='{yEnd - yStart}' />" +
                    $"</svg>".Trim();

            return svg;
        }

        #endregion

        #region WorkDbForField
        public static List<ArviPoint> ArchiveFolderWorkDbARVI(
            string imgDataPath,
            string productId,
            List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<ArviPoint>();

            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b02Files.Any() || !b04Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesARVI(b02Files, b04Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b02Dataset = Gdal.Open(granules[0].B02Path, Access.GA_ReadOnly);
            using var b04Dataset = Gdal.Open(granules[0].B04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
                return result;

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
                return result;

            var b02Band = b02Dataset.GetRasterBand(1);
            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b04Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} seasons for {Path.GetFileName(granules[0].B04Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float blue = b02Buffer[i];
                        float red = b04Buffer[i];
                        float nir = b08Buffer[i];

                        if (blue <= 0 || red <= 0 || nir <= 0)
                            continue;

                        result.Add(new ArviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero2 = blue,
                            BZero4 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                        });
                    }
                }
            }

            return result;
        }

        public static List<EviPoint> ArchiveFolderWorkDbEVI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<EviPoint>();

            var b02Files = Directory.GetFiles(imgDataPath, "*B02.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b02Files.Any() || !b04Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesEVI(b02Files, b04Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b02Dataset = Gdal.Open(granules[0].B02Path, Access.GA_ReadOnly);
            using var b04Dataset = Gdal.Open(granules[0].B04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b02Dataset == null || b04Dataset == null || b08Dataset == null)
                return result;

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize ||
                width != b02Dataset.RasterXSize || height != b02Dataset.RasterYSize)
                return result;

            var b02Band = b02Dataset.GetRasterBand(1);
            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b02Buffer = new float[width * height];
            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b02Band.ReadRaster(0, 0, width, height, b02Buffer, width, height, 0, 0);
            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b04Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} seasons for {Path.GetFileName(granules[0].B04Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float blue = b02Buffer[i];
                        float red = b04Buffer[i];
                        float nir = b08Buffer[i];

                        if (blue <= 0 || red <= 0 || nir <= 0)
                            continue;

                        result.Add(new EviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero2 = blue,
                            BZero4 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty
                        });
                    }
                }
            }

            return result;
        }

        public static List<GndviPoint> ArchiveFolderWorkDbGNDVI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<GndviPoint>();

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesGNDVI(b03Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b03Dataset = Gdal.Open(granules[0].B03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
                return result;

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b03Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} seasons for {Path.GetFileName(granules[0].B03Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b03Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        result.Add(new GndviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero3 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty
                        });
                    }
                }
            }

            return result;
        }

        public static List<MndwiPoint> ArchiveFolderWorkDbMNDWI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<MndwiPoint>();

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b11Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesMNDWI(b03Files, b11Files);
            if (granules.Count == 0)
                return result;

            using var b03Dataset = Gdal.Open(granules[0].B03Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(granules[0].B11Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b11Dataset == null)
                return result;

            int targetWidth = b03Dataset.RasterXSize;
            int targetHeight = b03Dataset.RasterYSize;
            double[] targetGeoTransform = new double[6];
            b03Dataset.GetGeoTransform(targetGeoTransform);
            string targetProjection = b03Dataset.GetProjection();

            using var memDriver = Gdal.GetDriverByName("MEM");
            using var b11Resampled = memDriver.Create("", targetWidth, targetHeight, 1, DataType.GDT_Float32, null);

            b11Resampled.SetGeoTransform(targetGeoTransform);
            b11Resampled.SetProjection(targetProjection);

            Gdal.ReprojectImage(b11Dataset, b11Resampled, null, null, ResampleAlg.GRA_Bilinear, 0, 0.0, null, null, null);

            var b03Band = b03Dataset.GetRasterBand(1);
            var b11Band = b11Resampled.GetRasterBand(1);

            var b03Buffer = new float[targetWidth * targetHeight];
            var b11Buffer = new float[targetWidth * targetHeight];

            b03Band.ReadRaster(0, 0, targetWidth, targetHeight, b03Buffer, targetWidth, targetHeight, 0, 0);
            b11Band.ReadRaster(0, 0, targetWidth, targetHeight, b11Buffer, targetWidth, targetHeight, 0, 0);

            string wkt = targetProjection;
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = targetGeoTransform;
            int width = targetWidth;
            int height = targetHeight;

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} seasons for {Path.GetFileName(granules[0].B03Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b03Buffer[i];
                        float nir = b11Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;


                        result.Add(new MndwiPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero3 = red,
                            B11 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty
                        });
                    }
                }
            }

            return result;
        }

        public static List<NdmiPoint> ArchiveFolderWorkDbNDMI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<NdmiPoint>();

            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b11Files = Directory.GetFiles(imgDataPath, "*B11.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b11Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesNDMI(b08Files, b11Files);
            if (granules.Count == 0)
                return result;

            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);
            using var b11Dataset = Gdal.Open(granules[0].B11Path, Access.GA_ReadOnly);

            if (b11Dataset == null || b08Dataset == null)
                return result;

            int targetWidth = b08Dataset.RasterXSize;
            int targetHeight = b08Dataset.RasterYSize;
            double[] targetGeoTransform = new double[6];
            b08Dataset.GetGeoTransform(targetGeoTransform);
            string targetProjection = b08Dataset.GetProjection();

            using var memDriver = Gdal.GetDriverByName("MEM");
            using var b11Resampled = memDriver.Create("", targetWidth, targetHeight, 1, DataType.GDT_Float32, null);

            b11Resampled.SetGeoTransform(targetGeoTransform);
            b11Resampled.SetProjection(targetProjection);

            Gdal.ReprojectImage(b11Dataset, b11Resampled, null, null, ResampleAlg.GRA_Bilinear, 0, 0.0, null, null, null);

            var b08Band = b08Dataset.GetRasterBand(1);
            var b11Band = b11Resampled.GetRasterBand(1);

            var b08Buffer = new float[targetWidth * targetHeight];
            var b11Buffer = new float[targetWidth * targetHeight];

            b08Band.ReadRaster(0, 0, targetWidth, targetHeight, b08Buffer, targetWidth, targetHeight, 0, 0);
            b11Band.ReadRaster(0, 0, targetWidth, targetHeight, b11Buffer, targetWidth, targetHeight, 0, 0);

            string wkt = targetProjection;
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = targetGeoTransform;
            int width = targetWidth;
            int height = targetHeight;

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} seasons for {Path.GetFileName(granules[0].B08Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;
                var cornersWgs84 = new (double X, double Y)[]
                {
            (envWgs84.MinX, envWgs84.MinY),
            (envWgs84.MinX, envWgs84.MaxY),
            (envWgs84.MaxX, envWgs84.MinY),
            (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b11Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0 || float.IsNaN(red) || float.IsNaN(nir))
                            continue;

                        result.Add(new NdmiPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero8 = nir,
                            B11 = red,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty
                        });
                    }
                }
            }

            return result;
        }

        public static List<NdviPoint> ArchiveFolderWorkDbNDVI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<NdviPoint>();

            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesNDVI(b04Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b04Dataset = Gdal.Open(granules[0].B04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b04Dataset == null || b08Dataset == null)
                return result;

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b04Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} seasons for {Path.GetFileName(granules[0].B04Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b04Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        result.Add(new NdviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero4 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                        });
                    }
                }
            }

            return result;
        }

        public static List<NdwiPoint> ArchiveFolderWorkDbNDWI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<NdwiPoint>();

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesGNDVI(b03Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b03Dataset = Gdal.Open(granules[0].B03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
                return result;

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b03Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} seasons for {Path.GetFileName(granules[0].B03Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b03Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        result.Add(new NdwiPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero3 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty
                        });
                    }
                }
            }

            return result;
        }

        public static List<OrviPoint> ArchiveFolderWorkDbORVI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<OrviPoint>();

            var b03Files = Directory.GetFiles(imgDataPath, "*B03.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b03Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesORVI(b03Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b03Dataset = Gdal.Open(granules[0].B03Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b03Dataset == null || b08Dataset == null)
                return result;

            int width = b03Dataset.RasterXSize;
            int height = b03Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b03Band = b03Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b03Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b03Band.ReadRaster(0, 0, width, height, b03Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b03Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b03Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} seasons for {Path.GetFileName(granules[0].B03Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float green = b03Buffer[i];
                        float nir = b08Buffer[i];

                        if (green <= 0 || nir <= 0)
                            continue;

                        result.Add(new OrviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero3 = green,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                        });
                    }
                }
            }

            return result;
        }

        public static List<OsaviPoint> ArchiveFolderWorkDbOSAVI(
    string imgDataPath,
    string productId,
    List<Masofa.Common.Models.CropMonitoring.Field> fields)
        {
            var result = new List<OsaviPoint>();

            var b04Files = Directory.GetFiles(imgDataPath, "*B04.jp2", SearchOption.TopDirectoryOnly).ToList();
            var b08Files = Directory.GetFiles(imgDataPath, "*B08.jp2", SearchOption.TopDirectoryOnly).ToList();

            if (!b04Files.Any() || !b08Files.Any())
                return result;

            var granules = IndicesHelper.MatchGranulesOSAVI(b04Files, b08Files);
            if (granules.Count == 0)
                return result;

            using var b04Dataset = Gdal.Open(granules[0].B04Path, Access.GA_ReadOnly);
            using var b08Dataset = Gdal.Open(granules[0].B08Path, Access.GA_ReadOnly);

            if (b04Dataset == null || b08Dataset == null)
                return result;

            int width = b04Dataset.RasterXSize;
            int height = b04Dataset.RasterYSize;

            if (width != b08Dataset.RasterXSize || height != b08Dataset.RasterYSize)
                return result;

            var b04Band = b04Dataset.GetRasterBand(1);
            var b08Band = b08Dataset.GetRasterBand(1);

            var b04Buffer = new float[width * height];
            var b08Buffer = new float[width * height];

            b04Band.ReadRaster(0, 0, width, height, b04Buffer, width, height, 0, 0);
            b08Band.ReadRaster(0, 0, width, height, b08Buffer, width, height, 0, 0);

            string wkt = b04Dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");
            // 👇 ЭТО ВАЖНО!
            wgs84Srs.SetAxisMappingStrategy(AxisMappingStrategy.OAMS_TRADITIONAL_GIS_ORDER);

            double[] geoTransform = new double[6];
            b04Dataset.GetGeoTransform(geoTransform);

            var fieldById = fields.Where(f => f.Id != Guid.Empty)
                                  .ToDictionary(f => f.Id, f => f);

            Console.Write($"\rProcessing {fields.Count} fiedls for {Path.GetFileName(granules[0].B04Path)}");
            Console.WriteLine();
            var seasonIndex = 0;
            foreach (var field in fields)
            {
                seasonIndex++;
                Console.Write($"\rProcessing {seasonIndex} of {fields.Count}");

                if (field.Polygon == null) continue;

                var envWgs84 = field.Polygon.EnvelopeInternal;

                var cornersWgs84 = new (double X, double Y)[]
                {
                    (envWgs84.MinX, envWgs84.MinY),
                    (envWgs84.MinX, envWgs84.MaxY),
                    (envWgs84.MaxX, envWgs84.MinY),
                    (envWgs84.MaxX, envWgs84.MaxY)
                };

                var cornersRaster = cornersWgs84
                    .Select(c => TransformCoord(c.X, c.Y, wgs84Srs, rasterSrs))
                    .ToArray();

                double rasterMinX = cornersRaster.Min(p => p.X);
                double rasterMaxX = cornersRaster.Max(p => p.X);
                double rasterMinY = cornersRaster.Min(p => p.Y);
                double rasterMaxY = cornersRaster.Max(p => p.Y);

                int xStart = (int)Math.Floor((rasterMinX - geoTransform[0]) / geoTransform[1]);
                int xEnd = (int)Math.Ceiling((rasterMaxX - geoTransform[0]) / geoTransform[1]);
                int yStart = (int)Math.Floor((rasterMaxY - geoTransform[3]) / geoTransform[5]);
                int yEnd = (int)Math.Ceiling((rasterMinY - geoTransform[3]) / geoTransform[5]);

                xStart = Math.Max(0, xStart);
                xEnd = Math.Min(width, xEnd);
                yStart = Math.Max(0, yStart);
                yEnd = Math.Min(height, yEnd);

                if (xStart >= xEnd || yStart >= yEnd)
                {
                    continue;
                }
                for (int y = yStart; y < yEnd; y++)
                {
                    for (int x = xStart; x < xEnd; x++)
                    {
                        double pixelCenterX = geoTransform[0] + (x + 0.5) * geoTransform[1];
                        double pixelCenterY = geoTransform[3] + (y + 0.5) * geoTransform[5];

                        var (lon, lat) = TransformCoord(pixelCenterX, pixelCenterY, rasterSrs, wgs84Srs);
                        var pointGeom = new NetTopologySuite.Geometries.Point(lon, lat);

                        if (!field.Polygon.Contains(pointGeom))
                            continue;

                        int i = y * width + x;
                        float red = b04Buffer[i];
                        float nir = b08Buffer[i];

                        if (red <= 0 || nir <= 0)
                            continue;

                        result.Add(new OsaviPoint
                        {
                            ProductSourceType = ProductSourceType.Sentinel2,
                            Point = pointGeom,
                            BZero4 = red,
                            BZero8 = nir,
                            RegionId = field?.RegionId ?? Guid.Empty,
                            FieldId = field?.Id ?? Guid.Empty,
                        });
                    }
                }
            }

            return result;
        }

        #endregion


        public static (double X, double Y) TransformCoord(double x, double y, SpatialReference srcSrs, SpatialReference dstSrs)
        {
            var ct = new CoordinateTransformation(srcSrs, dstSrs);
            double[] coords = { x, y, 0 };
            ct.TransformPoint(coords);
            return (coords[0], coords[1]);
        }

        private static NetTopologySuite.Geometries.Polygon GetRasterExtentAsPolygon(
            Dataset dataset,
            SpatialReference rasterSrs,
            SpatialReference wgs84Srs)
        {
            int width = dataset.RasterXSize;
            int height = dataset.RasterYSize;

            double[] geoTransform = new double[6];
            dataset.GetGeoTransform(geoTransform);

            // Координаты углов в системе растра
            double xUL = geoTransform[0];
            double yUL = geoTransform[3];
            double xUR = xUL + width * geoTransform[1];
            double yUR = yUL;
            double xLR = xUR;
            double yLR = yUL + height * geoTransform[5]; // geoTransform[5] < 0
            double xLL = xUL;
            double yLL = yLR;

            // Преобразуем в WGS84
            var coords = new[]
            {
                TransformCoord(xUL, yUL, rasterSrs, wgs84Srs),
                TransformCoord(xUR, yUR, rasterSrs, wgs84Srs),
                TransformCoord(xLR, yLR, rasterSrs, wgs84Srs),
                TransformCoord(xLL, yLL, rasterSrs, wgs84Srs),
                TransformCoord(xUL, yUL, rasterSrs, wgs84Srs) // замыкаем полигон
            };

            var points = new List<Coordinate>();
            foreach (var (lon, lat) in coords)
            {
                points.Add(new Coordinate(lon, lat));
            }
            var ring = new LinearRing(points.ToArray());
            return new NetTopologySuite.Geometries.Polygon(ring);
        }

        private static SixLabors.ImageSharp.PointF[] TransformToPixelCoords(Coordinate[] coords, double[] invGeoTransform, int yOff, int imgHeight)
        {
            var points = new SixLabors.ImageSharp.PointF[coords.Length];
            for (int i = 0; i < coords.Length; i++)
            {
                Gdal.ApplyGeoTransform(invGeoTransform, coords[i].X, coords[i].Y, out double px, out double py);
                float x = (float)(px - 0); // xOff уже учтён в обрезке, но координаты относительно обрезанного изображения
                float y = (float)(imgHeight - (py - yOff)); // переворачиваем Y (GDAL: верхний левый угол = (0,0), SVG: верхний левый = (0,0), но Y растёт вниз)
                points[i] = new SixLabors.ImageSharp.PointF(x, y);
            }
            return points;
        }


        public static List<NetTopologySuite.Geometries.Point> GetPixelCoordinates(Dataset dataset)
        {
            var result = new List<NetTopologySuite.Geometries.Point>();
            if (dataset == null)
            {
                return result;
            }

            int width = dataset.RasterXSize;
            int height = dataset.RasterYSize;

            double[] geoTransform = new double[6];
            dataset.GetGeoTransform(geoTransform);

            string wkt = dataset.GetProjection();
            var rasterSrs = new SpatialReference(wkt);
            var wgs84Srs = new SpatialReference(string.Empty);
            wgs84Srs.SetWellKnownGeogCS("WGS84");

            var coordinates = new List<(double Lon, double Lat)>(width * height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Центр пикселя в координатах растра
                    double pixelX = geoTransform[0] + (x + 0.5) * geoTransform[1] + (y + 0.5) * geoTransform[2];
                    double pixelY = geoTransform[3] + (x + 0.5) * geoTransform[4] + (y + 0.5) * geoTransform[5];

                    // Преобразуем в WGS84
                    var (lon, lat) = TransformCoord(pixelX, pixelY, rasterSrs, wgs84Srs);
                    coordinates.Add((lon, lat));
                }
            }

            var points = new List<Coordinate>();
            foreach (var item in coordinates)
            {
                result.Add(new NetTopologySuite.Geometries.Point(new Coordinate(item.Lon, item.Lat)));
            }

            return result;
        }
    }

    public class TiffGenerationResult
    {
        public string GrayTiffLocalPath { get; set; }
        public string GrayTiffFileName { get; set; }
        public string GrayTiffBucket { get; set; }

        public string ColorTiffLocalPath { get; set; }
        public string ColorTiffFileName { get; set; }
        public string ColorTiffBucket { get; set; }

        public string ProductId { get; set; }
        public Guid SatelliteProductId { get; set; }
        public DateTime OriginDate { get; set; }
    }

    public class DbGenerationResult
    {
        public List<ArviPoint> ArviPoints { get; set; }
        public List<EviPoint> EviPoints { get; set; }
        public List<GndviPoint> GndviPoints { get; set; }
        public List<MndwiPoint> MndwiPoints { get; set; }
        public List<NdmiPoint> NdmiPoints { get; set; }
        public List<NdviPoint> NdviPoints { get; set; }
        public List<NdwiPoint> NdwiPoints { get; set; }
        public List<OrviPoint> OrviPoints { get; set; }
        public List<OsaviPoint> OsaviPoints { get; set; }
        public Guid SatelliteProductId { get; set; }
        public DateTime OriginDate { get; set; }
        public string ProductId { get; set; }

    }
}

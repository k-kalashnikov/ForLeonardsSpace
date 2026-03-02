using OSGeo.GDAL;
using System.Runtime.InteropServices;

namespace Masofa.Web.Monolith.Services
{
    public sealed class GdalInitializer
    {
        private static bool _initialized;
        private static readonly object _lock = new();

        public static void Initialize()
        {
            lock (_lock)
            {
                if (_initialized) return;

                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var rid = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win-x64"
                        : RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux-x64"
                        : throw new PlatformNotSupportedException();
                var runtimes = Path.Combine(baseDir, "runtimes", rid, "native");
                var gdalData = Path.Combine(runtimes, "gdal-data");
                var projLib = Path.Combine(runtimes, "maxrev.gdal.core.libshared");

                Environment.SetEnvironmentVariable("GDAL_DATA", gdalData);
                Environment.SetEnvironmentVariable("PROJ_LIB", projLib);

                Gdal.AllRegister();

                _initialized = true;
            }
        }

        public Driver GetDriverByName(string name) => Gdal.GetDriverByName(name);

        public Dataset Open(string utf8_path, Access eAccess) => Gdal.Open(utf8_path, eAccess);
    }
}

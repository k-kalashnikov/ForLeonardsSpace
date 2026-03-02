namespace Masofa.Common.Enums
{
    /// <summary>
    /// Represents the supported coordinate systems for geospatial data.
    /// </summary>
    /// <remarks>This enumeration defines commonly used coordinate systems for mapping and geospatial
    /// applications: <list type="bullet"> <item> <term><see cref="Epsg4326Wgs84"/></term> <description>The WGS 84
    /// coordinate system (EPSG:4326), which uses latitude and longitude in degrees.</description> </item> <item>
    /// <term><see cref="Epsg3857WebMercator"/></term> <description>The Web Mercator coordinate system (EPSG:3857),
    /// commonly used in web mapping applications.</description> </item> </list></remarks>
    public enum CoordinateSystemEnum
    {
        Epsg4326Wgs84,
        Epsg3857WebMercator
    }
}

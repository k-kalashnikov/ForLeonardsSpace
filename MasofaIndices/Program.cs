//using OSGeo.GDAL;
using Masofa.Common.Models.Satellite;
using Masofa.Common.Models.Satellite.Indices;
using Masofa.Common.Models.Satellite.Sentinel;
using Masofa.Common.Models.Tiles;
using Masofa.DataAccess;
using MasofaIndices.Sentinel;
using MaxRev.Gdal.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;
using OSGeo.GDAL;
using OSGeo.OSR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace MasofaIndices
{
    class Program
    {
        public static double EPS = 1e-8;
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");

                    services.AddDbContext<MasofaCropMonitoringDbContext>(options =>
                        options.UseNpgsql("Host=185.100.234.107;Port=20010;Database=agrosence-crop-monitoring-prod;User ID=emin_mov;Password=Q2vX8zN3bT6y;Command Timeout=300", o => o.UseNetTopologySuite()));

                    services.AddDbContext<MasofaIndicesDbContext>(options =>
                        options.UseNpgsql("Host=185.100.234.107;Port=20010;Database=agrosence-indices-prod;User ID=emin_mov;Password=Q2vX8zN3bT6y", o => o.UseNetTopologySuite()));

                    services.AddSingleton<Masofa.Web.Monolith.Services.GdalInitializer>();
                    services.AddTransient<IndexProcessor>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .Build();

            var processor = host.Services.GetRequiredService<IndexProcessor>();
            await processor.ProcessAsync();
        }

        //static async Task Main(string[] args)
        //{
        //    try
        //    {
        //        string xmlPath = "C:\\Users\\k-kal\\Downloads\\00665d12-3feb-4703-a147-58a387200f03\\S2B_MSIL1C_20250604T060629_N0511_R134_T42SWF_20250604T070121.SAFE\\INSPIRE.xml";
        //        if (!File.Exists(xmlPath))
        //        {
        //            Console.WriteLine("Файл не найден!");
        //            return;
        //        }

        //        // 1. Читаем как строку
        //        string xmlContent = File.ReadAllText(xmlPath);

        //        // 2. Упрощаем
        //        string simplifiedXml = SimplifyIso19115Xml(xmlContent);
        //        string clearFilePath = Path.Combine(Path.GetDirectoryName(xmlPath), "debug_simplified.xml");
        //        File.WriteAllText(clearFilePath, simplifiedXml);
        //        Console.WriteLine("Упрощённый XML сохранён в debug_simplified.xml");

        //        string xml = File.ReadAllText(clearFilePath);
        //        var info = ParseMetadataFromXml(xml);

        //        Console.WriteLine($"Title: {info.FileIdentifier}");
        //        Console.WriteLine($"Email: {info.Email}");
        //        Console.WriteLine($"West: {info.WestBoundLongitude}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //        Console.WriteLine($"Error: {ex.InnerException?.Message}");
        //    }
        //}

        //public static string SimplifyIso19115Xml(string xmlContent)
        //{
        //    // 1. Удаляем ВСЕ xmlns и xsi атрибуты (включая их значения), но сохраняем переносы
        //    string result = System.Text.RegularExpressions.Regex.Replace(
        //        xmlContent,
        //        @"\s+(xmlns|xsi)(:\w+)?\s*=\s*""[^""]*""",
        //        "",
        //        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline
        //    );

        //    // 2. Удаляем остатки xsi:schemaLocation и подобные (на случай, если остались)
        //    result = System.Text.RegularExpressions.Regex.Replace(
        //        result,
        //        @"\s+xsi:\w+\s*=\s*""[^""]*""",
        //        "",
        //        System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline
        //    );

        //    // 3. Убираем префиксы у тегов: <gmd:fileIdentifier> → <fileIdentifier>
        //    result = System.Text.RegularExpressions.Regex.Replace(
        //        result,
        //        @"<(\w+):(\w+)",
        //        "<$2",
        //        System.Text.RegularExpressions.RegexOptions.Multiline
        //    );

        //    result = System.Text.RegularExpressions.Regex.Replace(
        //        result,
        //        @"</(\w+):(\w+)",
        //        "</$2",
        //        System.Text.RegularExpressions.RegexOptions.Multiline
        //    );

        //    // 4. Заменяем gco-обёртки на чистый текст (осторожно: только если внутри нет тегов)
        //    result = System.Text.RegularExpressions.Regex.Replace(
        //        result,
        //        @"<CharacterString>([^<]*)</CharacterString>",
        //        "$1",
        //        System.Text.RegularExpressions.RegexOptions.Multiline
        //    );
        //    result = System.Text.RegularExpressions.Regex.Replace(result, @"<Date>([^<]*)</Date>", "$1");
        //    result = System.Text.RegularExpressions.Regex.Replace(result, @"<Decimal>([^<]*)</Decimal>", "$1");
        //    result = System.Text.RegularExpressions.Regex.Replace(result, @"<Integer>([^<]*)</Integer>", "$1");
        //    result = System.Text.RegularExpressions.Regex.Replace(result, @"<Boolean>([^<]*)</Boolean>", "$1");

        //    // 5. Убираем ДВОЙНЫЕ пробелы/переносы ТОЛЬКО внутри тегов, но не между ними
        //    // Например: <MD_Metadata   > → <MD_Metadata>
        //    result = System.Text.RegularExpressions.Regex.Replace(
        //        result,
        //        @"<(\w+)\s+>",
        //        "<$1>",
        //        System.Text.RegularExpressions.RegexOptions.Multiline
        //    );

        //    // 6. НЕ удаляем переносы между тегами! (раньше было: @">\s+<" → "><")
        //    // Теперь оставляем как есть — форматирование сохранится

        //    string pattern = @"(<TimePeriod)\s+gml:id=""[^""]*""(\s*>)";
        //    string replacement = "$1$2";
        //    result = Regex.Replace(result, pattern, replacement);

        //    return result;
        //}

        //public static SentinelInspireMetadata ParseMetadataFromXml(string xmlContent)
        //{
        //    var doc = XDocument.Parse(xmlContent);
        //    var ns = doc.Root?.GetDefaultNamespace() ?? XNamespace.None;

        //    var metadata = new SentinelInspireMetadata();

        //    // Простые текстовые поля
        //    metadata.FileIdentifier = doc.Root?.Element(ns + "fileIdentifier")?.Value.Trim();
        //    metadata.LanguageCode = doc.Root?.Element(ns + "language")?.Element(ns + "LanguageCode")?.Value.Trim();
        //    metadata.CharacterSetCode = doc.Root?.Element(ns + "characterSet")?.Element(ns + "MD_CharacterSetCode")?.Value.Trim();
        //    metadata.HierarchyLevelCode = doc.Root?.Element(ns + "hierarchyLevel")?.Element(ns + "MD_ScopeCode")?.Value.Trim();
        //    metadata.MetadataStandardName = doc.Root?.Element(ns + "metadataStandardName")?.Value.Trim();
        //    metadata.MetadataStandardVersion = doc.Root?.Element(ns + "metadataStandardVersion")?.Value.Trim();

        //    // Дата
        //    if (DateTime.TryParse(doc.Root?.Element(ns + "dateStamp")?.Value.Trim(), out var dateStamp))
        //        metadata.DateStamp = dateStamp;

        //    // Contact (берём первый, если несколько)
        //    var contact = doc.Root?.Element(ns + "contact")?.Element(ns + "CI_ResponsibleParty");
        //    metadata.OrganisationName = contact?.Element(ns + "organisationName")?.Value.Trim();
        //    metadata.Email = contact?.Element(ns + "contactInfo")
        //                             ?.Element(ns + "CI_Contact")
        //                             ?.Element(ns + "address")
        //                             ?.Element(ns + "CI_Address")
        //                             ?.Element(ns + "electronicMailAddress")?.Value.Trim();
        //    metadata.RoleCode = contact?.Element(ns + "role")
        //                               ?.Element(ns + "CI_RoleCode")?.Value.Trim();

        //    // Reference System
        //    var rsIdentifier = doc.Root?.Element(ns + "referenceSystemInfo")
        //                                 ?.Element(ns + "MD_ReferenceSystem")
        //                                 ?.Element(ns + "referenceSystemIdentifier")
        //                                 ?.Element(ns + "RS_Identifier");
        //    metadata.ReferenceSystemCode = rsIdentifier?.Element(ns + "code")?.Value.Trim();
        //    metadata.ReferenceSystemCodeSpace = rsIdentifier?.Element(ns + "codeSpace")?.Value.Trim();

        //    // Geographic Bounding Box
        //    var bbox = doc.Root?.Element(ns + "identificationInfo")
        //                         ?.Element(ns + "MD_DataIdentification")
        //                         ?.Element(ns + "extent")
        //                         ?.Element(ns + "EX_Extent")
        //                         ?.Element(ns + "geographicElement")
        //                         ?.Element(ns + "EX_GeographicBoundingBox");

        //    if (bbox != null)
        //    {
        //        decimal.TryParse(bbox.Element(ns + "westBoundLongitude")?.Value.Trim(), out var west);
        //        decimal.TryParse(bbox.Element(ns + "eastBoundLongitude")?.Value.Trim(), out var east);
        //        decimal.TryParse(bbox.Element(ns + "southBoundLatitude")?.Value.Trim(), out var south);
        //        decimal.TryParse(bbox.Element(ns + "northBoundLatitude")?.Value.Trim(), out var north);

        //        metadata.WestBoundLongitude = west;
        //        metadata.EastBoundLongitude = east;
        //        metadata.SouthBoundLatitude = south;
        //        metadata.NorthBoundLatitude = north;
        //    }

        //    return metadata;
        //}
    }
}

using Masofa.Common.Models.SystemCrical;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace Masofa.Cli.DevopsUtil.Commands.CodeGenerators
{
    [BaseCommand("MarkDownFormatingCommand", "MarkDownFormatingCommand")]
    public class MarkDownFormatingCommand : IBaseCommand
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Execute()
        {
            Console.WriteLine("Pls enter ZIP Archive file FULLPATH");
            var zipFilePath = Console.ReadLine();
            var result = ProcessMarkdownZipToHybridMarkdown(zipFilePath);
            Console.WriteLine("Pls enter result file FULLPATH");
            var resultFilePath = Console.ReadLine();
            if (File.Exists(resultFilePath))
            {
                File.Delete(resultFilePath);
            }
            File.WriteAllText(resultFilePath, result, Encoding.UTF8);
            Console.WriteLine("Done");
            return Task.CompletedTask;

        }

        public static string ProcessMarkdownZipToHybridMarkdown(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("ZIP file not found.", zipFilePath);
            }

            string markdownContent;
            using (var archive = ZipFile.OpenRead(zipFilePath))
            {
                var mdEntry = archive.Entries.FirstOrDefault(e => e.Name.EndsWith(".md", StringComparison.OrdinalIgnoreCase) || e.Name.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase));

                if (mdEntry == null)
                {
                    throw new InvalidOperationException("No Markdown file found in the ZIP archive.");
                }

                using (var reader = new StreamReader(mdEntry.Open(), Encoding.UTF8))
                {
                    markdownContent = reader.ReadToEnd();
                }

                // 1. Заменить изображения на <img src="base64">
                markdownContent = ReplaceMarkdownImagesWithBase64Html(markdownContent, archive);

                // 2. Заменить Markdown-таблицы на стилизованные HTML-таблицы
                markdownContent = ReplaceMarkdownTables(markdownContent);
            }

            return markdownContent;
        }

        // Замена ![](file.png) → <img src="data:...base64">
        private static string ReplaceMarkdownImagesWithBase64Html(string markdown, ZipArchive archive)
        {
            var regex = new Regex(@"\!\[\]\((.+)\)");
            var match = regex.Match(markdown);
            foreach (System.Text.RegularExpressions.Group g in match.Groups)
            {
                Console.WriteLine(g.Value);
            }
            return regex.Replace(markdown, match =>
            {
                string fileName = match.Groups[1].Value.Trim();
                var entry = archive.Entries.FirstOrDefault(e =>
                    string.Equals(Path.GetFileName(e.FullName), Path.GetFileName(fileName),
                                  StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                    return match.Value; // оставить как есть

                string ext = Path.GetExtension(fileName).ToLowerInvariant();
                string mime = ext switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    ".bmp" => "image/bmp",
                    ".svg" => "image/svg+xml",
                    _ => "image/png"
                };

                byte[] bytes;
                using (var ms = new MemoryStream())
                {
                    entry.Open().CopyTo(ms);
                    bytes = ms.ToArray();
                }

                string base64 = Convert.ToBase64String(bytes);
                return $"<img src=\"data:{mime};base64,{base64}\" alt=\"{Path.GetFileName(fileName)}\" />";
            });
        }

        // Замена Markdown-таблиц на HTML с инлайновыми стилями
        private static string ReplaceMarkdownTables(string markdown)
        {
            // Регулярное выражение для Markdown-таблиц
            // Поддерживает многострочные таблицы
            var tablePattern = @"
            ^\|.*\|\s*$                     # строка, начинающаяся и заканчивающаяся на |
            (?:\n^\|.*\|\s*$)*              # последующие строки таблицы
        ";

            var regex = new Regex(tablePattern, RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            return regex.Replace(markdown, match =>
            {
                var tableLines = match.Value.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None)
                                            .Where(l => !string.IsNullOrWhiteSpace(l))
                                            .ToArray();

                if (tableLines.Length < 2) return match.Value;

                var headers = ParseTableRow(tableLines[0]);
                var separator = tableLines[1];
                var rows = tableLines.Skip(2).Select(ParseTableRow).ToArray();

                // Начало таблицы
                var html = new StringBuilder();
                html.Append("<table style=\"width: 100%; border-collapse: collapse; margin: 1em 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; font-size: 14px;\">");

                // Заголовок
                html.Append("<thead><tr style=\"background-color: #212529; color: #fff; text-align: left;\">");
                foreach (var cell in headers)
                {
                    html.Append($"<th style=\"padding: 12px 16px; border: 1px solid #495057;\">{EscapeHtml(cell)}</th>");
                }
                html.Append("</tr></thead>");

                // Тело
                html.Append("<tbody style=\"color: #000;\">");
                for (int i = 0; i < rows.Length; i++)
                {
                    var bgColor = i % 2 == 0 ? "#fff" : "#f8f9fa";
                    html.Append($"<tr style=\"background-color: {bgColor}; border-bottom: 1px solid #dee2e6;\">");
                    foreach (var cell in rows[i])
                    {
                        html.Append($"<td style=\"padding: 12px 16px; border: 1px solid #dee2e6;\">{EscapeHtml(cell)}</td>");
                    }
                    html.Append("</tr>");
                }
                html.Append("</tbody></table>");

                return html.ToString();
            });
        }

        private static string[] ParseTableRow(string line)
        {
            // Убираем начальный и конечный |
            var trimmed = line.Trim();
            if (trimmed.StartsWith("|")) trimmed = trimmed.Substring(1);
            if (trimmed.EndsWith("|")) trimmed = trimmed.Substring(0, trimmed.Length - 1);
            return trimmed.Split('|').Select(cell => cell.Trim()).ToArray();
        }

        private static string EscapeHtml(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace("\"", "&quot;")
                    .Replace("'", "&#39;");
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}

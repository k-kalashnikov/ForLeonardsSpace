using Masofa.Common.Models;
using Masofa.Common.Models.SystemCrical;
using Masofa.Common.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Xml.Linq;

namespace Masofa.Cli.DevopsUtil.Commands.CodeGenerators
{
    [BaseCommand("Generate Log Resource file", "Генератор списка сообщений логов")]
    public class GenerateLogMessageListResourceCommand : IBaseCommand
    {
        private List<string> DirectoryPath = new List<string>()
        {
            "Masofa.BusinessLogic",
            "Masofa.Web.Monolith"
        };


        public async Task Execute()
        {
            var classStruct = GenerateClassStruct();
            var classText = GenerateClass(classStruct);
            Console.WriteLine($"Enter pls filepath");
            var filePath = Console.ReadLine();
            var oldFiles = Directory.GetFiles(filePath);
            foreach (var item in oldFiles)
            {
                if (item.Contains("LogMessageResource_"))
                {
                    File.Delete(item);
                }
            }
            File.WriteAllText(Path.Combine(filePath, $"LogMessageResource_{DateTime.Now.ToString("yyyy-MM-dd-hh-mm")}.cs"), classText);
            await ImplimentLogMessagesAsync(classStruct);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
        }

        private async Task ImplimentLogMessagesAsync(LogMessageResource logMessageResource)
        {
            Console.WriteLine("Enter Pls rootDirectory");
            var rootDirectory = Console.ReadLine();
            if (!Directory.Exists(rootDirectory))
            {
                Console.WriteLine($"Directory {rootDirectory} is not exist");
                return;
            }


            foreach (var directory in DirectoryPath)
            {
                var projectDirectory = Path.Combine(rootDirectory, directory);
                var files = Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    var result = new List<LogMessageResultItem>();

                    var sourceText = await File.ReadAllTextAsync(file);
                    var syntaxTree = CSharpSyntaxTree.ParseText(sourceText);
                    var root = syntaxTree.GetRoot();

                    // Получаем семантику (опционально, но полезно для точности)
                    var compilation = CSharpCompilation.Create("Temp")
                        .AddSyntaxTrees(syntaxTree)
                        .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

                    var model = compilation.GetSemanticModel(syntaxTree);

                    // ... после получения syntaxTree и model ...

                    var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();
                    foreach (var invocation in invocations)
                    {
                        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                        {
                            var methodName = memberAccess.Name.Identifier.Text;
                            if (methodName.StartsWith("Log") && (methodName.EndsWith("Async") || methodName == "LogAsync"))
                            {
                                string logLevel = GetLogLevelFromMethodName(methodName);
                                var args = invocation.ArgumentList.Arguments;
                                if (args.Count == 0) continue;

                                var methodNode = invocation.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
                                if (methodNode == null) continue;

                                var classNode = methodNode.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
                                var namespaceNode = methodNode.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

                                string ns = namespaceNode?.Name.ToString() ?? "Global";
                                string className = classNode?.Identifier.Text ?? "UnknownClass";
                                string methodNameFull = methodNode.Identifier.Text;
                                string path = $"{ns}.{className}.{methodNameFull}";

                                var firstArgExpression = args[0].Expression;
                                var firstArgText = firstArgExpression.ToFullString();

                                // если первый аргумент уже LogMessageResource.* – считаем, что лог локализован
                                if (firstArgText.Contains("LogMessageResource."))
                                {
                                    continue;
                                }

                                // 🔥 Разрешаем значение аргумента
                                string originString = ResolveExpressionValue(args[0].Expression, model, methodNode.Body ?? (SyntaxNode)methodNode);
                                string clearString = originString.Replace("$", string.Empty).Replace("\"", string.Empty);
                                var neededLog = logMessageResource.Items.FirstOrDefault(lmr => lmr.PrimePatterns.Any(pp => pp == clearString));
                                if (neededLog == null)
                                {
                                    Console.WriteLine($"Log with text = {originString} not found");
                                    continue;
                                }
                                var tempParams = string.Join(",", neededLog.MessageVariables.Select(mv => mv.ValueName));
                                sourceText = sourceText.Replace(originString, $"LogMessageResource.{neededLog.MessageName}({tempParams})");

                                if (!sourceText.Contains("using Masofa.Common.Resources;"))
                                {
                                    sourceText = "using Masofa.Common.Resources;\n" + sourceText;
                                }

                                File.WriteAllText(file, sourceText);
                            }
                        }
                    }
                }
            }
        }

        private string GetLogLevelFromMethodName(string methodName)
        {
            if (methodName == "LogAsync") return "Custom"; // или определять из типа LogMessage

            var level = methodName.Replace("Log", "").Replace("Async", "");
            return level switch
            {
                "Information" => "Information",
                "Warning" => "Warning",
                "Error" => "Error",
                "Critical" => "Critical",
                "Debug" => "Debug",
                _ => "Unknown"
            };
        }

        private static string ExtractSummaryFromTrivia(SyntaxNode node)
        {
            if (node == null) return "No summary";

            var leadingTrivia = node.GetLeadingTrivia();
            foreach (var trivia in leadingTrivia)
            {
                if (trivia.IsKind(SyntaxKind.SingleLineDocumentationCommentTrivia) ||
                    trivia.IsKind(SyntaxKind.MultiLineDocumentationCommentTrivia))
                {
                    var docComment = trivia.ToFullString();
                    var startTag = "<summary>";
                    var endTag = "</summary>";
                    var start = docComment.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
                    if (start >= 0)
                    {
                        start += startTag.Length;
                        var end = docComment.IndexOf(endTag, start, StringComparison.OrdinalIgnoreCase);
                        if (end > start)
                        {
                            var summary = docComment.Substring(start, end - start).Trim();
                            return System.Text.RegularExpressions.Regex.Replace(summary, @"\s+", " ");
                        }
                    }
                }
            }
            return "No summary";
        }

        private void Output(List<LogMessageResultItem> logMessagesResult) 
        {
            if (logMessagesResult.Count == 0)
            {
                Console.WriteLine("No log messages found.");
                return;
            }

            // Заголовок таблицы
            var markdownLines = new List<string>
            {
                "| 🐉 | Level | Path | Context | Message | File |",
                "|-----|-------|------|---------|---------|------|"
            };

            // Экранируем символы Markdown в ячейках
            string EscapeMarkdown(string input)
            {
                if (string.IsNullOrEmpty(input)) return "";
                return input
                    .Replace("|", "\\|")
                    .Replace("\n", " ")
                    .Replace("\r", "")
                    .Trim();
            }
            var index = 1;
            foreach (var item in logMessagesResult)
            {
                var line = $"| {EscapeMarkdown(index.ToString())} " +
                           $"| {EscapeMarkdown(item.LogLevel)} " +
                           $"| {EscapeMarkdown(item.Path)} " +
                           $"| {EscapeMarkdown(item.Context)} " +
                           $"| {EscapeMarkdown(item.OriginString)} " +
                           $"| `{Path.GetFileName(item.FilePath)}` |";
                markdownLines.Add(line);
                index++;
            }

            var markdownTable = string.Join(Environment.NewLine, markdownLines);

            // Опционально: сохранить в файл
            File.WriteAllText("log-messages.md", markdownTable);
            Console.WriteLine("\n✅ Markdown table saved to 'log-messages.md'");
        }
        private LogMessageResource GenerateClassStruct()
        {
            Console.WriteLine("Enter pls json file path");
            var jsonPath = Console.ReadLine();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<LogMessageResource>(File.ReadAllText(jsonPath) ?? string.Empty) ?? new LogMessageResource();
        }

        private string GenerateClass(LogMessageResource logMessagesResult)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine("// ⚠️ Этот файл сгенерирован автоматически. Не редактируй вручную.");
            sb.AppendLine();
            sb.AppendLine("using Masofa.Common.Models;");
            sb.AppendLine();

            sb.AppendLine();
            sb.AppendLine("namespace Masofa.Common.Resources");
            sb.AppendLine("{");
            sb.AppendLine("     /// <summary>");
            sb.AppendLine("     /// Файл содержит метод для получения локализированного лога");
            sb.AppendLine("     /// </summary>");
            sb.AppendLine("     public static class LogMessageResource");// для поддержки partial
            sb.AppendLine("     {");

            foreach (var item in logMessagesResult.Items)
            {
                var tempParams = string.Join(",", item.MessageVariables.Select(mv => $"{mv.ValueTypeFullName} {mv.ValueName}"));
                sb.AppendLine($"          public static LocalizationString {item.MessageName}({tempParams})");
                sb.AppendLine("          {");
                sb.AppendLine("               return new Dictionary<string, string>()");
                sb.AppendLine("               {");
                sb.AppendLine("                    {");
                sb.AppendLine($"                         \"ru-RU\", $\"{item.MessagePattern}\"");
                sb.AppendLine("                    },");
                sb.AppendLine("                    {");
                sb.AppendLine($"                         \"en-US\", $\"{item.MessagePattern}\"");
                sb.AppendLine("                    },");
                sb.AppendLine("                    {");
                sb.AppendLine($"                         \"uz-Latn-UZ\", $\"{item.MessagePattern}\"");
                sb.AppendLine("                    },");
                sb.AppendLine("               };");
                sb.AppendLine("          }");
            }

            sb.AppendLine("     }");
            sb.AppendLine("}");
            sb.AppendLine();


            return sb.ToString();
        }

        private static string ResolveExpressionValue(
            ExpressionSyntax expression,
            SemanticModel model,
            SyntaxNode methodBody)
        {
            if (expression is LiteralExpressionSyntax literal)
                return literal.Token.ValueText;

            if (expression is IdentifierNameSyntax identifier)
            {
                var symbol = model.GetSymbolInfo(identifier).Symbol;
                if (symbol is ILocalSymbol local)
                {
                    var localDeclaration = methodBody.DescendantNodes()
                        .OfType<VariableDeclaratorSyntax>()
                        .FirstOrDefault(v => v.Identifier.Text == local.Name);

                    if (localDeclaration?.Initializer?.Value != null)
                    {
                        return localDeclaration.Initializer.Value.ToFullString().Trim();
                    }
                }
                else if (symbol is IParameterSymbol param)
                {
                    return $"[Parameter: {param.Name}]";
                }
                else if (symbol is IFieldSymbol field)
                {
                    return $"[Field: {field.Name}]";
                }

                return $"[Variable: {identifier.Identifier.Text}]";
            }

            // Если это вызов метода, конкатенация и т.д. — просто вернём исходный код
            return expression.ToFullString().Trim();
        }
    }

    public class LogMessageResource //Корневой объект JSON
    {
        public List<LogMessageResourceItem> Items { get; set; } = new List<LogMessageResourceItem>();
    }

    public class LogMessageResourceItem //Объект уникального сообщения
    {
        public string MessageName { get; set; } //Имя Паттерна сообщения, должен быть такое чтобы можно использовать как имя переменной: Каждое слово с большой буквы без пробелов, на английском языке, локаничное
        public string MessagePattern { get; set; } //Паттерн строки этого сообщения
        public List<LogMessageResourceItemVariable> MessageVariables { get; set; } = new List<LogMessageResourceItemVariable>(); //Список переменных использующих в этом сообщении
        public Dictionary<string, List<string>> UsageMethods { get; set; } = new Dictionary<string, List<string>>(); //Список методов в которых используется это сообщение, где ключ Dictionary - string - это FullNameType класса, а значение List<string> - это список методов
        public LogLevelType LogLevelType { get; set; } //уровень сообщения
        public List<string> PrimePatterns { get; set; } = new List<string>(); //изначальный список паттернов, которые ты брал и привёл к уникальному, нужен, чтобы я мог потом найти, что на что было заменено
    }

    public class LogMessageResourceItemVariable //Объект описывающий переменную для шаблона
    {
        public string ValueName { set; get; } //имя переменной
        public string ValueTypeFullName { set; get; } //тип переменной
    }
}

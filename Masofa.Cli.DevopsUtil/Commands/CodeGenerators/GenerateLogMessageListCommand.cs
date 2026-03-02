using Masofa.Common.Models.SystemCrical;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Xml.Linq;

namespace Masofa.Cli.DevopsUtil.Commands.CodeGenerators
{
    [BaseCommand("Generate Log List", "Генератор списка сообщений логов")]
    public class GenerateLogMessageListCommand : IBaseCommand
    {
        private List<string> DirectoryPath = new List<string>()
        {
            "Masofa.BusinessLogic",
            "Masofa.Web.Monolith"
        };


        public async Task Execute()
        {
            Console.WriteLine("Enter Pls rootDirectory");
            var rootDirectory = Console.ReadLine();
            if (!Directory.Exists(rootDirectory))
            {
                Console.WriteLine($"Directory {rootDirectory} is not exist");
                return;
            }

            var logMessagesResult = new List<LogMessageResultItem>();

            foreach (var directory in DirectoryPath)
            {
                var projectDirectory = Path.Combine(rootDirectory, directory);
                var files = Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);

                foreach (var file in files)
                {
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

                                string methodSummary = ExtractSummaryFromTrivia(methodNode);
                                string context = methodSummary != "No summary"
                                    ? methodSummary
                                    : (classNode != null ? ExtractSummaryFromTrivia(classNode) : "No summary");

                                // 🔥 Разрешаем значение аргумента
                                string originString = ResolveExpressionValue(args[0].Expression, model, methodNode.Body ?? (SyntaxNode)methodNode);

                                logMessagesResult.Add(new LogMessageResultItem
                                {
                                    OriginString = originString,
                                    Context = context,
                                    Path = path,
                                    LogLevel = logLevel,
                                    FilePath = file
                                });
                            }
                        }
                    }
                }
            }

            Output(logMessagesResult);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task Execute(string[] args)
        {
            throw new NotImplementedException();
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
            Console.WriteLine("Enter Pls outputDirectory");
            var outputDirectory = Console.ReadLine();
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
            Console.WriteLine(markdownTable);

            // Опционально: сохранить в файл
            File.WriteAllText(Path.Combine(outputDirectory, $"log-messages-{DateTime.Now.ToString("yyyy-MM-dd-hh-mm")}.md"), markdownTable);
            Console.WriteLine("\n✅ Markdown table saved to 'log-messages.md'");
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

    public class  LogMessageResultItem
    {
        public string OriginString { get; set; }          // Аргумент message
        public string Context { get; set; }               // <summary> из XML-doc метода
        public string Path { get; set; }                  // Namespace.Class.Method
        public string LogLevel { get; set; }              // "Information", "Error" и т.д.
        public string FilePath { get; set; }              // Путь к файлу (для отладки)
    }
}

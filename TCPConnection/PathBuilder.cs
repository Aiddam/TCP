using Spectre.Console;

namespace TCPConnection
{
    public static class PathBuilder
    {
        public static string SelectSaveDirectory()
        {
            string path = Directory.GetCurrentDirectory();

            while (true)
            {
                AnsiConsole.Write(path + ">");
                string userInput = AnsiConsole.Ask<string>("").Trim();

                if (userInput.EndsWith(":"))
                {
                    if (DriveExists(userInput))
                    {
                        path = userInput + "\\";
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Папка не знайдена![/]");
                    }
                }
                else if (userInput.Equals(".."))
                {
                    path = GetParentPath(path);
                }
                else if (userInput.Equals("s folder", StringComparison.OrdinalIgnoreCase))
                {
                    path = SelectDirectory(path);
                }
                else if (userInput.Equals("s clear", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.Clear();
                }
                else if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    return string.Empty;
                }
                else if (userInput.Equals("select", StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }
                else
                {
                    path = NavigatePath(path, userInput);
                }
            }
        }

        public static string SelectFilePath()
        {
            string path = Directory.GetCurrentDirectory();

            while (true)
            {
                AnsiConsole.Write(path + ">");
                string userInput = AnsiConsole.Ask<string>("").Trim();

                if (userInput.EndsWith(":"))
                {
                    if (DriveExists(userInput))
                    {
                        path = userInput + "\\";
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Папка не знайдена![/]");
                    }
                }
                else if (userInput.Equals(".."))
                {
                    path = GetParentPath(path);
                }
                else if (userInput.Equals("s folder", StringComparison.OrdinalIgnoreCase))
                {
                    path = SelectDirectory(path);
                }
                else if (userInput.Equals("s file", StringComparison.OrdinalIgnoreCase))
                {
                    return SelectFile(path);
                }
                else if (userInput.Equals("s clear", StringComparison.OrdinalIgnoreCase))
                {
                    AnsiConsole.Clear();
                }
                else if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    return string.Empty;
                }
                else if (userInput.Equals("select", StringComparison.OrdinalIgnoreCase))
                {
                    return path;
                }
                else
                {
                    path = NavigatePath(path, userInput);
                }
            }
        }

        private static bool DriveExists(string drive)
        {
            return DriveInfo.GetDrives().Any(d => d.Name.StartsWith(drive));
        }

        private static string GetParentPath(string path)
        {
            if (path.Length > 3)
            {
                return Directory.GetParent(path)?.FullName ?? path;
            }

            AnsiConsole.MarkupLine("[red]Не має вище директорії.[/]");
            return path;
        }

        private static string SelectDirectory(string path)
        {
            string[] directories = Directory.GetDirectories(path);
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("[blue]Оберіть папку:[/]")
                .AddChoices(directories.Prepend(path).ToArray()));

            return selection;
        }

        private static string SelectFile(string path)
        {
            string[] files = Directory.GetFiles(path);
            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>().Title("[blue]Оберіть файл:[/]")
                .AddChoices(files.Prepend(path).ToArray()));

            return selection;
        }

        private static string NavigatePath(string currentPath, string userInput)
        {
            string newPath = Path.Combine(currentPath, userInput);

            if (Directory.Exists(newPath))
            {
                return newPath;
            }
            else if (File.Exists(newPath))
            {
                return newPath;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Шлях не знайдено![/]");
                return currentPath;
            }
        }
    }
}

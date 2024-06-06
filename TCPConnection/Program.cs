using Spectre.Console;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TCPConnection
{
    public class TCPConnection
    {
        private const int Port = 5000;
        private const int BufferSize = 16384;
        private const string DefaultIP = "192.168.31.70";

        public static async Task Main()
        {
            bool isServer = IsServer();
            if (isServer)
            {
                TcpListener server = new TcpListener(IPAddress.Any, Port);
                server.Start();
                using (TcpClient client = await server.AcceptTcpClientAsync())
                {
                    client.NoDelay = true;
                    while (true)
                    {
                        string choice = ChoisesForTheServer();
                        switch (choice)
                        {
                            case "Відпрвити файл":
                                AnsiConsole.Clear();
                                await TCPServer.SendFileAsync(client, BufferSize);
                                break;
                            case "Отримати файл":
                                AnsiConsole.MarkupLine("[underline green]Анлак([/]");
                                break;
                            case "Відправити повідомлення клієнту":
                                AnsiConsole.MarkupLine("[underline green]Анлак([/]");
                                break;
                            case "Закрити підключення":
                                server.Stop();
                                return;
                        }
                    }
                }
            } else {
                using (TcpClient client = new TcpClient())
                {
                    client.NoDelay = true;
                    var isDefaultIP = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[underline red]Виберіть айпі:[/]")
                            .AddChoices("Дефолт айпи", "Ввести свій")
                    );
                    if(isDefaultIP == "Дефолт айпи")
                    {
                        await client.ConnectAsync(DefaultIP, Port);
                    }
                    else
                    {
                        var IP = Console.ReadLine();
                        await client.ConnectAsync(IP, Port);
                    }
                    while (true)
                    {
                        string choice = ChoisesForTheClient();
                        switch (choice)
                        {
                            case "Отримати файл":
                                AnsiConsole.Clear();
                                await TCPClient.GetFileAsync(client, BufferSize);
                                break;
                            case "Відправити файл":
                                AnsiConsole.MarkupLine("[underline green]Анлак([/]");
                                break;
                            case "Відправити повідомлення серверу":
                                AnsiConsole.MarkupLine("[underline green]Анлак([/]");
                                break;
                            case "Закрити підключення":
                                client.Close();
                                return;
                        }
                    }
                }
            }
        }
        private static bool IsServer()
        {
            var serverOrClient = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[underline red]Виберіть роль:[/]")
                    .AddChoices("Сервер", "Клієнт")
            );

            return serverOrClient == "Сервер";
        }
        public static string ChoisesForTheServer()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[underline red]Виберіть команду:[/]")
                    .AddChoices("Відпрвити файл", "Отримати файл", "Відправити повідомлення клієнту", "Закрити підключення")
            );

            return choice;
        }
        public static string ChoisesForTheClient()
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[underline red]Виберіть команду:[/]")
                    .AddChoices("Отримати файл", "Відправити файл", "Відправити повідомлення серверу", "Закрити підключення")
            );

            return choice;
        }

        private static async Task HandleClientAsync(TcpListener server, string filePath, long fileSize)
        {
            using (TcpClient client = await server.AcceptTcpClientAsync())
            {
                Console.WriteLine("Клієнт підключився");

                using (NetworkStream networkStream = client.GetStream())
                {

                    byte[] sizeBufferOfFileLength = BitConverter.GetBytes(fileSize);
                    byte[] responseBuffer = new byte[1];
                    int responseRead = await networkStream.ReadAsync(responseBuffer, 0, 1);

                    if (responseRead > 0 && responseBuffer[0] == (byte)'1')
                    {
                        Console.WriteLine("Клієнт погодився на передачу файла");
                        byte[] buffer = new byte[BufferSize];

                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            int bytesRead;
                            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, BufferSize)) > 0)
                            {
                                await networkStream.WriteAsync(buffer, 0, bytesRead);
                            }
                        }
                        Console.WriteLine($"Файл {filePath} відправлено клієнту.");
                    }
                    else
                    {
                        Console.WriteLine("Клієнт відмовився від передачі файла.");
                    }
                }
            }
        }
    }
}
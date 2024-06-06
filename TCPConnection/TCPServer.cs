using Spectre.Console;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;

namespace TCPConnection
{
    public class TCPServer
    {
        public static async Task<bool> SendFileAsync(TcpClient client, int bufferSize)
        {
            try
            {
                client.NoDelay = true;
                string filePath = PathBuilder.SelectFilePath();

                if (!File.Exists(filePath))
                {
                    AnsiConsole.MarkupLine($"[red]Файл {filePath} не знайдено.[/]");
                    return false;
                }

                long fileSize = new FileInfo(filePath).Length;
                string fileName = Path.GetFileName(filePath);

                using NetworkStream networkStream = client.GetStream();
                byte[] lengthBuffer = BitConverter.GetBytes(fileName.Length);
                await networkStream.WriteAsync(lengthBuffer);

                byte[] fileNameBuffer = Encoding.UTF8.GetBytes(fileName);
                await networkStream.WriteAsync(fileNameBuffer);

                AnsiConsole.MarkupLine("[underline blue]Ім'я файла надіслано.[/]");

                byte[] sizeBuffer = BitConverter.GetBytes(fileSize);
                await networkStream.WriteAsync(sizeBuffer);

                AnsiConsole.MarkupLine("[underline blue]Розмір файла надіслано.[/]");

                byte[] responseBuffer = new byte[1];
                int responseRead = await networkStream.ReadAsync(responseBuffer.AsMemory(0, 1));
                if (responseRead > 0 && responseBuffer[0] == (byte)'1')
                {
                    using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    using GZipStream compressionStream = new(networkStream, CompressionMode.Compress);
                    byte[] buffer = new byte[bufferSize];
                    int bytesRead;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await compressionStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    }

                    AnsiConsole.MarkupLine($"[underline green]Файл {filePath} відправлено до клієнта.[/]");
                    return true;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Клієнт не підтвердив відправку.[/]");
                    return false;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
                return false;
            }
        }
    }
}

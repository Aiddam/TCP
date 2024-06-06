using Spectre.Console;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;

namespace TCPConnection
{
    public class TCPClient
    {
        public static async Task GetFileAsync(TcpClient client, int bufferSize)
        {
            try
            {
                client.NoDelay = true;
                using NetworkStream networkStream = client.GetStream();
                string receivedFileName = await ReceiveFileNameAsync(networkStream);


                byte[] sizeBuffer = new byte[sizeof(long)];
                await networkStream.ReadAsync(sizeBuffer.AsMemory(0, sizeof(long)));
                long totalSize = BitConverter.ToInt64(sizeBuffer, 0);
                byte[] confirmBuffer = { (byte)'1' };

                string saveDirectory = PathBuilder.SelectSaveDirectory();
                if (string.IsNullOrEmpty(saveDirectory))
                {
                    AnsiConsole.MarkupLine("[red]Не обрана директорія.[/]");
                    return;
                }

                string saveFilePath = Path.Combine(saveDirectory, receivedFileName);

                await networkStream.WriteAsync(confirmBuffer);

                using FileStream fileStream = new(saveFilePath, FileMode.Create);
                using GZipStream decompressionStream = new(networkStream, CompressionMode.Decompress);
                byte[] buffer = new byte[bufferSize];
                int bytesRead;

                AnsiConsole.Progress()
                    .Start(ctx =>
                    {
                        var task = ctx.AddTask("[green]Отримання файлу[/]", maxValue: totalSize);
                        while ((bytesRead = decompressionStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            task.Increment(bytesRead);
                            fileStream.Write(buffer, 0, bytesRead);
                        }
                    });

                AnsiConsole.MarkupLine($"[underline green]Файл {saveFilePath} отримано успішно.[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]An error occurred: {ex.Message}[/]");
            }
        }

        private static async Task<string> ReceiveFileNameAsync(NetworkStream networkStream)
        {
            byte[] sizeBuffer = new byte[4];
            await networkStream.ReadAsync(sizeBuffer);
            int fileNameLength = BitConverter.ToInt32(sizeBuffer, 0);

            byte[] fileNameBuffer = new byte[fileNameLength];
            await networkStream.ReadAsync(fileNameBuffer);

            return Encoding.UTF8.GetString(fileNameBuffer);
        }
    }
}

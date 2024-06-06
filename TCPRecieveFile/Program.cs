using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Spectre.Console;

public class TcpFileClient
{
    private const int BufferSize = 16384; // Збільшення розміру буфера

    public static async Task Main()
    {
        string serverIp = "192.168.31.136";
        int port = 5000;
        string receivedFileName = "received_file.zip";

        using (TcpClient client = new TcpClient())
        {
            client.NoDelay = true; // Увімкнення TCP_NODELAY
            await client.ConnectAsync(serverIp, port);

            using (NetworkStream networkStream = client.GetStream())
            {
                byte[] sizeBuffer = new byte[sizeof(long)];
                await networkStream.ReadAsync(sizeBuffer, 0, sizeof(long));

                long totalSize = BitConverter.ToInt64(sizeBuffer, 0);
                byte[] confirmBuffer = new byte[] { (byte)'1' };
                await networkStream.WriteAsync(confirmBuffer, 0, confirmBuffer.Length);

                using (FileStream fileStream = new FileStream(receivedFileName, FileMode.Create))
                {
                    byte[] buffer = new byte[BufferSize];
                    int bytesRead;

                    AnsiConsole.Progress()
                        .Start(ctx =>
                        {
                            var task = ctx.AddTask("Отримання файлу", maxValue: totalSize);
                            while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                task.Increment(bytesRead);
                                fileStream.Write(buffer, 0, bytesRead);
                            }
                        });
                }
            }
        }
    }
}
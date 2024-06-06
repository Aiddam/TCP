using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

public class TcpFileServer
{
    private const int Port = 5000;
    private const int BufferSize = 16384;

    public static async Task Main()
    {
        Console.WriteLine("Оберіть шлях до файла");
        string filePath = Console.ReadLine(); //D:\\444.zip

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Файл {filePath} не знайдено.");
            return;
        }

        long fileSize = new FileInfo(filePath).Length;
        TcpListener server = new TcpListener(IPAddress.Any, Port);
        server.Start();

        await Task.WhenAll(HandleClientAsync(server, filePath, fileSize));

        server.Stop();
        Console.WriteLine("end");
    }

    private static async Task HandleClientAsync(TcpListener server, string filePath, long fileSize)
    {
        using (TcpClient client = await server.AcceptTcpClientAsync())
        {
            Console.WriteLine("Клієнт підключився");

            using (NetworkStream networkStream = client.GetStream())
            {
                byte[] sizeBuffer = BitConverter.GetBytes(fileSize);
                await networkStream.WriteAsync(sizeBuffer, 0, sizeBuffer.Length);

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

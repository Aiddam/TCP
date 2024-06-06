using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    // UDP-сервер, який отримує повідомлення
    static async Task UdpServerAsync()
    {
        using (UdpClient udpServer = new UdpClient(9000))
        {
            while (true)
            {
                UdpReceiveResult receivedResult = await udpServer.ReceiveAsync();
                string message = Encoding.UTF8.GetString(receivedResult.Buffer);
                Console.WriteLine($"Отримано повідомлення від {receivedResult.RemoteEndPoint}: {message}");
                string mess = Console.ReadLine();
                byte[] responseBytes = Encoding.UTF8.GetBytes(mess);
                await udpServer.SendAsync(responseBytes, responseBytes.Length, receivedResult.RemoteEndPoint);
            }
        }
    }

    // UDP-клієнт, який відправляє повідомлення
    static async Task UdpClientAsync()
    {
        using (UdpClient udpClient = new UdpClient())
        {
            string message = "Привіт від клієнта!";
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await udpClient.SendAsync(buffer, buffer.Length, "localhost", 9000);
            Console.WriteLine("Повідомлення відправлено.");
        }
    }

    static async Task Main(string[] args)
    {
        // Запускаємо UDP-сервер
        Task serverTask = UdpServerAsync();

        // Затримка для надання часу серверу на старт
        await Task.Delay(1000);

        // Запускаємо UDP-клієнта

        // Зачекаємо, поки сервер не завершиться
        await serverTask;
    }
}

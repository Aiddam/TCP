using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpServer
{
    public static void Main()
    {
        // Прослуховування на порту 12345
        int port = 5000;
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"TCP сервер прослуховує на порту {port}");

        while (true)
        {
            // Очікування підключення
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Клієнт підключений");

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            // Прочитування та виведення отриманого повідомлення
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Отримано повідомлення: {receivedMessage}");

            client.Close();
        }
    }
}

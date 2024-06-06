using System;
using System.Net.Sockets;
using System.Text;

class TcpClientApp
{
    public static void Main(string[] args)
    {
        // IP-адреса і порт сервера
        string serverIp = "192.168.31.34"; // Змініть на IP-адресу вашого сервера
        int port = 5000;

        // Підключення до сервера
        TcpClient client = new TcpClient(serverIp, port);
        Console.WriteLine("Підключено до сервера");

        NetworkStream stream = client.GetStream();

        // Відправлення повідомлення
        string message = "Привіт, сервер!";
        byte[] data = Encoding.UTF8.GetBytes(message);
        stream.Write(data, 0, data.Length);

        Console.WriteLine("Повідомлення відправлено");

        client.Close();
    }
}

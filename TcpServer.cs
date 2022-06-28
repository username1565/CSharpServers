using System;
using System.Text;
using System.Net;      // потребуется
using System.Net.Sockets;    // потребуется
    class Program
    {
		public static Encoding encoding = Encoding.GetEncoding("cp866"); //cmd.exe -> chcp -> https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding.getencodings?view=net-6.0

        static void Main()
        {
            // устанавливаем IP-адрес сервера и номер порта 1234
            TcpListener server = new TcpListener(IPAddress.Any, 1234);  
            server.Start();  // запускаем сервер
            while (true)   // бесконечный цикл обслуживания клиентов
            {
                TcpClient client = server.AcceptTcpClient();  // ожидаем подключение клиента
                NetworkStream ns = client.GetStream(); // для получения и отправки сообщений
                byte[] hello = new byte[100];   // любое сообщение должно быть сериализовано
                hello = encoding.GetBytes("hello world");  // конвертируем строку в массив байт

                ns.Write(hello, 0, hello.Length);     // отправляем сообщение
                while (client.Connected)  // пока клиент подключен, ждем приходящие сообщения
                {
                    byte[] msg = new byte[1024];     // готовим место для принятия сообщения
                    int count = ns.Read(msg, 0, msg.Length);   // читаем сообщение от клиента
                    Console.Write(encoding.GetString(msg, 0, count)); // выводим на экран полученное сообщение в виде строки
                }
            }
        }
    }
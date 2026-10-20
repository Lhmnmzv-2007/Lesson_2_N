
using System;
using System.IO;
using System.Net.Sockets;

namespace ChatClient;

class Program
{
    static void Main()
    {
        var client = new TcpClient("192.168.0.21", 27001);

        var serverStream = client.GetStream();

        var br = new BinaryReader(serverStream);
        var bw = new BinaryWriter(serverStream);

        while (true)
        {
            Console.WriteLine("\n1 - proclist");
            Console.WriteLine("2 - kill");
            Console.WriteLine("3 - run");
            Console.Write("Choice: ");

            string choice = Console.ReadLine() ?? "";

            string command = "";
            string param = "";

            if (choice == "1")
            {
                command = "proclist";
            }
            else if (choice == "2")
            {
                command = "kill";

                Console.Write("Process name: ");
                param = Console.ReadLine() ?? "";
            }
            else if (choice == "3")
            {
                command = "run";

                Console.Write("Process name or path: ");
                param = Console.ReadLine() ?? "";
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                continue;
            }

            bw.Write(command);
            bw.Write(param);
            bw.Flush();

            Console.WriteLine("\n" + br.ReadString());
        }
    }
}

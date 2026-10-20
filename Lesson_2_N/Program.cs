
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServer;

class Program
{
    class Command
    {
        public const string PROCLIST = "proclist";
        public const string KILL = "kill";
        public const string RUN = "run";

        public string Text { get; set; } = "";
        public string Param { get; set; } = "";
    }

    static void Main()
    {
        var ip = IPAddress.Parse("192.168.0.21");
        var listener = new TcpListener(ip, 27001);

        listener.Start();

        Console.WriteLine("Server started.");

        while (true)
        {
            var client = listener.AcceptTcpClient();

            Task.Run(() =>
            {
                using (client)
                using (var stream = client.GetStream())
                using (var br = new BinaryReader(stream))
                using (var bw = new BinaryWriter(stream))
                {
                    try
                    {
                        while (true)
                        {
                            var command = new Command();

                            command.Text = br.ReadString();
                            command.Param = br.ReadString();

                            string result = "";

                            if (command.Text == Command.PROCLIST)
                            {
                                result = string.Join("\n",
                                    Process.GetProcesses()
                                    .Select(p => p.ProcessName + " - " + p.Id));
                            }
                            else if (command.Text == Command.KILL)
                            {
                                var process = Process
                                    .GetProcessesByName(command.Param)
                                    .FirstOrDefault();

                                if (process != null)
                                {
                                    process.Kill();
                                    result = "Process stopped.";
                                }
                                else
                                {
                                    result = "Process not found.";
                                }
                            }
                            else if (command.Text == Command.RUN)
                            {
                                try
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = command.Param,
                                        UseShellExecute = true
                                    });

                                    result = "Process started.";
                                }
                                catch
                                {
                                    result = "Program not found. Enter full path.";
                                }
                            }
                            else
                            {
                                result = "Unknown command.";
                            }

                            bw.Write(result);
                            bw.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Aicup2020
{
    public class Runner
    {
        private BinaryReader reader;
        private BinaryWriter writer;

        public Runner(string host, int port, string token)
        {
            Console.Title = "Montece AICUP Bot";
            Console.WriteLine("Ожидание подключения...");

            Init(host, port, token);
        }

        void Init(string host, int port, string token)
        {
            try
            {
                var client = new TcpClient(host, port) { NoDelay = true };
                var stream = new BufferedStream(client.GetStream());
                reader = new BinaryReader(stream);
                writer = new BinaryWriter(stream);
                var tokenData = System.Text.Encoding.UTF8.GetBytes(token);
                writer.Write(tokenData.Length);
                writer.Write(tokenData);
                writer.Flush();
            }
            catch
            {
                Init(host, port, token);
            }
        }

        public static void Main(string[] args)
        {
            try
            {
            string host = args.Length < 1 ? "127.0.0.1" : args[0];
            int port = args.Length < 2 ? 31001 : int.Parse(args[1]);
                string token = args.Length < 3 ? "0000000000000000" : args[2];

                new Runner(host, port, token).Run();
            }
            catch (IOException)
            {

            }
            catch (Exception x)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(x.ToString());
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.ReadLine();
            }
        }

        public void Run()
        {
            var myStrategy = new MyStrategy();
            var debugInterface = new DebugInterface(reader, writer);
            var running = true;
            while (running)
            {
                switch (Model.ServerMessage.ReadFrom(reader))
                {
                    case Model.ServerMessage.GetAction message:
                        new Model.ClientMessage.ActionMessage(myStrategy.GetAction(message.PlayerView, message.DebugAvailable ? debugInterface : null)).WriteTo(writer);
                        writer.Flush();
                        break;
                    case Model.ServerMessage.Finish message:
                        running = false;
                        break;
                    case Model.ServerMessage.DebugUpdate message:
                        myStrategy.DebugUpdate(message.PlayerView, debugInterface);
                        new Model.ClientMessage.DebugUpdateDone().WriteTo(writer);
                        writer.Flush();
                        break;
                    default:
                        throw new Exception("Unexpected server message");
                }
            }
        }
    }
}
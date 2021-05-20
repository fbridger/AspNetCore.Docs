#region snippet2
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcGreeterClient
{
    class Program
    {
        public const int TaskCount = 50;
        #region snippet
        static void Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine($"{DateTime.Now} - Result {reply}");

            var pingTasks = new Task[TaskCount];
            Console.WriteLine($"{DateTime.Now} - Starting {pingTasks.Length} tasks");
            var stopwatch = Stopwatch.StartNew();
            for (var i = 0; i < pingTasks.Length; i++)
            {
                pingTasks[i] = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine($"{DateTime.Now} - Start task {i}");
                    var client = new Greeter.GreeterClient(channel);
                    var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
                    Console.WriteLine($"{DateTime.Now} - Result task {i}: {reply}");
                });
            }
            Console.WriteLine("Waiting for tasks to complete");
            Task.WaitAll(pingTasks);
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {pingTasks.Length} tasks took: {stopwatch.ElapsedMilliseconds}ms");


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        #endregion
    }
}
#endregion

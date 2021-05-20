#region snippet2
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;

namespace GrpcGreeterClient
{
    class Program
    {
        public const int TaskCount = 500;
        #region snippet
        static void Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine($"{DateTime.Now} - Result {reply}");

            var pingTasks = new List<Task>();
            Console.WriteLine($"{DateTime.Now} - Starting {TaskCount} tasks");
            var stopwatch = Stopwatch.StartNew();

            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 50 };
            Parallel.For(0, TaskCount, parallelOptions, (i) =>
            {
                Console.WriteLine($"{DateTime.Now} - Start task {i}");
                var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
                Console.WriteLine($"{DateTime.Now} - End task {i}: {reply}");
            });
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {TaskCount} tasks took: {stopwatch.ElapsedMilliseconds}ms");


            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        #endregion
    }
}
#endregion

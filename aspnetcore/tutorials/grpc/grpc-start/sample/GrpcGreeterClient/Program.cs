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
        private const int TotalRequests = 100;
        private const bool LogDetails = false;
        private static ParallelOptions parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 50 };

        #region snippet
        static void Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine($"{DateTime.Now} - Result {reply}");

            Test();
            TestReusingGrpcChannel();
            TestConcurrentTasks();
            TestConcurrentTasksReusingGrpcChannel();
            

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void Test()
        {
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(Test)} {TotalRequests} tasks");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start task {i}");
                using var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new Greeter.GreeterClient(channel);
                var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End task {i}: {reply}");

            }
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(Test)} for {TotalRequests} tasks took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestReusingGrpcChannel()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestReusingGrpcChannel)} {TotalRequests} tasks");

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < TotalRequests; i++)
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start task {i}");
                var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End task {i}: {reply}");

            }
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestReusingGrpcChannel)} for {TotalRequests} tasks took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestConcurrentTasks()
        {
            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrentTasks)} {TotalRequests} tasks");

            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start task {i}");
                using var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new Greeter.GreeterClient(channel);
                var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End task {i}: {reply}");
            });
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrentTasks)} for {TotalRequests} tasks took: {stopwatch.ElapsedMilliseconds}ms");

        }

        static void TestConcurrentTasksReusingGrpcChannel()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            Console.WriteLine($"{DateTime.Now} - Starting {nameof(TestConcurrentTasksReusingGrpcChannel)} {TotalRequests} tasks");
            var stopwatch = Stopwatch.StartNew();
            Parallel.For(0, TotalRequests, parallelOptions, (i) =>
            {
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - Start task {i}");
                var reply = client.SayHello(new HelloRequest { Name = "GreeterClient" });
                if (LogDetails) Console.WriteLine($"{DateTime.Now} - End task {i}: {reply}");
            });
            stopwatch.Stop();

            Console.WriteLine($"{DateTime.Now} - {nameof(TestConcurrentTasksReusingGrpcChannel)} for {TotalRequests} tasks took: {stopwatch.ElapsedMilliseconds}ms");

        }

        #endregion
    }
}
#endregion

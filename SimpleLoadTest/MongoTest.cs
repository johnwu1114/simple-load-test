using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using NUnit.Framework;

namespace SimpleLoadTest
{
    public class MongoTest
    {
        private readonly string _logPath;
        private readonly IMongoCollection<TestModel> _collection;

        public MongoTest()
        {
            _logPath =
                Environment.GetEnvironmentVariable("LOG_PATH")
                ?? $"../../../../logs/{DateTime.Now:yyyyMMdd}.log";

            var folderPath = Path.GetDirectoryName(_logPath);
            var exists = Directory.Exists(folderPath);

            if (!exists)
                Directory.CreateDirectory(folderPath);

            var mongoConnStr =
                Environment.GetEnvironmentVariable("MONGO_CONN")
                ?? "mongodb://localhost:27017";
            var client = new MongoClient(mongoConnStr);
            var database = client.GetDatabase("Test");
            _collection = database.GetCollection<TestModel>("InsertOneAsyncTest");
        }

        [TestCase(1)]
        [TestCase(50000)]
        public void InsertOneAsyncTest(int insertCount)
        {
            var speedLogs = new ConcurrentBag<long>();

            for (var i = 0; i < insertCount; i++)
            {
                speedLogs.Add(InsertOneAsync(_collection, GenerateTestModel()));
            }

            Log("=== Non Parallel ===");
            LogTemplates(speedLogs, insertCount);

            Assert.AreEqual(speedLogs.Count, insertCount);
        }

        [TestCase(1, 1, 8)]
        [TestCase(50000, 50, 25)]
        [TestCase(50000, 50, 50)]
        [TestCase(50000, 50, 100)]
        [TestCase(50000, 50, 200)]
        public void Parallel_InsertOneAsyncTest(int insertCount, int concurrentThreads, int workerThreads)
        {
            var speedLogs = new ConcurrentBag<long>();
            ThreadPool.GetMinThreads(out _, out var completionPortThreads);
            ThreadPool.SetMinThreads(workerThreads, completionPortThreads);

            Parallel.For(0, concurrentThreads, _ =>
            {
                for (var i = 0; i < insertCount / concurrentThreads; i++)
                {
                    speedLogs.Add(InsertOneAsync(_collection, GenerateTestModel()));
                }
            });

            Log("=== Parallel ===");
            Log($"ConcurrentThreads: {concurrentThreads:n0}");
            Log($"ThreadPool.MinThreads: {workerThreads:n0}");
            LogTemplates(speedLogs, insertCount);

            Assert.AreEqual(speedLogs.Count, insertCount);
        }

        #region "Private Methods"

        private void Log(string msg)
        {
            var message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}";
            Console.WriteLine(message);

            using (var sw = File.AppendText(_logPath))
            {
                sw.WriteLine(message);
            }
        }

        private void LogTemplates(ConcurrentBag<long> speedLogs, int totalCount)
        {
            Log($"Max: {speedLogs.Max():n0} ms");
            Log($"Min: {speedLogs.Min():n0} ms");
            Log($"Avg: {speedLogs.Average():n3} ms");
            Log($"Total: {speedLogs.Sum():n0} ms");
            Log($"InsertsInSecond: {speedLogs.Count() / (speedLogs.Sum() / (double) 1000):n3}");
            Log($"Success: {speedLogs.Count():n0}/{totalCount:n0}\n");
        }

        private static long InsertOneAsync(IMongoCollection<TestModel> collection, TestModel model)
        {
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Restart();
                collection.InsertOneAsync(model).GetAwaiter().GetResult();
                stopwatch.Stop();
            }
            catch
            {
                // ignored
            }

            return stopwatch.ElapsedMilliseconds;
        }

        private static TestModel GenerateTestModel()
        {
            return new TestModel
            {
                Id = Guid.NewGuid().ToString("N"),
                Filed_1 = Guid.NewGuid().ToString(),
                Filed_2 = int.MaxValue,
                Filed_3 = long.MaxValue,
                Filed_4 = DateTime.Now,
                Filed_5 = DateTimeOffset.Now,
                Filed_6 = true,
                Filed_7 = 1000m,
                Filed_8 = 1000m,
                Filed_9 = 1000m,
                Filed_10 = 1000m,
                Filed_11 = new List<int> {1, 2, 3, 4, 5, 6}
            };
        }

        #endregion
    }
}
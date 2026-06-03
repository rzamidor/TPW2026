using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class DiagnosticLogger : IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _loggingTask;
        private readonly string _filePath;

        public DiagnosticLogger()
        {
            _filePath = "balls_diagnostic_data.json";

            if (File.Exists(_filePath)) File.Delete(_filePath);

            _loggingTask = Task.Run(LogLoopAsync);
        }

        public void LogBallData(int id, double x, double y, double vx, double vy)
        {
            var logEntry = new
            {
                Timestamp = DateTime.Now.ToString("O"),
                BallId = id,
                X = Math.Round(x, 2),
                Y = Math.Round(y, 2),
                Vx = Math.Round(vx, 2),
                Vy = Math.Round(vy, 2)
            };

            string jsonString = JsonSerializer.Serialize(logEntry);
            _logQueue.Enqueue(jsonString);
        }

        private async Task LogLoopAsync()
        {
            using StreamWriter writer = new StreamWriter(_filePath, append: true, encoding: Encoding.ASCII);

            while (!_cts.Token.IsCancellationRequested)
            {
                if (_logQueue.TryDequeue(out string? logEntry))
                {
                    await writer.WriteLineAsync(logEntry);
                }
                else
                {
                    await Task.Delay(10);
                }
            }

            while (_logQueue.TryDequeue(out string? logEntry))
            {
                await writer.WriteLineAsync(logEntry);
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _loggingTask.Wait();
            _cts.Dispose();
        }
    }
}
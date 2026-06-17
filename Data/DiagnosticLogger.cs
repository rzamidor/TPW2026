using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class DiagnosticLogger : IDisposable
    {
        private readonly BlockingCollection<string> _logQueue = new();
        private readonly Task _loggingTask;
        private readonly string _filePath;

        public DiagnosticLogger()
        {
            _filePath = "balls_diagnostic_data.json";
            if (File.Exists(_filePath)) File.Delete(_filePath);
            _loggingTask = Task.Run(LogLoop);
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

            if (!_logQueue.IsAddingCompleted)
            {
                _logQueue.Add(jsonString);
            }
        }

        private void LogLoop()
        {
            using StreamWriter writer = new StreamWriter(_filePath, append: true, encoding: Encoding.ASCII);
            foreach (var logEntry in _logQueue.GetConsumingEnumerable())
            {
                writer.WriteLine(logEntry);
            }
        }

        public void Dispose()
        {
            _logQueue.CompleteAdding();
            _loggingTask.Wait();
            _logQueue.Dispose();
        }
    }
}
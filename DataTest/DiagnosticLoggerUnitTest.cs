using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class DiagnosticLoggerUnitTest
    {
        [TestMethod]
        public void Logger_WritesDataConcurrently_And_FlushesOnDispose()
        {
            string expectedFile = "balls_diagnostic_data.json";

            // Upewniamy się, że plik nie istnieje przed testem
            if (File.Exists(expectedFile)) File.Delete(expectedFile);

            // Tworzymy logger z warstwy DATA
            using (var logger = new DiagnosticLogger())
            {
                logger.LogBallData(1, 10.5, 20.0, 1.0, -1.0);
                logger.LogBallData(2, 100.0, 50.0, -2.0, 2.5);
            }

            // Sprawdzamy, czy plik powstał
            Assert.IsTrue(File.Exists(expectedFile), "Plik z logami nie został utworzony.");

            // Sprawdzamy jego zawartość
            string fileContent = File.ReadAllText(expectedFile);
            Assert.IsTrue(fileContent.Contains("\"BallId\":1"));
            Assert.IsTrue(fileContent.Contains("\"BallId\":2"));
            Assert.IsTrue(fileContent.Contains("\"X\":10.5"));

            // Sprzątamy po teście
            if (File.Exists(expectedFile)) File.Delete(expectedFile);
        }
    }
}
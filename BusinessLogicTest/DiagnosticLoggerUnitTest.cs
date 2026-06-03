using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class DiagnosticLoggerUnitTest
    {
        [TestMethod]
        public void Logger_WritesDataConcurrently_And_FlushesOnDispose()
        {
            string expectedFile = "balls_diagnostic_data.json";

            if (File.Exists(expectedFile)) File.Delete(expectedFile);

            using (var logger = new DiagnosticLogger())
            {
                logger.LogBallData(1, 10.5, 20.0, 1.0, -1.0);
                logger.LogBallData(2, 100.0, 50.0, -2.0, 2.5);
            }

            // Assert
            Assert.IsTrue(File.Exists(expectedFile), "Plik z logami nie został utworzony.");

            string fileContent = File.ReadAllText(expectedFile);
            Assert.IsTrue(fileContent.Contains("\"BallId\":1"));
            Assert.IsTrue(fileContent.Contains("\"BallId\":2"));
            Assert.IsTrue(fileContent.Contains("\"X\":10.5"));

            if (File.Exists(expectedFile)) File.Delete(expectedFile);
        }
    }
}
using PZEQuizes.Model;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PZEQuizes.ViewModels.Logic
{
    public class DataBaseHandler
    {
        public static async Task<QuizData> OpenDatabaseAsync(string databasePath= "C:/Users/Dawid/Desktop/PZEQuizes/PZEQuizes/DataSets/demoSet.json")
        {
            try
            {
                string json = await File.ReadAllTextAsync(databasePath);
                var quizData = JsonSerializer.Deserialize<QuizData>(json);

                if (quizData == null)
                {
                    return null;
                }

                return quizData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
                return null;
            }
        }
    }
}

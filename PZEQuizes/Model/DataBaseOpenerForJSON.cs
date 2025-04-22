using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using PZEQuizes.ViewModels.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZEQuizes.Model
{
    class DataBaseOpenerForJSON : IDataBaseOpener
    {
        public async Task<QuizData> OpenDatabaseAsync(IStorageProvider storageProvider,QuizData quizData)
        {
            var file = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Wybierz plik z pytaniami",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
            new FilePickerFileType("Pliki JSON") { Patterns = new[] { "*.json" } }
        }
            });

            if (file.Count == 0)
                return null; // User canceled the file picker

            string databasePath = file[0].Path.AbsolutePath;

            try
            {
                quizData = await DataBaseHandler.OpenDatabaseAsync(databasePath);
                return quizData; // Return the updated quizData
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null; // Or handle it as needed
            }
        }

    }
}

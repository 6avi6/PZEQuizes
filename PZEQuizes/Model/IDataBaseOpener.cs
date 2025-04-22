using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZEQuizes.Model
{
    interface IDataBaseOpener
    {
         Task<QuizData> OpenDatabaseAsync(IStorageProvider storageProvider,QuizData quizData);
    }
}

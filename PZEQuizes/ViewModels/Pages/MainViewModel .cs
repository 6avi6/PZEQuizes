using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PZEQuizes.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Avalonia.Media;
using System.Diagnostics;

using PZEQuizes.ViewModels.Logic;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using PZEQuizes.ViewModels.UIElements;
using System.ComponentModel;


namespace PZEQuizes.ViewModels
{
    public partial class MainViewModel : ObservableValidator
    {
        #region Data Control
        IDataBaseOpener dbOpener;
        private readonly IStorageProvider _storageProvider;
        private QuizData quizData { get; set; }

        //TD add dialog host info dipslay
        #endregion

        #region View Properties Control

        [ObservableProperty]
        private string dialogTextInfo = "";
        private int TotalQuestionCount{ get; set; } = 0;
        private int CorrectAnswersCount { get; set; } = 0;

        public string QuestionCounter
        {
            get
            {
                if (quizData == null || quizData.Questions.Count == 0)
                {
                    return "Brak pytań";
                }
                return $"Pytanie: {quizData.CurrentQuestionIndex + 1} / {quizData.Questions.Count}";
            }
        }
        private void UpdateCorrectness()
        {       
                // Counts correct points depending on scoring option
                switch (_navigationBarViewModel.SelectedScoringOption)
                {
                // "Wszystko Poprawne"
                case 0:
                    CorrectAnswersCount += _quizPanelViewModel.answersCount
                          - _quizPanelViewModel.wrongAnswersCount
                          - _quizPanelViewModel.uncheckedCorrectAnswersCount;
                    if(CorrectAnswersCount < 0) CorrectAnswersCount = 0;
                    TotalQuestionCount += _quizPanelViewModel.answersCount;
                    break;
                // "Wszystkie Poprawne"
                case 1:
                    CorrectAnswersCount += _quizPanelViewModel.correctAnswersCount;
                    TotalQuestionCount += _quizPanelViewModel.uncheckedCorrectAnswersCount + _quizPanelViewModel.correctAnswersCount;
                    break;
                // "Wszystkie Poprawne (złe zerują)"
                case 2: 
                    if (_quizPanelViewModel.wrongAnswersCount == 0)
                    {
                        CorrectAnswersCount += _quizPanelViewModel.correctAnswersCount;
                    }
                    TotalQuestionCount += _quizPanelViewModel.uncheckedCorrectAnswersCount + _quizPanelViewModel.correctAnswersCount;//liczba poprawnych odpowiedzi na pytanie

                    break;
                // "Inżyniersko"
                case 3:
                        if (_quizPanelViewModel.wrongAnswersCount==0 && _quizPanelViewModel.uncheckedCorrectAnswersCount == 0)
                        {
                        CorrectAnswersCount+= 1;
                        }
                        else
                        {
                        CorrectAnswersCount+= 0;
                        }
                        TotalQuestionCount += 1;

                    break;
                }
            // After calculating correctness, notify the UI about the updated correctness
            OnPropertyChanged(nameof(QuestionCorrectness));
        }

        [ObservableProperty]
        private bool isDialogOpen;
        public RelayCommand OpenDialogCommand { get; }
        public RelayCommand CloseDialogCommand { get; }

        private void HandleAnsweredQuestion()
        {
            if (quizData.CurrentQuestionIndex < quizData.Questions.Count - 1)
            {
                quizData.CurrentQuestionIndex++;
            }
            else
            {
                DialogTextInfo = $"Próba zakończona, twoje wyniki: \n Liczba pytań: {quizData.CurrentQuestionIndex + 1} \n {QuestionCorrectness}";
                quizData.CurrentQuestionIndex = 0;
                ResetProgress();
                OpenDialogCommand.Execute(null);
            }
            SetCurrentQuestion(quizData.CurrentQuestionIndex);
        }

        private void ResetProgress()
        {
            CorrectAnswersCount = 0;
            TotalQuestionCount = 0;
            _quizPanelViewModel.IsAnswearTrueFalseVisible = false;
            if (quizData != null)
                quizData.CurrentQuestionIndex = 0;
            OnPropertyChanged(nameof(QuestionCorrectness));
        }

        [RelayCommand]
        public void ResetQuestionsCommand(bool openComunicatDialog)
        {
            if  (quizData == null || quizData.Questions.Count == 0)
            {
                if (openComunicatDialog)
                {
                    DialogTextInfo = "Nie można zresetować pytań.\n Brak wczytanych pytań.";
                    OpenDialogCommand.Execute(null);
                }
            }
            else
            {
                quizData.CurrentQuestionIndex = 0;
                SetCurrentQuestion(quizData.CurrentQuestionIndex);
            }

            ResetProgress();
            OnPropertyChanged(nameof(QuestionCorrectness));
        }
        public string QuestionCorrectness
        {
            get
            {
                double correctnessLevel;
                if (quizData == null || quizData.Questions.Count == 0 || TotalQuestionCount == 0)
                {
                    correctnessLevel = 0.0;
                }
                else
                {
                    correctnessLevel = ((double)CorrectAnswersCount / TotalQuestionCount) * 100;
                }
                return $"Poprawność odpowiedzi: {correctnessLevel:F2}%";
            }
        }
        #endregion

        #region NavigationBar Control
        private void OnNavigationBarPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NavigationBarViewModel.IsShuffled) && quizData != null)
            {
                ResetProgress();
                quizData.IsShuffled = _navigationBarViewModel.IsShuffled;

                SetCurrentQuestion(quizData.CurrentQuestionIndex);
                UpdateCorrectness();
            }
            if (e.PropertyName == nameof(NavigationBarViewModel.SelectedScoringOption))
            {
                // Handle scoring option change if needed
                //Debug.WriteLine($"Wybrano opcję punktacji: {_navigationBarViewModel.SelectedScoringOption}");
                ResetProgress();
            }
        }
        #endregion

        #region QuizPanel Control

        // Property to manage quizes
        [ObservableProperty]
        private NavigationBarViewModel _navigationBarViewModel;

        // Property to hold the instance of QuizPanelViewModel
        [ObservableProperty]
        private QuizPanelViewModel _quizPanelViewModel;

        // Handle the property changes in QuizPanelViewModel
        private static readonly Random _random = new Random();

        private void OnQuizPanelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MainQuizPanelButtonText")
            {
                if (_quizPanelViewModel.IsChecked)
                {
                    UpdateCorrectness();
                }
            }

            if (e.PropertyName == nameof(QuizPanelViewModel.QuestionIsAnswered) &&
                _quizPanelViewModel.QuestionIsAnswered)
            {
                if (quizData != null)
                    HandleAnsweredQuestion();
                else
                    ResetQuestionsCommand(false);
            }
        }



        public void SetCurrentQuestion(int indexNumber)
        {
            if (quizData == null || quizData.Questions.Count == 0 || indexNumber < 0 || indexNumber >= quizData._questionIndexes.Count)
                return;

            var firstQuestion = quizData.Questions[quizData._questionIndexes[indexNumber]];

            // Update the question counter
            OnPropertyChanged(nameof(QuestionCounter));

            var random = new Random();
            var shuffledAnswers = firstQuestion.Answers.OrderBy(_ => random.Next()).ToList();

            var updatedAnswers = new ObservableCollection<AnswerOption>();
            foreach (var answer in shuffledAnswers)
            {
                updatedAnswers.Add(new AnswerOption
                {
                    Text = answer.Key,
                    IsCorrect = answer.Value,
                    IsChecked = false,
                    BackgroundColor = Brushes.Transparent
                });

            }

            var newQuestion = new Question(firstQuestion.QuestionText, updatedAnswers, firstQuestion.Type);

            _quizPanelViewModel.SetQuestion(newQuestion);
        }
        #endregion

        private readonly Window _parentWindow;
        // Constructor with default initialization
        public MainViewModel(IStorageProvider storageProvider, Window parentWindow)
        {
            _storageProvider = storageProvider;
            _parentWindow = parentWindow;
            OpenDialogCommand = new RelayCommand(() => IsDialogOpen = true);
            CloseDialogCommand = new RelayCommand(() => IsDialogOpen = false);

            _quizPanelViewModel = new QuizPanelViewModel();

            // Subscribe to property changes
            _quizPanelViewModel.PropertyChanged += OnQuizPanelPropertyChanged;

            var newAnswerOptions = new ObservableCollection<AnswerOption>
            {
                new AnswerOption("Nowa Odpowiedź 1", true),
                new AnswerOption("Nowa Odpowiedź 2", false),
                new AnswerOption("Nowa Odpowiedź 3", false),
                new AnswerOption("Nowa Odpowiedź 4", false),
            };

            var newQuestion = new Question("Nowe Pytanie", newAnswerOptions, "radio");

            // Set the question in the QuizPanelViewModel
            _quizPanelViewModel.SetQuestion(newQuestion);

            // Initialize the NavigationBarViewModel with commands
            dbOpener = new DataBaseOpenerForJSON();
            var openDbCommand = new AsyncRelayCommand(async () =>
            {
                var result = await dbOpener.OpenDatabaseAsync(_storageProvider, quizData);
                if (result is not null) {
                    quizData = result;// Bind the quizData to the ViewModel of OpenDatabase
                    ResetQuestionsCommand(true);// Refresh the question list after reading
                }
                if (quizData == null || quizData.Questions.Count == 0)
                {
                    DialogTextInfo = "Nie można wczytać pytań.\nBrak wczytanych pytań.";
                    OpenDialogCommand.Execute(null);
                }
                else if (quizData != null) {
                    DialogTextInfo = $"Wczytano pytania z bazy danych:\n{quizData.Questions.Count} pytań.";
                    OpenDialogCommand.Execute(null);
                }
            });
            var resetCommand = new RelayCommand<object>(param =>
            {
                // Default value
                bool resetAll = true;

                // Parsing parametr value
                if (param is bool b)
                    resetAll = b;
                else if (param is string s && bool.TryParse(s, out var result))
                    resetAll = result;
                ResetQuestionsCommand(resetAll);
            });
            var helpCommand = new RelayCommand(() =>
            {
                DialogTextInfo = "Pomoc nie nadejdzie \n ;}";
                OpenDialogCommand.Execute(null);
            });
            _navigationBarViewModel = new NavigationBarViewModel(resetCommand, openDbCommand, helpCommand);

            _navigationBarViewModel.PropertyChanged += OnNavigationBarPropertyChanged;
        }
    }
}

using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData.Kernel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PZEQuizes.ViewModels
{
    public partial class QuizPanelViewModel : ObservableValidator
    {
        public bool IsChecked { get; set; } = false;

        public bool _questionIsAnswered = false;
        public bool QuestionIsAnswered
        {
            get => _questionIsAnswered;
            set => SetProperty(ref _questionIsAnswered, value);
        }
        public int correctAnswersCount { get; private set; } = 0;
        public int wrongAnswersCount { get; private set; } = 0;
        public int uncheckedCorrectAnswersCount { get; private set; } = 0;
        public int answersCount { get; private set; } = 0;  

        [ObservableProperty]
        private Question _currentQuestion;

        [ObservableProperty]
        private bool _isSubmitButtonVisible = true; // Submit button visibility

        [ObservableProperty]
        private string _mainQuizPanelButtonText = "Sprawdź Odpowiedź";

        [ObservableProperty] private bool   _isAnswearTrueFalseVisible = false; // Answer visibility
        [ObservableProperty] private IBrush backgroundColorTrue;
        [ObservableProperty] private IBrush backgroundColorFalse;
        private static SolidColorBrush DefaultBrush() => new(Color.Parse("#7b857e"));

        [RelayCommand]
        private void Submit(string parameter)
        {
            if (!IsChecked)
            {
                HandleAnswerSubmission(parameter);
                IsSubmitButtonVisible = true;
                IsChecked = true;
                MainQuizPanelButtonText = "Następne pytanie";
            }
            else
            {
                PrepareNextQuestion();
            }
        }

        private void HandleAnswerSubmission(string parameter)
        {
            if (CurrentQuestion.IsTypeTrueFalse)
                HandleTrueFalseAnswer(parameter);
            else if (CurrentQuestion.IsTypeCheckBox || CurrentQuestion.IsTypeRadio)
                HandleMultipleChoiceAnswer();
        }

        private void HandleTrueFalseAnswer(string parameter)
        {
            var answer = CurrentQuestion.AnswerOptions.FirstOrDefault();
            if (answer == null) return;

            bool isTrueSelected = parameter == "true";
            bool isAnswerCorrect = isTrueSelected ? answer.IsCorrect : !answer.IsCorrect;

            var brush = isAnswerCorrect ? GreenBrush() : RedBrush();
            if (isTrueSelected) BackgroundColorTrue = brush;
            else BackgroundColorFalse = brush;
            if(parameter == null) {
                brush = !isAnswerCorrect ? GreenBrush() : RedBrush();
                if (!isTrueSelected) BackgroundColorTrue = brush;
                else BackgroundColorFalse = brush;
            }


            if (isAnswerCorrect) correctAnswersCount++;
            else wrongAnswersCount++;

            IsAnswearTrueFalseVisible = true;
            answersCount = 1;
        }

        private void HandleMultipleChoiceAnswer()
        {
            foreach (var answer in CurrentQuestion.AnswerOptions)
            {
                if (answer.IsChecked == answer.IsCorrect)
                {
                    answer.BackgroundColor = GreenBrush();
                    if (answer.IsChecked) correctAnswersCount++;
                }
                else
                {
                    if (answer.IsChecked && !answer.IsCorrect) wrongAnswersCount++;
                    if (!answer.IsChecked && answer.IsCorrect) uncheckedCorrectAnswersCount++;

                    answer.BackgroundColor = RedBrush();
                }
            }

            answersCount = CurrentQuestion.IsTypeCheckBox
                ? CurrentQuestion.AnswerOptions.Count
                : 1;
        }

        private void PrepareNextQuestion()
        {
            IsChecked = false;
            IsAnswearTrueFalseVisible = false;
            MainQuizPanelButtonText = "Sprawdź odpowiedź";
            QuestionIsAnswered = true;
            foreach (var answer in CurrentQuestion.AnswerOptions)
            {
                answer.IsChecked = false;
                answer.BackgroundColor = Brushes.Transparent;
            }
        }

        private SolidColorBrush GreenBrush() => new(Color.Parse("#90EE90"));
        private SolidColorBrush RedBrush() => new(Color.Parse("#FFB6C1"));

        public void SetQuestion(Question question)
        {
            CurrentQuestion = question;
            IsChecked = false;
            QuestionIsAnswered = false;
            correctAnswersCount = 0;
            wrongAnswersCount = 0;
            uncheckedCorrectAnswersCount = 0;
            answersCount = 0;
            BackgroundColorTrue = new SolidColorBrush(Color.Parse("#7b857e"));
            BackgroundColorFalse = new SolidColorBrush(Color.Parse("#7b857e"));
            if (_currentQuestion.IsTypeTrueFalse)
                _isSubmitButtonVisible = false;
        }

        public QuizPanelViewModel()
        {
            var defaultAnswers = new ObservableCollection<AnswerOption>
    {
        new("Domyślna Odpowiedź", false),
        new("Domyślna Odpowiedź", false),
        new("Domyślna Odpowiedź", false),
        new("Domyślna Odpowiedź", false)
    };

            SetQuestion(new Question("Domyślne Pytanie?", defaultAnswers, "radio"));
            MainQuizPanelButtonText = "Sprawdź odpowiedź";
            BackgroundColorTrue = DefaultBrush();
            BackgroundColorFalse = DefaultBrush();

        }
    }

    public enum QuestionType { Radio, Checkbox, TrueFalse }

    public partial class Question : ObservableValidator
    {
        [ObservableProperty] private string _currentQuestion;
        [ObservableProperty] private ObservableCollection<AnswerOption> _answerOptions;
        [ObservableProperty] private QuestionType _type;

        public bool IsTypeRadio => Type == QuestionType.Radio;
        public bool IsTypeCheckBox => Type == QuestionType.Checkbox;
        public bool IsTypeTrueFalse => Type == QuestionType.TrueFalse;

        public Question(string questionText, ObservableCollection<AnswerOption> options, string type)
        {
            CurrentQuestion = questionText;
            AnswerOptions = options;
            Type = type.ToLower() switch
            {
                "radio" => QuestionType.Radio,
                "checkbox" => QuestionType.Checkbox,
                "truefalse" => QuestionType.TrueFalse,
                _ => QuestionType.Radio
            };
        }
    }

    public partial class AnswerOption : ObservableValidator
    {
        [ObservableProperty] private string text;
        [ObservableProperty] private bool isCorrect;
        [ObservableProperty] private bool isChecked;
        [ObservableProperty] private IBrush backgroundColor;
        public void SetBackgroundColor(IBrush color)
        {
            BackgroundColor = color;
        }

        public AnswerOption(string text = "Przykładowa Odpowiedź", bool isCorrect = false)
        {
            Text = text;
            IsCorrect = isCorrect;
            IsChecked = false;
            BackgroundColor = Brushes.Transparent;
        }
    }
}

using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PZEQuizes.Model;
using PZEQuizes.Views.UIElements;
using PZEQuizes.ViewModels.Logic;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace PZEQuizes.ViewModels.UIElements
{
    public partial class NavigationBarViewModel : ObservableObject
    {
        private bool _isInitializing = false;
        public RelayCommand<object> ResetQuestionsCommand { get; }
        public AsyncRelayCommand OpenDatabaseCommand { get; }
        public RelayCommand ToggleShuffleCommand { get; }
        public RelayCommand HelpCommand { get; }

        [ObservableProperty]
        private int _selectedScoringOption;

        public ObservableCollection<ScoringOption> ScoringOptions { get; } = new()
        {
            new ScoringOption("0", "Wszystko Poprawne"),
            new ScoringOption("1", "Wszystkie Poprawne"),
            new ScoringOption("2", "Wszystkie Poprawne (złe zerują)"),
            new ScoringOption("3", "Inżyniersko"),
        };

        public IRelayCommand<ScoringOption> SetScoringOptionCommand { get; }

        public NavigationBarViewModel()
        {
            ToggleShuffleCommand = new RelayCommand(() => IsShuffled = !IsShuffled);
            ResetQuestionsCommand = new RelayCommand<object>(OnResetQuestions);
            OpenDatabaseCommand = new AsyncRelayCommand(() => OnOpenDatabaseAsync());
            HelpCommand = new RelayCommand(_helpCommandHandling);

            SetScoringOptionCommand = new RelayCommand<ScoringOption>(SetScoringOption);
            SelectedScoringOption = Int32.Parse(ScoringOptions[0].Id);
            SetScoringOption(ScoringOptions[0]);
        }

        public NavigationBarViewModel(RelayCommand<object> _ResetQuestionsCommand, AsyncRelayCommand _OpenDatabaseCommand, RelayCommand _HelpCommandHandling)
        {
            _isInitializing = true;
            ToggleShuffleCommand = new RelayCommand(() => IsShuffled = !IsShuffled);
            ResetQuestionsCommand = _ResetQuestionsCommand;
            OpenDatabaseCommand = _OpenDatabaseCommand;
            HelpCommand = _HelpCommandHandling;

            SetScoringOptionCommand = new RelayCommand<ScoringOption>(SetScoringOption);
            SelectedScoringOption = Int32.Parse(ScoringOptions[0].Id);
            SetScoringOption(ScoringOptions[0]);
            _isInitializing = false;
        }

        protected virtual void OnResetQuestions(object obj)
        {
            Debug.WriteLine("Default questions reset.");
        }

        protected virtual async Task<QuizData> OnOpenDatabaseAsync(IStorageProvider storageProvider = null , QuizData quizData = null)
        {
            Debug.WriteLine("Default data base opener.");
            return null;
        }

        private bool _isShuffled = false;
        public bool IsShuffled
        {
            get => _isShuffled;
            set
            {  
                SetProperty(ref _isShuffled, value);
                OnPropertyChanged(nameof(ShuffleStatus));
            }
        }

        public string ShuffleStatus => IsShuffled ? "Tryb Losowości: On" : "Tryb Losowości: Off";

        private void SetScoringOption(ScoringOption option)
        {
            ResetQuestionsCommand.Execute(!_isInitializing);
            if (option != null)
            {
                if (Int32.TryParse(option.Id, out int newSelectedOption))
                {
                    if (ScoringOptions[_selectedScoringOption].Label.StartsWith("+ "))
                        ScoringOptions[_selectedScoringOption].Label = ScoringOptions[_selectedScoringOption].Label.Substring("+ ".Length);
                    ScoringOptions[newSelectedOption].Label = "+ " + ScoringOptions[newSelectedOption].Label;
                }
                //Update new selected option
                SelectedScoringOption = newSelectedOption;
            }
        }

        private void _helpCommandHandling()
        {
            Debug.WriteLine("Default help command executed.");
        }
    }
    public partial class ScoringOption : ObservableObject
    {
        [ObservableProperty]
        public string id;

        [ObservableProperty]
        public string label;


        public ScoringOption(string id, string label)
        {
            Id = id;
            Label = label;
        }

        public override string ToString() => Label;

    }
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PZEQuizes.Model
{
    public class Question
    {
        [JsonPropertyName("question")]
        public string QuestionText { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("answers")]
        public Dictionary<string, bool> Answers { get; set; }
    }

    public class QuizData
    {
        private List<Question> _questions = new List<Question>();

        [JsonPropertyName("questions")]
        public List<Question> Questions
        {
            get => _questions;
            set
            {
                _questions = value ?? new List<Question>();  // If value is null, create a new empty list.
                restartAndGenerateQuestionOrder();
            }
        }

        public int CurrentQuestionIndex { get; set; } = 0;

        private bool _isShuffled = false;
        public bool IsShuffled
        {
            get => _isShuffled;
            set
            {
                _isShuffled = value;
                restartAndGenerateQuestionOrder();
            }
        }
        public List<int> _questionIndexes = new List<int>();

        private void restartAndGenerateQuestionOrder()
        {
            _questionIndexes.Clear();
            if (Questions.Count == 0) return; // If there are no questions, do nothing

            if (IsShuffled)
            {
                var usedIndexes = new HashSet<int>();  // To track already used indices
                var random = new Random();

                for (int i = 0; i < Questions.Count; i++)
                {
                    int randomIndex;

                    // Keep generating a random index until we find one that hasn't been used
                    do
                    {
                        randomIndex = random.Next(0, Questions.Count);  // Random number between 0 and Questions.Count-1
                    } while (usedIndexes.Contains(randomIndex));  // Check if the index has already been used

                    _questionIndexes.Add(randomIndex);  // Add the unique index to the list
                    usedIndexes.Add(randomIndex);  // Mark this index as used
                }
            }
            else
            {
                for (int i = 0; i < Questions.Count; i++)
                {
                    _questionIndexes.Add(i);
                }
            }
        }
    }


}

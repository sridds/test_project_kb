using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public enum QuestionDifficulty
{
    Easy,
    Medium,
    Hard
}

public class QuestionPrompt : MonoBehaviour
{
    // this is a container of all the question stuff
    [System.Serializable] // this allows the class to be seen in the inspector
    public class Question
    {
        public string Prompt;
        public string IncorrectOptionOne;
        public string IncorrectOptionTwo;
        public string IncorrectOptionThree;
        public string CorrectOption;

        public QuestionDifficulty Difficulty;
    }

    // Question data
    public List<Question> easyQuestions = new List<Question>();
    public List<Question> mediumQuestions = new List<Question>();
    public List<Question> hardQuestions = new List<Question>();

    // Option text references
    public GameObject holder;
    public TextMeshPro questionPromptText;
    public Option[] options;

    Question currentSelectedQuestion;

    private void Start()
    {
        holder.SetActive(false);
    }

    public void PromptEasyQuestion()
    {
        // Setup a random easy question
        if(easyQuestions.Count == 0)
        {
            // Ensures the compiler doesn't throw an error for having no more questions. 
            Debug.Log("Failed to show question, no more left!");
            return;
        }

        Setup(easyQuestions[Random.Range(0, easyQuestions.Count)]);
    }

    public void PromptMediumQuestion()
    {
        // Setup a random medium question
        if (mediumQuestions.Count == 0)
        {
            // Ensures the compiler doesn't throw an error for having no more questions. 
            Debug.Log("Failed to show question, no more left!");
            return;
        }

        Setup(mediumQuestions[Random.Range(0, mediumQuestions.Count)]);
    }

    public void PromptHardQuestion()
    {
        // Setup a random hard quesiton
        if (hardQuestions.Count == 0)
        {
            // Ensures the compiler doesn't throw an error for having no more questions. 
            Debug.Log("Failed to show question, no more left!");
            return;
        }

        Setup(hardQuestions[Random.Range(0, hardQuestions.Count)]);
    }

    public void Setup(Question question)
    {
        holder.SetActive(true);

        questionPromptText.text = question.Prompt;

        // Create a new list
        List<string> list = new List<string>();

        // Add all options to the list
        list.Add(question.IncorrectOptionOne);
        list.Add(question.IncorrectOptionTwo);
        list.Add(question.IncorrectOptionThree);
        list.Add(question.CorrectOption);

        // Shuffle the list elements around (https://discussions.unity.com/t/clever-way-to-shuffle-a-list-t-in-one-line-of-c-code/535113)
        list = list.OrderBy(x => Random.value).ToList();

        // Now that we have reordered the list, we need to go through the list and setup each option accordingly
        for(int i = 0; i < list.Count; i++)
        {
            // If the current list element matches the correct option, ensure the correct option knows that it is the correct one
            if (list[i] == question.CorrectOption)
            {
                // Setup as correct answer
                options[i].SetupOption(list[i], true);
                continue;
            }

            // Setup the other options as non correct answers
            options[i].SetupOption(list[i], false);
        }

        GameStateManager.Instance.lastQuestion = question;
    }

    /// <summary>
    /// Called externally by an Option class
    /// </summary>
    /// <param name="isCorrect"></param>
    public void SelectOption(bool isCorrect)
    {
        if (isCorrect)
        {
            // Tell the game manager that the player was correct
            GameStateManager.Instance.UpdateState(GameStateManager.GameState.CorrectState);
            holder.SetActive(false);

            if (GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Easy)
            {
                /// HERE -- Consider replacing with your own score script for now. I just made a function in GameStateManager for now.
                GameStateManager.Instance.IncreaseScore(1000);

                // Ensure the question never appears again
                easyQuestions.Remove(currentSelectedQuestion);
            }

            else if (GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Medium)
            {
                /// HERE -- Consider replacing with your own score script for now. I just made a function in GameStateManager for now.
                GameStateManager.Instance.IncreaseScore(2000);

                // Ensure the question never appears again
                mediumQuestions.Remove(currentSelectedQuestion);
            }

            else if (GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Hard)
            {
                /// HERE -- Consider replacing with your own score script for now. I just made a function in GameStateManager for now.
                GameStateManager.Instance.IncreaseScore(4000);

                // Ensure the question never appears again
                hardQuestions.Remove(currentSelectedQuestion);
            }
        }

        else
        {
            // Tell the game manager that the player was incorrect
            GameStateManager.Instance.UpdateState(GameStateManager.GameState.IncorrectState);
            holder.SetActive(false);
        }

        // Ensure current question is no longer set to anything
        currentSelectedQuestion = null;
    }
}

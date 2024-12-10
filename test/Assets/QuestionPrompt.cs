using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

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
    }

    // Question data
    public Question[] easyQuestions;
    public Question[] mediumQuestions;
    public Question[] hardQuestions;

    // Option text references
    public GameObject holder;

    public TextMeshPro questionPromptText;

    public Option[] options;

    private void Start()
    {
        holder.SetActive(false);
    }

    public void PromptEasyQuestion()
    {
        Setup(easyQuestions[Random.Range(0, easyQuestions.Length - 1)]);
    }

    public void PromptMediumQuestion()
    {
        Setup(mediumQuestions[Random.Range(0, mediumQuestions.Length - 1)]);
    }

    public void PromptHardQuestion()
    {
        Setup(hardQuestions[Random.Range(0, hardQuestions.Length - 1)]);
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

            GameStateManager.Instance.IncreaseScore(100);
        }

        else
        {
            // Tell the game manager that the player was incorrect
            GameStateManager.Instance.UpdateState(GameStateManager.GameState.IncorrectState);
            holder.SetActive(false);
        }
    }
}

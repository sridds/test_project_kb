using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private int _sceneIndex;
    [SerializeField]
    private TextMeshProUGUI _continueText;
    [SerializeField]
    private TextMeshProUGUI _resetText;

    int index = 0;

    void Update()
    {
        GetInput();
        UpdateSelected(); 

        if(Input.GetKeyDown(KeyCode.Z))
        {
            // delete data
            if (index == 1)
            {
                Debug.Log("Deleted save data!");
                SaveManager.DeleteData();
            }
            SceneManager.LoadScene(_sceneIndex);
        }
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            index++;
            index %= 2;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            index--;

            if (index < 0) index = 1;
        }
    }

    private void UpdateSelected()
    {
        if(index == 0)
        {
            _continueText.color = Color.yellow;
            _resetText.color = Color.gray;
        }
        else if(index == 1)
        {
            _continueText.color = Color.gray;
            _resetText.color = Color.yellow;
        }
    }
}

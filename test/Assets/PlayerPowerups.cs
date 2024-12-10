using UnityEngine;

public class PlayerPowerups : MonoBehaviour
{
    public enum ActivePowerup
    {
        None,
        Shield,
        DoubleFire,
        TripleFire
    }

    private ActivePowerup activePowerup;
    private float powerupTimer;

    public float maxPowerupTimer;

    public void Start()
    {
        GameStateManager.Instance.OnGameStateChanged += GameStateUpdate;
    }

    private void GameStateUpdate(GameStateManager.GameState gameState)
    {
        if(gameState == GameStateManager.GameState.CorrectState)
        {
            if(GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Easy)
            {
                // enable shield and set active powerup
                EnableShieldPowerup();
                activePowerup = ActivePowerup.Shield;
            }
            else if (GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Medium)
            {
                // enable double fire and set active powerup
                EnableDoubleFirePowerup();
                activePowerup = ActivePowerup.DoubleFire;
            }
            else if (GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Hard)
            {
                // enable triple fire and set active pwoerup
                EnableTripleFirePowerup();
                activePowerup = ActivePowerup.TripleFire;
            }
        }
    }

    private void Update()
    {
        // count up if there is an active powerup
        if(activePowerup != ActivePowerup.None)
        {
            powerupTimer += Time.deltaTime;
        }

        // check if powerup timer has exceeded the max powerup timer
        if(powerupTimer > maxPowerupTimer)
        {
            // If we are currently on the shield powerup, deactivate the shield
            if(activePowerup == ActivePowerup.Shield)
            {
                DisableShieldPowerup();
            }
            // if we are currently on double fire, deactivate double fire
            else if(activePowerup == ActivePowerup.DoubleFire)
            {
                DisableDoubleFirePowerup();
            }
            // if we are currently on triple fire, deactivate triple fire
            else if(activePowerup == ActivePowerup.TripleFire)
            {
                DisableTripleFirePowerup();
            }

            // reset active powerup and powerup timer
            activePowerup = ActivePowerup.None;
            powerupTimer = 0.0f;
        }
    }

    public void EnableShieldPowerup()
    {
        Debug.Log("Enabled shield powerup");

        // Add functionality for the shield here
    }

    public void DisableShieldPowerup()
    {
        Debug.Log("Disabled shield powerup");
    }

    public void EnableDoubleFirePowerup()
    {
        Debug.Log("Enabled double fire powerup");

        // Add functionality for double fire here
    }

    public void DisableDoubleFirePowerup()
    {
        Debug.Log("Disabled double fire powerup");
    }

    public void EnableTripleFirePowerup()
    {
        Debug.Log("Enabled triple fire powerup");

        // Add functionality for triple fire here
    }

    public void DisableTripleFirePowerup()
    {
        Debug.Log("Disabled triple fire powerup");
    }
}

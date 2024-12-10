using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public int enemiesToSpawn = 4;
    public float randomSpawnX;
    public float spawnY;

    public Enemy[] easyEnemies;
    public Enemy[] mediumEnemies;
    public Enemy[] hardEnemies;

    // Keep track of a list of active enemies
    private List<Enemy> activeEnemies = new List<Enemy>();


    private void Start()
    {
        // Subscribes the GameStateManager OnGameStateChanged event to this function. Listens for the event to be called
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;

        // Start out by spawning some enemies immediately
        SpawnEnemies();
    }

    private void OnGameStateChanged(GameStateManager.GameState gameState)
    {
        // are we in a state that allows us to spawn enemies?
        if(gameState == GameStateManager.GameState.CorrectState || gameState == GameStateManager.GameState.IncorrectState || gameState == GameStateManager.GameState.RegularState)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        for(int i = 0; i < enemiesToSpawn; i++)
        {
            // Choose a random spawn position
            Vector2 spawnPosition = new Vector2(Random.Range(-randomSpawnX, randomSpawnX), spawnY);

            // Choose an enemy and instantiate it 
            Enemy newEnemy = Instantiate(ChooseEnemy(), spawnPosition, Quaternion.identity);

            activeEnemies.Add(newEnemy);
        }
    }

    /// <summary>
    /// Called externally when an Enemy dies
    /// </summary>
    public void DeregisterEnemy(Enemy enemy)
    {
        Debug.Log("Killed " + enemy.name + "!");

        // Remove the enemy, then destroy it. This is important, we need to make sure the enemy is removed from the list first before destroying it
        activeEnemies.Remove(enemy);
        Destroy(enemy.gameObject);

        // Now that we've removed an enemy, its time to check if all enemies have been killed and execute any code if necessary
        CheckForAllEnemiesKilled();
    }

    /// <summary>
    /// Helper function that returns an Enemy
    /// </summary>
    /// <returns></returns>
    private Enemy ChooseEnemy()
    {
        QuestionPrompt questionPromptReference = FindObjectOfType<QuestionPrompt>();

        // Check if theres an easy question selected
        if (GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Easy)
        {
            // if so, return a random enemy from the array of random easy enemies
            /// TIP: if you don't want this randomness, you can remove the array and make it just return a single enemy
            return easyEnemies[Random.Range(0, easyEnemies.Length)];
        }

        // Check if theres a medium question selected
        else if (GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Medium)
        {
            // if so, return a random enemy from the array of random medium enemies
            return mediumEnemies[Random.Range(0, mediumEnemies.Length)];
        }

        // Check if theres a hard question selected
        else if (GameStateManager.Instance.lastQuestion.Difficulty == QuestionDifficulty.Hard)
        {
            // if so, return a random enemy from the array of random hard enemies
            return hardEnemies[Random.Range(0, hardEnemies.Length)];
        }

        // nothing selected, must be in regular state so just spawn a random easy enemy
        else
        {
            return easyEnemies[Random.Range(0, easyEnemies.Length)];
        }
    }

    private void CheckForAllEnemiesKilled()
    {
        // Check if the count of active enemies is zero
        if(activeEnemies.Count == 0)
        {
            // Set to the safe state
            GameStateManager.Instance.UpdateState(GameStateManager.GameState.SafeState);

            Debug.Log("All enemies have been killed, switching to SafeState!");
        }
        else
        {
            Debug.Log("Not all enemies have been killed yet");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int score;
    public int currentSceneIndex;
    private AudioManager audioManager;

    [SerializeField] private GameObject enemyPos;
    private bool isPaused = false;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyPoolSize = 10;
    private List<GameObject> enemyPool = new List<GameObject>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

                for (int i = 0; i < enemyPoolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
             
        // Find the AudioManager object in the scene and store a reference to it
        audioManager = GetComponentInChildren<AudioManager>();
        pauseMenu = transform.Find("PauseMenu").gameObject;
        scoreText = GetComponentInChildren<TMP_Text>();
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
    private void Update()
    {
     
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnEnemy(enemyPos.transform.position);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddScore(1);
        }
    }

    public void AddScore(int points)
    {
        score += points;
        updateScore();
        
        // Play a sound effect when the player gains points
        audioManager.PlaySoundEffect("creature");
    }
    private void updateScore(){
        scoreText.text = "Score: " + score;
    }

     public GameObject SpawnEnemy(Vector3 position)
    {
        // Find inactive enemy in the pool
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                enemy.transform.position = position;
                enemy.SetActive(true);
                return enemy;
            }
        }

        // If no inactive enemy found, create a new one and add it to the pool
        GameObject newEnemy = Instantiate(enemyPrefab);
        newEnemy.transform.position = position;
        enemyPool.Add(newEnemy);
        return newEnemy;
    }

    public void RecycleEnemy(GameObject enemy)
    {
        enemy.SetActive(false);
    }

    public void LoadNextLevel()
    {
        // Play a sound effect when the level is loaded
        audioManager.PlaySoundEffect("rooster");
        float delay = audioManager.GetSoundEffectDuration("rooster");
        Invoke("LoadNextLevelDelayed", delay);

    }
    private void LoadNextLevelDelayed(){
        score=0;
        updateScore();
       SceneManager.LoadScene(currentSceneIndex + 1);
    }

    public void RestartGame()
    {
        score = 0;
        updateScore();
        currentSceneIndex = 0;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
       public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
    }
        public void SaveGame()
    {
        // Create a binary formatter to serialize the game data
        BinaryFormatter formatter = new BinaryFormatter();

        // Create a file stream to write the data to disk
        FileStream fileStream = File.Create(Application.persistentDataPath + "/gameData.dat");

        // Create a new GameData object to hold the data we want to save
        GameData gameData = new GameData(score, currentSceneIndex);

        // Serialize the game data to the file stream
        formatter.Serialize(fileStream, gameData);

        // Close the file stream
        fileStream.Close();

        Debug.Log("Game saved!");
    }

    public void LoadGame()
    {
        // Check if the save file exists
        if (File.Exists(Application.persistentDataPath + "/gameData.dat"))
        {
            // Create a binary formatter to deserialize the game data
            BinaryFormatter formatter = new BinaryFormatter();

            // Open the save file
            FileStream fileStream = File.Open(Application.persistentDataPath + "/gameData.dat", FileMode.Open);

            // Deserialize the game data from the file stream
            GameData gameData = (GameData)formatter.Deserialize(fileStream);

            // Set the game state based on the loaded data
            score = gameData.score;
            updateScore();
            currentSceneIndex = gameData.currentSceneIndex;
            SceneManager.LoadScene(currentSceneIndex);
            // Close the file stream
            fileStream.Close();

            Debug.Log("Game loaded!");
        }
        else
        {
            Debug.LogWarning("Save file not found.");
        }
    }
}

[System.Serializable]
public class GameData
{
    public int score;
    public int currentSceneIndex;

    public GameData(int score, int currentSceneIndex)
    {
        this.score = score;
        this.currentSceneIndex = currentSceneIndex;
    }
}

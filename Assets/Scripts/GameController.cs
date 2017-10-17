using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject m_EnemyMinionPrefab;
    public GameObject m_EnemyShooterPrefab;
    public GameObject m_EnemyBrutePrefab;
    public GameObject m_EnemySpawnPoints;
    public GameObject m_MainMenu;
    public GameObject m_HighScoresMenu;



    // multi-dimentional array where each EnemyWave is defined
    // {Minion, Shooter, Brute }
    private int[,] m_EnemyWave = new int[2, 3] {
        { 10, 3, 1 },
        { 20, 5, 3 },
    };
    private int m_CurrentWave = 0;

    // Use this for initialization
    private void Start()
    {
        Time.timeScale = 0;
        // Currently spawns immediatley, should spawn when player clicks some sort of StartGame UI element 
        SpawnEnemyWave(m_CurrentWave);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;
            m_MainMenu.SetActive(true);


        }

        // Check for the number of enemies remaining
        // if 0 and m_CurrentWave < m_EnemyWave.length
        // increment m_CurrentWave then call SpawnEnemyWave(m_CurrentWave);
        // else EndGame
    }

    private void SpawnEnemyWave(int WaveNumber)
    {
        int numMinions = m_EnemyWave[WaveNumber, 0];
        int numShooters = m_EnemyWave[WaveNumber, 1];
        int numTanks = m_EnemyWave[WaveNumber, 2];

        GameObject[] m_Minions = new GameObject[numMinions];
        GameObject[] m_Shooters = new GameObject[numShooters];
        GameObject[] m_Tanks = new GameObject[numTanks];

        // Spawn EnemyMinions
        for (int i = 0; i < numMinions; i++)
        {
            // Select random spawn point
            Transform spawnPoint = GetEnemySpawnPoint();
            // Instansiate prefab
            m_Minions[i] = Instantiate(m_EnemyMinionPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
        }

        // Spawn EnemyShooters
        for (int i = 0; i < numShooters; i++)
        {
            // Select random spawn
            Transform spawnPoint = GetEnemySpawnPoint();
            // Instansiate prefab
            m_Shooters[i] = Instantiate(m_EnemyShooterPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
        }

        // Spawn EnemyBrute
        for (int i = 0; i < numTanks; i++)
        {
            // Select random spawn
            Transform spawnPoint = GetEnemySpawnPoint();
            // Instansiate prefab
            m_Tanks[i] = Instantiate(m_EnemyBrutePrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
        }
    }

    private Transform GetEnemySpawnPoint()
    {
        Transform spawnPoint;
        Vector3 viewPos;
        bool validSpawnPoint;
        int numSpawnPoints = m_EnemySpawnPoints.transform.childCount;

        do
        {
            spawnPoint = m_EnemySpawnPoints.transform.GetChild(Random.Range(0, numSpawnPoints)).transform;
            viewPos = Camera.main.WorldToViewportPoint(spawnPoint.position);

            if ((viewPos.x > 0f) && (viewPos.x < 1f))
            {
                validSpawnPoint = false;
            }
            else if (viewPos.z < 0f)
            {
                validSpawnPoint = false;
            }
            else
            {
                validSpawnPoint = true;
            }
        }
        while (validSpawnPoint == false);

        return spawnPoint;
    }

    public void OnButtonQuit()
    {
        Application.Quit();

    }

    public void OnButtonStart()
    {
        Time.timeScale = 1;
        m_MainMenu.SetActive(false);
        m_HighScoresMenu.SetActive(false);
    }

    public void OnButtonHighScores()
    {
        if (m_HighScoresMenu.activeInHierarchy)
        {
            m_MainMenu.SetActive(true);
            m_HighScoresMenu.SetActive(false);
        }
        else
        {
            m_MainMenu.SetActive(false);
            m_HighScoresMenu.SetActive(true);
        }

    }


}

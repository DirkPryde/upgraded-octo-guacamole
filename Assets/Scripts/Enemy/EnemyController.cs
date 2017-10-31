using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public GameObject m_EnemyMinionPrefab;
    public GameObject m_EnemyShooterPrefab;
    public GameObject m_EnemyBrutePrefab;
    public GameObject m_EnemySpawnPoints;
    public GameObject m_EnemyMinions;
    public GameObject m_EnemyShooters;
    public GameObject m_EnemyBrutes;
    public Vector3 m_MinionGroupPosition;

    // multi-dimentional array where each EnemyWave is defined
    private int[,] m_EnemyWave = new int[2, 3] {
    //  { Minion, Shooter, Brute }
        { 10, 3, 1 },
        { 20, 5, 3 },
    };

    // Use this for initialization
    void Start ()
    {
        m_MinionGroupPosition = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update ()
    {
        int numMinions = m_EnemyMinions.transform.childCount;
        for (int i = 0; i < numMinions; i++)
        {
            GameObject minion = m_EnemyMinions.transform.GetChild(i).gameObject;
            if (minion.activeInHierarchy)
            {
                m_MinionGroupPosition += minion.transform.position;
            }
        }

    }

    // FUNCTION CALLED BY THE GameController TO SPAWN THE NEXT WAVE OF ENEMIES
    public void SpawnEnemyWave(int WaveNumber)
    {
        int numMinions = m_EnemyWave[WaveNumber, 0];
        int numShooters = m_EnemyWave[WaveNumber, 1];
        int numBrutes = m_EnemyWave[WaveNumber, 2];

        GameObject[] m_Minions = new GameObject[numMinions];
        GameObject[] m_Shooters = new GameObject[numShooters];
        GameObject[] m_Tanks = new GameObject[numBrutes];

        // Spawn EnemyMinions
        for (int i = 0; i < numMinions; i++)
        {
            // Select random spawn point
            Transform spawnPoint = GetEnemySpawnPoint();
            // Instansiate prefab
            m_Minions[i] = Instantiate(m_EnemyMinionPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
            m_Minions[i].transform.parent = m_EnemyMinions.transform;
        }

        // Spawn EnemyShooters
        for (int i = 0; i < numShooters; i++)
        {
            // Select random spawn
            Transform spawnPoint = GetEnemySpawnPoint();
            // Instansiate prefab
            m_Shooters[i] = Instantiate(m_EnemyShooterPrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
            m_Shooters[i].transform.parent = m_EnemyShooters.transform;
        }

        // Spawn EnemyBrutes
        for (int i = 0; i < numBrutes; i++)
        {
            // Select random spawn
            Transform spawnPoint = GetEnemySpawnPoint();
            // Instansiate prefab
            m_Tanks[i] = Instantiate(m_EnemyBrutePrefab, spawnPoint.position, spawnPoint.rotation) as GameObject;
            m_Tanks[i].transform.parent = m_EnemyBrutes.transform;
        }
    }

    // FUNCTION TO RETURN TRANSFORM OF A VALID SPAWN POINT
    private Transform GetEnemySpawnPoint()
    {
        Transform spawnPoint;
        Vector3 viewPos;
        bool validSpawnPoint = false;
        int numSpawnPoints = m_EnemySpawnPoints.transform.childCount;

        do
        {
            // Select a random SpawnPoint
            spawnPoint = m_EnemySpawnPoints.transform.GetChild(Random.Range(0, numSpawnPoints)).transform;
            
            // Get the screen position of the random SpawnPoint  
            viewPos = Camera.main.WorldToViewportPoint(spawnPoint.position);
            
            // If the position is off the screen, it is a valid SpawnPoint, else pick another.
            if ( ( (viewPos.x < 0f) || (viewPos.x > 1f) ) && (viewPos.z > 0) )
            {
                validSpawnPoint = true;
            }
            else
            {
                //Debug.Log(viewPos);
                validSpawnPoint = false;
            }
        }
        while (validSpawnPoint == false);

        return spawnPoint;
    }
}
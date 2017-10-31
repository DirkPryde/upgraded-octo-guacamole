using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public int m_MaxHealth = 10;
    private int m_CurrentHealth;
    private EnemyAI m_EnemyAI;

	// Use this for initialization
	void Start ()
    {
        m_CurrentHealth = m_MaxHealth;
        m_EnemyAI = GetComponent<EnemyAI>();
	}
	
    public void TakeDamage(int Damage)
    {
        int Health = m_CurrentHealth - Damage;

        if (Health <= 0)
        {
            m_EnemyAI.m_IsAlive = false;
            transform.gameObject.SetActive(false);
            // Can do things here like, instantiate a broken prefab that falls to pieces
            // Or play an explosion particle effect (or both)
        }
        else
        {
            m_CurrentHealth = Health;
        }
    }
}

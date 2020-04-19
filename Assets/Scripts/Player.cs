using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public class PlayerStats
    {
        public int maxHealth = 100;

        private int _currentHealth;
        public int currentHealth
        {
            get { return _currentHealth; }
            set { _currentHealth = Mathf.Clamp(value, 0, maxHealth); }
        }

        public void Init()
        {
            currentHealth = maxHealth;
        }
    }

    public PlayerStats stats = new PlayerStats();

    public int fallBoundary = -20;

    public Random rnd = new Random();

    public string deathSoundName = "DeathVoice";
    public string damageSoundName01 = "GruntVoice01";
    public string damageSoundName02 = "GruntVoice02";

    private AudioManager audioManager;

    [SerializeField]
    private StatusIndicator statusIndicator;

    private void Start()
    {
        stats.Init();

        if(statusIndicator == null)
        {
            Debug.LogError("NO STATUS IND. ON PLAYER!");
        }
        else
        {
            statusIndicator.SetHealth(stats.currentHealth, stats.maxHealth); 
        }

        audioManager = AudioManager.instance;
        if(audioManager == null)
        {
            Debug.LogError("No audio manager!");
        }
    }

    void Update()
    {
        if(transform.position.y <= fallBoundary)
        {
            DamagePlayer(9999);
        }
    }

    public void DamagePlayer(int damage)
    {
        stats.currentHealth -= damage;
        if (stats.currentHealth <= 0)
        {
            audioManager.PlaySound(deathSoundName);
            GameMaster.KillPlayer(this);
        }
        else
        {
            double check = Random.Range(0, 1);
            if (check > 0.5)
            {
                audioManager.PlaySound(damageSoundName01);
            }
            else
            {
                audioManager.PlaySound(damageSoundName02);
            }
        }
      
        statusIndicator.SetHealth(stats.currentHealth, stats.maxHealth);
        
    }
}

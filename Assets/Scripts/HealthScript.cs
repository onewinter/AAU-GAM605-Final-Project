using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private int score = 1;
    [SerializeField] private AudioClip audioHit;

    private SpriteFlash spriteFlash;
    private AudioSource audioSource;
    private HealthBarScript healthBar;
    private int startHealth;
    private Vector3 startPosition;
    private GameManager gameManager;
    private bool didStart;

    void Awake()
    {
        spriteFlash = GetComponent<SpriteFlash>();
        audioSource = GetComponent<AudioSource>();
        healthBar = GetComponentInChildren<HealthBarScript>();
        gameManager = FindObjectOfType<GameManager>();
        startHealth = health;
    }

    private void Start()
    {
        startPosition = transform.position;
        didStart = true;
    }

    void OnEnable()
    {
        // nullable in case healthbar isn't initialized yet
        healthBar?.UpdateHealthBar(health, startHealth);
    }

    void OnDisable()
    {
        // only fire after Start() has been called or else positions will get messed up
        if (!didStart) return;

        // reset health & position for when object is reactivated later
        health = startHealth;
        transform.position = startPosition;
        spriteFlash?.StopFlash();
    }

    public void TakeDamage(int damage)
    {
        // change the health value and log name/new value to console
        health -= damage;
        healthBar.UpdateHealthBar(health, startHealth);
        gameManager.AddToScore(score);
        spriteFlash.Flash();
        gameManager.PlayAudioClip(audioHit);
        Debug.Log(gameObject.name + " health value is now: " + health);

        // if health isn't 0 or less, don't do anything else
        if (health > 0) return;

        // object is now dead
        Debug.Log(gameObject.name + " is now dead.");
        gameObject.SetActive(false);
    }
}
using UnityEngine;
using UnityEngine.Events;

public class LightningRodScript : MonoBehaviour
{
    // events to link up for rod charged/uncharged
    public UnityEvent onRodCharged;
    public UnityEvent onRodDischarged;

    [SerializeField] 
    private float health = 100;
    [SerializeField] 
    private float dischargeRate = 0f;
    [SerializeField]
    [Range(0.01f, 1)]
    private float chargedAt = 1f;
    [SerializeField]
    [Range(0, .99f)]
    private float dischargedAt = 0f;
    [SerializeField] private AudioClip soundCharge;
    [SerializeField] private AudioClip soundDischarge;

    private float maxHealth;
    private HealthBarScript healthBar;
    private SpriteFlash spriteFlash;
    private GameManager gameManager;
    private Animator animator;
    private bool isCharged;
    private bool chargeFired;
    private bool dischargeFired = true;

    void Awake()
    {
        healthBar = GetComponentInChildren<HealthBarScript>();
        spriteFlash = GetComponent<SpriteFlash>();
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.onGameReset += OnGameReset;
        maxHealth = health;
    }

    void OnGameReset()
    {
        // fired on game reset; reset rod to starting values
        onRodDischarged.Invoke();
        chargeFired = false;
        dischargeFired = true;
        isCharged = false;
        health = maxHealth;
        healthBar?.UpdateHealthBar((int)health, (int)maxHealth);
        animator.SetBool("isCharged", isCharged);
        Debug.Log(name + " onGameReset");
    }

    void Start()
    {
        healthBar?.UpdateHealthBar((int)health, (int)maxHealth);
    }

    
    void Update()
    {
        if (gameManager.IsPaused()) return;

        // first time the rod changes state from discharged->charged
        if (maxHealth * (1-chargedAt) >= health && !chargeFired)
        {
            chargeFired = true;
            dischargeFired = false;
            isCharged = true;
            onRodCharged.Invoke();
            animator.SetBool("isCharged", isCharged);
            gameManager.PlayAudioClip(soundCharge);
            Debug.Log(name + " onRodCharged");
        }
        // first time the rod changes state from charged->discharged
        else if (maxHealth * (1 - dischargedAt) < health && !dischargeFired)
        {
            dischargeFired = true;
            chargeFired = false;
            isCharged = false;
            onRodDischarged.Invoke();
            animator.SetBool("isCharged", isCharged);
            gameManager.PlayAudioClip(soundDischarge);
            Debug.Log(name + " onRodDischarged");
        }

        // if we set a discharge rate, regen health until full
        if (dischargeRate > 0 && health < maxHealth)
        {
            health += maxHealth * dischargeRate * Time.deltaTime;
            healthBar.UpdateHealthBar((int)health, (int)maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        // change the health value, update the healthbar, flash the sprite to show damage
        health -= damage;
        healthBar.UpdateHealthBar((int)health, (int)maxHealth);
        spriteFlash.Flash();
        Debug.Log(name + " health value is now: " + health);
    }

}

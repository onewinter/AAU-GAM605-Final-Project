using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    
    // patrol/movement speed
    [SerializeField] private float moverSpeed = 1f;
    // how much health to take from the player on impact
    [SerializeField] private int damage = 1;

    private Vector3 target;
    private Rigidbody2D rigidbody2d;
    private Vector3 startPos;
    private GameManager gameManager;
    private GameObject player;
    private float stunDuration;
    private bool hasOrb;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        player = GameObject.FindWithTag("Player");
    }

    private void Start()
    {
        startPos = transform.position;
    }

    public void SetOrb()
    {
        hasOrb = true;
    }

    public bool HasOrb()
    {
        return hasOrb;
    }
    
    void CleanUpOrbs()
    {
        // reset layer to default and delete any orbs that are hanging around
        gameObject.layer = 0;
        var oldOrbs = GetComponentsInChildren<LightningOrbScript>();
        foreach (var orb in oldOrbs)
        {
            Destroy(orb.gameObject);
        }

        // clear orb and stun status
        hasOrb = false;
        stunDuration = 0;
    }

    void OnDisable()
    {
        CleanUpOrbs();
    }

    void OnEnable()
    {
        CleanUpOrbs();
    }

    private void Update()
    {
        // only if player exists
        if (!player) return;

        // get our current target & move towards it
        target = player.transform.position;
        stunDuration -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        // don't move if game manager is paused or enemy is still stunned
        if (!gameManager.IsPaused() && stunDuration <= 0)
            // use normalized velocity to move (for the animator script)
            rigidbody2d.velocity = Vector3.Normalize(target - transform.position) * moverSpeed;
        else
            rigidbody2d.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // damage the player upon collisions
        if (!other.gameObject.CompareTag("Player")) return;
        other.gameObject.GetComponent<PlayerScript>().TakeDamage(damage);
    }
    
    public void ApplyStun(float duration)
    {
        stunDuration = duration;
    }

}
using System.Collections;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float firingSpeed = .5f;
    [SerializeField] private float lightningRange = 5f;
    [SerializeField] private float lightningLife = .1f;
    [SerializeField] private int lightningSegments = 5;
    [SerializeField] private int lightningDamage = 5;
    [SerializeField] private float lightningStunLength = 3f;
    [SerializeField] private int lightningPushback = 1000;
    [SerializeField] private GameObject lightningOrbPrefab;
    [SerializeField] private int health = 10;
    [SerializeField] private AudioClip audioHit;
    [SerializeField] private AudioClip audioShoot;

    private Rigidbody2D rig2d;
    private GameManager gameManager;
    private SpriteFlash spriteFlash;
    private Camera mainCamera;
    private int startHealth;
    private float timeSinceShot;
    private float timeSinceDamaged;
    private Vector2 newVelocity;
    private Vector3 start;
    private Vector3 direction;
    private LineRenderer lr;

    void Awake()
    {
        rig2d = GetComponent<Rigidbody2D>();
        spriteFlash = GetComponent<SpriteFlash>();
        lr = GetComponent<LineRenderer>();
        gameManager = FindObjectOfType<GameManager>();
        gameManager.onGameReset += OnGameReset;
        mainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
    }

    private void Start()
    {
        // initialize some variables
        start = transform.position;
        timeSinceShot = firingSpeed;
        direction = Vector2.down;
        startHealth = health;

        // set player to start values
        OnGameReset();
    }

    private void FixedUpdate()
    {
        // don't move if game manager is paused
        if (!gameManager.IsPaused())
            // move the character based on player input
            rig2d.velocity = newVelocity * playerSpeed;
        else
            rig2d.velocity = Vector2.zero;
    }

    private void Update()
    {
        // don't move if game manager is paused
        if (gameManager.IsPaused()) return;

        // get keyboard input
        var axisH = Input.GetAxis("Horizontal");
        var axisV = Input.GetAxis("Vertical");
        newVelocity = new Vector2(axisH, axisV);

        // fire when LMB or Space is pressed or held down
        if (Input.GetMouseButton(0) || Input.GetKey((KeyCode.Space)))
        {
            // limit firing rate
            if (timeSinceShot >= firingSpeed)
            {
                timeSinceShot = 0;
                gameManager.PlayAudioClip(audioShoot);

                // raycast looking for enemies and store the direction the mouse click was from the player
                direction = Vector3.Normalize(mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position);
                var r = Physics2D.Raycast(transform.position, direction, lightningRange);
                Debug.Log("player at " + transform.position.x + ", " + transform.position.y + " mouse at " + direction.x + ", " + direction.y);

                if (r.collider == null)
                {
                    // use line renderer to draw a lightning bolt that disappears over time
                    StartCoroutine(LightningStrike(transform.position + direction * lightningRange));
                }
                else if (r.collider.CompareTag("Enemy") || r.collider.CompareTag("LightningRod"))
                {
                    var rc = r.collider;
                    Debug.Log("hit " + rc.name);

                    // use line renderer to draw a lightning bolt that disappears over time
                    StartCoroutine(LightningStrike(rc.transform.position));

                    if (r.collider.CompareTag("Enemy"))
                    {
                        // damage enemy, apply stun, push them back
                        rc.GetComponent<HealthScript>().TakeDamage(lightningDamage);
                        rc.GetComponent<EnemyScript>().ApplyStun(lightningStunLength);
                        rc.GetComponent<Rigidbody2D>()
                            .AddForce(Vector3.Normalize(rc.transform.position - transform.position) *
                                      lightningPushback);

                        // create a lightning orb on the enemy to spread further damage
                        rc.GetComponent<EnemyScript>().SetOrb();
                        var newOrb = Instantiate(lightningOrbPrefab, rc.transform);
                    }
                    else if (r.collider.CompareTag("LightningRod"))
                    {
                        // charge the lightning rod ("damage" it)
                        rc.GetComponent<LightningRodScript>().TakeDamage(lightningDamage);
                    }
                }
                else
                {
                    // use line renderer to draw a lightning bolt that disappears over time
                    StartCoroutine(LightningStrike(transform.position + direction * lightningRange));
                }
            }
        }

        timeSinceShot += Time.deltaTime;
        timeSinceDamaged += Time.deltaTime;
    }

    IEnumerator LightningStrike(Vector3 target)
    {

        lr.positionCount = lightningSegments;
        float jitter = lightningSegments * .025f;

        // break the lightning into segments with random jitter to simulate a lightning bolt
        for (int i = 1; i < lightningSegments; i++)
        {
            var pos = Vector3.Lerp(transform.position, target, (float)i / lightningSegments);
            pos.x += Random.Range(-jitter, jitter);
            pos.y += Random.Range(-jitter, jitter);
            lr.SetPosition(i-1, pos);
        }
        lr.SetPosition(lightningSegments - 1, target);
        yield return new WaitForSeconds(lightningLife);
        lr.positionCount = 0;
    }

    public void TakeDamage(int damage)
    {
        // only take damage if it's been more than a second
        if (timeSinceDamaged < 1f) return;

        // change the health value and log name/new value to console
        health -= damage;
        gameManager.UpdateHealth(health);
        spriteFlash.Flash();
        gameManager.PlayAudioClip(audioHit);
        timeSinceDamaged = 0;
        Debug.Log(gameObject.name + " health value is now: " + health);

        // if health isn't 0 or less, don't do anything else
        if (health > 0) return;

        // object is now dead
        Debug.Log(gameObject.name + " is now dead.");

        // reset the game (lose)
        gameManager.ResetGame(false);
    }
    
    // called by game manager on game reset event invocation
    void OnGameReset()
    {
        health = startHealth;
        gameManager.UpdateHealth(health);
        transform.position = start;
        Debug.Log(name + " onGameReset");
    }
}
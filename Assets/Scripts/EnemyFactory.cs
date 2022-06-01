using UnityEngine;

// spawns enemies when enabled; can be set to disable enemies along with the factory when disabled
public class EnemyFactory : MonoBehaviour
{
    // the list of animation controllers to choose from when spawning
    [SerializeField]
    private RuntimeAnimatorController[] enemyControllers;
    // base enemy prefab to spawn
    [SerializeField]
    private GameObject enemyPrefab;
    // max active enemies at a time
    [SerializeField]
    private int maxActiveEnemies = 5;
    // max total enemies to spawn before stopping
    [SerializeField]
    private int stopAfterEnemies = 10;
    // time in between spawns
    [SerializeField]
    private float spawnDelay = 3f;
    // if true, enemies spawned by this factory will be disabled when the factory is turned off
    [SerializeField]
    private bool disableExistingEnemies;
    // whether to spawn or not
    [SerializeField]
    private bool factoryEnabled;

    private float timeSinceSpawn;
    private int enemiesSpawned;
    private GameObject[] objectPool;
    private GameManager gameManager;
    private Animator animator;
    private bool enabledAtStart;

    // set factory on or off
    public void SetFactoryState(bool onOrOff)
    {
        factoryEnabled = onOrOff;
        // change the animation state based on factory setting
        animator.SetBool("isAnimated", onOrOff);
        // if we should disable the existing enemies when the factory is disabled
        if (factoryEnabled) return;
        if(!disableExistingEnemies) return;

        foreach (var enemy in objectPool)
        {
            if (enemy == null) continue;
            enemy.SetActive(false);
        }
    }

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.onGameReset += onGameReset;
        animator = GetComponent<Animator>();
        enabledAtStart = factoryEnabled;
    }

    void onGameReset()
    {
        // reset the factory condition and disable all pool enemies
        factoryEnabled = enabledAtStart;
        enemiesSpawned = 0;

        foreach (var enemy in objectPool)
        {
            if (enemy == null) continue;
            enemy.SetActive(false);
        }
        
        Debug.Log(name + " onGameReset");
    }

    void Start()
    {
        // initialize our object pool of enemies
        objectPool = new GameObject[maxActiveEnemies];
        for (int i = 0; i < objectPool.Length; i++)
        {
            objectPool[i] = CreateEnemy();
            objectPool[i].SetActive(false);
        }

        // ensure we spawn our first enemy right away
        timeSinceSpawn = spawnDelay;
    }

    void Update()
    {
        // only spawn if factory is enabled
        if (!factoryEnabled) return;

        // don't spawn if game is paused, if we've spawned our total # of enemies, or if we're waiting on the spawn delay
        if (!gameManager.IsPaused() && enemiesSpawned < stopAfterEnemies && timeSinceSpawn >= spawnDelay)
        {
            // loop through the object pool and activate the first inactive enemy
            foreach (var t in objectPool)
            {
                if (t.activeInHierarchy != false) continue;
                timeSinceSpawn = 0;
                t.SetActive(true);
                enemiesSpawned++;
                break;
            }
        }

        timeSinceSpawn += Time.deltaTime;
    }

    // enemy spawn function
    private GameObject CreateEnemy()
    {
        // pick a random controller to pair with the enemy prefab
        var newController = Random.Range(0, enemyControllers.Length);
        var newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.Euler(0, 0, 0));
        newEnemy.GetComponent<Animator>().runtimeAnimatorController = enemyControllers[newController];
        return newEnemy;
    }

    void OnDisable()
    {
        // if we should disable the existing enemies when the factory is disabled
        if (!disableExistingEnemies) return;

        foreach (var enemy in objectPool)
        {
            if (enemy == null) continue;
            enemy.SetActive(false);
        }
    }
}

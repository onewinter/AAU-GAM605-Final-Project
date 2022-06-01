using System.Collections;
using UnityEngine;

public class LightningOrbScript : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private GameObject lightningOrbPrefab;
    [SerializeField] private int lightningSegments = 5;
    [SerializeField] private float lightningJumpDelay = .1f;
    [SerializeField] private float lightningJumpRange = 1.5f;
    [SerializeField] private int lightningMaxJumps = 10;
    [SerializeField] private float lightningNewOrbChance = .2f;

    private Collider2D[] hitColliders;
    private LineRenderer lineRenderer;


    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Start()
    {
        // get the list of colliders within range for lightning jumps and start coroutine to hit them one by one
        hitColliders = Physics2D.OverlapCircleAll(transform.position, lightningJumpRange);
        StartCoroutine(LightningStrike());
    }

    IEnumerator LightningStrike()
    {
        // loop through and change layer for all of the enemies collected
        foreach (var hitCollider in hitColliders)
        {
            // skip inactive objects and the player collider
            if (!hitCollider.gameObject.activeInHierarchy || hitCollider.CompareTag("Player")) continue;
            hitCollider.gameObject.layer = LayerMask.NameToLayer("LightningHit");
        }

        var jumpsMade = 0;

        // loop through and damage enemies that are still alive and haven't died since collection
        lineRenderer.positionCount = lightningSegments;
        foreach (var hitCollider in hitColliders)
        {
            // stop jumping after x jumps
            if (jumpsMade >= lightningMaxJumps) break;
            // skip enemies that have died since we started looping
            if (hitCollider.gameObject.layer != LayerMask.NameToLayer("LightningHit")) continue;
            Debug.Log("hit from " + transform.position.x + ", " + transform.position.y + " to " + hitCollider.transform.position.x + ", " + hitCollider.transform.position.y);

            // break the lightning into segments with random jitter to simulate a lightning bolt
            float jitter = lightningSegments * .01f;
            for (int i = 1; i < lightningSegments; i++)
            {
                var pos = Vector3.Lerp(transform.position, hitCollider.transform.position, (float)i / lightningSegments);
                pos.x += Random.Range(-jitter, jitter);
                pos.y += Random.Range(-jitter, jitter);
                lineRenderer.SetPosition(i - 1, pos);
            }
            lineRenderer.SetPosition(lightningSegments - 1, hitCollider.transform.position);

            // damage the enemies/lightning rods in range and reset their layer to default
            if (hitCollider.CompareTag("Enemy")) hitCollider.GetComponent<HealthScript>().TakeDamage(damage);
            else if (hitCollider.CompareTag("LightningRod")) hitCollider.GetComponent<LightningRodScript>().TakeDamage(damage);
            hitCollider.gameObject.layer = 0;
            jumpsMade++;
            Debug.Log("hitting for " + damage);

            if (hitCollider.CompareTag("Enemy"))
            {
                // chance to spawn a new orb
                if (!hitCollider.GetComponent<EnemyScript>().HasOrb() && Random.Range(0f, 1f) <= lightningNewOrbChance)
                {
                    hitCollider.GetComponent<EnemyScript>().SetOrb();
                    var newOrb = Instantiate(lightningOrbPrefab, hitCollider.gameObject.transform);
                    Debug.Log("spawning new orb");
                }
            }

            // pause in between hitting each enemy
            yield return new WaitForSeconds(lightningJumpDelay);
        }

        // set the line render back to empty and destroy the lightning orb after finish
        lineRenderer.positionCount = 0;
        Destroy(gameObject);
    }
    
}

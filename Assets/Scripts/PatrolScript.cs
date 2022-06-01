using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Serialization;

public class PatrolScript : MonoBehaviour
{
    [SerializeField] private Transform[] pathPoints; 
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float stopDistance = .1f;

    private Vector3 startPosition;
    private Vector3 target;
    private int targetIndex;
    private GameManager gameManager;
    private Rigidbody2D rig2d;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.onGameReset += OnGameReset;
        rig2d = GetComponent<Rigidbody2D>();
    }

    void OnGameReset()
    {
        transform.position = startPosition;
        Debug.Log(name + " onGameReset");
    }

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // only run if paths defined and game is unpaused
        if (gameManager.IsPaused() || pathPoints.Length == 0) return;

        // get our current target & move towards it
        target = pathPoints[targetIndex].position;

        // if we are close enough, switch to the next target
        if (Vector3.Distance(transform.position, target) < stopDistance) targetIndex++;

        // switch back to the first item in the array once we reach the end
        if (targetIndex >= pathPoints.Length) targetIndex = 0;
    }
    private void FixedUpdate()
    {
        // don't move if game manager is paused
        if (!gameManager.IsPaused())
            // use normalized velocity to move (for the animator)
            rig2d.velocity = Vector3.Normalize(target - transform.position) * moveSpeed;
        else
            rig2d.velocity = Vector2.zero;
    }
}

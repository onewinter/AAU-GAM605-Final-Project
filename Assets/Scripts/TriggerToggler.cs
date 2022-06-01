using UnityEngine;
using UnityEngine.Events;

public class TriggerToggler : MonoBehaviour
{
    public UnityEvent onTogglerEnter;
    public UnityEvent onTogglerExit;
    // set to true to have trigger that fires once on enter per game loop and that's it
    [SerializeField] private bool resetEnterOnExit;
    private bool enterFired;
    private bool exitFired;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.onGameReset += OnGameReset;
    }

    private void OnGameReset()
    {
        // reset trigger on game reset
        enterFired = false;
        exitFired = false;
        Debug.Log(name + " onGameReset");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // fire Enter event the first time the player enters the trigger collider
        if (enterFired || !other.CompareTag("Player")) return;

        enterFired = true;
        exitFired = false;
        onTogglerEnter.Invoke();
        Debug.Log(name + " onTogglerEnter");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // fire Exit event the first time the player exits the trigger collider
        if (exitFired || !other.CompareTag("Player")) return;

        exitFired = true;
        enterFired = !resetEnterOnExit;
        onTogglerEnter.Invoke();
        Debug.Log(name + " onTogglerExit");
    }
}

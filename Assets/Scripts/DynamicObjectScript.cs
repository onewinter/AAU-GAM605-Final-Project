using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// helper class to string objects up with Unity Events in the inspector; can set animation bool, play audio clip, or function as a multi-node circuit
public class DynamicObjectScript : MonoBehaviour
{
    private Animator animator;
    private GameManager gameManager;
    [SerializeField] private AudioClip audioOneShot;
    [SerializeField] private bool[] circuit;
    public UnityEvent onCircuitOpen;
    public UnityEvent onCircuitClose;
    private bool closeFired;
    private bool openFired = true;

    // turn an animated object on or off
    public void SetAnimationState(bool turnOn) => animator?.SetBool("isAnimated", turnOn);
    
    // play an audio clip
    public void PlayAudioOneShot() => gameManager.PlayAudioClip(audioOneShot);

    void Awake()
    {
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OpenCircuit(int index)
    {
        circuit[index] = false;
        CheckCircuit();
    }

    public void CloseCircuit(int index)
    {
        circuit[index] = true;
        CheckCircuit();
    }

    // check if a circuit is open or closed, then fire the corresponding event
    public void CheckCircuit()
    {
        // a circuit is only closed if all nodes are closed
        var closed = circuit.All(c => c);

        // only fire once when changing state from closed->open or vice versa
        switch (closed)
        {
            case true when !closeFired:
                closeFired = true;
                openFired = false;
                onCircuitClose.Invoke();
                Debug.Log(name + " onCircuitClose");
                break;
            case false when !openFired:
                openFired = true;
                closeFired = false;
                onCircuitOpen.Invoke();
                Debug.Log(name + " onCircuitOpen");
                break;
        }
    }
}

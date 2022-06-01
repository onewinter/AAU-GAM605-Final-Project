// credit Piers Issler, AAU GAM 602 1H22
using UnityEngine;


public class SimpleSmoothCameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed = 5f;

    void FixedUpdate()
    {
        var desiredPosition = target.position + offset;
        var smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    void LateUpdate()
    {
        Vector3 newPosition = target.position + offset;

        // Smoothly interpolate between current camera position and the new target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, newPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}


// References:
// [Camera]
// All Things Game Dev (2022). How To Make An FPS Player In Under A Minute - Unity Tutorial. [online] Available at: https://www.youtube.com/watch?v=qQLvcS9FxnY. [Accessed 3 Jan 2025].

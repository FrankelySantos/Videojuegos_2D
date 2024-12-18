using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothTime = 0.25f;
    Vector3 currentVelocity;

    void Update()
    {
        //get the players position and add it with offset, then store it to transform.position aka the cameras position
        transform.position = Vector3.SmoothDamp(
      transform.position,
      player.position + offset,
      ref currentVelocity,
      smoothTime
      );
    }
}

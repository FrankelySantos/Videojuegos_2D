using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    Vector3 currentVelocity;
    public float smoothTime = 0.25f;

    void Update()
    {
        //get the players position and add it with offset, then store it to transform.position aka health bar postion
        transform.position = Vector3.SmoothDamp(
       transform.position,
       player.position + offset,
       ref currentVelocity,
       smoothTime
       );
    }
}

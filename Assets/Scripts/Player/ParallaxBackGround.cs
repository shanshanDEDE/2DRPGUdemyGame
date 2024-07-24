using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackGround : MonoBehaviour
{
    private GameObject cam;

    [SerializeField] private float ParallaxEffect;

    private float xPosition;
    private float length;

    void Start()
    {
        cam = GameObject.Find("Main Camera");

        length = GetComponent<SpriteRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }

    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - ParallaxEffect);
        float distanceToMove = cam.transform.position.x * ParallaxEffect;


        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y, transform.position.z);

        if (distanceMoved > xPosition + length)
        {
            xPosition += length;
        }
        else if (distanceMoved < xPosition - length)
        {
            xPosition -= length;
        }
    }
}

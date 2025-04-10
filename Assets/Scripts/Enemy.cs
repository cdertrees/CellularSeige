using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public List<String> paths;
    public float speed;
    public int moveIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //Move towards next point in the Line Renderer. Thank you Unity forums i love you.
        Vector2 currentPos = GameManager.inst.Map.GetPosition(moveIndex);
        transform.position = Vector2.MoveTowards(transform.position, currentPos, speed*Time.deltaTime);
        float distance = Vector2.Distance(currentPos, transform.position);
        if (distance <= 0.05f)
        {
            moveIndex++;
        }

        // if (moveIndex > GameManager.inst.mapPoints.Count - 1)
        // {
        //     
        // }
        

    }
}

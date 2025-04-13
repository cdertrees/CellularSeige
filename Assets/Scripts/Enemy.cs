using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
   // public List<String> paths;
    public float speed;
    public int moveIndex = 0;
    public float health = 1;

    public EnemyTypes enemyType;
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


    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
public enum EnemyTypes
{
    Bacteria = 0,
    Virus = 1,
    Parasite = 2,
    Amoeba = 3,
    Cancer = 4,
    Allergy = 5,
    Fungi = 6,

}

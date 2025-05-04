using System;
using UnityEngine;

public class antigen : MonoBehaviour
{

    public Enemy target;
    public int damage = 1;
    public float speed = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Pathogen"))
        {
           var enemyScript =  other.gameObject.GetComponent<Enemy>();
           
           enemyScript.TakeDamage(damage);
           
           Destroy(gameObject);
           
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antibody : MonoBehaviour
{
    public float waitTime;
    public GameObject Antigen;

    public bool offset;
    public List<Enemy> EnemiesPresent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
       
        StartCoroutine("Attack");
    }

    
    
    IEnumerator Attack()
    {
        
        yield return new WaitForSeconds(waitTime);
        if (EnemiesPresent.Count > 0)
        {
            var anti = Instantiate(Antigen, transform.position, Quaternion.identity); 
            var antiScript = anti.GetComponent<antigen>();
            antiScript.target = EnemiesPresent[EnemiesPresent.Count - 1];
        }
        
        StartCoroutine("Attack");
       
    }
    
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pathogen"))
        {
            EnemiesPresent.Add(other.gameObject.GetComponent<Enemy>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        EnemiesPresent.Remove(other.gameObject.GetComponent<Enemy>());
    }
}

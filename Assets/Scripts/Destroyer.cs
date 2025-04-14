using System;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pathogen"))
        {
            GameManager.inst.currentPathogens.Remove(other.gameObject);
            //change later if we decide to have specific pathogens deal more dmg to the organ
            GameManager.inst.PathogenEnters(1);
            Destroy(other.gameObject);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private int health;
    private List<int> unitDamages = new List<int>();
    int totaldamages = 7;
    //Damages 1. Bacteria 2. Virus 3. Parasite 4. Cancer 5. Allergies 6. Fungi
    public ScriptableUnit unit;
    
    public List<Enemy> pathogensPresent = new List<Enemy>();
    
    private Animator anim;

    private float cooldownTime;
    private float timer;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        ReevaluateType(unit);
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    void ReevaluateType(ScriptableUnit _unit)
    {
        print("reevaluating");
        anim.Play(_unit.Animation.name);
        if (_unit.damages.Count == totaldamages)
        {
            unitDamages.Clear();
            unitDamages = _unit.damages;
        }
        else
        {
            Debug.LogError("Wrong Number of Damage Types!!");
        }

        cooldownTime = _unit.coolDown;
        health = _unit.health;
        timer = cooldownTime;

    }

    
    void Attack()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            //attack
            
            //change for types later
            if (pathogensPresent.Count > 0)
            {
                print("attacked");
                var pathogen = pathogensPresent[pathogensPresent.Count-1];
                var damage = unitDamages[(int)pathogen.enemyType];
                pathogen.TakeDamage(damage);
            }
            
           
            timer = cooldownTime;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Pathogen"))
        {
            Enemy script = other.gameObject.GetComponent<Enemy>();
            pathogensPresent.Add(script);
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Pathogen"))
        {
            Enemy script = other.gameObject.GetComponent<Enemy>();
            pathogensPresent.Remove(script);
        }
    }

    
    
}
// public enum Types
// {
//     Stem,
//     Basophil,
//     BCell,
//     Bone,
//     Eosinophil,
//     Fat,
//     Intestinal,
//     KillerT,
//     Monocyte,
//     Nerve,
//     Neutrophil,
//     Plasma,
//     Platelet,
//     RedBlood,
//
// }

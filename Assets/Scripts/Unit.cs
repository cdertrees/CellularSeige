using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float _health;
    private List<int> _unitDamages = new List<int>();
    private const int TotalDamages = 7;

    //Damages 1. Bacteria 2. Virus 3. Parasite 4. Cancer 5. Allergies 6. Fungi
    public ScriptableUnit unit;
    public List<ScriptableUnit> units;
    public List<Enemy> pathogensPresent = new List<Enemy>();
    private Animator _anim;
    private float _cooldownTime;
    private float _timer;
    
    void Start()
    {
        _anim = GetComponent<Animator>();
        ReevaluateType(unit);
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }

    void ReevaluateType(ScriptableUnit unitTemp)
    {
        var _unit = Instantiate(unitTemp);
        print("reevaluating unit!");
        _anim.Play(_unit.animation.name);
        if (_unit.damages.Count == TotalDamages)
        {
            _unitDamages.Clear();
            _unitDamages = _unit.damages;
        }
        else
        {
            Debug.LogError("Wrong Number of Damage Types!!");
        }

        _cooldownTime = _unit.coolDown;
        _health = _unit.health;
        _timer = _cooldownTime;

    }

    public void TakeDamage(float dmg)
    {
        _health -= dmg;
        if (_health <= 0)
        {
            print("i died");
            ReevaluateType(units[0]);
        }
    }
    
    void Attack()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            //attack
            
            //change for types later
            if (pathogensPresent.Count > 0)
            {
                print("attacked");
                var pathogen = pathogensPresent[pathogensPresent.Count-1];
                var damage = _unitDamages[(int)pathogen.enemyType];
                pathogen.TakeDamage(damage);
                
            }
            
           
           
            _timer = _cooldownTime;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Pathogen"))
        {
            Enemy script = other.gameObject.GetComponent<Enemy>();
            script.inRange.Add(this);
            pathogensPresent.Add(script);
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Pathogen"))
        {
            Enemy script = other.gameObject.GetComponent<Enemy>();
            script.inRange.Remove(this);
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

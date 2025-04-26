using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float _health;
    public float _maxHealth;
    private List<int> _unitDamages = new List<int>();
    private const int TotalDamages = 7;

    //Damages 1. Bacteria 2. Virus 3. Parasite 4. Cancer 5. Allergies 6. Fungi
    public ScriptableUnit unit;
   // public List<ScriptableUnit> units;
    public List<Enemy> pathogensPresent = new List<Enemy>();
    public Animator _anim;
    private float _cooldownTime;
    private float _timer;
    private Enemy nextTarget;
    public GameObject Child;

    

    public TextMeshPro tempText;
    private bool goForward = true;
    private bool attacks = true;

    public GameObject targeted;
    
    
    public List<GameObject> Antibodies;
    
    public Map map;
    private int mapPos = 0;
    public int plateletSpeed = 2;
    //private int pos;
    public bool Healing = false;

    public Unit targetHeal;
    public List<Unit> Units;

    //Fat Cell supplies
    public float addedHealth;
    void Start()
    {
        //targetHeal = GameManager.inst.Units[0];
        targeted.SetActive(false);
        ReevaluateType(unit);
        //mapPos = pos;
        // print( gameObject.name + Units[0]);
        targetHeal = Units[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Platelet")&& !Healing)
        {
           
            Vector2 currentPos = map.path[mapPos];
            transform.position = Vector2.MoveTowards(transform.position, currentPos, plateletSpeed*Time.deltaTime);
            float distance = Vector2.Distance(currentPos, transform.position);
            if (distance <= 0.05f)
            {
                mapPos++;
                if (mapPos >= map.path.Count)
                {
                    
                    mapPos = 0;
                }
                Healing = true;
                
            }

            
            
        }
        if (Healing)
        {
            print("healing");
             if (targetHeal._health < targetHeal._maxHealth)
             {
                 targetHeal._health += 3 * Time.deltaTime;   
             }
             else
             {
                 Healing = false;
                 targetHeal = Units[mapPos];
             }

        }
        
        if (attacks)
        {
            Attack();
        }
        tempText.text = "" + _health;
    }

    private void FixedUpdate()
    {
        //So poorly optimized fix pls
        if (GameManager.inst.clickedUnit == this)
        {
            targeted.SetActive(true);
        }
        else
        {
            targeted.SetActive(false);
        }
    }

    public void ReevaluateType(ScriptableUnit unitTemp)
    {
        
        var _unit = Instantiate(unitTemp);

        if (_unit.special == UnitSpecial.Fat)
        {
            print("newfatcell" + gameObject.name);
            GameManager.inst.AddPercent(1);
        } else if ( _unit.special != UnitSpecial.Fat && _anim.GetCurrentAnimatorStateInfo(0).IsName("Fat"))
        {
            print("fat cell die");
            GameManager.inst.AddPercent(-1);   
        }

        if (_unit.special == UnitSpecial.Slow)
        {
            print("newintestinal" + gameObject.name);
            GameManager.inst.AddIntestinal(1);
        } else if ( _unit.special != UnitSpecial.Slow && _anim.GetCurrentAnimatorStateInfo(0).IsName("Intestinal"))
        {
            GameManager.inst.AddIntestinal(-1);
        }
        
        if (_unit.animation.name == "BCellTest")
        {
            Antibodies[0].SetActive(true);
            Antibodies[1].SetActive(true);
        }
        else
        {
            Antibodies[0].SetActive(false);
            Antibodies[1].SetActive(false);
        }
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
        
        attacks =( _unit.animation.name != ("Stem"))&&( _unit.animation.name != ("Platelet")) && (_unit.animation.name != ("Intestinal"));
        _cooldownTime = (_unit.coolDown/2);
        _maxHealth = _unit.health + ((GameManager.inst.additionalHealthPercent/100f) * _unit.health);
        _health = _unit.health + ((GameManager.inst.additionalHealthPercent/100f) * _unit.health);
        _timer = _cooldownTime;

    }

    public void TakeDamage(float dmg)
    {
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Stem"))
        {
            _health -= dmg;
            if (_health <= 0)
            {
                print("i died");
                ReevaluateType(unit);
            }
        }
       
    }
    
    void Attack()
    {
        
            if (_timer > 0)
            {
                if (pathogensPresent.Count > 0 && goForward)
                {
                    if (pathogensPresent.Contains(nextTarget))
                    {
                        Child.transform.position = Vector3.MoveTowards(Child.transform.position, nextTarget.transform.position, Time.deltaTime*20);
                    }
                    else
                    {
                        nextTarget = pathogensPresent[pathogensPresent.Count - 1];
                    }

                    // if (pathogensPresent.Contains(nextTarget) && goForward)
                    // {
                    //         Child.transform.position = Vector3.MoveTowards(Child.transform.position, nextTarget.transform.position, Time.deltaTime*5);
                    //     //Nic saved me with this one
                    // }
                    // else if (!goForward)
                    // {
                    //     Child.transform.position = Vector3.MoveTowards(Child.transform.position, nextTarget.transform.position, Time.deltaTime*5);
                    // }
                    // else
                    // {
                    //     Child.transform.position = Vector3.MoveTowards(Child.transform.position, nextTarget.transform.position, Time.deltaTime*5);
                    //     nextTarget = pathogensPresent[pathogensPresent.Count - 1];
                    // }

                }
                else 
                {
                    Child.transform.position = Vector3.MoveTowards(Child.transform.position, transform.position, Time.deltaTime*20);
                }
                _timer -= Time.deltaTime;
            }
            else
            {
            
                //attack
                //change for types later
                if (pathogensPresent.Count > 0 && goForward)
                {
                    print("attacked");
                    var damage = _unitDamages[(int)nextTarget.enemyType];
                    nextTarget.TakeDamage(damage);
                    if (pathogensPresent.Count > 0)
                    {
                        nextTarget = pathogensPresent[pathogensPresent.Count-1];
                        nextTarget = pathogensPresent[pathogensPresent.Count-1];
                    }
                   
                
                }

                goForward = !goForward;
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

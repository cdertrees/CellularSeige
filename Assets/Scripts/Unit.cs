using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float _health;
    public float _maxHealth;
    public List<float> _unitDamages = new List<float>();
    private const int TotalDamages = 7;

    //Damages 1. Bacteria 2. Virus 3. Parasite 4. Cancer 5. Allergies 6. Fungi
    public ScriptableUnit unit;
   // public List<ScriptableUnit> units;
    public List<Enemy> pathogensPresent = new List<Enemy>();
    public Animator _anim;
    public float _cooldownTime;
    private float _timer;
    private Enemy nextTarget;
    public GameObject Child;

    //public int speedUpgradeCost = 5;
  


    public TextMeshPro tempText;
    private bool goForward = true;
    private bool attacks = true;

    public GameObject targeted;
    public GameObject mask;
    
    public List<GameObject> Antibodies;
    
    public Map map;
    public int mapPos = 0;
    public int plateletSpeed = 2;
    //private int pos;
    public bool Healing = false;

    public Unit targetHeal;
    public List<Unit> Units;

    public AudioSource AS;
    public AudioClip Heal;
    public AudioClip attackSound;
    public AudioClip die;
    
    public Vector2 ogPosition;

    public GameObject healthBar;


    
    
    //Fat Cell supplies
    public float addedHealth;
    void Start()
    {
        AS = GetComponent<AudioSource>();
        //targetHeal = GameManager.inst.Units[0];
        mask.SetActive(false);
        targeted.SetActive(false);
        ogPosition = transform.position;
        ReevaluateType(unit);
        //mapPos = pos;
        // print( gameObject.name + Units[0]);
        targetHeal = Units[0];
       
      
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.inst.inWave)
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
                    AS.PlayOneShot(Heal);
                    Healing = true;
                
                }

            
            
            }
            if (Healing)
            {
                targetHeal.calcHealthBar();
                print("healing");
                if (targetHeal._health < targetHeal._maxHealth)
                {
                    targetHeal._health += 3 * Time.deltaTime;   
                }
                else
                {
                    AS.Stop();
                    Healing = false;
                    targetHeal = Units[mapPos];
                }

            }
        }
        else
        {
            AS.Stop();
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
            mask.SetActive(true);
            targeted.SetActive(true);
        }
        else
        {
            mask.SetActive(false);
            targeted.SetActive(false);
        }
        
        //calcHealthBar();
        
    }

    public void ReevaluateType(ScriptableUnit unitTemp)
    {
       
        
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Stem"))
        {
            if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Basophil"))
            {
                GameManager.inst.Basophils.Remove(this);
            } 
            else if(_anim.GetCurrentAnimatorStateInfo(0).IsName("BCellTest"))

            {
                GameManager.inst.BCells.Remove(this);
            } 
            else if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Eosinophil"))

            {
                GameManager.inst.Eosinophils.Remove(this);
            } else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Fat"))
            {
                GameManager.inst.FatCells.Remove(this);
            } else if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Intestinal"))
            {
                GameManager.inst.IntestinalCells.Remove(this);
            } else if(_anim.GetCurrentAnimatorStateInfo(0).IsName("KillerT"))

            {
                GameManager.inst.KillerTCells.Remove(this);
            }
            else if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Monocyte"))

            {
                GameManager.inst.Monocytes.Remove(this);
            }
            else if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Neutrophil"))

            {
                GameManager.inst.Neutrophils.Remove(this);
            }
            else if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Plasma"))

            {
                GameManager.inst.Plasmas.Remove(this);
            }
            else if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Platelet"))

            {
                GameManager.inst.Platelets.Remove(this);
            }
            else if(_anim.GetCurrentAnimatorStateInfo(0).IsName("SmoothMuscle"))

            {
                GameManager.inst.SmoothMuscles.Remove(this);
            }
            
        }

        if (_anim.GetCurrentAnimatorStateInfo(0).IsName("Platelet"))
        {
            transform.position = ogPosition;
        }
        
        
        
        var _unit = Instantiate(unitTemp);
        Child.transform.localPosition = new Vector3(0, 0, 0);

        //transform.position = ogPosition;
        
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
        
        if (_unit.special == UnitSpecial.Generate)
        {
            print("newplasemal" + gameObject.name);
            GameManager.inst.AddPlasma(1);
        } else if ( _unit.special != UnitSpecial.Generate && _anim.GetCurrentAnimatorStateInfo(0).IsName("Plasma"))
        {
            GameManager.inst.AddPlasma(-1);
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
        
        
      
        
        
        if (_unit.damages.Count == TotalDamages)
        {
            _unitDamages.Clear();
            _unitDamages = _unit.damages.ConvertAll(i => (float)i);
        }
        else
        {
            Debug.LogError("Wrong Number of Damage Types!!");
        }

       
        attacks =( _unit.animation.name != ("Stem"))&&( _unit.animation.name != ("Platelet")) && (_unit.animation.name != ("Intestinal"));
        _cooldownTime = (_unit.coolDown/2) - ((GameManager.inst.additionalAttackSpeed/100f)* (_unit.coolDown/2) );
        print(_cooldownTime);
        _maxHealth = _unit.health + ((GameManager.inst.additionalHealthPercent/100f) * _unit.health);
        _health = _unit.health + ((GameManager.inst.additionalHealthPercent/100f) * _unit.health);
        _timer = _cooldownTime;
        _anim.Play(_unit.animation.name);
            //
            // if (_unit.animation.name =="Stem")
            // {
            //     print("died");
            //
            // }
        
            print(_unit.animation.name);
            if (_unit.animation.name == "Basophil")
            {
                GameManager.inst.Basophils.Add(this);
            } 
            else if(_unit.animation.name == "BCellTest")

            {
                GameManager.inst.BCells.Add(this);
            } 
            else if(_unit.animation.name == "Eosinophil")

            {
                GameManager.inst.Eosinophils.Add(this);
            } else if (_unit.animation.name == "Fat")
            {
                GameManager.inst.FatCells.Add(this);
            } else if (_unit.animation.name == "Intestinal")
            {
                GameManager.inst.IntestinalCells.Add(this);
            } else if(_unit.animation.name == "KillerT")

            {
                GameManager.inst.KillerTCells.Add(this);
            }
            else if(_unit.animation.name == "Monocyte")

            {
                GameManager.inst.Monocytes.Add(this);
            }
            else if(_unit.animation.name == "Neutrophil")

            {
                GameManager.inst.Neutrophils.Add(this);
            }
            else if(_unit.animation.name == "Plasma")

            {
                GameManager.inst.Plasmas.Add(this);
            }
            else if(_unit.animation.name == "Platelet")

            {
                GameManager.inst.Platelets.Add(this);
            }
            else if(_unit.animation.name == "SmoothMuscle")

            {
                GameManager.inst.SmoothMuscles.Add(this);
            }
            
        
        //calc health
        
        //between 0 and 5 (-5)
        // for 10 health guy
        // if health 0, its 0
        //if health is 5, its 2.5
        // if health is 10, it's 5

        calcHealthBar();

    }

    public void calcHealthBar()
    {
        float healthIncrement = 5 / _maxHealth;
        float position = (healthIncrement * _health) - 5;
        position = Mathf.Clamp(position, -5, 0);
        var barpos = healthBar.transform.localPosition;
        barpos.x = position;
        print(barpos.x);
        
        healthBar.transform.localPosition = barpos;
    }
    
    
    public void TakeDamage(float dmg)
    {
        if (!_anim.GetCurrentAnimatorStateInfo(0).IsName("Stem"))
        {
            _health -= dmg;
            calcHealthBar();
            if (_health <= 0)
            {
                AS.PlayOneShot(die);
                
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
                    AS.PlayOneShot(attackSound);
                    var damage = _unitDamages[(int)nextTarget.enemyType];

                    if (nextTarget)
                    {
                        nextTarget.TakeDamage(damage);
                    }
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

    public void Reset()
    {
        ReevaluateType(unit);
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

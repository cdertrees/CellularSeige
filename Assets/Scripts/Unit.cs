using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float _health;
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
    
    void Start()
    {   
        ReevaluateType(unit);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (attacks)
        {
            Attack();
        }
        tempText.text = "" + _health;
    }

    public void ReevaluateType(ScriptableUnit unitTemp)
    {
        var _unit = Instantiate(unitTemp);
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

        attacks = _unit.animation.name != "Stem";
        _cooldownTime = (_unit.coolDown/2);
        _health = _unit.health;
        _timer = _cooldownTime;

    }

    public void TakeDamage(float dmg)
    {
        _health -= dmg;
        if (_health <= 0)
        {
            print("i died");
            ReevaluateType(unit);
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

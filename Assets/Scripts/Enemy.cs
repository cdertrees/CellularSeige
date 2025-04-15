using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
   
    public float speed;
    public int moveIndex = 0;
    public float health = 1;
    private float _damage = 1;
    private float _cooldownTime = 0;
    public EnemyTypes enemyType;
    public Animator _anim;
    public ScriptablePathogen pathogen;
    private float _speedmod;
    [SerializeField]private float _timer = 0;

    public List<Unit> inRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _anim = GetComponent<Animator>();
       EvaluateType(pathogen);
    }

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
        
        Attack();
        
    }


    void Attack()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        } else {
            //attack
          
            if (inRange.Count() > 0)
            {
         
                print("Unit attacked");
                inRange[0].TakeDamage(_damage);
                
            }
           
            _timer = _cooldownTime;
        }
    }
    
    void EvaluateType(ScriptablePathogen _path)
    {
        print("reevaluating enemy");
        _anim.Play(_path.animation.name);
        _damage = _path.damage;
        _speedmod = _path.speed;
        enemyType = _path.enemyType;
        _cooldownTime = _path.coolDown;
        health = _path.health;
        _timer = _path.coolDown;
    }
    
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            GameManager.inst.currentPathogens.Remove(this.GameObject());
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

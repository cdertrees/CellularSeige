using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    private ScriptablePathogen pathogen;
    private int _dnaRewarded;

    public AudioSource AS;

    public AudioClip alert;
   // private float _speedmod;
    [SerializeField]private float _timer = 0;

    public TextMeshPro tempText;
    
    public List<Unit> inRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _anim = GetComponent<Animator>();

        pathogen = GameManager.inst.pathogenProbability[Random.Range(0, GameManager.inst.pathogenProbability.Count)];
        EvaluateType(pathogen);
    }

    private void Start()
    {
       // AS.PlayOneShot(alert);
    }

    void Update()
    {
        tempText.text = pathogen.name + "   " + health ;
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
         
                inRange[0].TakeDamage(_damage);
                
            }
           
            _timer = _cooldownTime;
        }
    }
    
    void EvaluateType(ScriptablePathogen _path)
    {
        _anim.Play(_path.anims[Random.Range(0,_path.anims.Count)].name);
        _damage = _path.damage;
        //_speedmod = _path.speed;
        enemyType = _path.enemyType;
        _cooldownTime = _path.coolDown;
        health = _path.health;
        _timer = _path.coolDown;
        _dnaRewarded = _path.dnaRewarded;
    }
    
    public void TakeDamage(float dmg)
    {
        GameManager.timesAttacked++;
        health -= dmg;
        if (health <= 0)
        {
            GameManager.inst.DNA += _dnaRewarded;
            GameManager.inst.dnaText.text = "DNA: " + GameManager.inst.DNA;
            GameManager.inst.currentPathogens.Remove(this.GameObject());
            GameManager.pathsKilled++;
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

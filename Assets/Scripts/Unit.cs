using System;
using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int health;
    public int damage;
    public float cooldownTime;
    private Animator anim;

    public Types type;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        type = Types.Stem;
        reevaluateType();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void reevaluateType()
    {
        //input info here
        switch (type)
        {
            case Types.Stem:
            {
                anim.Play("Stem");
                health = 10;
                damage = 1;
                return;
            }

            case Types.WhiteBlood:
            {
                anim.Play("WhiteBlood");
                health = 10;
                damage = 3;
                return;
            }
            
        }
    }
}
public enum Types
{
    Stem,
    WhiteBlood,

}

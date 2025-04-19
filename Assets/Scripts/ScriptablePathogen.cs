using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ScriptablePathogen", menuName = "Scriptable Objects/ScriptablePathogen")]
public class ScriptablePathogen : ScriptableObject
{
    public EnemyTypes enemyType;
    public int health;
    public float damage;
    public float coolDown;
    public float speed;
    public AnimationClip animation;
    public PathogenSpecial special;
    public int dnaRewarded;
}
public enum PathogenSpecial
{
    None
}

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inst = this;
    }

  
}

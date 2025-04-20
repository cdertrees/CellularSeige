using Unity.VisualScripting;
using UnityEngine;

public class ClickShop : MonoBehaviour
{
    public GameObject unit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void OnMouseDown()
    {
        GameManager.inst.startShopping(unit);
    }
}

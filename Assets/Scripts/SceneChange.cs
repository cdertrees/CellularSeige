using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Scene(string sceneChange)
    {
        SceneManager.LoadScene(sceneChange);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

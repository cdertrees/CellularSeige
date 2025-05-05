using UnityEngine;
using UnityEngine.UIElements;

public class AudioControl : MonoBehaviour
{
    public UnityEngine.UI.Slider Slider;
    private AudioSource AS;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AS = GetComponent<AudioSource>();
        AS.volume = Slider.value;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AS.volume = Slider.value;
    }
}

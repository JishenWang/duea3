using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class audio : MonoBehaviour
{
    public AudioSource bakcSource;
    public Slider slider1;
    void Start()
    {
        
    }


    void Update()
    {
        bakcSource.volume = slider1.value;
    }
}

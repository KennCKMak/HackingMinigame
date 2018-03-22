using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BGM {


    public string name;
    public AudioClip clip;

    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    [Range(0.1f, 3.0f)]
    public float pitch = 1.0f;
    
}

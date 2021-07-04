using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{

    [FMODUnity.EventRef] public string eventPath;

    private FMOD.Studio.EventInstance _instance;
    
    // Start is called before the first frame update
    void Start()
    {
        _instance = FMODUnity.RuntimeManager.CreateInstance(eventPath);
        _instance.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

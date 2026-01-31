using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SushiSpawner : MonoBehaviour
{
    public bool isOn = false;
    public float minTime = 1;
    public float maxTime = 5;
    public float targetTime = 0;
    public float curTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        if (isOn) {
            if (curTime>=targetTime) {
                targetTime = UnityEngine.Random.Range(minTime, maxTime);
                curTime = 0;
            }
            curTime += Time.fixedDeltaTime;
        }
    }

    protected void SpawnSushi () {
        
    }
}

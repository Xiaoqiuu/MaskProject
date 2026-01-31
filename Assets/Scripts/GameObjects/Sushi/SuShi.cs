using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuShi : MonoBehaviour
{
    public GameObject rice;
    public GameObject fish;
    public float surviveTime = 8f;
    public event Action OnFishAdded;
    public int type = 0;
    public int lvl = 0;
    public int bonus = 150;
    public List<Sprite> fishSprites;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, surviveTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

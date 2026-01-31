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
    public float bonus = 350;
    public bool hasAdd = false;

    public Sprite sprite;

    void Awake() {
        sprite = GetComponent<Sprite>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, surviveTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddFish() {
        if (hasAdd) return;
        hasAdd = true;
        Debug.Log("Add Fish!");
        OnFishAdded.Invoke();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuShi : MonoBehaviour {
    public GameObject rice;
    public GameObject fish;
    public float surviveTime = 8f;
    public event Action OnFishAdded;
    public int type = 0;
    public float bonus = 350;
    public bool hasAdd = false;
    
    private Image cover; // 盖子Image（自动查找）
    private GameObject fishDry; // 小鱼干对象（自动查找）
    private bool isSpecialMode = false; // 当前是否为奖励模式

    // Start is called before the first frame update
    void Start() {
        // 自动查找名为"Cover"的子对象
        Transform coverTransform = transform.Find("Cover");
        if (coverTransform != null) {
            cover = coverTransform.GetComponent<Image>();
            if (cover != null) {
                cover.gameObject.SetActive(false);
            }
            else {
                Debug.LogWarning($"[SuShi] 找到Cover对象但没有Image组件");
            }
        }
        else {
            Debug.LogWarning($"[SuShi] 预制体中找不到名为'Cover'的子对象");
        }
        
        // 自动查找名为"FishDry"的子对象（小鱼干）
        Transform fishDryTransform = transform.Find("FishDry");
        if (fishDryTransform != null) {
            fishDry = fishDryTransform.gameObject;
        }
        
        // 获取当前特殊模式状态
        if (GameManager.Instance != null) {
            isSpecialMode = GameManager.Instance.isSpecialMode;
            
            // 根据模式设置初始显示
            SetupVisualForMode();
        }
        
        Destroy(gameObject, surviveTime);
    }

    /// <summary>
    /// 根据当前模式设置显示
    /// </summary>
    private void SetupVisualForMode() {
        if (isSpecialMode) {
            // 奖励模式：隐藏寿司，显示小鱼干
            if (rice != null) rice.SetActive(false);
            if (fishDry != null) fishDry.SetActive(true);
        }
        else {
            // 普通模式：显示寿司，隐藏小鱼干
            if (rice != null) rice.SetActive(true);
            if (fishDry != null) fishDry.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() {

    }

    public void DoAddFish() {
        if (isSpecialMode) {
            // 奖励模式：隐藏小鱼干，传送空盘子
            if (fishDry != null) {
                fishDry.SetActive(false);
            }
        }
        else {
            // 普通模式：销毁fish，显示盖子
            Destroy(fish);
            
            if (cover != null) {
                cover.gameObject.SetActive(true);
            }
        }
    }

    public void AddFish() {
        if (hasAdd) {
            GameManager.Instance.Miss();
            return;
        }
        hasAdd = true;
        DoAddFish();
        
        Debug.Log("Add Fish!");
        OnFishAdded?.Invoke();
    }

    private void OnDestroy() {
        //if (!hasAdd) GameManager.Instance.Miss();
    }
}

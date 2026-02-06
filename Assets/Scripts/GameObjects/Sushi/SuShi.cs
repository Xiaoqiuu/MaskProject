using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuShi : MonoBehaviour {
    [SerializeField] private GameObject rice;
    [SerializeField] private GameObject fishDry; // 小鱼干对象
    [SerializeField] private GameObject cover; // 盖子Image
    public float surviveTime = 8f;
    public event Action OnFishAdded;
    public int type = 0;
    public float bonus = 564f;
    public bool hasAdd = false;
    public bool hasLose = false;
    public float speed = 564f;
    public float speedScale = 1f;

    public RectTransform rect;
    private bool isSpecialMode = false; // 当前是否在特殊模式

    // Start is called before the first frame update
    void Start() {
        // 订阅特殊模式变化事件
        if (GameManager.Instance != null) {
            GameManager.Instance.OnSpecialModeChanged += OnSpecialModeChanged;
            // 获取当前模式状态
            OnSpecialModeChanged(GameManager.Instance.isSpecialMode);
        }

        Destroy(gameObject, surviveTime);
    }

    void OnDestroy() {
        // 取消订阅
        if (GameManager.Instance != null) {
            GameManager.Instance.OnSpecialModeChanged -= OnSpecialModeChanged;
        }
    }

    /// <summary>
    /// 特殊模式状态改变回调
    /// </summary>
    private void OnSpecialModeChanged(bool isSpecial) {
        isSpecialMode = isSpecial;
        speedScale = isSpecial ? 1.25f : 1.0f;
        if (hasLose || hasAdd) return;
        if (isSpecial) {
            // 特殊模式：隐藏寿司，显示小鱼干
            if (rice != null) {
                rice.SetActive(false);
            }
            if (fishDry != null) {
                fishDry.SetActive(true);
            }
        }
        else {
            // 普通模式：显示寿司，隐藏小鱼干
            if (rice != null) {
                rice.SetActive(true);
            }
            if (fishDry != null) {
                fishDry.SetActive(false);
            }
        }
    }
   

    // Update is called once per frame
    void Update() {
        rect.anchoredPosition += speed * speedScale * Time.deltaTime * Vector2.right;
    }

    public void DoAddFish() {
        if (isSpecialMode) {
            // 特殊模式：隐藏小鱼干，传送空盘子
            if (fishDry != null) {
                fishDry.SetActive(false);
            }
        }
        else {
            // 普通模式：隐藏寿司，显示盖子
            if (cover != null) {
                cover.SetActive(true);
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

    public void Lose() {
        hasLose = true;
        GameManager.Instance.Lose();
    }

    public void SetType(Sprite sprite) {
        rice.GetComponent<Image>().sprite = sprite;
    }
}

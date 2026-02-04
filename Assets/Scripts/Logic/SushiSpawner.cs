using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SushiSpawner : MonoBehaviour {

    public bool isOn = false;
    public float minTime = 1;
    public float maxTime = 2;
    public float targetTime = 0;
    public float curTime = 0;
    [SerializeField] private SuShi suShiPrefab;
    [SerializeField] private List<Sprite> fishSprites;
    public GameObject belt;

    [Header("寿司大小设置")]
    [Tooltip("寿司的缩放比例")]
    public float sushiScale = 2.0f;

    private void Awake() {
        GameManager.Instance.sushiSpawner = this;
        GameManager.Instance.OnSpecialModeChanged += isSpecialMode => {
            Animator beletAnimator = belt.GetComponent<Animator>();
            beletAnimator.speed = isSpecialMode ? 1.25f : 1.0f;
        };
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {
        if (isOn) {
            if (curTime >= targetTime) {
                targetTime = UnityEngine.Random.Range(minTime, maxTime);
                curTime = 0;
                SpawnSushi();
            }
            curTime += Time.fixedDeltaTime;
        }
    }

    protected void SpawnSushi() {
        SuShi sushi = Instantiate<SuShi>(suShiPrefab, this.transform);
        SetSushiType(sushi, GameManager.Instance.GetSuShiType());

        // 设置寿司大小
        sushi.transform.localScale = Vector3.one * sushiScale;

        Debug.Log($"sushi Type = {sushi.type}");
        sushi.OnFishAdded += () => {
            GameManager.Instance.Success(sushi.bonus);
            Debug.Log("cur sushiCount = " + GameManager.Instance.sushiCount);
        };
    }

    protected void SetSushiType(SuShi sushi, int type) {
        sushi.type = type;
        if (type < this.fishSprites.Count && this.fishSprites[type] != null) sushi.SetType(fishSprites[type]);
        sushi.bonus = GameManager.Instance.GetBonus(type);
    }
}

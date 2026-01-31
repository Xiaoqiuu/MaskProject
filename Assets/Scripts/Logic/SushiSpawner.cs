using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SushiSpawner : MonoBehaviour {

    public bool isOn = false;
    public float minTime = 1;
    public float maxTime = 2;
    public float targetTime = 0;
    public float curTime = 0;
    public float originSpeed = 564f;
    public float speedScale = 1.0f;
    [SerializeField] private SuShi suShiPrefab;
    [SerializeField] private List<Sprite> fishSprites;
    public GameObject belt;

    private void Awake() {
        GameManager.Instance.sushiSpawner = this;
        GameManager.Instance.OnSpecialModeUpdated += () => {
            if (GameManager.Instance.isSpecialMode) {
                speedScale = 1.25f;
            }
            else {
                speedScale = 1.0f;
            }
            Animator beletAnimator = belt.GetComponent<Animator>();
            beletAnimator.speed = speedScale;
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
        Debug.Log($"sushi Type = {sushi.type}");
        sushi.GetComponent<Rigidbody2D>().velocity = Vector2.right * originSpeed * speedScale;
        sushi.OnFishAdded += () => {
            GameManager.Instance.Success(sushi.bonus);
            Debug.Log("cur sushiCount = " + GameManager.Instance.sushiCount);
        };
    }

    protected void SetSushiType(SuShi sushi, int type) {
        sushi.type = type;
        if (type < this.fishSprites.Count && this.fishSprites[type] != null) sushi.sprite = this.fishSprites[type];
        sushi.bonus = GameManager.Instance.GetBonus(type);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceObject : MonoBehaviour
{
    private SuShi TraceSuShi;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnTap += TryAddFish;
    }

    private void TryAddFish() {
        if (TraceSuShi) {
            TraceSuShi.AddFish();
        }
        else {
            GameManager.Instance.Miss();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, -transform.up);
        if (hitInfo) {
            //Debug.Log($"hit = {hitInfo.collider.gameObject.name}");
            if (!hitInfo.collider.gameObject.TryGetComponent<SuShi>(out TraceSuShi)) {
                TraceSuShi = null;
                //Debug.Log($"hit = Not SuShi");
            }
        }
        else {
            if (TraceSuShi != null && !TraceSuShi.hasAdd) {
                TraceSuShi.Miss();
            }
            TraceSuShi = null;
            //Debug.Log($"hit = Nothing");
        }
    }
}

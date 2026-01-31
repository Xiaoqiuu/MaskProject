using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceObject : MonoBehaviour
{
    private SuShi TraceSuShi;
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.OnPlayerInput += () => {
            if (TraceSuShi) {
                TraceSuShi.AddFish();
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, -transform.up);
        if (hitInfo) {
            Debug.Log(hitInfo.collider.gameObject.name);
            if (!hitInfo.collider.gameObject.TryGetComponent<SuShi>(out TraceSuShi)) {
                TraceSuShi = null;
            }
        }
    }
}

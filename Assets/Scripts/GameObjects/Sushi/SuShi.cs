using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuShi : MonoBehaviour
{
    public GameObject rice;
    public GameObject fish;
    public float surviveTime = 8f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SelfDestroy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SelfDestroy() {
        yield return new WaitForSeconds(surviveTime); // µÈ´ý2Ãë
        Destroy(this.gameObject);
    }
}

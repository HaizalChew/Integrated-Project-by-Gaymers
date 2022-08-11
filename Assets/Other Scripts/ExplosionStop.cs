using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionStop : MonoBehaviour
{
    public float timer = 5f;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= timer)
        {
            Destroy(this.gameObject);
        }
    }
}

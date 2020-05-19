using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{

    public GameObject a;
    public GameObject b;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapArea(a.transform.position, b.transform.position, LayerMask.NameToLayer("Player")))
        {
            //Debug.Log("Scenetrigger");

        }

        if (Physics2D.OverlapCircle(a.transform.position,1, LayerMask.GetMask("Player")))
        {
            Debug.Log("Scenetrigger");

        }

    }
}

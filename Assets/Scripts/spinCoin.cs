using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinCoin : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(new Vector3(0f, 5f, 0f));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 5f, 0f));
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.name == "Ethan") {
            this.gameObject.SetActive(false);
        }
        
    }
}

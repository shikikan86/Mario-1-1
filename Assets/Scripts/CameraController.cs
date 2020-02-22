using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject Player;
    public Transform tran;


    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Ethan");
        tran = Player.transform;
        Debug.Log(Player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate()
    {
        transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + 6, -11f);
    }
}

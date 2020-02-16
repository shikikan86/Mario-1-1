using UnityEngine;
using UnityEngine.EventSystems;

public class Ray : MonoBehaviour
{
    public float length = 300f;
    public LayerMask mask;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, length, mask))
            {
                if(hit.collider.name == "Brick(Clone)" || hit.collider.name == "Brick")
                {
                    Destroy(hit.collider.gameObject);
                }
               
               
                if (hit.collider.name == "Question(Clone)" || hit.collider.name == "Question")
                {
                    Destroy(hit.collider.gameObject);
                }

                Debug.Log(hit.collider.name);
            }
        }    
    }
}

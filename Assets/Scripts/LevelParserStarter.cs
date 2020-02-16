using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LevelParserStarter : MonoBehaviour
{
    public string filename;

    public GameObject Rock;

    public GameObject Brick;

    public GameObject QuestionBox;

    public GameObject Stone;

    public Transform parentTransform;

    //Raycast Stuff
    public float length = 300f;
    public LayerMask mask;

    public Text coins_text;
    public int coins = 0;

    public Text timer;
    public float start_time;
    // Start is called before the first frame update
    void Start()
    {
        RefreshParse();

        //Displays initial coin text with 2 digit positions
        string temp = string.Format("{0:00}", coins);
        coins_text.text = "x" + temp;

    }

    void Update()
    {

        //Updates the timer, subtracting time elapsed from 375. "f0" means no decimals.
        float t = 375f - Time.time;
        timer.text = "Time \n" + t.ToString("f0");

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            UnityEngine.Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, length, mask))
            {
                if (hit.collider.name == "Brick(Clone)" || hit.collider.name == "Brick")
                {
                    Destroy(hit.collider.gameObject);
                }


                if (hit.collider.name == "Question(Clone)" || hit.collider.name == "Question")
                {
                    coins = coins + 1;
                    string temp = string.Format("{0:00}", coins);
                    coins_text.text = "x" + temp;
                }

                Debug.Log(hit.collider.name);
            }
        }

        
    }


    private void FileParser()
    {
        string fileToParse = string.Format("{0}{1}{2}.txt", Application.dataPath, "/Resources/", filename);

        using (StreamReader sr = new StreamReader(fileToParse))
        {
            string line = "";
            int row = 0;

            while ((line = sr.ReadLine()) != null)
            {
                int column = 0;
                char[] letters = line.ToCharArray();
                Debug.Log(row + "," + column);
                Vector3 myvector = new Vector3(row, column, 0);
                foreach (var letter in letters)
                {
                    //Call SpawnPrefab
                    SpawnPrefab(letter, new Vector3(column,-row,0));
                    column++;
                }
                row++;

            }

            sr.Close();
        }
    }

    private void SpawnPrefab(char spot, Vector3 positionToSpawn)
    {
        GameObject ToSpawn;

        switch (spot)
        {
            case 'b': Debug.Log("Spawn Brick");
                ToSpawn = Brick;
                break;
            case '?': Debug.Log("Spawn QuestionBox");
                ToSpawn = QuestionBox;
                break;
            case 'x': Debug.Log("Spawn Rock");
                ToSpawn = Rock;
                break;
            case 's': Debug.Log("Spawn Stone");
                ToSpawn = Stone;
                break;
            //default: Debug.Log("Default Entered"); break;
            default: return;
                //ToSpawn = //Brick;       break;
        }

        ToSpawn = GameObject.Instantiate(ToSpawn, parentTransform);
        ToSpawn.transform.localPosition = positionToSpawn;
    }

    public void RefreshParse()
    {
        GameObject newParent = new GameObject();
        newParent.name = "Environment";
        newParent.transform.position = parentTransform.position;
        newParent.transform.parent = this.transform;
        
        if (parentTransform) Destroy(parentTransform.gameObject);

        parentTransform = newParent.transform;
        FileParser();
    }
}

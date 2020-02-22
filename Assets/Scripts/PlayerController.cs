using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //physics stuff
    float speed = 6f;
    float verticalSpeed;
    float jumpSpeed = 15;
    float gravity = 16;
    public int direction;
    Vector3 movement_direction = Vector3.zero;
    public Collider other;

    float t;

    CharacterController controller;

    Animator animate;

    bool inAir = false; //used for stopping the x movement upon landing after a jump
    bool death = false;
    bool win = false;

    public int score = 0;
    public int coins = 0;
    public Text score_text;
    public Text coins_text;
    public Text timer;
    public Text WinText;

    //Sounds
    public AudioClip Burn;
    public AudioClip wilhelm;
    public AudioClip rupee;
    public AudioClip song;
    public AudioClip opencan;
    public AudioClip victory;
    public AudioClip BrickSound;
    public AudioSource source;

    float y;

    //these store the string format before updating the text
    string temp;
    string temp2; 

    void Start()
    {
        temp = string.Format("{0:000000}", score);
        score_text.text = temp.ToString();
        temp2 = string.Format("{0:00}", coins);
        coins_text.text = temp2.ToString();
        controller = GetComponent<CharacterController>();
        animate = GetComponent<Animator>();
        direction = 1; //1 means right 0 means left
        source = GetComponent<AudioSource>();
        source.clip = song;
        source.PlayOneShot(source.clip);
        
        //other = GetComponent<Collider>();
    }

    void Update()
    {
        //timer counting down
        if (!death)
        {
            t = 100f - Time.time; //placed here so that upon losing, the block below will only execute once
        }
        
        
        //if the timer reaches zero, you lose
        if(t <= 0)
        {
            source.Stop();
            t = 1;
            death = true;
            source.clip = wilhelm;
            source.PlayOneShot(source.clip); //doesn't work and I don't know why
            score_text.text = "Player 1\nStatus: Withered";
            
            Time.timeScale = 0f; //freezes time (otherwise he falls through the map for some reason)
        }

        //update the text
        if (!death && !win)
        {
            temp = string.Format("{0:000000}", score);
            score_text.text = "Player 1\n" + temp.ToString();
            temp2 = string.Format("{0:00}", coins);
            coins_text.text = "x" + temp2.ToString();
            timer.text = "Time \n" + t.ToString("f0");
        }

        

        //if character is on the ground
        if (controller.isGrounded)
        {
            //when the player lands, make him stop moving
            if (inAir)
            {
                inAir = false;
                movement_direction = new Vector3(0, 0, 0);
                animate.SetInteger("isWalking", 0);
            }
            animate.SetInteger("inAir", 0);
            //movement to the right
            if (Input.GetKey(KeyCode.RightArrow))
            {
                if(direction == 1)
                {
                    animate.SetInteger("isWalking", 1);
                    movement_direction = new Vector3(1, 0, 0);
                    movement_direction *= speed;
                }
                //Rotates the character to face the correct direction
                else
                {
                    direction = 1;
                    transform.Rotate(0, 180, 0);
                }
                
                
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                animate.SetInteger("isWalking", 0);
                movement_direction = new Vector3(0, 0, 0);
            }
            //movement to the left
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if(direction == 0)
                {
                    animate.SetInteger("isWalking", 1);
                    movement_direction = new Vector3(-1, 0, 0);
                    movement_direction *= speed;
                }
                else
                {
                    direction = 0;
                    transform.Rotate(0, 180, 0);
                }
                
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                animate.SetInteger("isWalking", 0);
                movement_direction = new Vector3(0, 0, 0);
            }

            //Jump
            if (Input.GetKey(KeyCode.Space))
            {
                animate.SetInteger("isRunning", 0);
                speed = 6f;
                animate.SetInteger("isWalking", 0);
                movement_direction = new Vector3(movement_direction.x, jumpSpeed, 0);
                

            }
            if (Input.GetKey(KeyCode.Space))
            {
                animate.SetInteger("isWalking", 0);
                movement_direction = new Vector3(movement_direction.x, jumpSpeed, 0);

            }

            //Sprinting
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                animate.SetInteger("isRunning", 1);
                speed = 12f;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                animate.SetInteger("isRunning", 0);
                speed = 6f;
            }
        }

        //in the air
        else
        {
            inAir = true;
            animate.SetInteger("inAir", 1);
        }

        
        //If the player falls off the map
        if (y <= -20)
        {
            source.Stop();
            score_text.text = "Player 1\nStatus: Unknown";
            WinText.text = "Welcome to the Abyss";
            WinText.color = Color.blue;
            y = 100; //y can equal any value above -20, as long as this block is only called once
            death = true;
            source.clip = wilhelm;
            source.PlayOneShot(source.clip);
            Debug.Log("Falling");
        }

        //Makes the player disappear on death without deleting him
        //If the player is deleted, so is the audio source, so no death sounds will play
        if (death)
        {
            movement_direction = new Vector3(0, 0, -300); //throws the player off the map, making him "disappear"
            controller.Move(movement_direction); //effectively disables movement

        }

        //if everything is normal (Ethan is alive), gravity will be applied as normal
        else
        {
            movement_direction.y -= gravity * Time.deltaTime;
            controller.Move(movement_direction * Time.deltaTime);
            y = transform.position.y;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //colliding with a brick should make it disappear, but it's a bit wonky
        //It will only "collide" if the player is sprinting or jumping, which isn't my intention, but that's ok I guess
        if(collision.collider.name == "Brick(Clone)" || collision.collider.name == "Brick")
        {
            source.clip = BrickSound;
            source.PlayOneShot(source.clip);
            score += 100;
            Destroy(collision.collider.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Win Condition
        if(other.name == "Can(Clone)" || other.name == "Cylindre001" || other.tag == "Finish")
        {
            //without the !win, this block would be executed multiple times
            //This makes it only play once when you win.
            if (!win)
            {
                win = true;
                source.Stop();
                source.clip = opencan;
                source.PlayOneShot(source.clip);
                source.clip = victory;
                source.PlayOneShot(source.clip);
                Debug.Log("Win");
                WinText.text = "A Winner is You";
                WinText.color = Color.green;
                temp = string.Format("{0:000000}", score);
                score_text.text = "Player 1\n" + temp.ToString() + "\nStatus: Refreshed";
            }
            
        }
        //Coin pickup
        if(other.name == "Coin(Clone)" || other.name == "Coin")
        {
            source.clip = rupee;
            source.PlayOneShot(source.clip);
            coins = coins + 1;
            score = score + 100;
        }
        //Die by lava
        if(other.name == "Anomaly(Clone)" || other.name == "Anomaly")
        {
            score_text.text = "Player 1\nStatus: Crispy";
            WinText.text = "You are now a crispy critter";
            WinText.color = Color.red;
            source.Stop(); //stop the bgm
            death = true;
            Time.timeScale = 0f; //freezes the time effectively
            source.clip = Burn;
            source.PlayOneShot(source.clip);
            source.clip = wilhelm;
            source.PlayOneShot(source.clip);
            Debug.Log("Ouch");
        }

    }

}

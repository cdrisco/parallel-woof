using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.SceneManagement;

// movement and animation and collision for player character

public class PlayerController : MonoBehaviour
{
    //Movement Parameters
    private int speed;
    private int jumpForce;
    private float JUMP_DELAY = .24f;
    private float BARK_DELAY = .31f;
    private float LEFT_BOUND = -0.2200589f;

    private bool pressLeft;
    private bool pressRight;
    private bool pressJump;
    private bool pressBark;

    private Vector3 playerScale;
    private Rigidbody2D playerRB;

    private bool onGround = true;

    //Game Objects
    public GameObject dog;
    public GameObject happy;
    public GameObject sad;
    public ParticleSystem dustCloudLeft;
    public ParticleSystem dustCloudRight;
    public ParticleSystem dustCloudCentre;

    //Audio Clips
    private AudioSource playerAudio;
    public AudioClip walkingSound;
    public AudioClip jumpSound;
    public AudioClip barkSound;

    public GameObject obstacle;

    //Need particle effect for walking and jumping as well as one for switching the worlds. 
    public ParticleSystem worldSwitch;

    // can disable horizontal movement for the jumping delay
    public bool movementEnabled = true;
    public bool barkEnabled = true;
    public bool jumpEnabled = true;


    private bool isWalking = false;
    public bool isJumping = false;
    private bool isIdling = true;

    public bool happyScene;
    public bool sadScene;
    private bool FadeOutDone=false;

    private int curBark = 0;


    void Start()
    {
        speed = 5;
        jumpForce = 10;
        playerRB = GetComponent<Rigidbody2D>();

        //initialize player audio source
        playerAudio = GetComponent<AudioSource>();

        happyScene = happy.activeSelf;
        sadScene = false;

        // fade sad scene
        SpriteRenderer[] sadScenes = sad.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer SR in sadScenes)
        {
            SR.color = new Color32(255, 255, 255, 0);
        }

        AudioSource[] sadMusic = sad.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource AS in sadMusic)
        {
            AS.volume = 0;
        }
        AudioSource[] happyMusic = happy.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource AS in happyMusic)
        {
            AS.volume = 1;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Get player movement from horizontal and set the player scale. 
        playerScale = transform.localScale;
        pressLeft = Input.GetKey(KeyCode.LeftArrow);
        pressRight = Input.GetKey(KeyCode.RightArrow);
        pressJump = Input.GetKeyDown(KeyCode.UpArrow);
        pressBark = Input.GetKeyDown(".");

        //Checks for player movement every frame. 
        Move();
        SetAnim();
        if (pressBark && barkEnabled && onGround)
            StartCoroutine(DoBark());

        // Done fading scenes so disable past scene
        if (FadeOutDone ==true)
        {
            if (sadScene)
                happy.SetActive(false);
            else
                sad.SetActive(false);
        }

    }



    #region PLAYERCONTROL
    /*
     * Name: Move
     * Input: No Input
     * Output: No Output
     * Purpose: Moves the player, this includes jumping. This method also triggers the spine animation when moving.
     */
    private void Move()
    {

        // Move player and flip based on direction
        if (pressLeft && movementEnabled && transform.position.x> LEFT_BOUND)
        {
            //Make this dynamic if possible
            playerScale.x = -0.5f;

            transform.position += Vector3.right * -speed * Time.deltaTime;


        }
        else if (pressRight && movementEnabled)
        {
            //Make this dynamic if possible
            playerScale.x = 0.5f;

            transform.position += Vector3.right * speed * Time.deltaTime;
        }

        // Jump; checks if key was pressed and if player is on ground.
        if (pressJump && onGround && jumpEnabled)
        {
            movementEnabled = false;
            StartCoroutine(Jump());
        }

        //Sets the scale to flip player. 
        transform.localScale = playerScale;
    }

    private void SetAnim()
    {
        // Walk animation

        //Checks and plays spine animation
        if (Input.GetKeyDown(KeyCode.LeftArrow) && onGround && transform.position.x > LEFT_BOUND && movementEnabled)
        {
            dog.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Walk", true);
            isWalking = true;
            isIdling = false;
        }

        //Checks and plays spine animation
        if (Input.GetKeyDown(KeyCode.RightArrow) && onGround && movementEnabled)
        {
            dog.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Walk", true);
            isWalking = true;
            isIdling = false;
        }

        // jump animiation
        if (pressJump && onGround && jumpEnabled)
        {
            isJumping = true;
            isWalking = false;
            isIdling = false;
            dog.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Jump", false);
            onGround = false;
        }

        if (!Input.GetKey(KeyCode.RightArrow) && !pressLeft)
        {
            isWalking = false;
        }

        if (!isWalking && !isJumping && !isIdling)
        {
            dog.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Idle", true);
            isIdling = true;
        }

        //Debug.Log("IsWalking: " + isWalking + " isJumping: " + isJumping + " isIdling: "+isIdling);


    }

    IEnumerator Jump()
    {
        yield return new WaitForSeconds(JUMP_DELAY);
        playerRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        movementEnabled = true;
        isJumping = true;
        yield return null;
    }

    #endregion

    #region COLLISIONS

    /*
     * Name: OnCollisionEnter
     * Input: Collision Game Object
     * Output: No Output
     * Purpose: Handles collisions. 
     */
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("col: "+ collision.collider.name);
        if (collision.collider.tag == "Ground")
        {
            if (isJumping)
            {
                // Spawn dust cloud
                dustCloudLeft.transform.position = transform.position + new Vector3(-.1f, -3.8f, 0);
                dustCloudRight.transform.position = transform.position + new Vector3(-.1f, -3.8f, 0);
                dustCloudCentre.transform.position = transform.position + new Vector3(-.1f, -3.8f, -2);
                dustCloudLeft.Play();
                dustCloudRight.Play();
                dustCloudCentre.Play();

                // play jumping land sound
                playerAudio.PlayOneShot(jumpSound);
            }

            // Lands on ground
            onGround = true;
            isJumping = false;

            // if you are pressing left or right while you land, switch to walking animation
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                isWalking = true;
                dog.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Walk", true);
            }
        }

        if (collision.collider.tag == "CreditsTrigger")
        {
            Debug.Log("Credits");
            SceneManager.LoadScene("Credits");

        }
    }
    #endregion


    /*
 * Name: Bark
 * Input: No Input
 * Output: No Output
 * Purpose: Bark switch to switch worlds. Turns the grouping on or off depending on 
 */


    IEnumerator DoBark()
    {
        // if first bark, then hide instructions
        curBark++;
        if (curBark > 0)
            GameManager.instance.HideInstructions();

        FadeOutDone = false;
        if (!sadScene && happyScene)
        {
            sadScene = true;
            happyScene = false;
            sad.SetActive(true);
        }
        else if (sadScene && !happyScene)
        {
            sadScene = false;
            happyScene = true;
            happy.SetActive(true);
        }


        dog.GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "Bark", false);
        yield return new WaitForSeconds(BARK_DELAY);
        playerAudio.PlayOneShot(barkSound);
        yield return new WaitForSeconds(BARK_DELAY);

        if (sadScene)
        {
            //Add particle effect to whole scene / transistion
            
            FadeScene();
            AudioSource[] sadMusic = sad.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource AS in sadMusic)
            {
                AS.volume = 1;
            }
            AudioSource[] happyMusic = happy.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource AS in happyMusic)
            {
                AS.volume = 0;
            }
        }
        else if (happyScene)
        {
            //Add particle effect to whole scene / transistion
            
            FadeScene();
            AudioSource[] sadMusic = sad.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource AS in sadMusic)
            {
                AS.volume = 0;
            }
            AudioSource[] happyMusic = happy.GetComponentsInChildren<AudioSource>();
            foreach (AudioSource AS in happyMusic)
            {
                AS.volume = 1;
            }
        }

        yield return null;
    }


    private void FadeScene()
    {
        if (sadScene==true)
        {
            SpriteRenderer[] happyScenes = happy.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer SR in happyScenes)
            {
                StartCoroutine(FadeOut(SR));
            }

            SpriteRenderer[] sadScenes = sad.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer SR in sadScenes)
            {
                StartCoroutine(FadeIn(SR));
            }
        }
        else
        {
            SpriteRenderer[] happyScenes = happy.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer SR in happyScenes)
            {
                StartCoroutine(FadeIn(SR));
            }

            SpriteRenderer[] sadScenes = sad.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer SR in sadScenes)
            {
                StartCoroutine(FadeOut(SR));
            }
        }
        
            
    }

    IEnumerator FadeOut(SpriteRenderer SR)
    {

        for (int a=255; a>0; a=a-5)
        {
            //Debug.Log("Color: " + a);
            SR.color = new Color32(255, 255, 255, (byte)a);
            
            yield return null;
        }

        SR.color = new Color32(255, 255, 255, 0);
        FadeOutDone = true;
        yield return null;
    }

    IEnumerator FadeIn(SpriteRenderer SR)
    {

        for (int a = 0; a < 255; a += 5)
        {
            //Debug.Log("Color: " + a);
            SR.color = new Color32(255, 255, 255, (byte)a);

            yield return null;
        }

        SR.color = new Color32(255, 255, 255, 255);
        yield return null;
    }

    public void DisablePlayerControl()
    {
        // disable player control
        movementEnabled = false;
        jumpEnabled = false;
        barkEnabled = false;
    }


}

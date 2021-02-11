using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

// attached to each of the 3 animals who want you to collect
// each animal has their own logic in a function that is called from Update()


public class Animals : MonoBehaviour
{
    public List<GameObject> animal;

    public GameObject player;
    public GameObject eventScript;

    private float interactDistance = 10.5f;

    private SpriteRenderer[] BubbleMice;

    private int owlTasksCompleted;
    private int beaverTasksCompleted;
    private int bunnyTasksCompleted;

    private GameObject owl;
    private GameObject bunny;

    // Start is called before the first frame update
    void Start()
    {
        owl = animal[0];
        bunny = animal[1];
        BubbleMice = owl.GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Owl();
        Bunny();
        Beaver();
    }

    private void Owl()
    {

        float distance = DistX(player, owl);
        //Debug.Log("Dist: " + distance);
        //Debug.Log("Complete: "+ owl.GetComponent<AnimalCollectionScript>().TasksComplete);

        if (distance < interactDistance && owl.GetComponent<AnimalCollectionScript>().bubbleDisabled == false)
        {
            GameObject bubble = GameObject.Find("ThoughtBubbleOwl");
            if (bubble != null)
            {
                if (bubble.GetComponent<SpriteRenderer>().enabled == false)
                {
                    // count collected
                    owlTasksCompleted = owl.GetComponent<AnimalCollectionScript>().CountCollected();

                    bubble.GetComponent<BubbleScript>().EnableBubble(owlTasksCompleted);
                    owl.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Interact", true);
                    // play flap once
                    owl.GetComponent<AudioSource>().loop = false;
                    owl.GetComponent<AudioSource>().Play();
                    // enable collectables
                    owl.GetComponent<AnimalCollectionScript>().EnableCollectables();
                }
            }
        }
        else
        {
            GameObject bubble = GameObject.Find("ThoughtBubbleOwl");
            if (bubble != null && owl.GetComponent<AnimalCollectionScript>().bubbleDisabled == false)
            {
                if (bubble.GetComponent<SpriteRenderer>().enabled == true)
                {
                    bubble.GetComponent<BubbleScript>().DisableBubble();
                    owl.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Idle", true);
                }
            }
        }
    }

    private void Beaver()
    {
        GameObject beaver = animal[2];
        float distance = DistX(player, beaver);
        if (distance < interactDistance && beaver.GetComponent<AnimalCollectionScript>().bubbleDisabled == false)
        {
            GameObject bubble = GameObject.Find("ThoughtBubbleBeaver");
            if (bubble != null)
            {
                if (bubble.GetComponent<SpriteRenderer>().enabled == false)
                {
                    // count collected
                    beaverTasksCompleted = beaver.GetComponent<AnimalCollectionScript>().CountCollected();

                    bubble.GetComponent<BubbleScript>().EnableBubble(beaverTasksCompleted);
                    beaver.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Interact", true);
                    // play thump once
                    beaver.GetComponent<AudioSource>().loop = false;
                    beaver.GetComponent<AudioSource>().Play();
                    // enable collectables
                    beaver.GetComponent<AnimalCollectionScript>().EnableCollectables();
                }
            }
        }
        else
        {
            GameObject bubble = GameObject.Find("ThoughtBubbleBeaver");
            if (bubble != null && beaver.GetComponent<AnimalCollectionScript>().bubbleDisabled == false)
            {
                if (bubble.GetComponent<SpriteRenderer>().enabled == true)
                {
                    bubble.GetComponent<BubbleScript>().DisableBubble();
                    beaver.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Idle", true);
                }
            }
        }
    }

    private void Bunny()
    {
        GameObject bunny = animal[1];
        float distance = DistX(player, bunny);
        if (distance < interactDistance && bunny.GetComponent<AnimalCollectionScript>().bubbleDisabled == false)
        {
            GameObject bubble = GameObject.Find("ThoughtBubbleBunny");
            if (bubble != null)
            {
                if (bubble.GetComponent<SpriteRenderer>().enabled == false)
                {
                    // count collected
                    bunnyTasksCompleted = bunny.GetComponent<AnimalCollectionScript>().CountCollected();

                    bubble.GetComponent<BubbleScript>().EnableBubble(bunnyTasksCompleted);
                    bunny.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Interact", true);
                    // play hop once
                    bunny.GetComponent<AudioSource>().loop = false;
                    bunny.GetComponent<AudioSource>().Play();
                    // enable collectables
                    bunny.GetComponent<AnimalCollectionScript>().EnableCollectables();
                }
            }
        }
        else
        {
            GameObject bubble = GameObject.Find("ThoughtBubbleBunny");
            if (bubble != null && bunny.GetComponent<AnimalCollectionScript>().bubbleDisabled == false)
            {
                if (bubble.GetComponent<SpriteRenderer>().enabled == true)
                {
                    bubble.GetComponent<BubbleScript>().DisableBubble();
                    bunny.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Idle", true);
                }
            }
        }
    }

    // Get Player's horizontal distance from animal
    public float DistX(GameObject player, GameObject animal)
    {
        //return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y), new Vector2(animal.transform.position.x, animal.transform.position.y));
        return Mathf.Abs(player.transform.position.x - animal.transform.position.x);
    }

    // sequence for when owl flies dog over rock
    public void FlyDog()
    {
        GameObject.Find("Dog").GetComponent<PlayerController>().isJumping = true; // to make dust clouds when he drops

        StartCoroutine(WalkDog(new Vector2(48.5f, 0), 1.2f));


        // play owl flying animation
        owl.GetComponent<Animation>().Play("FlyDown");
        owl.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Fly", true);

        // play flap sound looping
        owl.GetComponent<AudioSource>().loop = true;
        owl.GetComponent<AudioSource>().Play();

        StartCoroutine(FlyUp());
    }

    // owl flying sequence
    private IEnumerator FlyUp()
    {
        yield return new WaitForSeconds(2.6f);

        yield return new WaitForSeconds(0.4f);
        Debug.Log("Fly up");
        owl.GetComponent<Animation>().Play("FlyUp");

        Debug.Log("pu dog " + owl.transform.name);
        player.transform.parent = owl.transform;
        // set dog anim to fly loop
        player.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "FlyLoop", true);
        // disable dog collider
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        player.GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(10);
        owl.SetActive(false);

        yield return null;
    }

    // bunny hopping in hole sequence
    public IEnumerator HopInHole()
    {
        // play bunny hopping anim
        yield return new WaitForSeconds(1);
        bunny.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Hop", false);
        bunny.GetComponent<AnimalCollectionScript>().DogDownHole();
        yield return new WaitForSeconds(2.9f);
        // make bunny disappear
        bunny.GetComponentInChildren<MeshRenderer>().enabled = false;
    }

    // make dog walk to destination
    public IEnumerator WalkDog(Vector2 Dest, float speed)
    {
        // face correct direction
        if (Dest.x < player.transform.position.x)
        {
            player.transform.localScale = new Vector3(-0.5f, 0.5f, 1.0f);
            Dest += new Vector2(-1.1f, 0);
        }
        
        if (Dest.x > player.transform.position.x)
            player.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);

        // set to walk anim
        player.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Walk", true);
        yield return null;
        Debug.Log("Dest: " + Dest.x + " cur: " + player.transform.position.x);
        // Move to dest
        while (Mathf.Abs(player.transform.position.x - Dest.x) > 0.04f)
        {
            Debug.Log("Dest: " + Dest.x + " cur: " + player.transform.position.x);
            if (Dest.x < player.transform.position.x)
            {
                player.transform.position += Vector3.right * -speed * Time.deltaTime;
                //Debug.Log("left");
            }
            else
                if (Dest.x > player.transform.position.x)
            {
                player.transform.position += Vector3.right * speed * Time.deltaTime;
                //Debug.Log("right");
            }
            yield return null;
        }

        Debug.Log("Reached Dest");
        // set to dog idle anim
        player.GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Idle", true);

        yield return null;
    }




}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string name;
    public Sprite sprite;
}


public class BubbleScript : MonoBehaviour
{
    Vector2 endScale;
    private float ScaleSpeed = 0.5f;

    public Item[] items;

    public GameObject Obstacle;
    public int TasksToComplete;

    private SpriteRenderer[] itemSpriteRenderers;
    private Vector2 FullScale;

    private int Checkmarks;

    private GameObject Animals;
    private GameObject player;

    private void Start()
    {
        // Get items
        itemSpriteRenderers = transform.GetComponentsInChildren<SpriteRenderer>();

        FullScale = transform.localScale;

        // count checkmarks
        Checkmarks = 0;
        foreach (SpriteRenderer SR in itemSpriteRenderers)
        {
            if (SR.tag == "Checkmark")
                Checkmarks += 1;
        }

        Animals = GameObject.Find("Animals");
        player = GameObject.Find("Dog");
    }

    // Note: Checkmarks must be prdered before the itemGraphics in the hierarchy in the editor
    // They should also be ordered in procession from left to right
    public void EnableBubble(int TasksCompete)
    {
        StartCoroutine(GrowBubble(FullScale, TasksCompete));
        SpriteRenderer SR;
        //Debug.Log("itemSpriteRenderers.Length: " + itemSpriteRenderers.Length);
        TasksToComplete = 0;
        // display checkmarks
        for (int index=0;index<itemSpriteRenderers.Length;index++)
        {
            SR = itemSpriteRenderers[index];
            if (SR.tag == "Checkmark")
            {
                TasksToComplete++;
                if (TasksCompete >= index)
                    SR.enabled = true;
            }
            else
                SR.enabled = true;
        }
    }

    private IEnumerator GrowBubble(Vector2 EndScale,int TasksComplete)
    {
        float t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / ScaleSpeed;
            transform.localScale = Vector2.Lerp(new Vector2(0,0), EndScale, Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }
        transform.localScale = EndScale;
        // if all objects have been found then remove obstacle
        if (TasksComplete == TasksToComplete)
        {
            TasksAreCompleted();
        }
        yield return null;
    }

    private void TasksAreCompleted()
    {
        // TODO: play task completed sound
        Debug.Log("Tasks completed!");
        transform.parent.GetComponent<AnimalCollectionScript>().TasksComplete = true;

        // if beaver, then enable dam and disable obstacle
        if (transform.parent.name == "Beaver")
        {
            //Display dam
            GameObject.Find("Dam").GetComponent<SpriteRenderer>().enabled = true;
            // play build dam sound
            GameObject.Find("Dam").GetComponent<AudioSource>().Play();
           // disable obstacle 
           Obstacle.SetActive(false);
        }

        // if owl, fly dog over boulder
        if (transform.parent.name == "Owl" && player.GetComponent<PlayerController>().sadScene==true && player.GetComponent<PlayerController>().happyScene==false)
        {
            //Debug.Log("sadScene: "+ player.GetComponent<PlayerController>().sadScene);
            player.GetComponent<PlayerController>().DisablePlayerControl();
            Animals.GetComponent<Animals>().FlyDog();
            StartCoroutine(ShrinkBuble(transform.localScale));
            transform.parent.GetComponent<AnimalCollectionScript>().bubbleDisabled = true;
            //Debug.Log("BubbleDisabled");
        }

        // if bunny, move bunny over to reveal hole, make dog walk to hole, disappear, and re-appear on other side of tree on the other hole.
        if (transform.parent.name == "Bunny" && player.GetComponent<PlayerController>().happyScene == true && player.GetComponent<PlayerController>().sadScene == false)
        {
            //Debug.Log("sadScene: " + player.GetComponent<PlayerController>().sadScene);
            player.GetComponent<PlayerController>().DisablePlayerControl();
            
            // disable dog collider
            player.GetComponent<Rigidbody2D>().gravityScale = 0;
            player.GetComponent<Collider2D>().enabled = false;

            StartCoroutine(Animals.GetComponent<Animals>().WalkDog(new Vector2(181.9f, 0),2.1f));
            StartCoroutine(Animals.GetComponent<Animals>().HopInHole());
            StartCoroutine(ShrinkBuble(transform.localScale));
            transform.parent.GetComponent<AnimalCollectionScript>().bubbleDisabled = true;
            Debug.Log("BubbleDisabled");
        }
    }


    public void DisableBubble()
    {
        StartCoroutine(GetComponent<BubbleScript>().ShrinkBuble(transform.localScale));
    }


    private IEnumerator ShrinkBuble(Vector2 startScale)
    {
        float t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / ScaleSpeed;
            transform.localScale = Vector2.Lerp(startScale, new Vector2(0, 0), Mathf.SmoothStep(0.0f, 1.0f, t));
            //Debug.Log("ShrinkBuble:" + bubble.transform.localScale);
            yield return null;
        }
        //transform.localScale = new Vector2(0, 0);

        //bubble.GetComponent<SpriteRenderer>().enabled = false;
        // disable SpriteRenderers for bubble, items and checkmarks
        foreach (SpriteRenderer SR in itemSpriteRenderers)
        {
            SR.enabled = false;
        }

        yield return null;
    }



}

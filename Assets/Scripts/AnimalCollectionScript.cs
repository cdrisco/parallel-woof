using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

// This script is for the items you need to collect for an animal and the events that occur when you collect all items

public class AnimalCollectionScript : MonoBehaviour
{
    public GameObject[] collectibles;
    public bool TasksComplete=false;
    private GameObject player;
    public bool bubbleDisabled = false;

    private void Start()
    {
        player = GameObject.Find("Dog");
    }

    public void EnableCollectables()
    {
        foreach(GameObject GO in collectibles)
        {
            GO.SetActive(true);
        }
    }


    public int CountCollected()
    {
        int collected = 0;

        foreach (GameObject GO in collectibles)
        {
            if (GO.GetComponent<CollectibleScript>().collected==true)
                collected += 1;
        }
        return collected;
    }


    // This is triggered by the animation clip attached to owl
    public void PlayPickupDogAnim()
    {
        Debug.Log("pickup dog anim");
        GameObject.Find("Dog").GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Fly", false);
    }

    // gets called from owl flyUp animation clip
    public void DropDog()
    {
        Debug.Log("Drop Dog");
        // disable dog collider
        player.GetComponent<Rigidbody2D>().gravityScale = 1;
        player.GetComponent<Collider2D>().enabled = true;
        player.transform.parent = null;
        StartCoroutine(SetIdleAnim());
    }

    // Only called by the bunny in Animals script
    public void DogDownHole()
    {
        StartCoroutine(Tumble());
        float speed = 4.5f;
        StartCoroutine(RunDog(new Vector3(190, 0, -1.85f), speed));
    }

    IEnumerator Tumble()
    {
        // face right
        player.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);

        yield return new WaitForSeconds(1);

        GameObject.Find("Dog").GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Tumble", false);

        yield return new WaitForSeconds(1.5f);
        Vector3 startPos = player.transform.position;

        yield return null;
    }

    // make dog run to destination
    public IEnumerator RunDog(Vector2 Dest, float speed)
    {
        yield return new WaitForSeconds(2.5f);
        Debug.Log("Run Dest: " + Dest.x + " cur: " + player.transform.position.x);
        // Move to dest
        while (Mathf.Abs(player.transform.position.x - Dest.x) > 0.04f)
        {
            Debug.Log("Dest: " + Dest.x + " cur: " + player.transform.position.x);
            if (Dest.x < player.transform.position.x)
            {
                player.transform.position += Vector3.right * -speed * Time.deltaTime;
                Debug.Log("left");
            }
            else
                if (Dest.x > player.transform.position.x)
            {
                player.transform.position += Vector3.right * speed * Time.deltaTime;
                Debug.Log("right");
            }
            yield return null;
        }

        // pop out of hole
        yield return new WaitForSeconds(1.5f);
        player.transform.position= new Vector3(206.56f, 0, -1.855155f);
        GameObject.Find("Dog").GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "PopOutOfHole", false);

        Debug.Log("Reached Dest");

        player.GetComponent<Rigidbody2D>().gravityScale = 1;
        player.GetComponent<Collider2D>().enabled = true;

        StartCoroutine(SetIdleAnim());

        yield return null;
    }


    IEnumerator SetIdleAnim()
    {
        yield return new WaitForSeconds(1.1f);
        GameObject.Find("Dog").GetComponentInChildren<SkeletonAnimation>().AnimationState.SetAnimation(0, "Idle", false);

        // disable player control
        player.GetComponent<PlayerController>().movementEnabled = true;
        player.GetComponent<PlayerController>().jumpEnabled = true;
        player.GetComponent<PlayerController>().barkEnabled = true;

        yield return null;
    }

}

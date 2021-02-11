using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script attached to each collectible item
public class CollectibleScript : MonoBehaviour
{

    private float seconds = 1f;
    public bool collected=false;

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.tag == "Player" && collected==false)
        {
            collected = true;
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(Shrink(transform.localScale));
            StartCoroutine(SmoothMoveCurved(transform.position, col.collider.transform.position));

            // play collectitem sound
            GetComponent<AudioSource>().Play();
        }
    }


    private IEnumerator Shrink(Vector2 startScale)
    {
        float t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            transform.localScale = Vector2.Lerp(startScale, new Vector2(0, 0), Mathf.SmoothStep(0.0f, 1.0f, t));
            yield return null;
        }

       // Destroy(gameObject);

        yield return null;
    }

    private IEnumerator SmoothMoveCurved(Vector3 startpos, Vector3 endpos)
    {
        float t = 0.0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            gameObject.transform.localPosition = Vector3.Slerp(startpos, endpos, Mathf.SmoothStep(0.0f, 1.0f, t));

            if (Vector3.Distance(gameObject.transform.position, endpos) < 1)
            {
                gameObject.transform.localPosition = endpos;
            }
            //Debug.Log("SmoothMove:" + ObjectToMove.transform.position);
            yield return null;
        }
    }


}



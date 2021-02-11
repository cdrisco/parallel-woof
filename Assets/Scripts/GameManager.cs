using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int mouse;

    public GameObject title;
    public GameObject player;
    public GameObject cabin;

    private bool titleGone;

    public GameObject instructions;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        //sticks = 0;
        mouse = 0;
    }

    // Update is called once per frame
    void Update()
    {
        StartGame();
        ObstaclePass();
        EndGame();
    }

    public void StartGame()
    {
        if (DistX(player, title) > 8 && titleGone==false )
        {
            titleGone = true;
            StartCoroutine(FadeOut(title.transform.GetChild(0).GetComponent<SpriteRenderer>()));
//            title.SetActive(false);
        }

        //Debug.Log(DistX(player, title));

    }

    public float DistX(GameObject player, GameObject animal)
    {
        //return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y), new Vector2(animal.transform.position.x, animal.transform.position.y));
        return Mathf.Abs(player.transform.position.x - animal.transform.position.x);
    }

    public void EndGame()
    {
        if (player.transform.position.x==15)
        {
            SceneManager.LoadScene("End");
        }
    }

    public void ObstaclePass()
    {
        if (mouse == 3)
        {
            player.transform.position = new Vector3(player.transform.position.x + 10, player.transform.position.y, player.transform.position.z);
        }
    }


    public float Dist(GameObject player, GameObject gameObject)
    {

        return Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y), new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
    }



    IEnumerator FadeOut(SpriteRenderer SR)
    {

        for (int a = 255; a > 0; a = a - 5)
        {
            //Debug.Log("Color: " + a);
            SR.color = new Color32(255, 255, 255, (byte)a);

            yield return null;
        }

        SR.color = new Color32(255, 255, 255, 0);
        yield return null;
    }

    public void HideInstructions()
    {
        instructions.SetActive(false);
    }


}

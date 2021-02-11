using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public PlayerController playerControllerScript;
    private float followSpeed;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        followSpeed = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerControllerScript.transform.position.x, transform.position.y, transform.position.z), (elapsedTime / followSpeed));
        elapsedTime += Time.deltaTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CameraController : MonoBehaviour
{
    Animator cameraAnim;
    GameObject cam;
    Transform playerPos;
    Vector3 playPos;

    public Vector3 offset;
    [Range(0, 100)]
    public float smoothSpeed;


    // Start is called before the first frame update
    void Awake()
    {
        cam = GameObject.Find("Main Camera");
        cameraAnim = cam.GetComponent<Animator>();

        GameObject player = GameObject.Find("Player");
        playerPos = player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowPlayer();
        //transform.position = new Vector3(playerPos.position.x, yOffset, transform.position.z);


    }

    public void FollowPlayer()
    {
        playPos = playerPos.transform.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, playPos, smoothSpeed * Time.deltaTime);

        transform.position = smoothPos;
    }

    public void Shake()
    {
        cameraAnim.SetTrigger("Shake1");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CameraController : MonoBehaviour
{
    Animator cameraAnim;
    GameObject cam;
    Transform playerPos;
    Vector3 playPos;
    Vector3 smoothPos;
    float camZoomTar;
    float camZoomPos;

    public Vector3 offset;
    [Range(0, 100)]
    public float defaultSmoothSpeed;
    public float smoothSpeed;
    public float zoomOffset;


    // Start is called before the first frame update
    void Awake()
    {
        cam = GameObject.Find("Main Camera");
        cameraAnim = cam.GetComponent<Animator>();

        GameObject player = GameObject.Find("Player");
        playerPos = player.GetComponent<Transform>();

        smoothSpeed = defaultSmoothSpeed;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowPlayer();
        //CamZoom();
        //transform.position = new Vector3(playerPos.position.x, yOffset, transform.position.z);


    }

    public void CamZoom()
    {

        var c = cam.GetComponent<Camera>();
        camZoomTar = c.orthographicSize + zoomOffset;
        camZoomPos = Mathf.Lerp(c.orthographicSize, camZoomTar, smoothSpeed * Time.deltaTime);
        c.orthographicSize = camZoomPos;

    }

    public void FollowPlayer()
    {
        playPos = playerPos.transform.position + offset;
        smoothPos = Vector3.Lerp(transform.position, playPos, smoothSpeed * Time.deltaTime);
        //cam.GetComponent<Camera>().orthographicSize = 

        transform.position = smoothPos;
    }

    public void Shake()
    {
        cameraAnim.SetTrigger("Shake1");
    }

    public void Zoom(float zoom)
    {
        var c = cam.GetComponent<Camera>();
        offset.y -= zoom;
        c.orthographicSize -= zoom;
    }
}

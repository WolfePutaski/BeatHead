using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SC_CameraController : MonoBehaviour
{
    

    Animator cameraAnim;
    public GameObject cam;
    Transform playerPos;
    Vector3 playPos;
    Vector3 smoothPos;
    float camZoomTar;
    float camZoomPos;

    public GameObject[] camList;
    public string activeCam;

    public Vector3 offset;
    [Range(0, 100)]
    public float defaultSmoothSpeed;
    public float smoothSpeed;
    public float zoomOffset;


    // Start is called before the first frame update
    void Awake()
    {
        Application.targetFrameRate = 60;
        camList = GameObject.FindGameObjectsWithTag("MainCamera");


        GameObject player = GameObject.Find("Player");
        playerPos = player.GetComponent<Transform>();

        smoothSpeed = defaultSmoothSpeed;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowPlayer();
        ActiveCamera();

        //CamZoom();
        //transform.position = new Vector3(playerPos.position.x, yOffset, transform.position.z);


    }

    public void ActiveCamera()
    {
        foreach (GameObject a in camList)
        {
            if (a.name == activeCam)
            {
                a.SetActive(true);
                cam = GameObject.Find(activeCam);
                cameraAnim = cam.GetComponent<Animator>();
            }
            else
            {
                a.SetActive(false);
            }
        }
    }

    //public void CamZoom()
    //{

    //    var c = cam.GetComponent<Camera>();
    //    camZoomTar = c.orthographicSize + zoomOffset;
    //    camZoomPos = Mathf.Lerp(c.orthographicSize, camZoomTar, smoothSpeed * Time.deltaTime);
    //    c.orthographicSize = camZoomPos;

    //}

    public void FollowPlayer()
    {
        playPos = playerPos.transform.position + offset;
        smoothPos = Vector3.Lerp(transform.position, playPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothPos;
    }

    public void Shake()
    {
        cameraAnim.SetTrigger("Shake1");
    }

    //public void Zoom(float zoom)
    //{
    //    var c = cam.GetComponent<Camera>();
    //    offset.y -= zoom;
    //    c.orthographicSize -= zoom;
    //}
}

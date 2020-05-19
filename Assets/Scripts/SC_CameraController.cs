using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CameraState
{
    Gameplay, Cinematic
}

public class SC_CameraController : MonoBehaviour
{

    public CameraState camState;
    Animator cameraAnim;
    public GameObject cam;
    public GameObject playerPos;
    Vector3 playPos;
    Vector3 smoothPos;
    float camZoomTar;
    float camZoomPos;

    //public GameObject[] camList;
    //public string activeCam;

    public bool isFollowing;

    public Vector3 offset;
    [Range(0, 100)]
    public float defaultSmoothSpeed;
    public float smoothSpeed;
    public bool isZoom;
    public Vector3 defaultOffset;
    public Vector3 zoomOffset;

    public float camDefaultSize;
    public float camZoomSize;
    public float camZoomSpeed;

    public Animator blackBars;

    [Header("BGM")]
    public AudioClip BGM;


    // Start is called before the first frame update
    void Awake()
    {

        //camList = GameObject.FindGameObjectsWithTag("MainCamera");

        cam = Camera.main.gameObject;
        GameObject player = GameObject.Find("Player");
        //playerPos = player;
        cameraAnim = cam.GetComponent<Animator>();
        smoothSpeed = defaultSmoothSpeed;

    }

    void Start()
    {
        //QualitySettings.vSyncCount = 2;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isFollowing)
            {
            FollowPlayer();

        }
        //ActiveCamera();

        //CamZoom();
        //transform.position = new Vector3(playerPos.position.x, yOffset, transform.position.z);

        if (camState == CameraState.Gameplay)
        {
            blackBars.SetBool("isCinematic", false);

            Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("UI");

            if (isZoom)
            {
                offset = Vector3.Lerp(offset, zoomOffset, 1);
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, camZoomSize, 1);

            }
            else
            {
                offset = Vector3.Lerp(offset, defaultOffset, 1);
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, camDefaultSize, 1);
            }
        }


        if (camState == CameraState.Cinematic)
        {
            blackBars.SetBool("isCinematic", true);

            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));

            if (isZoom)
            {
                offset = Vector3.Lerp(offset, zoomOffset, Time.deltaTime*2);
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, camZoomSize, Time.deltaTime);

            }
            else
            {
                offset = Vector3.Lerp(offset, defaultOffset, Time.deltaTime*2);
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, camDefaultSize, Time.deltaTime);
                
            }
        }


    }

    //public void ActiveCamera()
    //{
    //    foreach (GameObject a in camList)
    //    {
    //        if (a.name == activeCam)
    //        {
    //            a.SetActive(true);
    //            cam = GameObject.Find(activeCam);
    //            cameraAnim = cam.GetComponent<Animator>();
    //        }
    //        else
    //        {
    //            a.SetActive(false);
    //        }
    //    }
    //}

    //public void CamZoom()
    //{

    //    var c = cam.GetComponent<Camera>();
    //    camZoomTar = c.orthographicSize + zoomOffset;
    //    camZoomPos = Mathf.Lerp(c.orthographicSize, camZoomTar, smoothSpeed * Time.deltaTime);
    //    c.orthographicSize = camZoomPos;

    //}

    public void FollowPlayer()
    {
        playPos = playerPos.transform.position;
        smoothPos = Vector3.Lerp(smoothPos, playPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothPos + offset;
    }

    public void Shake()
    {
        cameraAnim.SetTrigger("Shake1");
    }

    public void ChangeTarget(string targetName)
    {
        playerPos = GameObject.Find(targetName);
    }

    //public void Zoom(float zoom)
    //{
    //    var c = cam.GetComponent<Camera>();
    //    offset.y -= zoom;
    //    c.orthographicSize -= zoom;
    //}
}

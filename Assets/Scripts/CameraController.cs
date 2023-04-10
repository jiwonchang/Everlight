using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class CameraController : MonoBehaviour
{
    public Transform target;
    public Tilemap theMap;
    private Vector3 bottomLeftLimit;
    private Vector3 topRightLimit;
    private Vector3 mapBottomLeft;
    private Vector3 mapTopRight;
    private float halfHeight;
    private float halfWidth;

    // audio variables
    public int musicToPlay;
    private bool musicStarted;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerController.instance.transform;

        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;

        mapBottomLeft = theMap.localBounds.min;
        
        Debug.Log("Map Bottom Left: " + mapBottomLeft);

        mapTopRight = theMap.localBounds.max;
        // for the CAMERA ONLY, add a buffer of half the camera's width/height so that the edge (not the center) of the camera keeps inside the map bounds
        bottomLeftLimit = mapBottomLeft + new Vector3(halfWidth, halfHeight, 0);
        topRightLimit = mapTopRight - new Vector3(halfWidth, halfHeight, 0);

        // set the player bounds, so that the player stays inside the map bounds
        PlayerController.instance.SetBounds(mapBottomLeft, mapTopRight);
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Update is called once per frame AFTER Update()
    // updating the camera here helps to prevent stuttering (due to conflicting updates between camera and player)
    void LateUpdate()
    {
        transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);

        // keep the camera inside the map bounds
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeftLimit.x, topRightLimit.x), Mathf.Clamp(transform.position.y, bottomLeftLimit.y, topRightLimit.y), transform.position.z);

        // if we haven't started any music yet, start playing a music
        if (!musicStarted)
        {
            musicStarted = true;
            AudioManager.instance.PlayBGM(musicToPlay);
        }
    }
}

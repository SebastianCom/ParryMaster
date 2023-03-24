using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraType
{
    Follow,
    SpringArm,
    Zoom,
}

public class Camera : MonoBehaviour
{
    // Start is called before the first frame update

    public Quaternion Rotation;

    public CameraType m_CameraType;

    public GameObject Player;

    public float SpringArmDistance = 0.5f;
    public float SpringArmMinDistance = 0.01f;
    public float CameraSpeed = 1.0f;
    public float yOffset = 2.0f;
    public float xOffset = 2.0f;

    void Start()
    {
        if(Player == null)
        {
            Player = GameObject.Find("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_CameraType == CameraType.Follow)
        {
            FollowCameraBehviour();
        }
        else if(m_CameraType == CameraType.SpringArm) 
        {
            SpringArmBehaviour();
        }
    }

    void FollowCameraBehviour()
    {
        transform.rotation = Rotation;
        transform.position = new Vector3(Player.transform.position.x + xOffset, Player.transform.position.y + yOffset, transform.position.z);
    }

    void SpringArmBehaviour()
    {
        Vector3 NewCamPos = new Vector3(Player.transform.position.x + xOffset, Player.transform.position.y + yOffset, transform.position.z);

        if (Vector3.Distance(transform.position, NewCamPos) > SpringArmDistance && Player.GetComponent<Player>().bMoving == true)
        {
            transform.position = Vector3.Lerp(transform.position, NewCamPos, CameraSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, NewCamPos.y, transform.position.z);
        }
        else if (Player.GetComponent<Player>().bMoving == false)
        {
            transform.position = Vector3.Lerp(transform.position, NewCamPos, CameraSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, NewCamPos.y, transform.position.z);
        }

        if (Vector3.Distance(transform.position, NewCamPos) < SpringArmMinDistance)
        {
            transform.position = NewCamPos;
        }
    }
}

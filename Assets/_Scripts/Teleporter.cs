using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Teleporter : MonoBehaviour
{
    public GameObject m_Pointer;
    public SteamVR_Action_Boolean m_TeleporterAction;
    private SteamVR_Behaviour_Pose m_Pose = null;
    private bool m_HasPosition = false;

    private bool m_IsTeleporting = false;
    private float m_FadeTime = 0.5f;
    public Transform[] teleportLocations;

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Pointer
        m_HasPosition = UpdatePointer();
        m_Pointer.SetActive(m_HasPosition);
        //Teleport
        if (m_TeleporterAction.GetStateUp(m_Pose.inputSource))
            TryTeleport();
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                m_Pointer.transform.position = teleportLocations[0].position;
                TryTeleport();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                m_Pointer.transform.position = teleportLocations[1].position;
                TryTeleport();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                m_Pointer.transform.position = teleportLocations[2].position;
                TryTeleport();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                m_Pointer.transform.position = teleportLocations[3].position;
                TryTeleport();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                m_Pointer.transform.position = teleportLocations[4].position;
                TryTeleport();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                m_Pointer.transform.position = teleportLocations[5].position;
                TryTeleport();
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                m_Pointer.transform.position = teleportLocations[6].position;
                TryTeleport();
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                m_Pointer.transform.position = teleportLocations[7].position;
                TryTeleport();
            }
        }
    }

    private void TryTeleport()
    {
        //check for valid position
        if(!m_HasPosition || m_IsTeleporting)
            return;
        //get camera rig and head position
        Transform cameraRig = SteamVR_Render.Top().origin;
        Vector3 headPosition = SteamVR_Render.Top().head.position;
        //figure out translation
        Vector3 groundPosition = new Vector3(headPosition.x, cameraRig.position.y, headPosition.z);
        Vector3 tranlationVector = m_Pointer.transform.position - groundPosition;
        //Move
        IEnumerator cortune = MoveRig(cameraRig, tranlationVector);
        StartCoroutine(cortune);
    }



    private IEnumerator MoveRig(Transform cameraRig, Vector3 translation)
    {
        //Flag
        m_IsTeleporting = true;
        //Fade to black
        SteamVR_Fade.Start(Color.black, m_FadeTime, true);
        //Apply Tranlation 
        yield return new WaitForSeconds(m_FadeTime);
        cameraRig.position += translation;
        //Fade to clear
        SteamVR_Fade.Start(Color.clear, m_FadeTime, true);
        //De flag 
        m_IsTeleporting = false;
        yield return null; 
        
    }

    private bool UpdatePointer()
    {
        //Ray from the controller
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        // if its a hit
        int layer_mask = LayerMask.GetMask("Player", "Enemy");
        if (Physics.Raycast(ray,out hit))
        {
            m_Pointer.transform.position = hit.point;
            return true;
        }
        //if not a hit

        return false;
    }
}

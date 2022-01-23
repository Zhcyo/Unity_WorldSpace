using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerMove :NetworkBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float angle=400;
   
    public float speed = 5f;

    private Vector3 move;


    [HideInInspector] public Camera cameraMain;
  

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
      
    }

    public override void OnStartAuthority()
    {
    
        cameraMain = Camera.main;
        GameObject.FindObjectOfType<CinemachineFreeLook>().Follow=this.transform.GetChild(0);
        GameObject.FindObjectOfType<CinemachineFreeLook>().LookAt=this.transform.GetChild(0);
    
    }
    
  
    private void LateUpdate()
    {
        if (hasAuthority)
        {

       
        }
    }
    private void Update() {
      
        if (hasAuthority)
        {

            PlayMove();
           


        }
    }
    private void PlayMove() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        move = new Vector3(h, 0, v);
        move = Quaternion.AngleAxis(cameraMain.transform.rotation.eulerAngles.y,Vector3.up)*move;
        if (move.x != 0 || move.z != 0)
        {
            move.y = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(move), angle * Time.deltaTime);
        }
        characterController.Move(move * Time.deltaTime * speed);    
    }

    private void OnApplicationFocus(bool focus) {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    
    
    }
    private void CaulateVerticalSpeed() {
        if (characterController.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {

            }

        }
    
    }
}

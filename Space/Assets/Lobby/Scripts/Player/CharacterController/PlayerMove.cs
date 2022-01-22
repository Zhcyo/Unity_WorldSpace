using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerMove :NetworkBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float angle=400;
    public Vector2 _look;
    public float aimValue;
    public Vector3 nextPosition;
    public Quaternion nextRotation;
    public GameObject followTransform;
    public float rotationPower = 3f;
    public float rotationLerp = 0.5f;

    public float speed = 5f;

    private Vector3 move;


    [HideInInspector] public Camera cameraMain;
  

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

    }

    public override void OnStartAuthority()
    {
    
       // InitializeTpCamera();
        cameraMain = Camera.main;
        GameObject.FindObjectOfType<CinemachineVirtualCamera>().Follow=this.transform.GetChild(1);
        //GameObject.FindObjectOfType<CinemachineVirtualCamera>().LookAt=this.transform.GetChild(1);
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
       
        move = new Vector3(h,0,v)*Time.deltaTime*speed;

        if (move.x!=0||move.z!=0)
        {
            // Vector3 targetDirection= cameraMain.transform.TransformDirection(move.x,0,move.y);
            //targetDirection.y = 0;
            move.y = 0;
          transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(move),angle*Time.deltaTime);
             
        }
  
        characterController.Move(move);    
    }
    private void Rotation() {
        var angles = followTransform.transform.localEulerAngles;
        angles.z = 0;

        var angle = followTransform.transform.localEulerAngles.x;

        //Clamp the Up/Down rotation
        if (angle > 180 && angle < 340)
        {
            angles.x = 340;
        }
        else if (angle < 180 && angle > 40)
        {
            angles.x = 40;
        }


        followTransform.transform.localEulerAngles = angles;


        nextRotation = Quaternion.Lerp(followTransform.transform.rotation, nextRotation, Time.deltaTime * rotationLerp);

        if (move.x == 0 && move.y == 0)
        {
            nextPosition = transform.position;

            if (aimValue == 1)
            {
                //Set the player rotation based on the look transform
                transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
                //reset the y rotation of the look transform
                followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
            }

            return;
        }
        float moveSpeed = speed / 100f;
        Vector3 position = (transform.forward * move.y * moveSpeed) + (transform.right * move.x * moveSpeed);
        nextPosition = transform.position + position;


        //Set the player rotation based on the look transform
        transform.rotation = Quaternion.Euler(0, followTransform.transform.rotation.eulerAngles.y, 0);
        //reset the y rotation of the look transform
        followTransform.transform.localEulerAngles = new Vector3(angles.x, 0, 0);
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

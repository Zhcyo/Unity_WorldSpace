using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;

public class PlayerMove :NetworkBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CinemachineVirtualCamera virtualCamera = null;
    private Vector3 move;
    private float speed=5;
    private float  verticalSpeed;
    private bool isGround;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public override void OnStartAuthority()
    {
        enabled = true;

    }
    [ClientCallback]
    private void Update() {
        if (isLocalPlayer)
        {
         PlayMove();

        }
    }
    [Client]
    private void PlayMove() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        move = new Vector3(h,0,v)*Time.deltaTime*speed;
       // move = virtualCamera.transform.TransformDirection(move);
        if (move.x!=0||move.z!=0)
        {
            move.y = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(move),100);

        }
        characterController.Move(move);    
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

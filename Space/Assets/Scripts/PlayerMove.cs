using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMove :NetworkBehaviour
{
    private CharacterController characterController;
    private Vector3 move;
    private float speed=5;
    private float  verticalSpeed;
    private bool isGround;
    private Rigidbody rigidbody;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

 
    void Update()
    {
        if (isLocalPlayer)
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

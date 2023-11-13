using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Linq;

public class PlayerController : MonoBehaviourPun,IPunObservable
{   
    private Animator _animator;

    private CharacterController _characterController;

    public float Gravity = 20.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private Vector3 smoothmove;

    private bool hasHat = false;



    public float Speed = 5.0f;

    public float RotationSpeed = 240.0f;

    public float JumpSpeed = 7.0f;

    public float attackRange = 5.0f;

    public PhotonView view;

   


    void Start()
    {
        _animator = GetComponent<Animator>();

        _characterController = GetComponent<CharacterController>();
       
    }


    public void Talk()
    {
        _animator.SetTrigger("tr_talk");
    }

    private bool mIsControlEnabled = true;

    public void EnableControl()
    {
        mIsControlEnabled = true;
    }

    public void DisableControl()
    {
        mIsControlEnabled = false;
    }

    private Vector3 mExternalMovement = Vector3.zero;

    public Vector3 ExternalMovement
    {
        set
        {
            mExternalMovement = value;
        }
    }


    void LateUpdate()
    {
        if (mExternalMovement != Vector3.zero)
        {
            _characterController.Move(mExternalMovement);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ( mIsControlEnabled && view.IsMine)
        {       
            ProcessInputs();
        }
        else
        {
            SmoothMovement();
        }      
    }

   /* public void ChangePlayerColor(Material newMaterial)
    {
        if (playerSkinnedMeshRenderer != null && playerSkinnedMeshRenderer.material != null)
        {
            playerSkinnedMeshRenderer.material = newMaterial;
        }
    }*/



    private void SmoothMovement()
    {
        transform.position = Vector3.Slerp(transform.position, smoothmove, Time.deltaTime * 10);

    }

    public void ProcessInputs()
    {
        if (_characterController.isGrounded)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 camForward_Dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;

            if (move.magnitude > 1f) move.Normalize();

            move = transform.InverseTransformDirection(move);

            float turnAmount = Mathf.Atan2(move.x, move.z);

            transform.Rotate(0, turnAmount * RotationSpeed * Time.deltaTime, 0);

            _moveDirection = transform.forward * move.magnitude * Speed;

            if(Input.GetMouseButton(0))
            {
                _animator.SetBool("attack_1", true);
            }
            else
            {
                _animator.SetBool("attack_1", false);
            }

            if (Input.GetButton("Jump"))
            {
                _animator.SetBool("is_in_air", true);
                _moveDirection.y = JumpSpeed;
            }
            else
            {
               _animator.SetBool("is_in_air", false);
                _animator.SetBool("run", move.magnitude > 0);
            }
        }
        else
        {
            

            _moveDirection.y -= Gravity * Time.deltaTime;
        }

        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            smoothmove = (Vector3)stream.ReceiveNext();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("box"))
        {
            Destroy(collision.gameObject);

            Debug.Log("collided");
        }
    }
}

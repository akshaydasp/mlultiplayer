using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class playerMovement : MonoBehaviourPun,IPunObservable
{
    #region Private Members

    private Animator _animator;

    private CharacterController _characterController;

    private float Gravity = 20.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private InventoryItemBase mCurrentItem = null;
   
    private int startHealth = 50;
  
    private bool mCanTakeDamage = true;

    private Vector3 smoothmove;

    private bool hasFiredRaycast = false;

    private bool isDead = false;

    #endregion

    #region Public Members

    public float Speed = 5.0f;

    public float RotationSpeed = 240.0f;

    public Inventory Inventory;

    public GameObject Hand;

    public HUD Hud;

    public float JumpSpeed = 7.0f;

    

    public PhotonView pv;

    public PlayerInventory playerInventory;

    public Transform barreltransform;

    private Transform cameraTransform;

    public HealthBar healthBar;

    public GameObject bulletHolePrefab;

    public GameObject deathCanvas;

    public TMP_Text killcount;

    public int count = 0;



    #endregion

    public UnityEvent QuestCompleted;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        healthBar.SetMaxHealth(startHealth);
        deathCanvas.SetActive(false);
     
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
        if(photonView.IsMine)
        {
            ProcessInputs();
        }
        else
        {
            SmoothMovement();
        }
        
        
    }
    public void SmoothMovement()
    {
        transform.position = Vector3.Slerp(transform.position, smoothmove, Time.deltaTime * 10);
    }

    public void ProcessInputs()
    {
        if ( mIsControlEnabled)
        {
            // Interact with the item
            if (mInteractItem != null && Input.GetKeyDown(KeyCode.F))
            {
                // Interact animation
                mInteractItem.OnInteractAnimation(_animator);
            }

            // Execute action with item
            if (Input.GetMouseButtonDown(0) && !hasFiredRaycast) // Use GetMouseButtonDown to detect the first click.
            {
                hasFiredRaycast = true; // Set the flag to true to prevent multiple raycasts.

                _animator.SetTrigger("attack_1");
                ShootGun();
            }

            // Reset the flag when the mouse button is released.
            

            // Get Input for axis
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // Calculate the forward vector
            Vector3 camForward_Dir = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 move = v * camForward_Dir + h * Camera.main.transform.right;

            if (move.magnitude > 1f) move.Normalize();


            // Calculate the rotation for the player
            move = transform.InverseTransformDirection(move);

            // Get Euler angles
            float turnAmount = Mathf.Atan2(move.x, move.z);

            transform.Rotate(0, turnAmount * RotationSpeed * Time.deltaTime, 0);

            if (_characterController.isGrounded || mExternalMovement != Vector3.zero)
            {
                _moveDirection = transform.forward * move.magnitude;

                _moveDirection *= Speed;

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
                Gravity = 20.0f;
            }


            _moveDirection.y -= Gravity * Time.deltaTime;

            _characterController.Move(_moveDirection * Time.deltaTime);
        }

    }

    private void ShootGun()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
        {
            Transform hitObject = hit.transform; // The object that was hit.
            Vector3 hitPoint = hit.point; // The point in world space where the ray hit.
            

            Debug.Log("Ray hit object: " + hitObject.name + " at point: " + hitPoint);
            // Check if the hit object is a player
            playerMovement otherPlayer = hit.transform.GetComponent<playerMovement>();

            if (otherPlayer != null && otherPlayer != this)
            {
                // You may want to implement a method in the playerMovement script to handle damage.
                otherPlayer.TakeDamage(10); // Adjust the damage value as needed.

                // Instantiate bullet hole
                bulletHolePrefab = PhotonNetwork.Instantiate("bulletHolePrefab", hit.point, Quaternion.FromToRotation(Vector3.forward, hit.normal));

                StartCoroutine(DestroyBulletHole(bulletHolePrefab));
            }
            hasFiredRaycast = false;
        }
    }

    private IEnumerator DestroyBulletHole(GameObject bulletHoleprefab)
    {
        yield return new WaitForSeconds(1.0f); // Adjust the duration as needed (1.0f = 1 second).

        if (bulletHolePrefab != null)
        {
            PhotonNetwork.Destroy(bulletHolePrefab); // Destroy the bullet hole on all clients.
        }
    }




    public void TakeDamage(int damage)
    {
        if (mCanTakeDamage)
        {
            startHealth -= damage;

            // Call a method to update health across the network
            photonView.RPC("UpdateHealth", RpcTarget.All, startHealth);

            if (startHealth <= 0)
            {
                Die();
            }
        }
    }

    [PunRPC]
    void UpdateHealth(int newHealth)
    {
        startHealth = newHealth;
        healthBar.SetHealth(startHealth);
    }


    void Die()
    {
        isDead = true;
        count++;
        killcount.text = count.ToString();
        // Trigger the death animation on the local player
        _animator.SetTrigger("death");

        // Wait for the animation to finish, then show the deathCanvas
        StartCoroutine(ShowDeathCanvasAfterAnimation());
    }

    IEnumerator ShowDeathCanvasAfterAnimation()
    {
        // Wait for the duration of the death animation
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        // Show the deathCanvas
        deathCanvas.SetActive(true);

        // Notify other players that this player has died
        photonView.RPC("PlayerDied", RpcTarget.All);
    }

    [PunRPC]
    void PlayerDied()
    {
        // Set the "dead" state on all clients
        isDead = true;

        // Play the death animation on all clients
        _animator.SetTrigger("death");

        // After a delay, destroy the player's GameObject
        StartCoroutine(DestroyPlayer());
    }

    IEnumerator DestroyPlayer()
    {
        yield return new WaitForSeconds(3f); // Adjust the delay as needed.

        // Destroy the player's GameObject on all clients
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }


    private InteractableItemBase mInteractItem = null;

   

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.eulerAngles);
        }
        else if (stream.IsReading)
        {
            smoothmove = (Vector3)stream.ReceiveNext();

            Vector3 receivedRotation = (Vector3)stream.ReceiveNext();
            transform.eulerAngles = receivedRotation;
        }
    }
}

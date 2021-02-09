using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{


    private MyVectorThings mmv;

    //private Variables
    //cam rotation variables
    private float bodyRotX, camRotY, camMinY = -90, camMaxY = 90;


    //movement variables
    private float sidewaysMovementInput;
    private float forwardMovementInput;
    private Vector3 sidewaysMovementSpeed;
    private Vector3 forwardMovementSpeed;
    private bool isSprinting;
    private Vector3 slopeNormal;
    private float extraDownForce = 0;
    private Vector3 hVel;


    //jump variables
    private bool jumpWasPressed;
    private int numberOfAirJumps;



    public Rigidbody rb;                                                   
    [SerializeField] private bool isGrounded;
    private bool cancelGround;









    //public variables

    //cam parameters
    [SerializeField] private float camRotSpeed, rotSmoothSpeed;                  


    //movement speed parameters
    [SerializeField] private float baseSpeed = 5;
    [SerializeField] private float sprintSpeed = 10;
    [SerializeField] private float playerSpeed = 400;
    [SerializeField] private float maxSpeed = 10;
    [SerializeField] private float airControl;
    [SerializeField] private float maxFloorAngle = 40;
    [SerializeField] private float zeroThreshold = .5f;
    [SerializeField] private float frictionForce = 2;

    //jump parameters
    [SerializeField] private int maxNumberOfAirJumps = 1;                                         
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float airJumpForce = 5;
    [SerializeField] private float jumpForwardForce;

    [SerializeField] private Transform playerCamera;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float showSpeed;
    



    





    // Start is called before the first frame update
    void Awake()
    {
        mmv = gameObject.AddComponent<MyVectorThings>();
        rb = GetComponent<Rigidbody>();
        
    }




    // Update is called once per frame
    void Update()
    {

        //get the input from keys
        



        //Mouse look
        LookRotation();
        
    }




    //FixedUpdate called every physics updates
    private void FixedUpdate()
    {
        hVel = mmv.MakeHorizontal(rb.velocity);
        GetInput();
        showSpeed = mmv.MakeHorizontal(rb.velocity).magnitude;
        rb.AddForce(Vector3.down * extraDownForce);

        //jump of space was pressed
        if (jumpWasPressed)
        {
            //update variable so it wont trigger again
            jumpWasPressed = false;

            Jump();
        }



        //run if player is on the ground
        Movement();


        //clamp velocity to max
        rb.velocity = mmv.ClampHorizonal(rb.velocity, maxSpeed);
    }





    private void GetInput()
    {
        //jump input
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpWasPressed = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpWasPressed = true;
        }
             
        


        //movement input
        sidewaysMovementInput = Input.GetAxisRaw("Horizontal");
        forwardMovementInput = Input.GetAxisRaw("Vertical");


        //adjust speed based on leftshift sprint button
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }
    }




    // while overlapping something check if it is groudn and whether it is walkable
    private void OnCollisionStay(Collision other)
    {

        //check if in ground layer
        if (groundLayerMask.value != (1 << other.gameObject.layer)) return;

        
        Vector3 contactNormal;
        for (int i = 0; i < other.contactCount; i++)
        {


            contactNormal = other.contacts[i].normal;
            if (mmv.FloorAngle(contactNormal) < maxFloorAngle) //if contact point is at an angle that is a floor then say we are grounded
            {

                //print(contactNormal);
                isGrounded = true;
                numberOfAirJumps = 0;
                slopeNormal = contactNormal;

                CancelInvoke(nameof(StopGrounded));             //reset the timer on the stop grounded
                cancelGround = false;
            }
        }


        
        if (!cancelGround)
        {
            float delay = 1f;
            cancelGround = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);   // if we are not already canceling the grounded start cancelling because we have no way of knowing the contact angle when we stop the colision
        }
        
    }


    private void StopGrounded()
    {
        isGrounded = false;
    }




    //function to move camera
    private void LookRotation() 
    {
        Cursor.lockState = CursorLockMode.Locked; //lock cursor abd hide it


        //set rotation from mouse movements
        bodyRotX += Input.GetAxis("Mouse X") * camRotSpeed;
        camRotY += Input.GetAxis("Mouse Y") * camRotSpeed;


        //bodyRotX = Mathf.Round(bodyRotX * 8) / 8;

        //clamp vertical mouse so player cant look behind
        camRotY = Mathf.Clamp(camRotY, camMinY, camMaxY);

        //convert to quaternion maybe idk what im doing its what the website said
        Quaternion camTargetRot = Quaternion.Euler(-camRotY, 0, 0);
        Quaternion bodyTargetRot = Quaternion.Euler(0, bodyRotX, 0);


        //interpolate from current camera postion to new camera postion or just set the camera rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, bodyTargetRot, Time.deltaTime * rotSmoothSpeed);
        playerCamera.localRotation = Quaternion.Lerp(playerCamera.localRotation, camTargetRot, Time.deltaTime * rotSmoothSpeed);
        //transform.rotation = bodyTargetRot;
        //playerCamera.localRotation = camTargetRot;
    }


    //get the direction to move in 
    private Vector3 MovementDir()
    {
        Vector3 speedVector;
        //rigidbodyComponent.velocity = new Vector3(sidewaysMovementInput*speed, rigidbodyComponent.velocity.y, forwardMovementInput*speed);


        //get the speeds for moveing forward and right,
        sidewaysMovementSpeed = sidewaysMovementInput * mmv.RightVector(rb.transform);
        forwardMovementSpeed = forwardMovementInput * mmv.ForwarVector(rb.transform);

        //coombine forward and right speed  and clamp to max speed
        speedVector = (sidewaysMovementSpeed + forwardMovementSpeed).normalized;
        
        
        //return speed vector
        return speedVector;
        
        
        
    }


    


    private void Movement()
    {
        float moveForce = playerSpeed;


        //if (forwardMovementInput > 0 && forwardMovementSpeed > 0);
        //{
        //
        //}
        //
        if (!isSprinting && (hVel.magnitude > baseSpeed) && isGrounded)
        {
            moveForce = 0;
        }


        if (isSprinting && !(forwardMovementInput > 0) && (hVel.magnitude > baseSpeed))
        {
            moveForce = 0;
        }


        if (isSprinting && (hVel.magnitude > sprintSpeed) && isGrounded)
        {
            moveForce = 0;
        }


        if (hVel.magnitude > maxSpeed && isGrounded)
        {
            moveForce = 0;
        }




        rb.AddForce(MovementDir() * moveForce);
        CounterMovement();



    }

    private void CounterMovement()
    {

        Vector3 counterForce = Vector3.zero;
        if (isGrounded)
        {
            counterForce = -rb.velocity.normalized;
            counterForce = mmv.MakeHorizontal(counterForce) * mmv.MakeHorizontal(rb.velocity).magnitude * frictionForce;
        }


        if (isGrounded && mmv.MakeHorizontal(rb.velocity).magnitude < zeroThreshold)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            counterForce = Vector3.zero;
        }


        if (mmv.FloorAngle(slopeNormal) > 2 && isGrounded)
        {
            Vector3 slopeRight = Vector3.Cross(mmv.MakeHorizontal(slopeNormal).normalized, Vector3.up);
            


            counterForce += -mmv.MakeHorizontal(slopeNormal) * -(Physics.gravity.y * rb.mass - extraDownForce);

        }

        if (isGrounded) rb.AddForce(counterForce);
    }


    private void Jump()
    {

        //set the jump vector to be just up if on the ground
        Vector3 jumpVector = Vector3.zero;
        if (isGrounded)
        {
            jumpVector = Vector3.up * jumpForce;
            
        }


        //set jump vector to be up and forward in the diraction the player is facing
        if (!isGrounded && numberOfAirJumps < maxNumberOfAirJumps)
        {
            numberOfAirJumps++;
            jumpVector = jumpForwardForce * mmv.ForwarVector(playerCamera) + Vector3.up * airJumpForce;


            //if player is falling faster than almost zero reset fall speed
            if (rb.velocity.y < .5f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            }
        }


        //apply force
        rb.AddForce(jumpVector);
        
        
    }



    




}


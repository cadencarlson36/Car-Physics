using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Wheel : MonoBehaviour
{
    private Rigidbody rb;

    public bool wheelFrontLeft, wheelFrontRight, wheelRearLeft, wheelRearRight; //wheel positions assigned in engine

    //Spring force equation: F = kx
    public float restLength;  //spring resting length
    public float springDisplacement;    //- or + direction
    public float springConstant;    //spring constant
    private float minLength, maxLength;  //spring length restrictions
    private float springLength;  //current length in fixedupdate

    private float springForce;  //force applied to spring

    private float lastLength;   //length on last frame
    public float damperStiffness;   //damping constant
    private float damperForce;  //force applied to damper
    private float springVelocity; //velocity of spring

    public float steerAngle; //angle of wheel set from angle in CarController script
    public float steerTime; //time it takes to reach target angle

    private UnityEngine.Vector3 suspensionForce; //force applied to car
    private UnityEngine.Vector3 wheelVelocityLS; //velocity of wheel in local space
    private float Fx; //force in x direction
    private float Fy; //force in y direction
    private float wheelAngle; //angle of wheel in world space
   
    public float wheelRadius;   //needed for raycast length
    
    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();

        minLength = restLength - springDisplacement;
        maxLength = restLength + springDisplacement;
    }

    void Update()
    {
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, Time.deltaTime * steerTime); //smoothly interpolate between current angle and target angle
        transform.localRotation = UnityEngine.Quaternion.Euler(UnityEngine.Vector3.up * wheelAngle); //add steer angle to rotation every frame

        Debug.DrawRay(transform.position, -transform.up * (springLength + wheelRadius), Color.green); //draw ray to visualize suspension
    }

    void FixedUpdate()  //FixedUpdate is used for physics or when dealing with RigidBody
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius))
        {
            lastLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);

            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            springForce = springConstant * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;
            suspensionForce = (springForce + damperForce) * transform.up;

            wheelVelocityLS = transform.InverseTransformDirection(rb.GetPointVelocity(hit.point)); //get velocity of wheel in local space
            Fx = Input.GetAxis("Vertical") * 1500;
            Fy = wheelVelocityLS.x * 1500;


            rb.AddForceAtPosition(suspensionForce + (Fx * transform.forward) + (Fy * -transform.right), hit.point);
        }
    }
}

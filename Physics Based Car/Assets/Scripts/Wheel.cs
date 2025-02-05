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

    public bool wheelFrontLeft, wheelFrontRight, wheelRearLeft, wheelRearRight; //wheel positions

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

    private UnityEngine.Vector3 suspensionForce; //force applied to car
   
    public float wheelRadius;   //needed for raycast length
    
    void Start()
    {
        rb = transform.root.GetComponent<Rigidbody>();

        minLength = restLength - springDisplacement;
        maxLength = restLength + springDisplacement;
    }

    void Update()
    {
        
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
            rb.AddForceAtPosition(suspensionForce, hit.point);
        }
    }
}

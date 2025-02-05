using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Wheel[] wheels;

    public float wheelBase; //distance between front and rear wheels (m)
    public float rearTrack; //distance between rear wheels (m)
    public float turnRadius; //(m)
    private float steerInput; //input from player

    private float leftAngle, rightAngle; //angles of each front wheel respective to center of turn
   
    void Update()
    {
        steerInput = Input.GetAxis("Horizontal");

        if (steerInput > 0) //turning right
        {
            leftAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + rearTrack / 2)); //wheels use dif right triangles 
            rightAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - rearTrack / 2)); //therefore different lengths for total turn radius
        }
        else if (steerInput < 0) //turning left
        {
            leftAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - rearTrack / 2)); 
            rightAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + rearTrack / 2));
        }
        else //not turning
        {
            leftAngle = 0;
            rightAngle = 0;
        }

        foreach (Wheel w in wheels)
        {
            if (w.wheelFrontLeft)
            {
                w.steerAngle = leftAngle;
            }
            if (w.wheelFrontRight)
            {
                w.steerAngle = rightAngle;
            }
        }
    }
}

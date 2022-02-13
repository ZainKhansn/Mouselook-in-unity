using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour {

    [Flags]
    // FLAGS{this lets multiple values for enum VAL}
    public enum RotationDirection {
        None,
        Horizontal = (1 << 0),
        Vertical = (1 << 1)
    }
    [Tooltip("Which directions this object can rotate")]
    [SerializeField] private RotationDirection rotationDirections;
    [Tooltip("The rotation acceleration, in degrees / second")]
    [SerializeField] private Vector2 acceleration;
    [Tooltip("A multiplier to the input. Describes the maximum speed in degrees/ seconds. To flip vertical rotation, set Y to a negative value")]
    [SerializeField] private Vector2 sensitivity;
    [Tooltip("The max angle from the horizon the player can rotate, in degrees")]
    [SerializeField] private float maxVerticalAngleFromHorizon;
    [Tooltip("The period to wait until resetting the input value. Set this as low as possible, without encountering stutering")]
    [SerializeField] private float inputLagPeriod;
    private Vector2 velocity;

    private Vector2 rotation;

    private Vector2 lastInputEvent;

    private float inputLagTimer;
    private float ClampVerticalAngle(float angle) {
        return Mathf.Clamp(angle, -maxVerticalAngleFromHorizon, maxVerticalAngleFromHorizon);
    }
    private Vector2 GetInput() {
        inputLagTimer += Time.deltaTime;
        Vector2 input = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );
        if((Mathf.Approximately(0, input.x) && Mathf.Approximately(0, input.y)) == false || inputLagTimer >= inputLagPeriod) {
            
            lastInputEvent = input;
            inputLagTimer = 0;
        }
        return lastInputEvent;
    }
    private void Update() {

        Vector2 wantedVelocity = GetInput() * sensitivity;

        if((rotationDirections & RotationDirection.Horizontal) == 0) {
            wantedVelocity.x = 0;
        }
        if((rotationDirections & RotationDirection.Vertical) == 0) {
            wantedVelocity.y = 0;
        }
        velocity = new Vector2(
            Mathf.MoveTowards(velocity.x, wantedVelocity.x, acceleration.x * Time.deltaTime),
            Mathf.MoveTowards(velocity.y, wantedVelocity.y, acceleration.y * Time.deltaTime));

        rotation += velocity * Time.deltaTime;
        rotation.y = ClampVerticalAngle(rotation.y);
        transform.localEulerAngles = new Vector3(rotation.y, rotation.x, 0);
    }
}

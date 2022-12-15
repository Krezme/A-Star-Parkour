using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PhysicsStatistics {
    [Range(0, 6)]
    public float speed = 6;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;
    public float jumpHeight = 2;
}

public class PhysicsAIController : MonoBehaviour
{
    public PhysicsStatistics stats;
    public CharacterController controller;

    public float groundCheckSphereRadius = 0.4f;
    public float groundedOffset = 0.2f;
    public LayerMask groundMask;
    public float jumpThreshold = 0.2f;
    public float defDownwardForce = -3f;
    public float verticalVelocity;
    public float terminalVelocity;
    public float gravity = -9.81f;

    [HideInInspector]
    public bool jump;
    [HideInInspector]
    public bool isGrounded;

    void Update () {
        JumpAndGravity(jump);
        GroundedCheck();
        jump = false;
    }

    public bool JumpAndGravity(bool _jump) {
        if (isGrounded)
        {
            // stop our velocity dropping infinitely when grounded
            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -3f;
            }

            // Jump
            if (_jump)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                verticalVelocity = Mathf.Sqrt(stats.jumpHeight * -2f * gravity);

            }

        }
        else
        {
            // if we are not grounded, do not jump
            _jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        return _jump;
    }

    public void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundCheckSphereRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    public void RotatePlayer(Vector3 target) {
        Quaternion targetRotation = Quaternion.LookRotation(target);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * stats.turnSpeed);
    }

    public void Move(Vector3 motion) {
        controller.Move(motion + new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }

    public void OnDrawGizmos() {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        // draw sphere
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(spherePosition, groundCheckSphereRadius);
    }
}

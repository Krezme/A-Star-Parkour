using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TPSController : MonoBehaviour {

    float vertical;
    float horizontal;

    bool shift;

    Animator anim;
    CrossInput crossInput;

    public Transform m_camera;

    float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    Collider col;

    [HideInInspector]
    public bool lockMoving = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        crossInput = GetComponent<CrossInput>();

        col = transform.root.GetComponent<Collider>();
        if(col == null)
        {
            col = transform.GetComponentInChildren<Collider>();
        }
    }

    void Update()
    {
        if (!lockMoving)
        {
            shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            
            Vector2 inputDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
            if (inputDir != Vector2.zero)
            {
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + m_camera.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
            }

            vertical = crossInput.Vertical;
            horizontal = crossInput.Horizontal;

            if (vertical != 0 || horizontal != 0)
                anim.SetFloat("input", Mathf.Lerp(anim.GetFloat("input"), Mathf.Clamp(vertical + horizontal, 0, .5f) + (Input.GetKey(KeyCode.LeftShift) ? 0.5f : 0), Time.deltaTime * 4));
            else
                anim.SetFloat("input", Mathf.Lerp(anim.GetFloat("input"), Mathf.Clamp(vertical + horizontal, 0, .5f), Time.deltaTime * 4));
        }
    }

    public void LockMovement()
    {
        lockMoving = true;
    }

    public void TurnOffCollider()
    {
        col.enabled = false;
    }

    public void TurnOnCollider()
    {
        col.enabled = true;
        lockMoving = false;
        anim.SetInteger("parkourState", 0);
    }
}

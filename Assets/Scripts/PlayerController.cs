using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Declare all variables
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed = 10;
    [SerializeField] private float turnSmoothTime = 0.1f;

    Vector2 input;
    Vector3 movementInput;

    private void Update()
    {
        Look();
    }

    void FixedUpdate()
    {
        Move();
    }

    //Controlled by Input System
    void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
        movementInput = new Vector3(input.x, 0, input.y);
    }

    //Functions
    void Move()
    {
        rb.MovePosition(transform.position + (transform.forward * movementInput.magnitude) * speed * Time.deltaTime);
    }

    void Look()
    {
        if (movementInput != Vector3.zero)
        {
            var matrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45 , 0));

            var skewedInput = matrix.MultiplyPoint3x4(movementInput);

            var relative = (transform.position + skewedInput) - transform.position;
            var rotation = Quaternion.LookRotation(relative, Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSmoothTime);
        }
        
    }
}

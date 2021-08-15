using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public float moveSpeed = 8;
    public float accelSpeed = 10;
    public float rotationSpeed = 8;

    private Animator animator;
    private new Rigidbody rigidbody;
    private Vector3 velocity;
    private Grabbable grabbedObject;
    private ParticleSystem particles;

    void Awake()
    {
        instance = this;
        rigidbody = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (!GameManager.instance.gameStarted)
        {
            return;
        }

        if (Input.GetButtonDown("Interact"))
        {
            if (InteractionManager.instance.GetInteractable() != null)
            {
                InteractionManager.instance.Interact();
            }
            else if (grabbedObject != null)
            {
                grabbedObject.Throw();
            }
        }

        float ratio = velocity.magnitude / moveSpeed;
        animator.SetFloat("speed", velocity.magnitude / moveSpeed);

        if (ratio > 0.5 && !particles.isEmitting)
        {
            particles.Play();

        }
        else if (ratio < 0.5 && particles.isEmitting)
        {
            particles.Stop();
        }
    }

    public Grabbable GetGrabbedObject()
    {
        return grabbedObject;
    }

    public void SetGrabbedObject(Grabbable grabbable)
    {
        grabbedObject = grabbable;
        animator.SetBool("hasItem", true);
    }

    public void ClearGrabbedObject()
    {
        grabbedObject = null;
        animator.SetBool("hasItem", false);
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.gameStarted)
        {
            return;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        Vector3 moveDir = new Vector3(input.x, 0, input.y);

        velocity = Vector3.Lerp(velocity, moveDir * moveSpeed, Time.fixedDeltaTime * accelSpeed);
        velocity.y = rigidbody.velocity.y;
        rigidbody.velocity = velocity;

        // Rotate to movement direction
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!GameManager.instance.gameStarted)
        {
            return;
        }

        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null && interactable.isInteractable)
        {
            if (InteractionManager.instance.GetInteractable() == null)
            {
                InteractionManager.instance.SetInteractable(interactable);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable != null)
        {
            Interactable currInteractable = InteractionManager.instance.GetInteractable();
            if (currInteractable == interactable)
            {
                InteractionManager.instance.ClearInteractable();
            }
        }
    }
}

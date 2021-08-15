using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : Interactable
{
    public string itemName;
    public Vector2 throwVelocity = new Vector2(8, 6);
    public Vector3 grabOffset = new Vector3(0, 1);
    public bool flip;
    public bool isCrop;

    private bool grabbed;
    private new Rigidbody rigidbody;

    public Transform grabber;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (isCrop)
        {
            GameManager.instance.AddToCrops(this);
        }
    }

    public override void Interact()
    {
        if (grabbed)
        {
            Throw();
        }
        else
        {
            Grab(Player.instance.transform);
        }
    }

    public override string GetInteractAction()
    {
        return grabbed ? "THROW" : "GRAB";
    }

    void Update()
    {
        if (grabbed)
        {
            transform.position = grabber.position + grabOffset;
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y = grabber.eulerAngles.y;
            eulerAngles.x = flip ? 180 : 0;
            transform.eulerAngles = eulerAngles;
        }
    }

    private void SetAnimalComponents(bool grabbed)
    {
        if (GetComponent<Pig>())
        {
            if (grabbed)
                GetComponent<Pig>().OnGrabbed();
            else
                GetComponent<Pig>().OnThrown();
        }
        if (GetComponent<Chicken>())
        {
            if (grabbed)
                GetComponent<Chicken>().OnGrabbed();
            else
                GetComponent<Chicken>().OnThrown();
        }
    }

    public void Grab(Transform grabber)
    {
        this.grabber = grabber;
        if (grabber == Player.instance.transform)
        {
            Player.instance.SetGrabbedObject(this);
        }
        else
        {
            grabber.GetComponent<Chicken>().ChangeState(Chicken.State.RUNNING);
        }
        rigidbody.isKinematic = true;
        grabbed = true;
        // isInteractable = false;
        SetAnimalComponents(true);

    }

    public void Drop()
    {
        if (grabber == Player.instance.transform)
        {
            Player.instance.ClearGrabbedObject();
        }
        rigidbody.isKinematic = false;
        grabbed = false;
        // isInteractable = true;
        if (flip)
        {
            transform.Rotate(180, 0, 0);
        }

        SetAnimalComponents(false);

        grabber = null;
    }

    public void Throw()
    {
        SetAnimalComponents(false);

        Player.instance.ClearGrabbedObject();
        rigidbody.isKinematic = false;
        Vector3 velocity = Player.instance.transform.forward * throwVelocity.x;
        velocity.y = throwVelocity.y;
        rigidbody.velocity = velocity;
        grabbed = false;
        // isInteractable = true;
        if (flip)
        {
            transform.Rotate(0, 0, 180);
        }

        grabber = null;
    }

    public void PutInTruck()
    {
        GameManager.instance.crops.Remove(this);
    }

}

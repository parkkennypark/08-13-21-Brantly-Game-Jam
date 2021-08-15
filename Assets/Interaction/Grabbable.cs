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

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (isCrop)
        {
            GameManager.instance.crops.Add(this);
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
            Grab();
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
            transform.position = Player.instance.transform.position + grabOffset;
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.y = Player.instance.transform.eulerAngles.y;
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
            GetComponent<Chicken>().enabled = grabbed;
        }
    }

    public void Grab()
    {
        Player.instance.SetGrabbedObject(this);
        rigidbody.isKinematic = true;
        grabbed = true;
        // isInteractable = false;
        SetAnimalComponents(true);

    }

    public void Drop()
    {
        Player.instance.ClearGrabbedObject();
        rigidbody.isKinematic = false;
        grabbed = false;
        // isInteractable = true;
        if (flip)
        {
            transform.Rotate(180, 0, 0);
        }

        SetAnimalComponents(false);
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

    }

    public void PutInTruck()
    {
        GameManager.instance.crops.Remove(this);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Requester : Interactable
{
    public string requestedItemName;
    public Transform requestTooltip;
    public TextMeshProUGUI requestText;
    public float tooltipOffsetY;

    void Awake()
    {
        requestText.text = requestedItemName;
    }

    public override void Interact()
    {
        if (Player.instance.GetGrabbedObject() && Player.instance.GetGrabbedObject().itemName == requestedItemName)
        {
            print("IT IS GIVEN");
        }
        else
        {
            print("NOPE");
        }
    }

    public override string GetInteractAction()
    {
        return "GIVE";
    }

    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        requestTooltip.position = screenPos + Vector2.up * tooltipOffsetY;
    }
}

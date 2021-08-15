using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;
    public Transform interactionTooltip;
    public TextMeshProUGUI interactionText;

    Interactable currentInteractable;

    void Awake()
    {
        instance = this;
        SetInteractionTooltipActive(false);
    }

    void Update()
    {
        if (currentInteractable != null)
        {
            if (!currentInteractable.isInteractable)
            {
                ClearInteractable();
                return;
            }
            Vector2 screenPos = Camera.main.WorldToScreenPoint(currentInteractable.transform.position);
            interactionTooltip.position = screenPos;
        }
    }

    private void SetInteractionTooltipActive(bool active)
    {
        interactionTooltip.gameObject.SetActive(active);
    }

    private void SetInteractionText(string text)
    {
        interactionText.text = "[E] " + text;
    }

    public void SetInteractable(Interactable interactable)
    {
        if (Player.instance.GetGrabbedObject() != null && interactable != Player.instance.GetGrabbedObject())
        {
            return;
        }

        if (interactable.GetComponent<Grabbable>() && interactable.GetComponent<Grabbable>().grabber && interactable.GetComponent<Grabbable>().grabber != Player.instance.transform)
        {
            return;
        }

        currentInteractable = interactable;
        SetInteractionTooltipActive(true);
        SetInteractionText(currentInteractable.GetInteractAction());
    }

    public void ClearInteractable()
    {
        currentInteractable = null;
        SetInteractionTooltipActive(false);
    }

    public Interactable GetInteractable()
    {
        return currentInteractable;
    }

    public void Interact()
    {
        if (currentInteractable == null)
        {
            return;
        }

        currentInteractable.Interact();
    }

}

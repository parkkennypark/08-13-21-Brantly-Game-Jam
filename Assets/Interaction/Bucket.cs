using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bucket : Grabbable
{
    public GameObject model, filledModel;

    private bool filled;

    void OnTriggerEnter(Collider other)
    {
        if (!filled && other.tag == "Spigot")
        {
            other.GetComponent<Animator>().SetTrigger("cooped");
            filled = true;

            model.SetActive(false);
            filledModel.SetActive(true);
            itemName = "WATER";
        }

        // else if (filled && other.tag == "Well")
        // {
        //     other.GetComponent<Well>().TryFeed(this);
        // }
    }
}

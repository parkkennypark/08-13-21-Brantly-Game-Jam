using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Animator animator;
    public List<Grabbable> crops = new List<Grabbable>();

    public TextMeshProUGUI lossText;
    public float cameraDamper = 0.4f;
    public float cameraSmoothSpeed = 4f;

    private bool lost;
    private Transform lossTarget;

    private Quaternion cameraBaseRot;

    [TextArea(3, 10)]
    public string[] failMessages;

    void Awake()
    {
        instance = this;
        cameraBaseRot = Camera.main.transform.rotation;
    }

    void Update()
    {
        Transform camera = Camera.main.transform;
        if (!lost)
        {
            Quaternion toPlayerRot = Quaternion.LookRotation(Player.instance.transform.position - camera.position, Vector3.up);
            Quaternion dampedRot = Quaternion.Lerp(cameraBaseRot, toPlayerRot, cameraDamper);
            camera.rotation = Quaternion.Lerp(camera.rotation, dampedRot, Time.deltaTime * cameraSmoothSpeed);
        }

        if (lost)
        {
            Quaternion targetRot = Quaternion.LookRotation(lossTarget.position - camera.position, Vector3.up);
            camera.rotation = Quaternion.Lerp(camera.rotation, targetRot, Time.deltaTime * 2f);
            camera.Translate(camera.forward * Time.deltaTime * 1);
        }


    }

    public void LoseGame(int loseCode, Transform lossTarget)
    {
        if (lost)
        {
            return;
        }

        lost = true;
        this.lossTarget = lossTarget;
        lossText.text = failMessages[loseCode];
        animator.SetTrigger("lost");
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

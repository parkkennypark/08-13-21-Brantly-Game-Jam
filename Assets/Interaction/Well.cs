using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : MonoBehaviour
{
    private string currentRequest = "";
    private Patience patience;
    private float timer;
    private int timesFed;

    public Sprite[] sprites;
    public SpriteRenderer spriteRenderer;
    public float timeBetweenRequests = 20;
    public float startTime = 10;


    // Start is called before the first frame update
    void Start()
    {
        timer = 1000;
        GameManager.instance.OnGameStart += OnGameStart;

        patience = GetComponentInChildren<Patience>();
        patience.StopTimer();
        patience.SetVisible(false);
        spriteRenderer.enabled = false;
        patience.OnTimerTimeout += LoseGame;
    }

    void OnGameStart()
    {
        timer = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0 && currentRequest == "")
        {
            StartNewRequest();
        }
    }

    void LoseGame()
    {
        patience.StopTimer();
        GameManager.instance.LoseGame(2, transform);
    }

    public void TryFeed(Grabbable grabbable)
    {
    }

    void OnTriggerEnter(Collider other)
    {
        Grabbable grabbable = other.GetComponent<Grabbable>();
        if (grabbable && grabbable.itemName == currentRequest)
        {
            patience.StopTimer();
            patience.SetVisible(false);
            GetComponent<Animator>().SetTrigger("cooped");
            StartTimer();
            timesFed++;
            currentRequest = "";
            spriteRenderer.enabled = false;
            Destroy(other.gameObject);
        }
    }

    void StartNewRequest()
    {
        int request = 0;
        int feedCase = timesFed % 4;
        if (feedCase == 0)
        {
            request = 1;
        }
        else if (feedCase == 1 || feedCase == 3)
        {
            request = 0;
        }
        else if (feedCase == 2)
        {
            request = 2;
        }

        spriteRenderer.sprite = sprites[request];
        spriteRenderer.enabled = true;
        if (request == 0)
        {
            currentRequest = "WATER";
        }
        else if (request == 1)
        {
            currentRequest = "PIG";
        }
        else
        {
            currentRequest = "CHICKEN";
        }

        patience.StartTimer();
        patience.SetVisible(true);
    }

    void StartTimer()
    {
        timer = timeBetweenRequests;
    }
}

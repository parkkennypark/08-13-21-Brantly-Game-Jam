using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Truck : MonoBehaviour
{
    public static Truck instance;

    [System.Serializable]
    public class Request
    {
        public string itemName;
        public int amount;
        public TextMeshProUGUI amountText;
    }

    public Request[] requests;
    public GameObject requestUIPrefab;
    public Transform requestUIParent;
    public Transform requestTooltip;
    public Vector2 tooltipOffset;

    private Patience patience;
    private Dictionary<string, int> currentStock = new Dictionary<string, int>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupStockDictionary();
        SetupGUI();

        patience = GetComponentInChildren<Patience>();
        patience.StartTimer();
        patience.OnTimerTimeout += OutOfPatience;
    }

    void Update()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        requestTooltip.position = screenPos + tooltipOffset;
    }

    private void SetupStockDictionary()
    {
        foreach (Request request in requests)
        {
            currentStock.Add(request.itemName, 0);
        }
    }

    private void SetupGUI()
    {
        foreach (Request request in requests)
        {
            GameObject requestUI = Instantiate(requestUIPrefab, requestUIParent);
            TextMeshProUGUI itemText = requestUI.transform.Find("Item Name Text").GetComponent<TextMeshProUGUI>();
            itemText.text = request.itemName;

            TextMeshProUGUI amountText = requestUI.transform.Find("Amount Text").GetComponent<TextMeshProUGUI>();
            request.amountText = amountText;
        }

        UpdateGUI();
    }

    private void UpdateGUI()
    {
        foreach (Request request in requests)
        {
            request.amountText.text = currentStock[request.itemName] + "/" + request.amount;
        }
    }

    private void CheckConditions()
    {
        bool requestsFilled = true;
        foreach (Request request in requests)
        {
            if (currentStock[request.itemName] < request.amount)
            {
                requestsFilled = false;
            }
        }

        if (requestsFilled)
        {
            Leave();
        }
    }

    private void Leave()
    {
        print("CYA");
    }

    private void OnTriggerEnter(Collider other)
    {
        Grabbable grabbable = other.gameObject.GetComponent<Grabbable>();
        if (grabbable != null)
        {
            foreach (Request request in requests)
            {
                bool nameMatches = grabbable.itemName == request.itemName;
                bool stockStillNeeded = currentStock[grabbable.itemName] < request.amount;
                if (nameMatches && stockStillNeeded && GameManager.instance.crops.Contains(grabbable))
                {
                    grabbable.PutInTruck();
                    currentStock[grabbable.itemName]++;
                    UpdateGUI();
                    CheckConditions();
                    GetComponent<Animator>().SetTrigger("cooped");
                }
            }
        }
    }

    private void OutOfPatience()
    {
        patience.StopTimer();
        GameManager.instance.LoseGame(0, transform);
    }
}

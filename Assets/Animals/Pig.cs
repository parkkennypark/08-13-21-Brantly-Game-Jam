using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pig : MonoBehaviour
{
    public enum State
    {
        IN_PEN,
        ROAMING,
        BEING_HELD,
        THROWN,
        EATING
    }

    public float roamDelayBase;
    public float roamDelayVariance;

    private State state;
    private Animator animator;
    private new Rigidbody rigidbody;
    private float currentRoamDelay;
    private NavMeshAgent nav;
    private Grabbable navTarget;
    private Patience patience;

    void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        patience = GetComponentInChildren<Patience>();
    }

    void Start()
    {
        ChangeState(State.IN_PEN);
        // currentRoamDelay -= Random.Range(0, roamDelayBase);

        patience.OnTimerTimeout += EatFood;
    }

    void SetRoamDelay()
    {
        currentRoamDelay = roamDelayBase + Random.Range(-roamDelayVariance, roamDelayVariance);
    }

    void Update()
    {
        if (!GameManager.instance.gameStarted)
        {
            return;
        }

        switch (state)
        {
            case State.IN_PEN:
                currentRoamDelay -= Time.deltaTime;
                if (currentRoamDelay <= 0)
                {
                    ChangeState(State.ROAMING);
                }
                break;
            case State.ROAMING:
                if (navTarget != null)
                {
                    nav.destination = navTarget.transform.position;
                }
                if ((navTarget.transform.position - transform.position).magnitude < 0.5f)
                {
                    ChangeState(State.EATING);
                }
                if (!GameManager.instance.crops.Contains(navTarget))
                {
                    print("PANIC!");
                    NavToRandomCrop();
                }
                break;
            case State.BEING_HELD:
                break;
            case State.THROWN:
                if (rigidbody.velocity.magnitude <= 0.2)
                {
                    ChangeState(State.ROAMING);
                }
                break;
            case State.EATING:
                if ((navTarget.transform.position - transform.position).magnitude > 1f)
                {
                    ChangeState(State.ROAMING);
                }
                break;
        }

        animator.SetFloat("speed", nav.velocity.magnitude / nav.speed);
    }

    public void ReturnedToPen()
    {
        ChangeState(State.IN_PEN);
    }

    void ChangeState(State newState)
    {
        state = newState;

        nav.enabled = true;
        patience.SetVisible(false);
        patience.StopTimer();

        switch (state)
        {
            case State.IN_PEN:
                SetRoamDelay();
                nav.isStopped = true;
                break;
            case State.ROAMING:
                NavToRandomCrop();
                nav.isStopped = false;
                break;
            case State.BEING_HELD:
                nav.isStopped = true;
                break;
            case State.EATING:
                nav.isStopped = true;
                patience.SetVisible(true);
                patience.StartTimer();
                break;
            case State.THROWN:
                nav.enabled = false;
                break;
        }
    }

    public void OnGrabbed()
    {
        ChangeState(State.BEING_HELD);
    }

    public void OnThrown()
    {
        ChangeState(State.THROWN);
    }

    void NavToRandomCrop()
    {
        List<Grabbable> crops = GameManager.instance.crops;
        if (navTarget && crops.Contains(navTarget))
        {
            return;
        }

        if (crops.Count == 0)
        {
            ChangeState(State.IN_PEN);
            return;
        }

        navTarget = crops[Random.Range(0, crops.Count)];
        // navTarget = crop;
    }

    void EatFood()
    {
        patience.StopTimer();
        GameManager.instance.LoseGame(1, transform);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Pen")
        {
            ChangeState(State.IN_PEN);
            other.GetComponent<Animator>().SetTrigger("cooped");
        }
    }
}

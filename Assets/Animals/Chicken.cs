using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chicken : MonoBehaviour
{
    public enum State
    {
        NAVING_TO_CROP,
        RUNNING,
        GRABBED,
        THROWN
    }

    public float speed;
    public float rotationSpeed;
    public float changeDirectionDelay;

    private new Rigidbody rigidbody;
    private float timeUntilDirectionChange;
    private Grabbable navTarget;
    private NavMeshAgent nav;

    private State state;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        ChangeState(State.NAVING_TO_CROP);
    }

    void Update()
    {
        switch (state)
        {
            case State.NAVING_TO_CROP:
                if ((navTarget.transform.position - transform.position).magnitude < 0.5)
                {
                    navTarget.Grab(transform);
                }
                if (navTarget != null)
                {
                    nav.destination = navTarget.transform.position;
                }
                break;
            case State.RUNNING:
                timeUntilDirectionChange -= Time.deltaTime;
                if (timeUntilDirectionChange <= 0 || (transform.position - nav.destination).magnitude < 1)
                {
                    SetRandomPosition();
                    timeUntilDirectionChange = changeDirectionDelay * Random.Range(0.5f, 2);
                }
                break;

            case State.THROWN:
                if (rigidbody.velocity.magnitude <= 0.2)
                {
                    ChangeState(State.NAVING_TO_CROP);
                }
                break;
        }

        // Quaternion targetRot = Quaternion.LookRotation(rigidbody.velocity, Vector3.up);
        // transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

    }

    public void ChangeState(State newState)
    {
        state = newState;

        switch (state)
        {
            case State.NAVING_TO_CROP:
                nav.enabled = true;
                NavToRandomCrop();
                nav.isStopped = false;
                break;
            case State.RUNNING:
                nav.enabled = true;
                SetRandomPosition();
                nav.isStopped = false;
                break;
            case State.GRABBED:
                nav.enabled = false;
                nav.isStopped = true;
                // rigidbody.isKinematic = true;
                if (navTarget)
                {
                    navTarget.Drop();
                }
                break;
            case State.THROWN:
                nav.enabled = false;
                // rigidbody.isKinematic = false;
                break;
        }
    }

    public void OnThrown()
    {
        ChangeState(State.THROWN);
    }

    public void OnGrabbed()
    {
        ChangeState(State.GRABBED);
    }

    void SetRandomPosition()
    {
        nav.destination = GetRandomLocation();
    }

    Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Pick the first indice of a random triangle in the nav mesh
        int t = Random.Range(0, navMeshData.indices.Length - 3);

        // Select a random point on it
        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

        return point;
    }


    private void OnCollisionEnter(Collision other)
    {
        SetRandomPosition();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Coop" && state == State.THROWN)
        {
            other.GetComponent<Animator>().SetTrigger("cooped");
            Destroy(gameObject);
        }
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
            ChangeState(State.RUNNING);
            return;
        }

        navTarget = crops[Random.Range(0, crops.Count)];
        // navTarget = crop;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chicken : MonoBehaviour
{
    public enum State
    {
        IN_PEN,
        CRAZED
    }

    public float speed;
    public float rotationSpeed;
    public float changeDirectionDelay;

    private new Rigidbody rigidbody;
    private float timeUntilDirectionChange;

    private State state;
    private Vector3 direction;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        ChangeState(State.CRAZED);
    }

    void Update()
    {
        if (state == State.CRAZED)
        {
            rigidbody.velocity = direction * speed;
            timeUntilDirectionChange -= Time.deltaTime;
            if (timeUntilDirectionChange <= 0)
            {
                SetRandomDirection();
                timeUntilDirectionChange = changeDirectionDelay * Random.Range(0.5f, 2);
            }

        }

        Quaternion targetRot = Quaternion.LookRotation(rigidbody.velocity, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

    }

    void ChangeState(State newState)
    {
        state = newState;

        if (state == State.CRAZED)
        {
            SetRandomDirection();
        }
    }

    void SetRandomDirection()
    {
        direction = (GetRandomLocation() - transform.position).normalized;
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
        print("HIT SOMETHING");
        SetRandomDirection();
    }
}

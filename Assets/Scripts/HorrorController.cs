using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HorrorController : MonoBehaviour {

    public NavMeshAgent agent;
    public float moveSpeed;
    public float rotationSpeed;
    public float fleeDistance;

    public Transform flashlight;
    public float flashlightAngle;
    public float flashlightDistance;

    private GameObject player;
    private Rigidbody rb;

    void Start() {
        player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();

        StartCoroutine(Chase());
    }


    IEnumerator Flee(){
        while (true){
            var distance = (transform.position - player.transform.position);

            if (distance.magnitude > fleeDistance){
                break;
            }

            agent.SetDestination(transform.position + distance.normalized);
            yield return null;
        }

        StartCoroutine(Chase());
    }

    IEnumerator Chase(){
        while (true){
            RotateEnemy();
            MovementToPlayer();

            if (isInView()){
                StartCoroutine(Flee());
                yield break;
            }
            
            yield return null;
        }
    }

    private void MovementToPlayer() {
        agent.SetDestination(player.transform.position);
    }

    private void RotateEnemy() {
        rb.constraints &= ~RigidbodyConstraints.FreezeRotationY;

        Quaternion targetRotation = Quaternion.LookRotation((new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z) - transform.position).normalized, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        rb.constraints |= RigidbodyConstraints.FreezeRotationY; 
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            // DEATH EVENT
            Debug.Log("DEATH");
        }
    }

    private bool isInView() {
        var distance = transform.position - flashlight.position;

        return distance.magnitude <= flashlightDistance && Vector3.Angle(flashlight.forward, distance.normalized) < flashlightAngle / 2;
    }
}

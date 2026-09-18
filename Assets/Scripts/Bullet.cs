using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 20f;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        rb.linearVelocity = transform.forward * bulletSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
}

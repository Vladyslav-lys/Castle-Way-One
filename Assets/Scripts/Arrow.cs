using System;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform target;
    public float speed;
    private bool _canKill;

    private void OnEnable()
    {
        _canKill = true;
    }

    void Update()
    {
        if (target)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_canKill)
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
            _canKill = false;
        }
    }
}

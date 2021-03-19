using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ArrowShootCastle : MonoBehaviour
{
    public Transform shootTransform;
    public float shootDelay;
    public GameObject bullet;
    public GameObject currentTarget;
    public Transform cursorDirection;
    public ArrowCastle arrowCastle;
    public List<int> collisionLayers;

    private bool _isShooting;
    private bool _hasTarget;
    private Transform _target;

    private void Update()
    {
        if(_target && !_isShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (var layer in collisionLayers)
        {
            if (other.gameObject.layer == layer && !_hasTarget)
            {
                currentTarget = other.gameObject;
                _target = currentTarget.transform;
                _hasTarget = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _hasTarget = false;
    }

    private IEnumerator Shoot()
    {
        if (currentTarget == null)
        {
            _hasTarget = false;
        }

        _isShooting = true;
        cursorDirection.rotation = Quaternion.LookRotation(_target.position - cursorDirection.position);
        GameObject localBullet = GameObject.Instantiate(bullet, shootTransform.position, 
            Quaternion.Euler(0f, cursorDirection.rotation.eulerAngles.y - 90f, -90f)) as GameObject;
        localBullet.GetComponent<Arrow>().target = currentTarget.transform;
        yield return new WaitForSeconds(shootDelay);

        _hasTarget = false;
        _isShooting = false;
    }
}
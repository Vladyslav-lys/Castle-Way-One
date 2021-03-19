using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragArrowPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public GameManager gm;
    private Camera _cam;
    private GameObject _arrow;
    private Transform _targetTransform;
    private RaycastHit _hit;
    private int _clicked;
    private float _clicktime;
    private float _clickdelay;
    public List<LineRenderer> lineRenderers;
    public Vector3 offset;
    
    private void Start()
    {
        gm.arrowController = this;
        _cam = Camera.main;
        _clicked = 0;
        _clicktime = 0;
        _clickdelay = 0.7f;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        _clicked++;
        
        if (_clicked > 2 || Time.time - _clicktime > 0.7f)
        {
            _clicked = 1;
        }
        
        if (_clicked == 1)
            _clicktime = Time.time;
 
        if (_clicked > 1 && Time.time - _clicktime < _clickdelay)
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out _hit) 
                && _hit.transform.gameObject.layer == 10
                && _hit.transform.gameObject.GetComponent<Castle>()
                && !_hit.transform.gameObject.GetComponent<Castle>().isBuilding
                && gm.isStarted)
            {
                _hit.transform.gameObject.GetComponent<Castle>().CastleLevelUp();
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out _hit) 
            && _hit.transform.gameObject.layer == 10
            && _hit.transform.gameObject.GetComponent<Castle>()
            && !_hit.transform.gameObject.GetComponent<Castle>().isBuilding
            && gm.isStarted)
        {
            gm.spawners.Add(_hit.transform.gameObject.GetComponent<Castle>().spawner);
            var castle = _hit.transform.gameObject.GetComponent<Castle>();
            castle.spawner.isIncluded = true;
            lineRenderers.Add(castle.lineRenderer);
            lineRenderers[0].positionCount = 1;
            lineRenderers[0].SetPosition(0, _hit.point + offset);
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out _hit))
        {
            if (_hit.transform.gameObject.GetComponent<Castle>() 
                && _hit.transform.gameObject.layer == 10 
                && !_hit.transform.gameObject.GetComponent<Castle>().spawner.isIncluded
                && !_hit.transform.gameObject.GetComponent<Castle>().isBuilding
                && gm.isStarted)
            {
                var castle = _hit.transform.gameObject.GetComponent<Castle>();
                lineRenderers.Add(castle.lineRenderer);
                lineRenderers[lineRenderers.Count - 1].positionCount = 1;
                lineRenderers[lineRenderers.Count - 1].SetPosition(0, _hit.point + offset);
                gm.spawners.Add(castle.spawner);
                gm.spawners[gm.spawners.Count - 1].isIncluded = true;
            }
            
            if(lineRenderers.Count == 0)
                return;
            
            foreach (var lineRenderer in lineRenderers)
            {
                if (lineRenderer.positionCount != 0 && _hit.point.y > 2f)
                {
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(1, _hit.point + offset);
                }
            }
            
            if ((_hit.transform.gameObject.layer == 11 || _hit.transform.gameObject.layer == 10 
                || _hit.transform.gameObject.layer == 13)
                && _hit.transform.gameObject.GetComponent<Castle>())
            {
                _targetTransform = _hit.transform.gameObject.GetComponent<Castle>().castleTransform;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.positionCount = 0;
        }
        lineRenderers.Clear();
        if (gm.spawners.Count > 0
           && _hit.transform
           && (_hit.transform.gameObject.layer == 11 || _hit.transform.gameObject.layer == 10 || _hit.transform.gameObject.layer == 13)
           && _targetTransform != gm.spawners[0].spawnPoint)
        {
            if (gm.spawners[gm.spawners.Count - 1].castle.castleTransform == _targetTransform)
            {
                gm.spawners[gm.spawners.Count - 1].isIncluded = false;
                gm.spawners.RemoveAt(gm.spawners.Count - 1);
            }
            
            for (int i = 0; i < gm.spawners.Count; i++)
            {
                gm.spawners[i].targetTransform = _targetTransform;
            }
            gm.SetSpawner();
        }

        if (gm.spawners.Count > 0)
        {
            for (int i = 0; i < gm.spawners.Count; i++)
            {
                gm.spawners[i].isIncluded = false;
            }
            gm.spawners.Clear();
        }
    }
}

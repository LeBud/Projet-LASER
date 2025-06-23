using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastMancini : MonoBehaviour
{
    RaycastHit hit;
    public int maxRaycast = 10;
    public float macDistance = 10;

    public bool castReflection = true;
    public bool drawRay = false;

    public LineRenderer lineRenderer;

    private void Update()
    {
        Physics.Raycast(transform.position, transform.forward, out hit);

        Vector3 reflectDir = Vector3.Reflect(transform.forward, hit.normal);

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        if (castReflection)
            RecursiveHit(hit, reflectDir, maxRaycast, 0);
        else
            RecursiveHitDistance(hit, reflectDir, macDistance,0);
    }


    RaycastHit RecursiveHit(RaycastHit hit, Vector3 dir, int _maxRaycast, int lr)
    {
        RaycastHit _hit;

        if (_maxRaycast <= 0)
            return new RaycastHit();
        else if (Physics.Raycast(hit.point, dir, out _hit) && _maxRaycast > 0)
        {
            if(drawRay)
                Debug.DrawRay(hit.point, dir * _hit.distance, Color.blue);
            else
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lr, hit.point);
            }
            return RecursiveHit(_hit, Vector3.Reflect(dir, _hit.normal), _maxRaycast-1, lr + 1);
        }
        else
            return new RaycastHit();
    }

    RaycastHit RecursiveHitDistance(RaycastHit hit, Vector3 dir, float distance, int lr)
    {
        RaycastHit _hit;

        if (distance <= 0)
            return new RaycastHit();
        else if (Physics.Raycast(hit.point, dir, out _hit, distance - hit.distance))
        {
            if (drawRay)
                Debug.DrawRay(hit.point, dir * _hit.distance, Color.blue);
            else
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lr, hit.point);
            }
            return RecursiveHitDistance(_hit, Vector3.Reflect(dir, _hit.normal), distance - hit.distance, lr +1);
        }
        else
        {
            Debug.DrawRay(hit.point, dir * (distance - hit.distance), Color.yellow);
            return new RaycastHit();
        }
    }
}

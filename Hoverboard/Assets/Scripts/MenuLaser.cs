using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class MenuLaser : MonoBehaviour
{
    public LineRenderer lineRenderer;
    List<Vector3> points = new List<Vector3>();

    public ParticleSystem laserParticle;
    public ParticleSystem portalParticle;

    private void Start()
    {
        points.Clear();
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        RecursiveHit(lineRenderer, transform.position, transform.forward, 100, 1, points, false);

        for(int i = 0; i < points.Count; i++)
        {
            Instantiate(laserParticle, points[i], Quaternion.identity);
        }

        ParticleSystem p = Instantiate(portalParticle, lineRenderer.GetPosition(0), transform.rotation);
        p.Play();
    }

    void RecursiveHit(LineRenderer line, Vector3 origin, Vector3 dir, int _maxRaycast, int lr, List<Vector3> points, bool isPlaced)
    {
        RaycastHit hit;

        if (_maxRaycast <= 0)
            return;
        else if (Physics.Raycast(origin, dir, out hit) && _maxRaycast > 0)
        {
            Debug.DrawRay(origin, dir * hit.distance, Color.blue);
            line.positionCount++;
            line.SetPosition(lr, hit.point);
            points.Add(hit.point);
            if (hit.collider.CompareTag("Receptor"))
            {
                return;
            }
            else if (hit.collider.CompareTag("Relay"))
            {
                
            }
            else if (!hit.collider.CompareTag("Reflect"))
                return;

            RecursiveHit(line, hit.point, Vector3.Reflect(dir, hit.normal), _maxRaycast - 1, lr + 1, points, isPlaced);
        }

    }
}

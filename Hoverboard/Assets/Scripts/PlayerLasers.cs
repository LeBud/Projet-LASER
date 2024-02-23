using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLasers : MonoBehaviour
{
    [Header("Line Renderer")]
    [SerializeField] LineRenderer teleportRenderer;
    [SerializeField] LineRenderer laserRenderer;

    [Header("Settings")]
    [SerializeField] Transform origin;
    [SerializeField] float teleportDistance;
    [SerializeField] int maxReflection;

    [Header("Laser Mat")]
    [SerializeField] Material laserPreviewMat;
    [SerializeField] Material laserPlaceMat;

    AudioSource audioSource;
    [Header("Audio")]
    [SerializeField] AudioClip laserShotSound, teleportSound;

    List<Vector3> teleportsPoints = new List<Vector3>();
    List<Vector3> laserPoints = new List<Vector3>();

    bool holdingRight;
    bool holdingLeft;

    List<LaserRelay> relays = new List<LaserRelay>();

    List<ParticleSystem> laserParticlesList = new List<ParticleSystem>();

    [SerializeField] ParticleSystem laserParticle;
    [SerializeField] ParticleSystem teleportParticle;

    [SerializeField] ParticleSystem colisionParticle;

    ParticleSystem laserPortalParticle;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(PauseMenu.IsPaused) return;

        MyInputs();
    }

    void MyInputs()
    {
        if (InputsBrain.Instance.teleport.IsPressed() && !holdingLeft)
            HandleTeleportsFunction(false);
        else if(InputsBrain.Instance.teleport.WasReleasedThisFrame())
            HandleTeleportsFunction(true);

        if (InputsBrain.Instance.laser.IsPressed() && !holdingRight)
            handleLaserFunction(false);
        else if (InputsBrain.Instance.laser.WasReleasedThisFrame())
            handleLaserFunction(true);
    }

    void HandleTeleportsFunction(bool teleport)
    {
        if (!teleport)
        {
            if(!teleportParticle.isPlaying)
                teleportParticle.Play();
            if (!audioSource.isPlaying)
                audioSource.Play();

            holdingRight = true;
            teleportsPoints.Clear();
            teleportRenderer.positionCount = 1;
            teleportRenderer.SetPosition(0, origin.position);
            RecursiveHitDistance(teleportRenderer, origin.position, Camera.main.transform.forward, teleportDistance, 1, teleportsPoints);
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(teleportSound);
            teleportParticle.Stop();
            holdingRight = false;
            teleportsPoints.Clear();
            RecursiveHitDistance(teleportRenderer, origin.position, Camera.main.transform.forward, teleportDistance, 1, teleportsPoints);
            Vector3 tpPos = teleportsPoints[teleportsPoints.Count - 1];
            transform.position = new Vector3(tpPos.x, tpPos.y + 1, tpPos.z);
            teleportRenderer.positionCount = 0;
        }
    }

    void handleLaserFunction(bool place)
    {
        if (!place)
        {
            if(!laserParticle.isPlaying)
                laserParticle.Play();
            if (!audioSource.isPlaying)
                audioSource.Play();

            holdingLeft = true;
            laserPoints.Clear();
            laserRenderer.positionCount = 1;
            laserRenderer.SetPosition(0, origin.position);
            laserRenderer.material = laserPreviewMat;
            RecursiveHit(laserRenderer, origin.position, Camera.main.transform.forward, maxReflection, 1, laserPoints, false);

            if(laserParticlesList.Count > 0)
                foreach(var part in laserParticlesList)
                    Destroy(part);
            laserParticlesList.Clear();
            if(laserPortalParticle != null)
                Destroy(laserPortalParticle);
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(laserShotSound);
            laserParticle.Stop();
            holdingLeft = false;
            laserPoints.Clear();
            laserRenderer.positionCount = 1;
            laserRenderer.SetPosition(0, origin.position);
            laserRenderer.material = laserPlaceMat;

            foreach (var relay in relays.Where(r => r.isActivated))
                relay.DeactivateRelay();
            relays.Clear();

            RecursiveHit(laserRenderer, origin.position, Camera.main.transform.forward, maxReflection, 1, laserPoints, true);

            foreach (var p in laserPoints)
                laserParticlesList.Add(Instantiate(colisionParticle, p, Quaternion.identity));

            laserPortalParticle = Instantiate(laserParticle, laserRenderer.GetPosition(0), Camera.main.transform.rotation);
            laserPortalParticle.Play();
        }
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
                if (isPlaced)
                    hit.collider.GetComponent<LaserReceptor>().Receptor();
                return;
            }
            else if (hit.collider.CompareTag("Relay"))
            {
                if (isPlaced)
                {
                    relays.Add(hit.collider.GetComponent<LaserRelay>());
                    hit.collider.GetComponent<LaserRelay>().Relay();
                }
            }
            else if (!hit.collider.CompareTag("Reflect"))
                return;

            RecursiveHit(line, hit.point, Vector3.Reflect(dir, hit.normal), _maxRaycast - 1, lr + 1, points, isPlaced);
        }
        
    }

    void RecursiveHitDistance(LineRenderer line, Vector3 origin, Vector3 dir, float distance, int lr, List<Vector3> points)
    {
        RaycastHit hit;

        if (distance <= 0)
            return;
        else if (Physics.Raycast(origin, dir, out hit, distance))
        {
            Debug.DrawRay(origin, dir * hit.distance, Color.blue);
            line.positionCount++;
            line.SetPosition(lr, hit.point);
            points.Add(hit.point);

            if (!hit.collider.CompareTag("Reflect"))
                return;

            RecursiveHitDistance(line, hit.point, Vector3.Reflect(dir, hit.normal), distance - hit.distance, lr + 1, points);
        }
        else
        {
            Debug.DrawRay(origin, dir * distance, Color.yellow);
            line.positionCount++;
            line.SetPosition(lr, origin + dir * distance);
            points.Add(origin + dir * distance);
            return;
        }
    }
}

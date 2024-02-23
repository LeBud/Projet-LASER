using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserRelay : MonoBehaviour
{
    MeshRenderer mesh;

    public bool isActivated = false;
    [SerializeField] Material activatedMat;
    [SerializeField] Material desactivatedMat;

    [SerializeField] AudioClip relayOnSound;

    AudioSource audio;

    public bool receptorActivated;

    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        audio = GetComponent<AudioSource>();
    }

    public void Relay()
    {
        mesh.material = activatedMat;
        audio.PlayOneShot(relayOnSound);
        isActivated = true;
    }

    public void DeactivateRelay()
    {
        if (receptorActivated)
            return;

        mesh.material = desactivatedMat;
        isActivated = false;
    }
}

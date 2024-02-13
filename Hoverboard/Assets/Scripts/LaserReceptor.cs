using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserReceptor : MonoBehaviour
{
    MeshRenderer mesh;

    [SerializeField] bool needRelay = true;
    [SerializeField] List<LaserRelay> laserRelays = new List<LaserRelay>();

    [SerializeField] GameObject doorObject;

    [SerializeField] Material notActivatedMat;
    [SerializeField] Material activatedMat;

    bool isCompleted = false;



    private void Start()
    {
        mesh = GetComponent<MeshRenderer>();
    }

    public void Receptor()
    {
        if(isCompleted) return;

        if (!needRelay)
        {
            Completed();
            return;
        }

        int num = 0;
        foreach (var relay in laserRelays.Where(r => r.isActivated))
            num++;

        if (num == laserRelays.Count)
            Completed();
        else
        {
            Debug.Log("Not all relay activated");
            mesh.material = notActivatedMat;
        }
    }

    void Completed()
    {
        Debug.Log("All relay activated");
        foreach (var relay in laserRelays.Where(r => r.isActivated))
            relay.receptorActivated = true;

        mesh.material = activatedMat;
        isCompleted = true;
        doorObject.SetActive(false);
    }
}

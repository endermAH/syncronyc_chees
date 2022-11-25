using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tryr : MonoBehaviour
{

    [SerializeField] Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<ParticleSystem>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

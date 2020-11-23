﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObjectOnParticleStop : MonoBehaviour
{
    [SerializeField]
    GameObject gameObjectToDestroy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnParticleSystemStopped() {
        Destroy(gameObjectToDestroy);
    }

    public void SetGameObjectToDestroy(GameObject gameObjectToDestroy) {
        this.gameObjectToDestroy = gameObjectToDestroy;
    }
}
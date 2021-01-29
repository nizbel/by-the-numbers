using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDestructibleOnParticleStop : MonoBehaviour
{
    DestructibleObject destructibleScript;

    public void OnParticleSystemStopped() {
        destructibleScript.SetIsDestructibleNow(true);
        // Destroy itself after job is done
        Destroy(this);
    }

    public void SetDestructibleScript(DestructibleObject destructibleScript) {
        this.destructibleScript = destructibleScript;
    }
}

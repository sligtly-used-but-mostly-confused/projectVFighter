using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAIPlayerController : PlayerController {

    public override void Kill()
    {
        base.Kill();
        Destroy(gameObject);
    }
}

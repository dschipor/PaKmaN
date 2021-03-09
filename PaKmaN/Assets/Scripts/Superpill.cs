using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Superpill : Collectible {
    
    protected override void collected()
    {
        GameManager.makeGhostsVulnerable();
        base.collected();
    }
}

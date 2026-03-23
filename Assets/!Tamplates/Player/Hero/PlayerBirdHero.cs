using System;
using UnityEngine;

public class PlayerBirdHero : Player2DAction<ControllerBirdHero>
{
    public override void Init(IInput input)
    {
        base.Init(input);

        //damageCollider.OnPush += Push;
    }

    public void Push(Vector2 force)
    {
        playerController.Push(new Vector2(force.x, 0f));        
    }

    public void ReloadDash()
    {
        playerController.ReloadDash();
    }
}

using UnityEngine;

public class PlayerEscape : Player2DAction<ControllerPlayerEscape>
{
    public override void SetSpeedKof(float speed)
    {
        //base.SetSpeedKof(speed);

        playerController.SetCoefSpeed(speed);
    }
}

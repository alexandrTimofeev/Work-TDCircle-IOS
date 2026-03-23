using System;
using System.Collections;

[Serializable]
public class ClearObstaclesGrapAction : GrappableObjectAction
{
    public float Procent;

    public override void Work(GrappableObjectContext context)
    { }

    public override IEnumerator WorkRoutine(GrappableObjectContext context)
    {
        yield break;
    }
}
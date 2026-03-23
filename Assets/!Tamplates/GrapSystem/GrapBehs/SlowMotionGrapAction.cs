using System;
using System.Collections;

[Serializable]
public class SlowMotionGrapAction : GrappableObjectAction
{
    public float Duration;

    public override void Work(GrappableObjectContext context)
    { }

    public override IEnumerator WorkRoutine(GrappableObjectContext context)
    {
        yield break;
    }
}
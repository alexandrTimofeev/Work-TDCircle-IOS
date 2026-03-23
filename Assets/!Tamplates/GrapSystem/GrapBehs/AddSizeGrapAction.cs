using System;
using System.Collections;

[Serializable]
public class AddSizeGrapAction : GrappableObjectAction
{
    public float AddSize;
    public float Duration;

    public override void Work(GrappableObjectContext context)
    {  }

    public override IEnumerator WorkRoutine(GrappableObjectContext context)
    { yield break; }
}
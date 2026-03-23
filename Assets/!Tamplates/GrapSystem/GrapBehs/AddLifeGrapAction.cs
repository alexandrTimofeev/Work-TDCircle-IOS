using System;
using System.Collections;


[Serializable]
public class AddLifeGrapAction : GrappableObjectAction
{
    public int AddLife;

    public override void Work(GrappableObjectContext context)
    { }

    public override IEnumerator WorkRoutine(GrappableObjectContext context)
    { yield break; }
}

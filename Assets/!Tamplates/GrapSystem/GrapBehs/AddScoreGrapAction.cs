using System;
using System.Collections;

[Serializable]
public class AddScoreGrapAction : GrappableObjectAction
{
    public int AddScore;

    public override void Work(GrappableObjectContext context)
    {
        GameG.ScoreSys.AddScore(AddScore, context.GrapObject.transform.position);
    }

    public override IEnumerator WorkRoutine(GrappableObjectContext context)
    { yield break; }
}
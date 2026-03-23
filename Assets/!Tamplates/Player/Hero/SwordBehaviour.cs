using System;
using UnityEngine;

public class SwordBehaviour : MonoBehaviour
{
    [SerializeField] private ControllerBirdHero controllerBirdHero;
    [SerializeField] private PlayerBirdHero playerBirdHero;
    [SerializeField] private Vector2 jagleForce;

    [SerializeField] private float angleJungle = 20f;

    [Space]
    [SerializeField] private int addScoreForKill = 500;
    [SerializeField] private int addScoreForJugle = 400;

    [Space]
    [SerializeField] private GameObject jagleVFXPref;

    private DamageHurtBox2D damageCollider;

    private void Start()
    {
        damageCollider = GetComponent<DamageHurtBox2D>();
        //damageCollider.OnDamage += DamageProcess;
    }

    public void Jagle ()
    {
        if (IsCanJagle() == false)
            return;

        controllerBirdHero.Push(jagleForce);
        playerBirdHero.ReloadDash();
        //GameEntryGameplayCCh.GameScoreSystem.AddScore(addScoreForJugle, transform.position);
        InterfaceManager.CreateFlyingText("Hit Jump!", Color.white, transform.position - (Vector3.down * 1f), null);
        Destroy(Instantiate(jagleVFXPref, transform.position - (Vector3.down * 1f), transform.rotation), 10f);

    }

    private bool IsCanJagle()
    {
        Debug.Log($"IsCanJagle {Vector2.Angle(Vector2.down, transform.right)} {controllerBirdHero.Rigidbody2D.linearVelocity.y}");
        return Vector2.Angle(Vector2.down, transform.right) < angleJungle && controllerBirdHero.Rigidbody2D.linearVelocity.y < 0;
    }

    private void DamageProcess(DamageHitBox2D damageContainer)
    {
        if (damageContainer.gameObject.tag == "Enemy")
        {
            //GameEntryGameplayCCh.GameScoreSystem.AddScore(addScoreForKill, damageContainer.transform.position);
            InterfaceManager.CreateFlyingText("Kill!", Color.white, damageContainer.transform.position - (Vector3.down * 1f), null);
        }
        //GameEntryGameplayCCh.ObjectMediator.InvokeEventEnter(new AddScoreGrapAction() { AddScore = addScoreForKill }, null);
    }
}

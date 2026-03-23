using System;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] private Transform gun;
    [SerializeField] private Transform shootTr;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject shootVFXPref;
    [SerializeField] private float minAngle = -60f;
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private float shoorForce = 100f;
    [SerializeField] private bool notRotateProjectile;

    private IInput input;

    public event Action<GameObject> OnShoot;

    public void Init(IInput input)
    {
        this.input = input;
        input.OnMoved += OnPointerMoved;
        input.OnEnded += OnPointerEnded;
    }

    private void OnPointerMoved(IInput.InputMoveScreenInfo info)
    {
        if (enabled == false)
            return;

        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(input.GetOverPosition());
        worldPos.z = gun.position.z;

        Vector3 dir = worldPos - gun.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        gun.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnPointerEnded(Vector2 screenPos)
    {
        Shoot();
    }

    private void Shoot()
    {
        if (projectilePrefab == null || enabled == false)
            return;

        GameObject ngo = Instantiate(projectilePrefab, shootTr.position, (notRotateProjectile ? Quaternion.identity : gun.rotation));
        ngo.GetComponent<Rigidbody2D>().AddForce(gun.transform.right * shoorForce, ForceMode2D.Impulse);

        GameObject vfx = Instantiate(shootVFXPref, shootTr.position, (notRotateProjectile ? Quaternion.identity : gun.rotation));
        Destroy(vfx, 10f);

        OnShoot?.Invoke(ngo);
    }

    private void OnDestroy()
    {
        if (input != null)
        {
            input.OnMoved -= OnPointerMoved;
            input.OnEnded -= OnPointerEnded;
        }
    }
}
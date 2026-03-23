using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Основной класс турели с гибкими настройками поведения
/// </summary>
public class Attaker : MonoBehaviour
{
    [Header("Оружие")]
    [SerializeField] private GunForAttaker gun;

    [Header("Цели")]
    [SerializeField] private List<TargetsPreset> myTagsTarget = new List<TargetsPreset>();
    [SerializeField] private TargetPriority targetPriority = TargetPriority.Nearest;
    [SerializeField] private LayerMask targetLayerMask = -1;
    [SerializeField] private float detectionInterval = 0.2f; // как часто искать цели (для оптимизации)

    [Header("Дальность")]
    [SerializeField] private float minDistanceToAttack = 0f;
    [SerializeField] private float maxDistanceToAttack = 20f;
    [SerializeField] private bool useDistanceFalloff = false; // урон падает с расстоянием

    [Header("Поворот")]
    [SerializeField] private float rotationSpeed = 180f; // градусов в секунду
    [SerializeField] private bool instantRotation = false; // мгновенный поворот (для лазеров)
    [SerializeField] private bool rotateContinuously = true; // постоянно поворачиваться к цели
    [SerializeField] private float rotationSmoothing = 5f; // сглаживание поворота

    [Header("Ограничения поворота")]
    [SerializeField] private bool hasRotationLimits = false;
    [SerializeField] private float minRotationAngle = -90f; // относительно начального угла
    [SerializeField] private float maxRotationAngle = 90f;
    [SerializeField] private bool clampRotationInstantly = false; // мгновенно ограничивать или плавно

    [Header("Стрельба")]
    [SerializeField] private FireMode fireMode = FireMode.Single;
    [SerializeField] private float fireRate = 1f; // выстрелов в секунду
    [SerializeField] private int burstCount = 3; // для режима Burst
    [SerializeField] private float burstDelay = 0.1f; // задержка между выстрелами в очереди
    [SerializeField] private float chargeTime = 0f; // время зарядки перед выстрелом
    [SerializeField] private bool continuousFire = false; // стрелять пока цель в зоне (для пулемётов)
    [SerializeField] private float windUpTime = 0f; // время раскрутки
    [SerializeField] private float windDownTime = 0f; // время остановки после стрельбы

    [Header("Точность")]
    [SerializeField] private float accuracy = 1f; // 1 = идеально, 0.5 = разброс
    [SerializeField] private float accuracyDegradation = 0f; // ухудшение точности при стрельбе
    [SerializeField] private float accuracyRecoveryRate = 1f; // восстановление точности
    [SerializeField] private bool useLeading = false; // стрелять с упреждением
    [SerializeField] private float leadingFactor = 1f; // коэффициент упреждения

    [Header("Углы атаки")]
    [SerializeField] private bool hasFiringArc = false;
    [SerializeField] private float firingArcAngle = 90f; // угол сектора обстрела
    [SerializeField] private bool requireLineOfSight = false; // требовать прямую видимость
    [SerializeField] private LayerMask obstacleMask; // что считается препятствием

    [Header("Поведение")]
    [SerializeField] private bool searchNewTargetWhenLost = true;
    [SerializeField] private float targetLostDelay = 1f; // сколько ждать перед сменой цели
    [SerializeField] private bool canSwitchTargetDuringReload = false;
    [SerializeField] private bool prioritizeLowHealth = false;
    [SerializeField] private float targetMemoryTime = 2f; // сколько помнить скрывшуюся цель

    [Header("Специальные режимы")]
    [SerializeField] private bool targetClosestToBase = false; // цель, ближайшую к базе
    [SerializeField] private bool targetFastest = false; // самую быструю цель
    [SerializeField] private bool targetStrongest = false; // цель с наибольшим HP
    [SerializeField] private bool ignoreUntargetable = true; // игнорировать неуязвимые цели
    [SerializeField] private bool canTargetSameEnemy = true; // несколько турелей могут целиться в одного врага

    [Header("Состояния")]
    [SerializeField] private bool startActive = true;
    [SerializeField] private bool canFireWhileReloading = false;
    [SerializeField] private bool requiresPower = false;
    [SerializeField] private float powerConsumption = 0f;

    // Приватные переменные
    private Transform currentTarget;
    private float currentAccuracy;
    private float currentWindUp;
    private float nextFireTime;
    private float nextDetectionTime;
    private float targetLostTimer;
    private float currentRotation;
    private float targetAngle;
    private Quaternion initialRotation;
    private bool isWindingUp;
    private bool isFiring;
    private Coroutine burstCoroutine;
    private Coroutine chargeCoroutine;
    private Dictionary<Transform, float> targetMemory = new Dictionary<Transform, float>();

    // Свойства для доступа из других скриптов
    public Transform CurrentTarget => currentTarget;
    public bool IsFiring => isFiring;
    public float CurrentAccuracy => currentAccuracy;
    public float HeatLevel;
    public int Ammo = -1; // -1 = бесконечно

    public enum TargetPriority
    {
        Nearest,           // ближайший
        Farthest,          // самый дальний
        First,             // первый в списке
        Last,              // последний в списке
        Random,            // случайный
        Strongest,         // самый сильный
        Weakest,           // самый слабый
        ClosestToEnd,      // ближайший к выходу (для TD)
        MostExpensive,     // самый ценный
        MostDangerous      // самый опасный
    }

    public enum FireMode
    {
        Single,            // одиночный
        Burst,             // очередь
        Continuous,        // непрерывный
        Charged,           // с зарядкой
        Beam               // лазер (мгновенное попадание)
    }

    private void Awake()
    {
        initialRotation = transform.rotation;
        currentAccuracy = accuracy;
        currentRotation = transform.eulerAngles.z;

        if (startActive)
            StartSearching();
    }

    private void StartSearching()
    {
        enabled = true;
        nextDetectionTime = Time.time + UnityEngine.Random.Range(0f, detectionInterval); // разносим по времени
    }

    private void Update()
    {
        if (!canFireWhileReloading && IsReloading())
            return;

        HandleTargeting();
        HandleRotation();
        HandleFiring();
        UpdateAccuracy();
        UpdateTargetMemory();
    }

    private void HandleTargeting()
    {
        // Поиск новых целей с заданным интервалом
        if (Time.time >= nextDetectionTime)
        {
            FindBestTarget();
            nextDetectionTime = Time.time + detectionInterval;
        }

        // Проверка текущей цели
        if (currentTarget != null)
        {
            if (!IsValidTarget(currentTarget))
            {
                targetLostTimer += Time.deltaTime;

                if (targetLostTimer >= targetLostDelay)
                {
                    currentTarget = null;
                    targetLostTimer = 0f;

                    if (fireMode == FireMode.Continuous)
                        isFiring = false;
                }
            }
            else
            {
                targetLostTimer = 0f;
            }
        }
        else if (searchNewTargetWhenLost)
        {
            FindBestTarget();
        }
    }

    private void FindBestTarget()
    {
        var allTargets = GetAllTargets();
        if (allTargets.Length == 0)
        {
            currentTarget = null;
            return;
        }

        // Фильтруем по дистанции и линии видимости
        var validTargets = allTargets
            .Where(t => IsInRange(t.transform) && IsInFiringArc(t.transform) && (!requireLineOfSight || HasLineOfSight(t.transform)))
            .Select(t => t.transform)
            .ToList();

        if (validTargets.Count == 0)
        {
            // Проверяем память целей
            CheckTargetMemory();
            return;
        }

        // Выбираем цель по приоритету
        currentTarget = SelectTargetByPriority(validTargets);
    }

    private Transform SelectTargetByPriority(List<Transform> targets)
    {
        if (targets == null || targets.Count == 0)
            return null;

        // Фильтруем цели через TestTarget
        var validTargets = targets.Where(t => TestTarget(t)).ToList();

        if (validTargets.Count == 0)
            return null;

        switch (targetPriority)
        {
            case TargetPriority.Nearest:
                return validTargets.OrderBy(t => Vector2.Distance(transform.position, t.position)).First();

            case TargetPriority.Farthest:
                return validTargets.OrderByDescending(t => Vector2.Distance(transform.position, t.position)).First();

            case TargetPriority.Random:
                return validTargets[Random.Range(0, validTargets.Count)];

            case TargetPriority.First:
                return validTargets.First();

            case TargetPriority.Last:
                return validTargets.Last();

            case TargetPriority.Strongest:
                return validTargets.OrderByDescending(t => GetTargetHealth(t)).First();

            case TargetPriority.Weakest:
                return validTargets.OrderBy(t => GetTargetHealth(t)).First();

            case TargetPriority.ClosestToEnd:
                return validTargets.OrderByDescending(t => GetDistanceToEnd(t)).FirstOrDefault() ?? validTargets.First();

            case TargetPriority.MostExpensive:
                return validTargets.OrderByDescending(t => GetTargetValue(t)).First();

            case TargetPriority.MostDangerous:
                return validTargets.OrderByDescending(t => GetTargetDamage(t)).First();

            default:
                return validTargets.First();
        }

        // Локальная функция для проверки цели
        bool TestTarget(Transform potentialTarget)
        {
            if (potentialTarget == null) return false;

            Vector2 lineTo = potentialTarget.position - transform.position;

            if (hasRotationLimits)
            {
                float angle = Vector2.Angle(lineTo, transform.up);
                if (angle > maxRotationAngle)
                    return false;
            }

            return true;
        }
    }

    private float GetTargetDamage(Transform t)
    {
        throw new NotImplementedException();
    }

    private float GetTargetValue(Transform t)
    {
        throw new NotImplementedException();
    }

    private void HandleRotation()
    {
        if (currentTarget == null || !rotateContinuously)
            return;

        // Вычисляем угол до цели
        Vector2 direction = currentTarget.position - transform.position;
        targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // Применяем ограничения поворота
        if (hasRotationLimits)
        {
            float relativeAngle = Mathf.DeltaAngle(targetAngle, initialRotation.eulerAngles.z);

            if (relativeAngle < minRotationAngle || relativeAngle > maxRotationAngle)
            {
                if (!clampRotationInstantly)
                {
                    // Плавно поворачиваем к границе
                    targetAngle = Mathf.Clamp(relativeAngle, minRotationAngle, maxRotationAngle) + initialRotation.eulerAngles.z;
                }
                else if (!hasFiringArc) // Если есть сектор обстрела, может стрелять и за пределами поворота
                {
                    return;
                }
            }
        }

        // Поворачиваем
        if (instantRotation)
        {
            transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        }
        else
        {
            float step = rotationSpeed * Time.deltaTime;
            currentRotation = Mathf.MoveTowardsAngle(currentRotation, targetAngle, step);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, currentRotation), rotationSmoothing * Time.deltaTime);
        }
    }

    private void HandleFiring()
    {
        if (currentTarget == null)
        {
            if (continuousFire)
                isFiring = false;
            return;
        }

        if (!CanFire())
            return;

        switch (fireMode)
        {
            case FireMode.Single:
                TryFireSingle();
                break;

            case FireMode.Burst:
                TryFireBurst();
                break;

            case FireMode.Continuous:
                TryFireContinuous();
                break;

            case FireMode.Charged:
                TryFireCharged();
                break;

            case FireMode.Beam:
                FireBeam();
                break;
        }
    }

    private void TryFireSingle()
    {
        if (Time.time >= nextFireTime && IsAimed())
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void TryFireBurst()
    {
        if (Time.time >= nextFireTime && burstCoroutine == null && IsAimed())
        {
            burstCoroutine = StartCoroutine(FireBurstRoutine());
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private IEnumerator FireBurstRoutine()
    {
        for (int i = 0; i < burstCount; i++)
        {
            Fire();
            yield return new WaitForSeconds(burstDelay);
        }
        burstCoroutine = null;
    }

    private void TryFireContinuous()
    {
        if (IsAimed())
        {
            if (!isFiring)
            {
                isFiring = true;
                isWindingUp = true;
            }

            if (isWindingUp)
            {
                currentWindUp += Time.deltaTime;
                if (currentWindUp >= windUpTime)
                {
                    isWindingUp = false;
                    currentWindUp = windUpTime;
                }
            }

            if (!isWindingUp && Time.time >= nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        else if (continuousFire)
        {
            if (isFiring && windDownTime > 0)
            {
                currentWindUp -= Time.deltaTime;
                if (currentWindUp <= 0)
                {
                    isFiring = false;
                    currentWindUp = 0;
                }
            }
        }
    }

    private void TryFireCharged()
    {
        if (IsAimed() && chargeCoroutine == null && Time.time >= nextFireTime)
        {
            chargeCoroutine = StartCoroutine(FireChargedRoutine());
        }
    }

    private IEnumerator FireChargedRoutine()
    {
        // Визуальный эффект зарядки
        float chargeProgress = 0;
        while (chargeProgress < 1f && currentTarget != null && IsAimed())
        {
            chargeProgress += Time.deltaTime / chargeTime;
            // Здесь можно обновлять визуал зарядки
            yield return null;
        }

        if (currentTarget != null && IsAimed())
        {
            Fire();
            nextFireTime = Time.time + 1f / fireRate;
        }

        chargeCoroutine = null;
    }

    private void FireBeam()
    {
        if (IsAimed() && Time.time >= nextFireTime)
        {
            // Лазер стреляет мгновенно и постоянно
            gun.Shoot(currentTarget);
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Fire()
    {
        if (gun == null || Ammo == 0 || currentTarget == null) return;

        // Применяем точность
        if (accuracy < 1f)
        {
            float inaccuracy = (1f - currentAccuracy) * 10f;
            Vector3 spread = UnityEngine.Random.insideUnitSphere * inaccuracy;
            // Можно модифицировать направление выстрела
        }

        // Применяем упреждение
        Vector3 targetPosition = currentTarget.position;
        if (useLeading)
        {
            var rb = currentTarget.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float distance = Vector2.Distance(transform.position, targetPosition);
                float bulletSpeed = gun.GetBulletSpeed();
                float timeToTarget = distance / bulletSpeed;
                targetPosition += (Vector3)rb.linearVelocity * timeToTarget * leadingFactor;
            }
        }

        // Стреляем
        gun.Shoot(currentTarget);

        // Ухудшаем точность при стрельбе
        currentAccuracy = Mathf.Max(0.1f, currentAccuracy - accuracyDegradation);

        // Увеличиваем нагрев
        HeatLevel = Mathf.Clamp01(HeatLevel + 0.1f);

        // Расходуем боеприпасы
        if (Ammo > 0)
        {
            Ammo--;
            //if (Ammo == 0)
             //   enabled = false;
        }
    }

    private bool IsValidTarget(Transform target)
    {
        if (target == null || !target.gameObject.activeInHierarchy)
            return false;

        if (ignoreUntargetable)
        {
            var targetable = target.GetComponent<ITargetable>();
            if (targetable != null && !targetable.IsTargetable)
                return false;
        }

        return IsInRange(target) && IsInFiringArc(target);
    }

    private bool IsInRange(Transform target)
    {
        float distance = Vector2.Distance(transform.position, target.position);
        return distance >= minDistanceToAttack && distance <= maxDistanceToAttack;
    }

    private bool IsInFiringArc(Transform target)
    {
        if (!hasFiringArc || currentTarget == null)
            return true;

        Vector2 directionToTarget = target.position - transform.position;
        float angleToTarget = Vector2.Angle(transform.up, directionToTarget);
        return angleToTarget <= firingArcAngle / 2f;
    }

    private bool HasLineOfSight(Transform target)
    {
        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, obstacleMask);
        return hit.collider == null || hit.transform == target;
    }

    private bool IsAimed()
    {
        if (currentTarget == null)
            return false;

        if (!hasFiringArc)
            return true;

        Vector2 directionToTarget = currentTarget.position - transform.position;
        float angleToTarget = Vector2.Angle(transform.up, directionToTarget);
        return angleToTarget <= firingArcAngle / 2f;
    }

    private bool CanFire()
    {
        if (requiresPower && !HasPower())
            return false;

        if (Ammo == 0)
            return false;

        return true;
    }

    public float reloadTime = 2f;
    private float lastReloadTime;
    private int maxAmmo = 30;
    private bool isReloadingProcess = false;

    private bool IsReloading()
    {
        if (Ammo == 0)
        {
            if (isReloadingProcess == false)
            {
                lastReloadTime = Time.time;
                isReloadingProcess = true;
            }

            // Запускаем перезарядку
            if (Time.time - lastReloadTime > reloadTime)
            {
                Ammo = maxAmmo;
                lastReloadTime = Time.time;
            }
            return true;
        }
        isReloadingProcess = false;
        return false;
    }

    private float currentPower = 100f;

    private bool HasPower()
    {
        if (currentPower >= powerConsumption * Time.deltaTime)
        {
            currentPower -= powerConsumption * Time.deltaTime;
            return true;
        }
        return false;
    }

    private float GetDistanceToEnd(Transform target)
    {
        // Получаем расстояние цели до выхода

        //var pathFollower = target.GetComponent<PathFollower>();
        //return pathFollower != null ? pathFollower.DistanceToEnd : 0f;

        return Vector2.Distance(target.position, GameG.Planet.transform.position);
    }

    private float GetTargetHealth(Transform target)
    {
        var health = target.GetComponent<IHealth>();
        return health != null ? health.CurrentHealth : 0f;
    }

    private void UpdateAccuracy()
    {
        if (currentAccuracy < accuracy)
        {
            currentAccuracy = Mathf.Min(accuracy, currentAccuracy + accuracyRecoveryRate * Time.deltaTime);
        }
    }

    private void UpdateTargetMemory()
    {
        // Удаляем старые записи из памяти
        var toRemove = targetMemory.Where(kvp => Time.time - kvp.Value > targetMemoryTime).Select(kvp => kvp.Key).ToList();
        foreach (var key in toRemove)
            targetMemory.Remove(key);
    }

    private void CheckTargetMemory()
    {
        // Проверяем, не появилась ли снова цель из памяти
        foreach (var remembered in targetMemory.Keys)
        {
            if (remembered != null && IsValidTarget(remembered))
            {
                currentTarget = remembered;
                break;
            }
        }
    }

    public GameObject[] GetAllTargets()
    {
        HashSet<GameObject> targets = new HashSet<GameObject>();

        foreach (var preset in myTagsTarget)
        {
            var found = MyTagsObserver.FindGameObjectsWithAnyTags(preset.Tags);
            foreach (var go in found)
            {
                if (IsValidTarget(go.transform))
                    targets.Add(go);
            }
        }

        return targets.ToArray();
    }

    public void SetTarget(Transform target)
    {
        if (canTargetSameEnemy || target != currentTarget)
        {
            currentTarget = target;
        }
    }

    public void RememberTarget(Transform target)
    {
        if (target != null)
            targetMemory[target] = Time.time;
    }

    public void Reload(int ammoAmount)
    {
        if(Ammo != -1)
            Ammo = ammoAmount;
        //enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        // Отрисовка дальности
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxDistanceToAttack);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minDistanceToAttack);

        // Отрисовка сектора обстрела
        if (hasFiringArc)
        {
            Gizmos.color = Color.cyan;
            Vector3 forward = transform.up * maxDistanceToAttack;
            Quaternion leftRot = Quaternion.Euler(0, 0, firingArcAngle / 2);
            Quaternion rightRot = Quaternion.Euler(0, 0, -firingArcAngle / 2);

            Vector3 leftDir = leftRot * forward;
            Vector3 rightDir = rightRot * forward;

            Gizmos.DrawLine(transform.position, transform.position + leftDir);
            Gizmos.DrawLine(transform.position, transform.position + rightDir);
        }

        // Отрисовка ограничений поворота
        if (hasRotationLimits)
        {
            Gizmos.color = Color.magenta;
            Quaternion minRot = Quaternion.Euler(0, 0, initialRotation.eulerAngles.z + minRotationAngle);
            Quaternion maxRot = Quaternion.Euler(0, 0, initialRotation.eulerAngles.z + maxRotationAngle);

            Vector3 minDir = minRot * Vector3.up * maxDistanceToAttack;
            Vector3 maxDir = maxRot * Vector3.up * maxDistanceToAttack;

            Gizmos.DrawLine(transform.position, transform.position + minDir);
            Gizmos.DrawLine(transform.position, transform.position + maxDir);
        }
    }
}

// Интерфейсы для взаимодействия
public interface ITargetable
{
    bool IsTargetable { get; }
}

public interface IHealth
{
    float CurrentHealth { get; }
    float MaxHealth { get; }

    void TakeDamage(int damage);
}

[Serializable]
public class TargetsPreset
{
    public string PresetName;
    public string[] Tags;
}
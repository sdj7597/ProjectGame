using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public LayerMask groundLayer;

    [Header("AI 설정")]
    public bool isAI = true;
    public float followDistance = 3f;
    public float sightRange = 10f;

    private Rigidbody rb;
    private Animator animator;
    private PlayerStats stats;

    private Vector3 targetPosition;
    private bool hasTarget = false;
    private GameObject currentEnemy;
    private float lastAttackTime;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (stats.isDie) return;

        GameObject masterPlayer = PlayerManager.instance.GetMasterPlayer();

        if (isAI)
        {
            // AI: 적 찾기
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float nearestDist = Mathf.Infinity;
            currentEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                if (enemy.GetComponent<Enemy>().isDie) continue;
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < nearestDist && dist <= sightRange)
                {
                    nearestDist = dist;
                    currentEnemy = enemy;
                }
            }

            // AI: 행동
            if (currentEnemy != null)
            {
                float distToEnemy = Vector3.Distance(transform.position, currentEnemy.transform.position);
                if (distToEnemy <= stats.attackRange)
                {
                    hasTarget = false;
                    rb.velocity = Vector3.zero;

                    Attack(currentEnemy);
                }
                else
                {
                    targetPosition = currentEnemy.transform.position;
                    hasTarget = true;
                }
            }
            else if (masterPlayer != null)
            {
                // 적이 없으면 메인플레이어 따라다니기
                float distToMaster = Vector3.Distance(transform.position, masterPlayer.transform.position);
                if (distToMaster > followDistance)
                {
                    targetPosition = masterPlayer.transform.position;
                    hasTarget = true;
                }
                else
                {
                    hasTarget = false;
                    rb.velocity = Vector3.zero;
                }
            }
        }
        else
        {
            // 플레이어 입력
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
                {
                    targetPosition = hit.point;
                    hasTarget = true;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Enemy"))
                {
                    float distToEnemy = Vector3.Distance(transform.position, hit.transform.position);
                    if (distToEnemy <= stats.attackRange)
                    {
                        hasTarget = false;
                        rb.velocity = Vector3.zero;

                        Attack(hit.collider.gameObject);
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (hasTarget)
        {
            Vector3 direction = (targetPosition - rb.position);
            direction.y = 0;
            direction.Normalize();

            if (Vector3.Distance(new Vector3(rb.position.x, 0, rb.position.z),
                new Vector3(targetPosition.x, 0, targetPosition.z)) < 0.5f)
            {
                hasTarget = false;
                animator.Play("Idle");
            }
            else
            {
                rb.velocity = new Vector3(direction.x * moveSpeed, rb.velocity.y, direction.z * moveSpeed);
                transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
                animator.Play("Move");
            }
        }
    }

    void Attack(GameObject target)
    {
        if (Time.time >= lastAttackTime + stats.attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");

            Enemy targetStats = target.GetComponent<Enemy>();
            int damage = stats.attackPower;

            // 크리티컬
            if (Random.Range(0f, 100f) <= stats.criticalChance)
                damage = Mathf.RoundToInt(damage * 1.5f);

            targetStats.TakeDamage(damage);

            #region 공격시 경험치 획득
            float value;
            if (damage > targetStats.hp)
                value = targetStats.haveExp * damage / targetStats.hpMax;
            else
                value = targetStats.haveExp * damage / targetStats.hpMax;

            stats.GetExp(value);
            #endregion

            if (targetStats.isDie)
                currentEnemy = null;
        }
    }
}

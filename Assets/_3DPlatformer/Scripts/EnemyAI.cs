using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Attack }
    public EnemyState state;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    public float patrolSpeed = 1.5f;
    public float waitAtPoint = 1.5f;
    public float pointReachedDistance = 0.3f;

    [Header("Chase")]
    public float chaseRange = 6f;
    public float chaseSpeed = 3.5f;
    public float leashRange = 12f; // Won't follow the player farther than this from its start position

    [Header("Attack")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public float attackHitDelay = 0.4f; // Time into the animation when the hit lands
    public int attackDamage = 1;

    [Header("Ledge Check")]
    public LayerMask groundLayer;
    public float ledgeCheckDistance = 0.6f;

    public float turnSpeed = 10f;

    Rigidbody rb;
    Animator anim;
    EnemyHealthScript health;
    Transform player;
    PlayerHealth playerHealth;

    Vector3 homePosition;
    Vector3 moveDirection;
    float currentSpeed;
    int patrolIndex;
    float waitTimer;
    float attackTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        health = GetComponent<EnemyHealthScript>();
        homePosition = transform.position;
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            playerHealth = p.GetComponent<PlayerHealth>();
        }
    }

    private void Update()
    {
        if (health.IsDead || health.IsHurt || player == null)
        {
            Stop();
            anim.SetFloat("Speed", 0);
            return;
        }

        attackTimer -= Time.deltaTime;
        float distToPlayer = FlatDistance(transform.position, player.position);
        bool playerInLeash = FlatDistance(homePosition, player.position) < leashRange;

        switch (state)
        {
            case EnemyState.Patrol:
                Patrol();
                if (distToPlayer < chaseRange && playerInLeash) state = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (distToPlayer <= attackRange) state = EnemyState.Attack;
                else if (distToPlayer > chaseRange * 1.5f || !playerInLeash) state = EnemyState.Patrol;
                else MoveTowards(player.position, chaseSpeed);
                break;

            case EnemyState.Attack:
                Stop();
                FaceTarget(player.position);
                if (distToPlayer > attackRange + 0.3f) state = EnemyState.Chase;
                else if (attackTimer <= 0)
                {
                    attackTimer = attackCooldown;
                    anim.SetTrigger("Attack");
                    Invoke(nameof(DealDamage), attackHitDelay);
                }
                break;
        }

        // 0 = Idle, 0.5 = Walk, 1 = Run
        float animSpeed = currentSpeed == 0 ? 0 : (currentSpeed >= chaseSpeed ? 1 : 0.5f);
        anim.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (health.IsDead || health.IsHurt) return; // Let knockback play out
        Vector3 v = moveDirection * currentSpeed;
        rb.linearVelocity = new Vector3(v.x, rb.linearVelocity.y, v.z);
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) { Stop(); return; }

        if (waitTimer > 0)
        {
            waitTimer -= Time.deltaTime;
            Stop();
            return;
        }

        Vector3 target = patrolPoints[patrolIndex].position;
        if (FlatDistance(transform.position, target) < pointReachedDistance)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            waitTimer = waitAtPoint;
            Stop();
            return;
        }

        MoveTowards(target, patrolSpeed);
    }

    void MoveTowards(Vector3 target, float speed)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;
        dir.Normalize();
        FaceTarget(target);

        if (!GroundAhead(dir)) { Stop(); return; } // Don't walk off ledges

        moveDirection = dir;
        currentSpeed = speed;
    }

    void Stop()
    {
        moveDirection = Vector3.zero;
        currentSpeed = 0;
    }

    void FaceTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
    }

    bool GroundAhead(Vector3 dir)
    {
        if (groundLayer.value == 0) return true; // No layer set, skip the check
        Vector3 origin = transform.position + dir * ledgeCheckDistance + Vector3.up * 0.5f;
        return Physics.Raycast(origin, Vector3.down, 1.5f, groundLayer);
    }

    void DealDamage()
    {
        if (health.IsDead || player == null || playerHealth == null) return;
        // Only hits if the player is still close when the attack lands
        if (FlatDistance(transform.position, player.position) <= attackRange + 0.5f &&
            Mathf.Abs(player.position.y - transform.position.y) < 1.5f)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0; b.y = 0;
        return Vector3.Distance(a, b);
    }

    // Shows the ranges in the Scene view when the enemy is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
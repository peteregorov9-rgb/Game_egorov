using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Настройки атаки")]
    [SerializeField] private float attackCooldown = 0.5f;   // пауза между атаками

    // Публичное свойство — куда смотрит персонаж
    public bool FacingRight { get; private set; } = true;

    // Публичное свойство — идёт ли атака (для других скриптов, если нужно)
    public bool IsAttacking => isAttacking;

    private float nextAttackTime = 0f;
    private bool isAttacking = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // === АТАКА ===
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= nextAttackTime && !isAttacking)
            {
                Attack();
            }
        }

        // === ДВИЖЕНИЕ ===
        // Если атакуем — не двигаемся и не меняем анимацию ходьбы
        if (isAttacking)
        {
            animator.SetFloat("horizontal", 0);
            animator.SetFloat("Vertical", 0);
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(moveX, moveY, 0).normalized;
        transform.position += move * moveSpeed * Time.deltaTime;

        // Запоминаем направление (только если реально двигаемся по X)
        if (moveX > 0) FacingRight = true;
        else if (moveX < 0) FacingRight = false;

        // Передаём параметры в Animator
        animator.SetFloat("horizontal", moveX);
        animator.SetFloat("Vertical", moveY);
    }

    void Attack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        // 1. Сначала направление
        animator.SetBool("FacingRight", FacingRight);

        // 2. Потом триггер
        animator.SetTrigger("Attack");
    }

    // Вызывается из Animation Event в конце анимации атаки
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }
}
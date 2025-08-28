using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float launchAngle = 45f; // �߻� ����
    private Rigidbody2D rigid;

    public Transform Target;

    private Vector3 endPos;

    private float destroyTime = 2f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        endPos=Target.position;
        LaunchAtTarget(endPos);
    }

    /// <summary>
    /// Ȱ���� ȣ���ϴ� �Լ�: ��ǥ ���� ����
    /// </summary>
    public void LaunchAtTarget(Vector2 targetPos, float gravity = -9.81f)
    {
        Vector2 startPos = rigid.position;
        Vector2 dir = targetPos - startPos;
        float yOffset = dir.y;
        dir.y = 0;

        float distance = dir.magnitude;
        float angle = launchAngle * Mathf.Deg2Rad;

        // �ʱ� �ӵ� ���
        float velocity = Mathf.Sqrt(distance * -gravity / Mathf.Sin(2 * angle));
        Vector2 velocityVec = dir.normalized * (velocity * Mathf.Cos(angle));
        velocityVec.y = velocity * Mathf.Sin(angle);

        rigid.gravityScale = 1f; // �߷� Ȱ��ȭ
        rigid.linearVelocity = velocityVec;

        // ȭ�� ���� ���߱�
        float angleDeg = Mathf.Atan2(rigid.linearVelocity.y, rigid.linearVelocity.x) * Mathf.Rad2Deg;
        rigid.rotation = angleDeg;
    }

    void Update()
    {
        UpdateDirection();
        AutoDestroy();
    }

    public virtual void UpdateDirection()
    {
        // ���ư��� ���� ���� ��� ���߱�
        if (rigid.linearVelocity.sqrMagnitude > 0.1f)
        {
            float angleDeg = Mathf.Atan2(rigid.linearVelocity.y, rigid.linearVelocity.x) * Mathf.Rad2Deg;
            rigid.rotation = angleDeg;
        }
    }

    public virtual void AutoDestroy()
    {
        if (rigid.gravityScale == 0)
        {
            destroyTime -= Time.deltaTime;
            if (destroyTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            rigid.linearVelocity= Vector2.zero;
            rigid.gravityScale = 0;


            this.transform.GetComponent<DamageOnTouch>().enabled = false;
        }
    }
}

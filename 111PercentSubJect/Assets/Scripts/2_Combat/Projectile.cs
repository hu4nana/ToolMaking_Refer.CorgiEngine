using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    public float launchAngle = 45f; // 발사 각도
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
    /// 활에서 호출하는 함수: 목표 지점 지정
    /// </summary>
    public void LaunchAtTarget(Vector2 targetPos, float gravity = -9.81f)
    {
        Vector2 startPos = rigid.position;
        Vector2 dir = targetPos - startPos;
        float yOffset = dir.y;
        dir.y = 0;

        float distance = dir.magnitude;
        float angle = launchAngle * Mathf.Deg2Rad;

        // 초기 속도 계산
        float velocity = Mathf.Sqrt(distance * -gravity / Mathf.Sin(2 * angle));
        Vector2 velocityVec = dir.normalized * (velocity * Mathf.Cos(angle));
        velocityVec.y = velocity * Mathf.Sin(angle);

        rigid.gravityScale = 1f; // 중력 활성화
        rigid.linearVelocity = velocityVec;

        // 화살 방향 맞추기
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
        // 날아가는 동안 방향 계속 맞추기
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

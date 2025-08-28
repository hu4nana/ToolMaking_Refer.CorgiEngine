using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DamageOnTouch : MonoBehaviour
{
    public float Damage = 10f;
    public LayerMask TargetLayers;
    public bool DestroyOnHit = true;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!enabled) return;

        if (((1 << collision.gameObject.layer) & TargetLayers.value) == 0) return;


        // 데미지 인터페이스 호출
        var dmg = collision.GetComponentInParent<IDamageable>();
        if (dmg != null)
        {
            Vector2 p = collision.ClosestPoint(transform.position);
            Vector2 n = (collision.transform.position - transform.position).normalized;
            dmg.ApplyDamage(Damage);
            if (DestroyOnHit) Destroy(gameObject);
        }
    }
}

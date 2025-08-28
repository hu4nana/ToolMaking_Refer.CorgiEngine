using UnityEngine;
using UnityEngine.TextCore.Text;

public class InstallationObject : MonoBehaviour
{
    public float lifeTime = 5f;

    public GameObject Projectile = null;

    private void Update()
    {
        lifeTime-= Time.deltaTime;

        if(lifeTime<0)
        {
            Destroy(gameObject);
        }
    }

    public void InstantiateProjectile()
    {
        for(int i = 0; i < 5; i++)
        {
            GameObject _normalProj= Instantiate(Projectile, this.transform.position, Quaternion.identity);
            _normalProj.GetComponent<Projectile>().LaunchAtTarget(Vector2.right * (this.transform.position.x - 2 + i));
        }
        
    }
}

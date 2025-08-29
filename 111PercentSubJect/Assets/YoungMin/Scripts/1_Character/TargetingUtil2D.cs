using System.Linq;
using UnityEngine;

public static class TargetingUtil2D
{
    public static Transform AcquireFrontmost(Transform origin, bool facingRight, float radius, LayerMask enemyLayers)
    {
        var hits = Physics2D.OverlapCircleAll(origin.position, radius, enemyLayers);
        if (hits == null || hits.Length == 0) return null;

        var list = hits.Select(h => h.transform)
                       .Where(t => t && t.gameObject.activeInHierarchy);

        if (facingRight)
            list = list.Where(t => t.position.x >= origin.position.x)
                       .OrderByDescending(t => t.position.x);
        else
            list = list.Where(t => t.position.x <= origin.position.x)
                       .OrderBy(t => t.position.x);

        return list.FirstOrDefault();
    }
}
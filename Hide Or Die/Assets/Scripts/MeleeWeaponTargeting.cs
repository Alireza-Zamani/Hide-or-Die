using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponTargeting : MonoBehaviour
{

    [SerializeField] private float targetingRadius = 10;
    [SerializeField] private LayerMask target;

    public Transform GetMeleeTarget()
    {
        return ClosestTargetCalculator(FindAvailableTargets(this.transform.position, targetingRadius, target));
    }
    
    
    private List<Transform> FindAvailableTargets(Vector2 origin, float radius, LayerMask target)
    {
        Collider2D[] targetsHit = Physics2D.OverlapCircleAll(origin, radius, target);

        List<Transform> availableTargets = new List<Transform>();
        foreach (var item in targetsHit)
        {
            availableTargets.Add(item.transform);
        }

        return availableTargets;
    }

    private Transform ClosestTargetCalculator(List<Transform> targets)
    {
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (var i in targets)
        {
            float newDistance = Mathf.Abs(Vector2.Distance(transform.position, i.position));

            if (newDistance < closestDistance)
            {
                closestDistance = newDistance;
                closestTarget = i;
            }
        }
        if (closestTarget != null)
        {
            return closestTarget.transform;
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, targetingRadius);
    }
}

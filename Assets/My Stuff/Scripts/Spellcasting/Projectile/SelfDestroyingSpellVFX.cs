using UnityEngine;
using static UnityEngine.ParticleSystem;

public class SelfDestroyingSpellVFX : MonoBehaviour
{
    private void Awake()
    {
        float longestTime = 0;
        foreach (var ps in GetComponentsInChildren<ParticleSystem>())
        {
            switch (ps.main.startLifetime.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    if(longestTime < ps.main.startLifetime.constant)
                        longestTime = ps.main.startLifetime.constant;
                    break;
                case ParticleSystemCurveMode.Curve:
                    if (longestTime < (ps.main.startLifetime.curveMax.length > 0 ? ps.main.startLifetime.curveMultiplier : 0f))
                        longestTime = ps.main.startLifetime.curveMax.length > 0 ? ps.main.startLifetime.curveMultiplier : 0f;
                    break;
                case ParticleSystemCurveMode.TwoCurves:
                    if (longestTime < ps.main.startLifetime.curveMultiplier)
                        longestTime = ps.main.startLifetime.curveMultiplier;
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    if (longestTime < ps.main.startLifetime.constantMax)
                        longestTime = ps.main.startLifetime.constantMax;
                    break;
            }

        }

        Destroy(gameObject, longestTime);
    }
}

using Alchemy.Inspector;
using UnityEngine;

public class ColliisionIgnorer : MonoBehaviour
{
    [Title("Collider Choose Method")]
    [SerializeField] bool UseChildColliders = false;

    [HideIf("UseChildColliders")]    
    [SerializeField] Collider[] ColliderList;
    
 
    private void Awake()
    {
        if(UseChildColliders)
            ColliderList = GetComponentsInChildren<Collider>();


        for (int i = 0; i < ColliderList.Length - 1; i++)
        {
            for(int j = i; j < ColliderList.Length; j++)
            {
                Physics.IgnoreCollision(ColliderList[i], ColliderList[j], true);
            }
        }
    }
}

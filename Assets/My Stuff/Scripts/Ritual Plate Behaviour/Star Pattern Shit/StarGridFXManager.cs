using Alchemy.Inspector;
using UnityEngine;

public class StarGridFXManager : MonoBehaviour
{
    [Title("Sound Clips")]
    [SerializeField] AudioClip StarSelectClip;
    [Title("Volumes")]
    [SerializeField][Range(0, 1)] float StarSelectVolume = 0.05f;



    public void PlayStarSelectClip()
    {
        if (StarSelectClip != null)
        {
            SoundManagerSO.PlayClipAtPoint(StarSelectClip, transform.position, StarSelectVolume);
        }
    }
}

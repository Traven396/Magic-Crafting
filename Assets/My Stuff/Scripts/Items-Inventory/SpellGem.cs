using AgeOfEnlightenment.Stats;
using UnityEngine;

public class SpellGem : MonoBehaviour
{
    [SerializeField] float _Quality;
    [SerializeField] GemCutPlate.DebugCutTypes _CutType;
    [SerializeField] MagicElement _Element;

    public GemCutPlate.DebugCutTypes CutType { get => _CutType; set => _CutType = value; }
    public float Quality { get => _Quality; set => _Quality = value; }
    public MagicElement Element => _Element;

    MeshFilter _Filter;
    public MeshFilter Filter => _Filter;
    Renderer _Renderer;
    public Renderer Renderer => _Renderer;
    private void Start()
    {
        _Filter = GetComponent<MeshFilter>();
        _Renderer = GetComponent<Renderer>();
    }
}

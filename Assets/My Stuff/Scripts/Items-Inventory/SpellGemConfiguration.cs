using AgeOfEnlightenment.Spellcasting;
using UnityEngine;

[CreateAssetMenu(menuName = "Misc/Spell Gem Config", fileName = "Gem - Cut")]
public class SpellGemConfiguration : ScriptableObject
{
    [SerializeField] MagicElement _Element;
    [SerializeField] GemCutPlate.DebugCutTypes _Cut;

    [SerializeField] SpellDefinitionSO _Spell;

    public MagicElement Element { get => _Element; }
    public GemCutPlate.DebugCutTypes Cut { get => _Cut; }
    public SpellDefinitionSO Spell { get => _Spell; }
}
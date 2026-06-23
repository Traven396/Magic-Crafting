using Alchemy.Inspector;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class IngredientInstance : MonoBehaviour
{
    //This class will be on every item that COULD be crafted into something
    //This will also contain methods for burning, cutting, etc that allow it to be "processed" further.
    // ^Though I might want to abstract those out into different things in case I make an item that can be processed yet isnt an ingredient
    [InlineEditor] public IngredientItemSO Item;
    XRGrabInteractable interactable;
    Rigidbody rb;

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    public Rigidbody AttachedRB { get => rb; }

    //public static explicit operator IngredientItemSO(IngredientInstance instance) => instance.Item;
    public static implicit operator IngredientItemSO(IngredientInstance instance) => instance.Item;
    public List<AspectTag> Tags => Item.GetTags();
    public XRGrabInteractable GetInteractable() {  return interactable; }
}

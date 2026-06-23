using Alchemy.Inspector;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSection : BookSection
{
    

    [Title("Actual Page Objects")]
    public GameObject LeftTOC;
    public GameObject RightTOC;
    [Space(10)]
    public GameObject LeftIngredientPage;
    public GameObject RightIngredientPage;
    [Title("Table of Contents")]
    public Button[] TableOfContentsButtons;
    [Title("Specific Content")]
    public Image LeftIngredientImage;
    public Image RightIngredientImage;
    public TMP_Text LeftTitle, RightTitle;
    public TMP_Text LeftDescription, RightDescription;
    public Transform LeftAspectGroup, RightAspectGroup;
    public GameObject AspectCardPrefab;
    //Maybe split the load page method into a loadToc and a regular content method.
    //Since they will only ever be called off an if statement.










    private List<IngredientItemSO> DiscoveredIngredients;
    private List<AspectCardUI> leftAspectCards = new(), rightAspectCards = new();

    //Get the discovered ingredients
    //Reload the pages
    public void DebugMethod()
    {
        Debug.Log("The button called this yay");
    }
    private void Awake()
    {
        DiscoveredIngredients = DiscoveriesSingleton.Instance.GetAllDiscoveredIngredients();


        RecountPages();
    }

    //An event from the DiscoveriesSingleton will cause this to trigger when a new thing is discovered
    public void RecountPages()
    {
        //This will be called when a new ingredient is discovered, and it will reload the pages to show the new ingredient

        tocPageCount = Mathf.Max(Mathf.CeilToInt((float)DiscoveredIngredients.Count / 6f), 2);
        contentPageCount = DiscoveredIngredients.Count;

        if(contentPageCount % 2 == 1)
        {
            contentPageCount += 1;
        }

        LoadPage(CurrentPageNumber);
    }



    protected override void LoadTableOfContents(int startingIndex)
    {
        LeftIngredientPage.SetActive(false);
        RightIngredientPage.SetActive(false);

        LeftTOC.SetActive(true);
        RightTOC.SetActive(true);


        for (int i = 0; i < TableOfContentsButtons.Length; i++)
        {
            int ingredientIndex = startingIndex + i;
            if (ingredientIndex < DiscoveredIngredients.Count)
            {
                TableOfContentsButtons[i].gameObject.SetActive(true);
                TableOfContentsButtons[i].GetComponentInChildren<TMP_Text>().text = DiscoveredIngredients[ingredientIndex].name;

                TableOfContentsButtons[i].onClick.RemoveAllListeners();
                TableOfContentsButtons[i].onClick.AddListener(() => GoToPage(tocPageCount + ingredientIndex + 1));
                //Here we would also set the button to load the appropriate page for that ingredient
            }
            else
            {
                TableOfContentsButtons[i].gameObject.SetActive(false);
                //Here we would also disable the button since there is no ingredient to show
            }
        }
    }

    protected override void LoadContentPage(int pageNumber)
    {
        LeftIngredientPage.SetActive(true);
        RightIngredientPage.SetActive(true);

        LeftTOC.SetActive(false);
        RightTOC.SetActive(false);

        IngredientItemSO leftIngredient = DiscoveredIngredients[pageNumber - tocPageCount - 2];

        IngredientItemSO rightIngredient = null;
        if(pageNumber - tocPageCount - 1 < DiscoveredIngredients.Count)
            rightIngredient = DiscoveredIngredients[pageNumber - tocPageCount - 1];


        //If we are turning to this page then we for sure have a left ingredient. But there is a chance of no right so we test first
        LeftIngredientImage.sprite = leftIngredient.GetIcon();
        LeftTitle.text = leftIngredient.name;

        LeftDescription.text = leftIngredient.GetDescription();

        leftAspectCards.ForEach(card => Destroy(card.gameObject));
        leftAspectCards = new();

        foreach (AspectTag tag in leftIngredient.GetTags())
        {
            GameObject newCard = Instantiate(AspectCardPrefab, LeftAspectGroup);
            var cardBrain = newCard.GetComponent<AspectCardUI>();

            cardBrain.SetAspect(tag);

            leftAspectCards.Add(cardBrain);
        }


        if (rightIngredient != null)
        {
            RightIngredientPage.gameObject.SetActive(true);

            RightIngredientImage.sprite = rightIngredient.GetIcon();
            RightTitle.text = rightIngredient.name;

            RightDescription.text = rightIngredient.GetDescription();

            rightAspectCards.ForEach(card => Destroy(card.gameObject));
            rightAspectCards = new();

            foreach (AspectTag tag in rightIngredient.GetTags())
            {
                GameObject newCard = Instantiate(AspectCardPrefab, RightAspectGroup);
                var cardBrain = newCard.GetComponent<AspectCardUI>();

                cardBrain.SetAspect(tag);

                rightAspectCards.Add(cardBrain);
            }
        }
        else
        {
            RightIngredientPage.gameObject.SetActive(false);
        }
    }
}

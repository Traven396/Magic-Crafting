using Alchemy.Inspector;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Book : MonoBehaviour
{
    private enum BookStatus
    {
        Open,
        Opening,
        Closed,
        Closing
    }
    //Each book has a list of sections
    //Will create a section tab on the right on the book
        //It will also move that tab to the left when the page goes past
    //The Book will handle changing pages and sections, but the sections will handle what is on the page

    public List<BookSection> Sections = new();


    private BookSection currentSection;
    private BookStatus currentBookStatus = BookStatus.Closed;

    public void ActivateBook()
    {
        switch (currentBookStatus)
        {
            case BookStatus.Open:
                break;
            case BookStatus.Opening:
                break;
            case BookStatus.Closed:
                break;
            case BookStatus.Closing:
                break;
        }
    }
    private void Start()
    {
        if (Sections.Count > 0) 
        {
            CreateSections();
            LoadSection(0);
        }
    }

    private void CreateSections()
    {
        //Here is where the prefabs of the section marks will be placed at the top of the book
        //Then they will be assigned the appropriate section to load when clicked
    }
    public void LoadSection(int sectionIndex)
    {
        //Load the section at the given index
        currentSection = Sections[sectionIndex];

        Sections[sectionIndex].Initialize();
    }

    [Button]
    public void NextPage()
    {
        currentSection.NextPage();
    }
    [Button]
    public void PrevPage() 
    {
        currentSection.PreviousPage();
    }
}

using System;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class BookSection : MonoBehaviour
{
    //This is the generalized class for a section in a book
    //Every section should create a table of contents entry, and then make a page for each list of entries
    //There will be some kind of next page method?
    //Maybe a go to page

    public int CurrentPageNumber { get; private set; }

    protected int tocPageCount;
    protected int contentPageCount;
    public int TotalPages => tocPageCount + contentPageCount;



    public void Initialize()
    {
        CurrentPageNumber = 2;
        LoadPage(CurrentPageNumber);
    }

    //This will return false once we are at the end of possible pages
    public bool NextPage()
    {
        CurrentPageNumber += 2;

        CurrentPageNumber = Math.Min(TotalPages, CurrentPageNumber);

        if (CurrentPageNumber <= TotalPages)
        {
            LoadPage(CurrentPageNumber);
            return false;
        }
        else
        {
            return true;
        }
    }

    //Or this will be the general method for loading the entire page and setting the specific information to the correct places
    protected void LoadPage(int pageNumber)
    {
        if (pageNumber <= tocPageCount)
        {
            LoadTableOfContents((pageNumber - 2) / 6);
        }
        else
        {
            LoadContentPage(pageNumber);
        }
    }

    protected virtual void LoadTableOfContents(int startingIndex) { }
    protected virtual void LoadContentPage(int pageNumber) { }

    public void GoToPage(int pageNumber)
    {
        Debug.Log("Going to page: " + pageNumber);
        if (pageNumber % 2 == 1)
        {
            pageNumber += 1;
        }

        CurrentPageNumber = pageNumber;

        LoadPage(pageNumber);
    }


    public bool PreviousPage() 
    {
        CurrentPageNumber -= 2;

        CurrentPageNumber = Math.Max(2, CurrentPageNumber);

        if(CurrentPageNumber < 0)
        {
            return true;
        }
        else
        {
            LoadPage(CurrentPageNumber);
            return false;
        }
    }
}

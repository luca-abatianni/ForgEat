using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    public static int currentPG;

    private void Awake()
    {
        SelectPG(0);
    }

    private void SelectPG(int _index)
    {
        previousButton.interactable =  _index != 0 ;
        nextButton.interactable =  _index != transform.childCount - 1 ;
        for (int i=0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _index);
        }
    }

    public void ChangePG(int _change)
    {
        currentPG += _change;
        SelectPG(currentPG);
    }
}

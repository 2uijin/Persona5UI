using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SkillPageNavi : MonoBehaviour
{
    [SerializeField] List<CharacterItem> items;

      int curBtn = -1;
       int btnCnt = 1;

    void Awake()
    {
        btnCnt = items.Count; 
    }

    private void OnEnable()
    {
        SelectBtn(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectBtn((curBtn + 1) % btnCnt);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectBtn((curBtn - 1 + btnCnt) % btnCnt);
        }
    }

    private void SelectBtn(int num)
    {
        if (curBtn == num) return;

        if (curBtn >= 0) items[curBtn].SetHighlight(false);
        curBtn = num;
        items[curBtn].SetHighlight(true);
        items[curBtn].transform.SetAsLastSibling();
    }
}

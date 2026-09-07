using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SettingPageNavi : MonoBehaviour
{
    [SerializeField] List<MenuItem> items;

    [SerializeField] List<Sprite> btnInfoSprites;
    [SerializeField] Image btnInfoImg;
    [SerializeField] RectTransform btnInfo;
    [SerializeField] float sizeMag;

    [SerializeField] int curBtn = -1;
    [SerializeField] int btnCnt = 1;
    [SerializeField]List<Vector2> btnInfoSpriteSizes;

    private void Awake()
    {
        btnInfoSpriteSizes = new List<Vector2>();
        foreach (var item in btnInfoSprites)
        {
            btnInfoSpriteSizes.Add(new Vector3(item.rect.width * sizeMag, item.rect.height * sizeMag));
        }

        btnCnt = items.Count;
    }

    private void OnEnable()
    {
        SelectBtn(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            SelectBtn((curBtn + 1) % btnCnt);
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            SelectBtn((curBtn - 1 + btnCnt) % btnCnt);
        else if (Input.GetKeyDown(KeyCode.Return))
            items[curBtn].onButtonEnter?.Invoke();

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

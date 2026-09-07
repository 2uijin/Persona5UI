using UnityEngine;

public class KeyManager : MonoBehaviour
{
    [SerializeField] SettingPage settingPage;
    [SerializeField] SkillPage skillPage;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (skillPage.isOpen)
            {
                skillPage.Close();
            }
            else
            {
                settingPage.Toggle();
            }
        }
    }
}

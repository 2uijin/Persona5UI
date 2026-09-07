using UnityEngine;

public class JokerCam : MonoBehaviour
{
    [Header ("setting")]
    [SerializeField] Vector3 setPos = new Vector3(-864.65f, 572.82f, -18.1f);
    [SerializeField] Vector3 setRot = new Vector3();

    [Header ("skill")]
    [SerializeField] Vector3 skillPos = new Vector3(-864.65f, 572.82f, -18.1f);
    [SerializeField] Vector3 skillRot = new Vector3();

    public void ChangePos(bool isSet)
    {
        if (isSet)
        {
            transform.position = setPos;
            transform.rotation = Quaternion.Euler(setRot);
        }
        else
        {
            transform.position = skillPos;
            transform.rotation = Quaternion.Euler(skillRot);
        }
    }
}

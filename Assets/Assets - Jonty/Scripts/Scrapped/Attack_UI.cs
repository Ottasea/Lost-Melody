using UnityEngine;
using UnityEngine.UI;

public class Attack_UI : MonoBehaviour
{
    //====================|   Variables   |==========================================
    [SerializeField] RectTransform meleeBar_img;
    float meleeBar_width;

    public static Attack_UI Instance;


    //====================|   Start()   |==========================================
    private void Start()
    {
        meleeBar_width = meleeBar_img.sizeDelta.x;
        Instance = this;
    }

    //====================|   SetMeleeBarWidth()   |==========================================
    public void SetMeleeBarWidth(float t)
    {
        meleeBar_img.sizeDelta = new Vector2(Mathf.Lerp(0, meleeBar_width, t), meleeBar_img.sizeDelta.y);
    }

}
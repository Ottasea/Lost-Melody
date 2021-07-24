using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Attack_NumbersPop : MonoBehaviour
{
    //=======================|   Variables   |====================================
    const float duration = 1.0f;
    const float maxX = 20.0f;
    const float maxY = 60.0f;

    [SerializeField] Transform parent;
    [SerializeField] GameObject popNumbersObj;

    public static Attack_NumbersPop Instance;


    //=======================|   Awake()   |===============================
    private void Awake()
    {
        Instance = this;
    }

    //=======================|   IEnumerator - PopNumbers()   |===============================
    public IEnumerator PopNumbers (float amount, bool critB)
    {
        RectTransform numObj = Instantiate(popNumbersObj, parent).GetComponent<RectTransform>();
        string crit = critB ? " - VIBE CRITICAL" : "";
        numObj.gameObject.GetComponent<Text>().text = string.Format("{0} Damage{1}",  Mathf.RoundToInt(amount), crit);
        numObj.localPosition = Vector3.zero;

        Vector3 translation = new Vector3(Random.Range(-maxX, maxX), Random.Range(maxY / 2, maxY), 0);

        float t = 0;
        while (t < 1)
        {
            float increment = Time.deltaTime / duration;
            t += increment;
            numObj.localPosition += translation * increment;
            yield return null;
        }

        Destroy(numObj.gameObject);
    }
}

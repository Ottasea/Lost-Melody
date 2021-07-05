using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasProjections : MonoBehaviour
{
    //=============================|   Variables   |===================================================
    [Header("Projections")]
    public List<Projection> projections = new List<Projection>();

    public static CanvasProjections Instance;


    //=============================|   Start()   |===================================================
    private void Awake()
    {
        Instance = this;
    }


    //=============================|   Update()   |===================================================
    void Update()
    {
        foreach (Projection projection in projections)
        {
            if (projection.target != null && projection.projection.gameObject.activeSelf)
            {
                CanvasProject.Instance.Project(true, projection.projection, projection.target, projection.yOffset);
                //Debug.Log(string.Format("Projecting '{0}' onto '{1}'", projection.projection.name, projection.target.name));
            }
        }
    }


    //=============================|   struct - Projection   |===================================================
    [System.Serializable]
    public class Projection
    {
        public RectTransform projection;
        public Transform target;
        public float yOffset;
    }
}
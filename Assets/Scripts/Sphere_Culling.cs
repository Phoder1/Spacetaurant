using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere_Culling : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Sphere_Parts;
    [SerializeField]
    float Distance;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 300;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Enable_Parts();

    }
  
    void Enable_Parts()
    {
        foreach (GameObject Part in Sphere_Parts)
        {
            if (Vector3.Distance(transform.position, Part.GetComponent<Renderer>().bounds.center) < Distance)
            {
                Part.SetActive(true);
            }
            else 
            {
                Part.SetActive(false);
            }
        }
    }
}

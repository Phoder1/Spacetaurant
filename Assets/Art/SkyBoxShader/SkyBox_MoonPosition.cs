using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spacetaurant
{
    public class SkyBox_MoonPosition : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Shader.SetGlobalVector("_MoonDirection", transform.forward);

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}

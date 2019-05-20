using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderInteractor : MonoBehaviour
{

    public Material mat;

    List<Vector3> normals = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        mat.SetVector("_Point", transform.position);

       print( mat.GetVector("_StoredNormal").x);

    }
}

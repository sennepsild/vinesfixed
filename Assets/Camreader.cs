using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camreader : MonoBehaviour
{

    Camera cam;
    //Texture2D tex;
    public Renderer canvas;
    bool grab = false;

    public float entropy;

    public bool entropySetToUpate = false;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        cam.pixelRect = new Rect(0, 0, 412, 258);
        cam.enabled = false;


       // CalculateEntropyOnNextRender();
    }

    // Update is called once per frame
    

    public void CalculateEntropyOnNextRender()
    {
        grab = true;
        cam.enabled = true;
        entropySetToUpate = true;
    }

    
    private void OnPostRender()
    {
        
            //print("Gay");
            TakePicture();
            cam.enabled = false;
        
    }

    void TakePicture()
    {
        
        //Create a new texture with the width and height of the screen
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        texture.Apply();
        //Check that the display field has been assigned in the Inspector
        if (canvas != null)
            //Give your GameObject with the renderer this texture
            canvas.material.mainTexture = texture;
        //Reset the grab state
        grab = false;

        entropy = entropyOfPicture(texture);
        //print(entropy);
        
    }

    float entropyOfPicture(Texture2D tex)
    {
        /*
        Color[] pixs = tex.GetPixels();
        List<float> grayLevels = new List<float>();
        List<float> numberInLevel = new List<float>();

        for (int i = 0; i < pixs.Length; i++)
        {
            float avg = (pixs[i].r + pixs[i].g + pixs[i].b)/3;
            bool isNotInLevel = true;
            for (int j = 0; j < grayLevels.Count; j++)
            {
                if(avg == grayLevels[j])
                {
                    numberInLevel[j] = numberInLevel[j] + 1;
                    isNotInLevel = false;
                }
            }
            if (isNotInLevel)
            {
                grayLevels.Add(avg);
                numberInLevel.Add(1);
            }
        }

        float entropy = 0;
        for (int i = 0; i < numberInLevel.Count; i++)
        {
            entropy += numberInLevel[i] * Mathf.Log(numberInLevel[i], 2);
        }


        print(entropy);
        return entropy;

    */

        Color[] pixs = tex.GetPixels();
        List<float> grayLevels = new List<float>();
        List<float> numberInLevel = new List<float>();

        for (int i = 0; i < pixs.Length; i++)
        {
            float avg = (pixs[i].r + pixs[i].g + pixs[i].b) / 3;
            bool isNotInLevel = true;
            for (int j = 0; j < grayLevels.Count; j++)
            {
                if (avg == grayLevels[j])
                {
                    numberInLevel[j] = numberInLevel[j] + 1;
                    isNotInLevel = false;
                }
            }
            if (isNotInLevel)
            {
                grayLevels.Add(avg);
                numberInLevel.Add(1);
            }
        }

        float value = 0;
        for (int i = 0; i < grayLevels.Count; i++)
        {
            value += 1 / numberInLevel[i];
        }
        value = Mathf.Log10(value);

        return value;
    }

}

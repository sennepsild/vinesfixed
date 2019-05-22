using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{


    public bool useCameras = false;


    float stemcount = 5;

    public int Particles = 30;
    public GameObject particleSystemPrefab;

    public GameObject Leaf;

    public GameObject cameraPrefab;

    List<GameObject> cameras = new List<GameObject>();
    
    List<Vector3> anchorPoints = new List<Vector3>();
    List<bool> pause = new List<bool>();
    List<Vector3> oldAnchorPoints = new List<Vector3>();
    List<Vector3> AnchorNormals = new List<Vector3>();
    List<ParticleSystem> psystems = new List<ParticleSystem>();
    List<ParticleCollisions> psCollisions = new List<ParticleCollisions>();

    List<bool> newAnchorFound = new List<bool>();

    List<bool> add = new List<bool>();

    Lsystem lsystem;

    GameObject vineParent;


    public GameObject prefab;
    public GameObject VinePiece;

    // Start is called before the first frame update
    void Start()
    {
        lsystem = GetComponent<Lsystem>();
        
        AddBranch(transform.position,Vector3.up);

        



        vineParent = new GameObject();
    }
    private void AddBranch( Vector3 position, Vector3 normal)
    {
        anchorPoints.Add(position);
        AnchorNormals.Add(normal);
        GameObject temp = Instantiate(particleSystemPrefab, position + normal, Quaternion.identity);
        psystems.Add(temp.GetComponent<ParticleSystem>());
        psCollisions.Add(temp.GetComponent<ParticleCollisions>());
        newAnchorFound.Add(false);
        oldAnchorPoints.Add(position);
        add.Add(true);
        pause.Add(false);
        if (useCameras)
        {
            for (int i = 0; i < Particles; i++)
            {
                GameObject camTemp = Instantiate(cameraPrefab, transform.position, Quaternion.identity);
                cameras.Add(camTemp);
            }
        }
    }
    

    private void Update()
    {
        
        updateBranches();

    }

    void updateBranches()
    {
        for (int i = 0; i < add.Count; i++)
        {
            if (add[i] && !pause[i])
            {
                StartCoroutine(AddBranch(i));
                add[i] = false;
            }
        }


        for (int i = 0; i < newAnchorFound.Count; i++)
        {
            if (newAnchorFound[i])
            {
                UpdateBranch(i);
            }
        }
    }

    void UpdateBranch(int pointId)
    {
        

        Vector3 localstep = oldAnchorPoints[pointId];
        float dist = Vector3.Distance(anchorPoints[pointId], localstep);

        int steps = Mathf.RoundToInt( dist / 0.4f);
        //print(steps);
        for (int i = 0; i < steps; i++)
        {
            GameObject piece = Instantiate(VinePiece, localstep, Quaternion.identity);
            piece.transform.LookAt(anchorPoints[pointId]);

            if (Vector3.Distance(anchorPoints[pointId], localstep) > 0.4f)
            {
                piece.transform.localScale = new Vector3(0.3f, 0.3f, 0.4f);
            }
            else
            {
                piece.transform.localScale = new Vector3(0.3f, 0.3f, Vector3.Distance(anchorPoints[pointId], localstep));
            }

            


            GameObject leaf = Instantiate(Leaf, localstep + (AnchorNormals[pointId] * 0.1f), Quaternion.identity);
            leaf.transform.LookAt(localstep-AnchorNormals[pointId]);

            leaf.transform.Rotate(Vector3.back, Random.Range(0, 360));

            leaf.transform.Rotate(Vector3.right, Random.Range(0, 20));
            float size = Random.Range(0.7f, 0.5f);
            leaf.transform.localScale = new Vector3(size, size, 1);



            leaf.transform.SetParent(piece.transform);
            piece.transform.SetParent(vineParent.transform);

            localstep = piece.GetComponent<stem>().end.position;
        }
       

        
        oldAnchorPoints[pointId] = anchorPoints[pointId];
        psystems[pointId].gameObject.transform.position = anchorPoints[pointId] + (AnchorNormals[pointId]*0.5f);
        
        newAnchorFound[pointId] = false;
        add[pointId] = true;

      

        updateLsystem();
        

    }

    void updateLsystem()
    {
        if (lsystem.sentence.Length < 6)
        {
            lsystem.Generate();

        string sentence = lsystem.sentence;
        


            if (sentence.Length > anchorPoints.Count)
            {
                for (int i = 0; i < sentence.Length; i++)
                {
                    if (sentence[i] == 'E')
                    {

                        AddBranch(anchorPoints[i - 1], AnchorNormals[i - 1]);
                    }
                }
            }
        }
        
        
    }



    IEnumerator AddBranch(int pointId)
    {
       
        psystems[pointId].Emit(Particles);
        yield return new WaitForSeconds(2);
        /*
        print(22);
        int numParticlesAlive = particleSys.GetParticles(Particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            
            
            //Particles[i].velocity = Particles[i].velocity * -1;
        }
        particleSys.SetParticles(Particles, numParticlesAlive);

    */
        if (useCameras)
            vineParent.SetActive(false);
        print("Cameras engaged");




        List<Vector3> potentialPositions = psCollisions[pointId].potentialPositions;
        List<Vector3> theNormals = psCollisions[pointId].theNormals;

        if (useCameras)
        {
            for (int i = 0; i < potentialPositions.Count; i++)
            {

                Vector3 check = cameras[(pointId * Particles) + i].transform.position;
                cameras[(pointId * Particles) + i].transform.position = potentialPositions[i] + (theNormals[i] * 0.4f);
                cameras[(pointId * Particles) + i].transform.LookAt(potentialPositions[i]);

                if (cameras[(pointId * Particles) + i].transform.position != check)
                {
                    cameras[(pointId * Particles) + i].GetComponent<Camreader>().CalculateEntropyOnNextRender();
                }

            }
        }

        if (useCameras)
            yield return new WaitForSeconds(0.3f);

        vineParent.SetActive(true);

        float[] entropyscores = new float[potentialPositions.Count];

        if (useCameras)
        {
            for (int i = 0; i < entropyscores.Length; i++)
            {
                if (i < 29)
                {
                    if (cameras[(pointId * Particles) + i].GetComponent<Camreader>().entropySetToUpate)
                    {
                        entropyscores[i] = cameras[(pointId * Particles) + i].GetComponent<Camreader>().entropy;
                    }
                    else
                        entropyscores[i] = float.MinValue;
                }
                else
                    entropyscores[i] = float.MinValue;
            }
        }


        float[] scores= new float[potentialPositions.Count];


        for (int i = 0; i < potentialPositions.Count; i++)
        {
            float score = 0;
            score += potentialPositions[i].y - oldAnchorPoints[pointId].y;

            float dist = Vector3.Distance(potentialPositions[i], oldAnchorPoints[pointId]);
            score *= dist;
            score += entropyscores[i];
            if (dist > 2)
            {
                score = float.MinValue;
            }
            scores[i] = score;
        }

        float[] highScores = { float.MinValue,float.MinValue,float.MinValue };
        int[] scoreNumber = { 0, 0, 0 };
        
        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] > highScores[0])
            {
                highScores[0] = scores[i];
                scoreNumber[0] = i;
            }
            else if(scores[i] > highScores[1])
            {
                highScores[1] = scores[i];
                scoreNumber[1] = i;
            }
            else if (scores[i] > highScores[2])
            {
                highScores[2] = scores[i];
                scoreNumber[2] = i;
            }
        }

        //print(highScores[0]+","+ highScores[1] + ","+ highScores[2]);

        int chosenInt = Mathf.RoundToInt(Random.Range(0, 2));
        anchorPoints[pointId] = potentialPositions[scoreNumber[chosenInt]];
        AnchorNormals[pointId] = theNormals[scoreNumber[chosenInt]];
        
        psCollisions[pointId].potentialPositions = new List<Vector3>();
        psCollisions[pointId].theNormals = new List<Vector3>();
        newAnchorFound[pointId] = true;

       
        yield return false;
    }


    
}

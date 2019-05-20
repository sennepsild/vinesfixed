using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisions : MonoBehaviour
{

    ParticleSystem particleSys;
    ParticleSystem.Particle[] Particles;
    List<ParticleCollisionEvent> collisionEvents;

    public List<Vector3> potentialPositions;
    public List<Vector3> theNormals;


    // Start is called before the first frame update
    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        potentialPositions = new List<Vector3>();
        theNormals = new List<Vector3>();

        Particles = new ParticleSystem.Particle[particleSys.maxParticles];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(particleSys, other, collisionEvents);
        for (int i = 0; i < collisionEvents.Count; i++)
        {
            potentialPositions.Add(collisionEvents[i].intersection);
            theNormals.Add(collisionEvents[i].normal);

            // Instantiate(prefab, collisionEvents[i].intersection,Quaternion.identity);
        }


    }

}

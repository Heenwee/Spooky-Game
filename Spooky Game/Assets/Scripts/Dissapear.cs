using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissapear : MonoBehaviour
{
    public string[] tags;
    public bool colDestroy;
    public AudioClip sound;
    public float volume = 1;
    AudioSource source;
    public GameObject[] effect;

    public float DissapearTime;

    public bool trail;
    public GameObject trailParticles;

    public float camShakeDur, camShakeMag;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DissapearTime);

        if (sound != null)
        {
            gameObject.AddComponent(typeof(AudioSource));
            source = GetComponent<AudioSource>();
            source.spatialBlend = 0.1f;
            source.PlayOneShot(sound);
            source.volume = volume;
        }
        if (trail)
        {
            trailParticles = Instantiate(trailParticles, transform.position, transform.rotation);
        }

        StartCoroutine(Camera.main.GetComponent<CamShake>().Shake(camShakeDur, camShakeMag));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(colDestroy)
        {
            foreach (string tag in tags)
            {
                if(col.CompareTag(tag))
                {
                    Destroy(gameObject);
                }
            }
            if(tags.Length == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        if(trail)
        {
            trailParticles.transform.position = transform.position;
            trailParticles.transform.rotation = transform.rotation;
        }
    }

    private void OnDestroy()
    {
        if(effect.Length != 0) Instantiate(effect[Random.Range(0, effect.Length)], transform.position, transform.rotation);

        if (trail)
        {
            //trailParticles.transform.parent = trailParticles.transform;
            trailParticles.GetComponent<ParticleSystem>().Stop();
            Destroy(trailParticles, 1f);
        }
    }
}

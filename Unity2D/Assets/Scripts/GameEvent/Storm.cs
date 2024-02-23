using System.Collections.Generic;
using UnityEngine;

public class Storm : MonoBehaviour
{
    [SerializeField] private ParticleSystem missile;
    [SerializeField] private ParticleSystem explode;
    [SerializeField] private AudioSource effSound;
    [SerializeField] private List<Vector3> spawnPos;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            explode.Play();
            effSound.Play();
            GameManager.Instance.TopBar.AddDead((other.gameObject.name == "Player1") ? "p1" : "p2", -2);
            transform.position = spawnPos[Random.Range(0, spawnPos.Count)];
            explode.Play();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    [SerializeField] private BoxCollider2D zone;
    private UnityEngine.Vector2 _playerRes;
    void Start()
    {
        _playerRes = player.GetComponent<Movement>().resPos;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _playerRes.x = transform.position.x;
            _playerRes.y = transform.position.y;
            player.GetComponent<Movement>().resPos = _playerRes;
        }
    }
}

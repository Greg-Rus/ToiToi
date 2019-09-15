using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentCollector : MonoBehaviour
{
    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        _gameManager.OnMomentCollected();
        Destroy(other.gameObject);
    }
}

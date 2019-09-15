using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int _moments;

    private float _distance;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMomentCollected()
    {
        Debug.Log("Moment Collected!");
        _moments++;
    }

    public void UpdateDistance(float distance)
    {
        _distance += distance;
    }
}

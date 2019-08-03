using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedJoystick : Joystick
{
    [SerializeField] public float Hor;
    [SerializeField] public float Ver;

    void Update()
    {
        Hor = Horizontal;
        Ver = Vertical;
    }
}
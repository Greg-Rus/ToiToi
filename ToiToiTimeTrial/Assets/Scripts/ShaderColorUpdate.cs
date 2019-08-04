using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderColorUpdate : MonoBehaviour
{
    public Color Color1;

    public Color Color2;
    public float Color1ChangeSpeed;
    public float Color2ChangeSpeed;

    public Material Material;

    public float _heu1, s1, v1;
    public float _heu2, s2, v2 ;
    // Start is called before the first frame update
    void Start()
    {
        Color.RGBToHSV(Color1, out _heu1, out s1, out v1);
        Color.RGBToHSV(Color1, out _heu2, out s2, out v2);
        
        Material.SetColor("Color1", Color1);
        Material.SetColor("Color2", Color2);
    }

    // Update is called once per frame
    void Update()
    {
        _heu1 += Color1ChangeSpeed;
        if (_heu1 > 360) _heu1 = _heu1 - 360;

        _heu2 += Color2ChangeSpeed;
        if (_heu2 > 360) _heu2 = _heu2 - 360;

        Color1 = Color.HSVToRGB(_heu1, s1, v1);
        Color2 = Color.HSVToRGB(_heu2, s2, v2);

        Material.SetColor("Color1", Color1);
        Material.SetColor("Color2", Color2);
    }
}

using TMPro;
using UnityEngine;
using CellularSwarm.Core;

public class UnityCell : MonoBehaviour
{
    public string text;
    private TextMeshPro textMeshPro;
    public Cell cell;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        if (textMeshPro != null)
        {
            textMeshPro.text = text;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

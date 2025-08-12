using TMPro;
using UnityEngine;

public class UnityCell : MonoBehaviour
{
    public string text;
    private TextMeshPro textMeshPro;

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

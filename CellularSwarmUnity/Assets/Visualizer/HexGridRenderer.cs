using UnityEngine;

public class HexGridRenderer : MonoBehaviour
{
    public GameObject hexPrefab;
    public int radius;
    public float hexSize = 1.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void GenerateHexGrid()
    {
        for (int q = -radius; q <= radius; q++)
        {
            for (int r = -radius; r <= radius; r++)
            {
                if (Mathf.Abs(q + r) <= radius)
                {
                    CreateHexagon(q, r);
                }
            }
        }
    }

    void CreateHexagon(int q, int r)
    {
        float x = hexSize * q * 3 / 2;
        float y = hexSize * r * Mathf.Sqrt(3) + hexSize * q * Mathf.Sqrt(3) / 2; // hex_size * r * math.sqrt(3) + hex_size * q * math.sqrt(3) / 2

        Vector3 position = new Vector3(x, y, 0);

        UnityCell cell = hexPrefab.GetComponent<UnityCell>();
        if (cell != null)
        {
            cell.text = $"({q}, {r})";
        }
        // instantiate it as a child
        GameObject hexagon = Instantiate(hexPrefab, position, Quaternion.identity);
        hexagon.transform.localScale = new Vector3(hexSize * 2, hexSize * 2, 1);
        hexagon.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        // remove all children
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GenerateHexGrid();
    }
}

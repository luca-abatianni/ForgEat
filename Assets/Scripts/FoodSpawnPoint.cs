using UnityEngine;

[ExecuteInEditMode]
public class FoodSpawnPoint : MonoBehaviour
{
    public Vector3 cubeSize = new Vector3(1, 1, 1);
    public Color cubeColor = Color.green;

    private void OnDrawGizmos()
    {
        Color originalColor = Gizmos.color;
        Gizmos.color = cubeColor;
        Gizmos.DrawCube(transform.position, cubeSize);
        Gizmos.color = originalColor;
    }
}

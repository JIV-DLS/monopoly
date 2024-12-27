using UnityEngine;

public class HallowCube : MonoBehaviour
{
    void Start()
    {
        // Create a transparent material with Fade mode and a white color with alpha 0 (fully transparent)
        Material transparentMaterial = new Material(Shader.Find("Standard"));
        transparentMaterial.SetFloat("_Mode", 2); // Set the material to Fade mode
        transparentMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        transparentMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        transparentMaterial.SetInt("_ZWrite", 0);
        transparentMaterial.DisableKeyword("_ALPHATEST_ON");
        transparentMaterial.EnableKeyword("_ALPHABLEND_ON");
        transparentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        transparentMaterial.renderQueue = 3000; // Set render queue to transparent

        // Set the color to white with full transparency (RGBA: 255, 255, 255, 0)
        transparentMaterial.color = new Color(255, 255, 1f, 1);

        // Get the size of the cube (assuming uniform scaling)
        Vector3 cubeSize = transform.localScale;

        // Define the offset based on the cube's size to position planes correctly
        Vector3[] positions = new Vector3[]
        {
            new Vector3(0, cubeSize.y / 2, 0), // Top
            new Vector3(0, -cubeSize.y / 2, 0), // Bottom
            new Vector3(0, 0, cubeSize.z / 2), // Front
            new Vector3(0, 0, -cubeSize.z / 2), // Back
            new Vector3(-cubeSize.x / 2, 0, 0), // Left
            new Vector3(cubeSize.x / 2, 0, 0), // Right
        };

        // Rotation angles for each plane to match the cube faces
        Vector3[] rotations = new Vector3[]
        {
            new Vector3(90, 0, 0), // Top
            new Vector3(-90, 0, 0), // Bottom
            new Vector3(0, 0, 0), // Front
            new Vector3(0, 180, 0), // Back
            new Vector3(0, 90, 0), // Left
            new Vector3(0, -90, 0), // Right
        };

        // Loop through each side of the cube and create the planes
        for (int i = 0; i < 6; i++)
        {
            // Create a new plane GameObject
            GameObject plane = new GameObject("Plane_" + i);
            plane.transform.SetParent(transform); // Set the cube as the parent

            // Add a MeshRenderer and MeshFilter to the plane
            MeshRenderer renderer = plane.AddComponent<MeshRenderer>();
            MeshFilter filter = plane.AddComponent<MeshFilter>();

            // Add a MeshCollider to the plane
            MeshCollider meshCollider = plane.AddComponent<MeshCollider>();

            // Create a plane mesh
            Mesh planeMesh = new Mesh();
            filter.mesh = planeMesh;

            // Set up vertices and triangles for a simple plane
            planeMesh.vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.5f, 0), // Bottom-left
                new Vector3(0.5f, -0.5f, 0), // Bottom-right
                new Vector3(0.5f, 0.5f, 0), // Top-right
                new Vector3(-0.5f, 0.5f, 0) // Top-left
            };

            planeMesh.triangles = new int[]
            {
                0, 1, 2, // First triangle
                0, 2, 3 // Second triangle
            };

            // Apply the dynamically created transparent material to the plane
            renderer.material = transparentMaterial;

            // Position and rotate the plane to match the cube's faces
            plane.transform.localPosition = positions[i];
            plane.transform.localEulerAngles = rotations[i];

            // Adjust the scale to match the cube's faces
            if (i == 0 || i == 1) // Top/Bottom planes
            {
                plane.transform.localScale = new Vector3(cubeSize.x, cubeSize.z, 1);
            }
            else if (i == 2 || i == 3) // Front/Back planes
            {
                plane.transform.localScale = new Vector3(cubeSize.x, 1, cubeSize.z);
            }
            else if (i == 4 || i == 5) // Left/Right planes
            {
                plane.transform.localScale = new Vector3(1, cubeSize.y, cubeSize.z);
            }

            // Set the MeshCollider's mesh to match the plane's mesh
            meshCollider.sharedMesh = planeMesh;
        }
    }
}
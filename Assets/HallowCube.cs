using UnityEngine;

public class HollowCube : MonoBehaviour
{
        void Start()
    {
        // Create a cube and set its position
        // GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // Get the Renderer component of the cube attached to this GameObject
        Renderer cube = GetComponent<Renderer>();
        // cube.transform.position = new Vector3(0, 0, 0);  // Position at the origin
        // cube.transform.localScale = new Vector3(2, 2, 2);  // Scale to make the cube bigger

        // Get the Renderer component to access the bounds
        Renderer renderer = cube.GetComponent<Renderer>();
        
        // Get the bounds of the cube
        Bounds bounds = renderer.bounds;

        // Calculate the 8 corners of the cube
        Vector3[] corners = new Vector3[8];
        corners[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z); // Bottom-Left-Front
        corners[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z); // Bottom-Left-Back
        corners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z); // Top-Left-Front
        corners[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z); // Top-Left-Back
        corners[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z); // Bottom-Right-Front
        corners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z); // Bottom-Right-Back
        corners[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z); // Top-Right-Front
        corners[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z); // Top-Right-Back

       
        // Create spheres at each corner and set their color
        /*foreach (var corner in corners)
        {
            CreateASphereAt(corner);
        }*/
        
        // Create squares (quads) for each face of the cube with custom names and colors
        CreateA3dRectangleAt("SquareFront", corners[0], corners[1], corners[2], corners[3], Color.red, this.gameObject); // Front face
        CreateA3dRectangleAt("SquareBack", corners[4], corners[5], corners[6], corners[7], Color.blue, this.gameObject); // Back face
        CreateA3dRectangleAt("SquareBottom", corners[0], corners[1], corners[4], corners[5], Color.green, this.gameObject); // Bottom face
        CreateA3dRectangleAt("SquareTop", corners[2], corners[3], corners[6], corners[7], Color.yellow, this.gameObject); // Top face
        CreateA3dRectangleAt("SquareLeft", corners[0], corners[2], corners[4], corners[6], Color.cyan, this.gameObject); // Left face
        CreateA3dRectangleAt("SquareRight", corners[1], corners[3], corners[5], corners[7], Color.magenta, this.gameObject); // Right face

    }
        private static void CreateA3dRectangleAt(string planeName, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color, GameObject currentObject)
{
    // Determine which coordinate (x, y, or z) is fixed across the four points
    float fixedX = p1.x;
    float fixedY = p1.y;
    float fixedZ = p1.z;

    // Initialize width, height, and fixed depth
    float width = 0f, height = 0f, depth = 0.8f; // Set depth to 0.8f for all cases

    // Check if x is the fixed coordinate
    if (p1.x == p2.x && p1.x == p3.x && p1.x == p4.x)
    {
        width = Vector3.Distance(p1, p2);  // Distance between p1 and p2 on the y or z axis
        height = Vector3.Distance(p1, p3); // Distance between p1 and p3 on the z axis (if y is fixed)
    }
    // Check if y is the fixed coordinate
    else if (p1.y == p2.y && p1.y == p3.y && p1.y == p4.y)
    {
        height = Vector3.Distance(p1, p2); // Distance between p1 and p2 on the x or z axis
        width = Vector3.Distance(p1, p3);  // Distance between p1 and p3 on the x axis (if z is fixed)
    }
    // Check if z is the fixed coordinate
    else if (p1.z == p2.z && p1.z == p3.z && p1.z == p4.z)
    {
        width = Vector3.Distance(p1, p2);  // Distance between p1 and p2 on the x or y axis
        height = Vector3.Distance(p1, p3); // Distance between p1 and p3 on the y axis (if x is fixed)
    }
    else
    {
        Debug.LogError("The points must have one coordinate that is the same for all.");
        return;
    }

    // Calculate the center of the rectangle
    Vector3 center = (p1 + p2 + p3 + p4) / 4;

    // Create the rectangle (a cube with varying dimensions)
    GameObject rectangle = GameObject.CreatePrimitive(PrimitiveType.Cube);
    rectangle.transform.position = center;
    Debug.Log($"Rectangle Center ({planeName}): " + center);

    // Set the scale based on the calculated width, height, and fixed depth
    rectangle.transform.localScale = new Vector3(width, height, depth);

    // Apply rotation based on color
    if (color == Color.yellow || color == Color.green)
    {
        rectangle.transform.Rotate(90, 0, 0); // Rotate 90 degrees along the X-axis
    }
    else if (color == Color.cyan || color == Color.magenta)
    {
        rectangle.transform.Rotate(0, 0, 90); // Rotate 90 degrees along the Z-axis
    }
    else if (color == Color.blue || color == Color.red)
    {
        rectangle.transform.Rotate(0, 90, 0); // Rotate 90 degrees along the Y-axis
    }

    // Retrieve the material from the current object
    Material currentMaterial = null;
    if (currentObject != null)
    {
        Renderer currentRenderer = currentObject.GetComponent<Renderer>();
        if (currentRenderer != null)
        {
            currentMaterial = currentRenderer.material;
        }
    }

    // Set the rectangle's material to match the current object's material
    Renderer rectangleRenderer = rectangle.GetComponent<Renderer>();
    if (currentMaterial != null)
    {
        rectangleRenderer.material = currentMaterial;
    }
    else
    {
        // Fall back to using the color-based material
        rectangleRenderer.material.color = color;
        Debug.LogWarning("No material found on the current object. Using default color-based material.");
    }

    // Add a Box Collider to the rectangle
    BoxCollider boxCollider = rectangle.AddComponent<BoxCollider>();
    boxCollider.size = new Vector3(1f, 1f, 1f); // Default size (you can adjust based on your needs)
}
        private static void CreateA3dRectangleAt(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
{
    // Determine which coordinate (x, y, or z) is fixed across the four points
    float fixedX = p1.x;
    float fixedY = p1.y;
    float fixedZ = p1.z;

    // Initialize width, height, and depth variables
    float width = 0f, height = 0f, depth = 0f;

    // Check if x is the fixed coordinate
    if (p1.x == p2.x && p1.x == p3.x && p1.x == p4.x)
    {
        width = Vector3.Distance(p1, p2);  // Distance between p1 and p2 on the y or z axis
        height = Vector3.Distance(p1, p3); // Distance between p1 and p3 on the z axis (if y is fixed)
        depth = Vector3.Distance(p2, p3);  // Distance between p2 and p3 (y-z or x-z depending on the fixed coordinate)
    }
    // Check if y is the fixed coordinate
    else if (p1.y == p2.y && p1.y == p3.y && p1.y == p4.y)
    {
        height = Vector3.Distance(p1, p2); // Distance between p1 and p2 on the x or z axis
        depth = Vector3.Distance(p1, p3);  // Distance between p1 and p3 on the x axis (if z is fixed)
        width = Vector3.Distance(p2, p3);  // Distance between p2 and p3 (x-z or y-z depending on fixed axis)
    }
    // Check if z is the fixed coordinate
    else if (p1.z == p2.z && p1.z == p3.z && p1.z == p4.z)
    {
        depth = Vector3.Distance(p1, p2);  // Distance between p1 and p2 on the x or y axis
        height = Vector3.Distance(p1, p3); // Distance between p1 and p3 on the y axis (if x is fixed)
        width = Vector3.Distance(p2, p3);  // Distance between p2 and p3 (x-y or y-z depending on the fixed coordinate)
    }
    else
    {
        Debug.LogError("The points must have one coordinate that is the same for all.");
        return;
    }

    // Calculate the center of the rectangle
    Vector3 center = (p1 + p2 + p3 + p4) / 4;

    // Create the rectangle (a cube with varying dimensions)
    GameObject rectangle = GameObject.CreatePrimitive(PrimitiveType.Cube);
    rectangle.transform.position = center;
    Debug.Log("Rectangle Center: " + center);

    // Set the color to blue for the rectangle
    Renderer rectangleRenderer = rectangle.GetComponent<Renderer>();
    rectangleRenderer.material.color = Color.blue;

    // Set the scale based on the calculated dimensions
    rectangle.transform.localScale = new Vector3(width, height, depth);
}
    private static void CreateA3dSquareAt(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        // Create a cube (3D square) that spans between the given four points.
        // We will calculate the center of the square from the four points.
    
        // Calculate the center (centroid) of the 4 points
        Vector3 center = (p1 + p2 + p3 + p4) / 4;

        // Create the cube at the centroid of the 4 points
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = center;
        Debug.Log("Square Center: " + center);
    
        // Set the cube's color to green (you can change this to any color)
        Renderer cubeRenderer = cube.GetComponent<Renderer>();
        cubeRenderer.material.color = Color.green;
    
        // Calculate the dimensions of the square or rectangle from the points
        // We assume the points form a rectangular shape; adjust size accordingly
        float width = Vector3.Distance(p1, p2);
        float height = Vector3.Distance(p2, p3);

        // Optionally, adjust the cube's size based on the dimensions calculated
        cube.transform.localScale = new Vector3(width, height, 0.1f); // Thin cube, to represent a square in 3D space
    }
    private static void CreateA3dSquareAt(Vector3 position)
    {
        // Create a cube (3D square) at the given position
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        Debug.Log("Square Position: " + position);
    
        // Set the cube's color to blue (you can change this to any color)
        Renderer cubeRenderer = cube.GetComponent<Renderer>();
        cubeRenderer.material.color = Color.blue;
    
        // Optionally, adjust the cube's size for better visibility (you can set any scale)
        cube.transform.localScale = new Vector3(1f, 1f, 1f); // Default size; you can change it as needed
    }
    private static void CreateASphereAt(Vector3 corner)
    {
        // Create a sphere at the corner
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = corner;
        Debug.Log("Corner: " + corner);
            
        // Set the sphere's color to red (you can change this to any color)
        Renderer sphereRenderer = sphere.GetComponent<Renderer>();
        sphereRenderer.material.color = Color.red;
            
        // Optionally, adjust the sphere's size for better visibility
        sphere.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    // Method to create a square from 4 points, name the square, and apply a color
    void CreateSquare(string squareName, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color)
    {
        // Create a new GameObject (quad) and set its vertices
        GameObject square = new GameObject(squareName);
        MeshFilter meshFilter = square.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = square.AddComponent<MeshRenderer>();

        // Create a new mesh for the square
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Define the vertices for the square
        mesh.vertices = new Vector3[] { p1, p2, p3, p4 };

        // Define the triangle indices for the square (since Unity uses triangles for meshes)
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        // Define the normals for the square
        mesh.normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };

        // Apply the color to the square
        meshRenderer.material.color = color;
    }
    // Method to create a plane (quad) from 4 points, name the plane, and apply a color
    void CreatePlane(string planeName, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color color)
    {
        // Create a new GameObject for the plane
        GameObject plane = new GameObject(planeName);
        MeshFilter meshFilter = plane.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = plane.AddComponent<MeshRenderer>();

        // Create a new mesh for the plane (quad)
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        // Define the vertices for the plane (quad)
        mesh.vertices = new Vector3[] { p1, p2, p3, p4 };

        // Define the triangle indices for the plane (quad)
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        // Define the normals for the plane (since the faces are flat)
        mesh.normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };

        // Apply the color to the plane
        meshRenderer.material.color = color;

        // Optionally, align the plane with the camera or adjust it as necessary
        plane.transform.position = (p1+ p2+ p3+ p4)/4;
        CreateA3dRectangleAt(p1, p2, p3, p4);
        //plane.transform.LookAt(p1); // Optionally orient the plane towards a specific direction
    }
    // Method to calculate the centroid of 4 points with one fixed coordinate
    public static Vector3 CalculateCentroid(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        // Check which coordinate is fixed (all values should be the same for that coordinate)
        float fixedX = p1.x;
        float fixedY = p1.y;
        float fixedZ = p1.z;

        // Determine which coordinate is fixed
        if (p1.x == p2.x && p1.x == p3.x && p1.x == p4.x)
        {
            // x is the fixed coordinate
            return new Vector3(fixedX, (p1.y + p2.y + p3.y + p4.y) / 4, (p1.z + p2.z + p3.z + p4.z) / 4);
        }
        else if (p1.y == p2.y && p1.y == p3.y && p1.y == p4.y)
        {
            // y is the fixed coordinate
            return new Vector3((p1.x + p2.x + p3.x + p4.x) / 4, fixedY, (p1.z + p2.z + p3.z + p4.z) / 4);
        }
        else if (p1.z == p2.z && p1.z == p3.z && p1.z == p4.z)
        {
            // z is the fixed coordinate
            return new Vector3((p1.x + p2.x + p3.x + p4.x) / 4, (p1.y + p2.y + p3.y + p4.y) / 4, fixedZ);
        }
        else
        {
            Debug.LogError("The points must have one coordinate that is the same for all.");
            return Vector3.zero; // Return zero vector in case of error
        }
    }
}
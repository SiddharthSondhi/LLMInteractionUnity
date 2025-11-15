using UnityEngine;

public static class CubeFunctions {

    public static Transform cube;
    public static string functionDescriptions = 
        @"
        - RotateCube        
          description - Rotates the cube given number of degrees around the x, y, or z axis.
          arguments - degrees : float, axis: 'x'|'y'|'z'
        
        - MoveCubeUp
          description - Moves Cube up by .5 units
        
        - MoveCubeDown
          description - Moves Cube down by .5 units

        - ScaleCube
          description - Scales cube by the scale factor
          arguments - scaleFactor : float

        - SetCubeColor
          description - Sets the color of the cube to the using rgb values (range 0 - 255)
          arguments - r: int, g: int, b: int 
        ";

    public static string RotateCube(float degrees, string axis) {
        Vector3 axisVec = Vector3.up;
        switch (axis) {
            case "x": axisVec = Vector3.right; break;
            case "y": axisVec = Vector3.up; break;
            case "z": axisVec = Vector3.forward; break;
        }

        cube.Rotate(axisVec, degrees, Space.World);

        return $"Rotated cube {degrees} degrees around the {axis} axis";
    }

    public static string MoveCubeUp() {
        cube.Translate(0.0f, 0.5f, 0.0f);
        return "Moved Cube up";
    }

    public static string MoveCubeDown() {
        cube.Translate(0.0f, -0.5f, 0.0f);
        return "Moved Cube down";
    }

    public static string ScaleCube(float scaleFactor) {

        cube.localScale = new Vector3 (scaleFactor, scaleFactor, scaleFactor);
        return $"Scaled cube by factor of {scaleFactor}";
    }

    public static string SetCubeColor(int r, int g, int b) {

        r = Mathf.Clamp(r, 0, 255);
        g = Mathf.Clamp(g, 0, 255);
        b = Mathf.Clamp(b, 0, 255);

        cube.GetComponent<Renderer>().material.color = new Color(r / 255f, g / 255f, b / 255f);
        return $"Set the cubes color to r: {r}, g: {g}, b: {b}";
    }
}
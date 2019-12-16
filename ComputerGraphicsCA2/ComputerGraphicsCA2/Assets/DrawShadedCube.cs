using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawShadedCube : MonoBehaviour
{
    Texture2D texture;
    Light light;

    Vector3[] cube;

    [SerializeField]Vector3 scale;
    [SerializeField]Vector3 translate;
    [SerializeField]float rotationAngle;
    [SerializeField]Vector3 startingAxis;
    Quaternion rotation;

    Color defaultColour;

    Matrix4x4 viewingMatrix;
    Matrix4x4 projectionMatrix;

    Matrix4x4 rotationMatrix;
    Matrix4x4 scaleMatrix;
    Matrix4x4 translateMatrix;

    Matrix4x4 transformMatrix;
    Matrix4x4 BIGMATRIX;

    Vector3[] finalImage;
    Vector2[] finalPostDivisionImage;

    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.mainTexture = texture;

        light = FindObjectOfType<Light>();

        defaultColour = new Color(texture.GetPixel(1, 1).r, texture.GetPixel(1, 1).g, texture.GetPixel(1, 1).b, texture.GetPixel(1, 1).a);

        cube = new Vector3[8];
        cube[0] = new Vector3(1, 1, 1);
        cube[1] = new Vector3(-1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(1, 1, -1);
        cube[5] = new Vector3(-1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);

        // viewing matrix
        Vector3 cameraPosition = new Vector3(0, 0, 20);
        Vector3 cameraLookAt = new Vector3(0, 0, 0);
        Vector3 cameraUp = new Vector3(0, 1, 0);

        Vector3 lookRotationDir = cameraLookAt - cameraPosition;

        Quaternion camRotation = Quaternion.LookRotation(lookRotationDir.normalized, cameraUp.normalized);

        viewingMatrix = Matrix4x4.TRS(-cameraPosition, camRotation, Vector3.one);

        // projection matrix
        projectionMatrix = Matrix4x4.Perspective(45, Screen.width / Screen.height, 1, 1000);

        startingAxis = Vector3.up;
        startingAxis.Normalize();

        rotationAngle = 0;

        scale = new Vector3(1, 1, 1);

        translate = new Vector3(0, 0, 0);

        drawCube();
    }

    void Update()
    {
        // modify transform values in editor

        drawCube();
    }

    private void drawCube()
    {
        Destroy(texture);

        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.mainTexture = texture;

        // rotation matrix
        rotation = Quaternion.AngleAxis(rotationAngle, startingAxis);
        rotationMatrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

        // scale matrix
        scaleMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);

        // translation matrix
        translateMatrix = Matrix4x4.TRS(translate, Quaternion.identity, Vector3.one);

        // transform matrix
        transformMatrix = rotationMatrix * scaleMatrix * translateMatrix;

        // super matrix
        BIGMATRIX = projectionMatrix * viewingMatrix * transformMatrix;

        finalImage = MatrixTransform(cube, BIGMATRIX);

        finalPostDivisionImage = dividebyz(finalImage);

        Vector2[] frontFace = new Vector2[4];
        Vector2[] rightFace = new Vector2[4];
        Vector2[] topFace = new Vector2[4];
        Vector2[] backFace = new Vector2[4];
        Vector2[] leftFace = new Vector2[4];
        Vector2[] bottomFace = new Vector2[4];

        // front
        frontFace[0] = finalPostDivisionImage[0];
        frontFace[1] = finalPostDivisionImage[1];
        frontFace[2] = finalPostDivisionImage[2];
        frontFace[3] = finalPostDivisionImage[3];

        // right
        rightFace[0] = finalPostDivisionImage[4];
        rightFace[1] = finalPostDivisionImage[0];
        rightFace[2] = finalPostDivisionImage[3];
        rightFace[3] = finalPostDivisionImage[7];

        // top
        topFace[0] = finalPostDivisionImage[4];
        topFace[1] = finalPostDivisionImage[5];
        topFace[2] = finalPostDivisionImage[1];
        topFace[3] = finalPostDivisionImage[0];

        // back
        backFace[0] = finalPostDivisionImage[5];
        backFace[1] = finalPostDivisionImage[4];
        backFace[2] = finalPostDivisionImage[7];
        backFace[3] = finalPostDivisionImage[6];

        // left
        leftFace[0] = finalPostDivisionImage[1];
        leftFace[1] = finalPostDivisionImage[5];
        leftFace[2] = finalPostDivisionImage[6];
        leftFace[3] = finalPostDivisionImage[2];

        // bottom
        bottomFace[0] = finalPostDivisionImage[6];
        bottomFace[1] = finalPostDivisionImage[7];
        bottomFace[2] = finalPostDivisionImage[3];
        bottomFace[3] = finalPostDivisionImage[2];

        drawFace(frontFace);
        drawFace(rightFace);
        drawFace(topFace);
        drawFace(backFace);
        drawFace(leftFace);
        drawFace(bottomFace);

        texture.Apply();
    }

    void drawFace(Vector2[] points)
    {
        Vector2 i = points[0];
        Vector2 j = points[1];
        Vector2 k = points[2];
        Vector2 l = points[3];

        if (isFrontFace(points))
        {
            drawLine(i, j);
            drawLine(j, k);
            drawLine(k, l);
            drawLine(l, i);

            Vector2 floodFillPoint = getFloodFillPoint(points);
            Vector2 midPoint = convertToScreenSpace(floodFillPoint);

            Vector3 normal = getNormal(points);

            floodFill((int)midPoint.x, (int)midPoint.y, Color.red, defaultColour, normal);
        }
    }

    public void floodFill(int x, int y, Color fill, Color oldColour, Vector3 normal)
    {
        Stack<Vector2> points = new Stack<Vector2>();
        points.Push(new Vector2(x, y));

        while (points.Count > 0)
        {
            Vector2 p = points.Pop();

            if (isInView(p))
            {
                if (texture.GetPixel((int)p.x, (int)p.y) == oldColour)
                {
                    Vector3 lightDir = getLightDir(p);
                    float dotProduct = Vector3.Dot(lightDir, normal);
                    Color colour = new Color(dotProduct * fill.r * light.intensity, dotProduct * fill.g * light.intensity, dotProduct * fill.b * light.intensity, 1);

                    texture.SetPixel((int)p.x, (int)p.y, colour);
                    points.Push(new Vector2(p.x + 1, p.y));
                    points.Push(new Vector2(p.x - 1, p.y));
                    points.Push(new Vector2(p.x, p.y + 1));
                    points.Push(new Vector2(p.x, p.y - 1));
                }
            }
        }
    }

    bool isInView(Vector2 point)
    {
        if ((point.x < 0) || (point.x >= texture.width))
            return false;

        else if ((point.y < 0) || (point.y >= texture.height))
            return false;

        else
            return true;
    }

    Vector3 getNormal(Vector2[] face)
    {
        Vector2 i = face[0];
        Vector2 j = face[1];
        Vector2 k = face[2];

        Vector2 a = j - i;
        Vector2 b = k - i;

        return Vector3.Cross(a, b);
    }

    public Vector3 getLightDir(Vector3 point)
    {
        return Vector3.Normalize((point - light.transform.position));
    }

    void drawLine(Vector2 start, Vector2 finish)
    {
        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
    }
    
    bool isFrontFace(Vector2[] points)
    {
        Vector2 i = points[0];
        Vector2 j = points[1];
        Vector2 k = points[2];

        float z = (j.x - i.x) * (k.y - j.y) - (j.y - i.y) * (k.x - j.x);

        return z >= 0;
    }

    Vector2 getFloodFillPoint(Vector2[] points)
    {
        float x = (points[0].x + points[1].x + points[2].x + points[3].x) / 4;
        float y = (points[0].y + points[1].y + points[2].y + points[3].y) / 4;

        return new Vector2(x, y);
    }

    private void plot(List<Vector2Int> list)
    {
        foreach (Vector2Int v in list)
        {
            Color color = Color.blue;
            texture.SetPixel(v.x, v.y, color);
        }

        texture.Apply();
    }

    private Vector2Int convertToScreenSpace(Vector2 v)
    {
        int x = (int)Math.Round(((v.x + 1) / 2) * (Screen.width - 1));

        int y = (int)Math.Round(((1 - v.y) / 2) * (Screen.height - 1));

        return new Vector2Int(x, y);
    }

    private Vector2[] dividebyz(Vector3[] finalImage)
    {
        List<Vector2> output_list = new List<Vector2>();
        foreach (Vector3 v in finalImage)
            output_list.Add(new Vector2(v.x / v.z, v.y / v.z));

        return output_list.ToArray();

    }

    /// <summary>
    /// Checks if line is clipped
    /// </summary>
    /// <param name="v">1st point of line passed in</param>
    /// <param name="u">2nd point of line passed in</param>
    /// <returns>Returns true if the line is not clipped</returns>
    public static bool lineClip(ref Vector2 v, ref Vector2 u)
    {
        Outcode v0 = new Outcode(v);
        Outcode u0 = new Outcode(u);
        Outcode inViewPort = new Outcode();

        //Trivially Accept
        if ((v0 + u0) == inViewPort)
        {
            return true;
        }

        //Trivially Reject
        if (v0 * u0 != inViewPort)
        {
            return false;
        }

        // If niether trivially rejected or triavally accepted, checks if the first point of the line is in the viewport
        if (v0 == inViewPort)
        {
            /*Calls method again but with points enterd in reverse.
             *Will not be trivially accepted or rejected and will reach this point again and checks if the
             *second point is in the viewport or not (It wont be as it wasnt trivially rejected, so it continues with the method) */
            return lineClip(ref u, ref v);
        }

        if (v0.up)
        {
            v = intercept(u, v, 0);

            return true;
        }

        if (v0.down)
        {
            v = intercept(u, v, 1);

            return true;
        }

        if (v0.left)
        {
            v = intercept(u, v, 2);

            return true;
        }

        v = intercept(u, v, 3);
        return true;
    }

    /// <summary>
    /// Clips a line with given points p1 and p2, and the side of the viewport identified by v2(0 = up, 1 = down, 2 = left, other values are right).
    /// </summary>
    /// <param name="p1">1st point of line passed in</param>
    /// <param name="p2">2nd point of line passed in</param>
    /// <param name="v2">Side of the viewport of point to be clipped</param>
    /// <returns>The new point of the clipped line</returns>
    public static Vector2 intercept(Vector2 p1, Vector2 p2, int v2)
    {
        float slope = (p2.y - p1.y) / (p2.x - p1.x);

        if (v2 == 0)
        {
            return new Vector2(p1.x + (1 / slope) * (1 - p1.y), 1);
        }

        if (v2 == 1)
        {
            return new Vector2(p1.x + (1 / slope) * (1 - p1.y), -1);
        }

        if (v2 == 2)
        {
            return new Vector2(-1, p1.y + slope * (-1 - p1.x));
        }

        return new Vector2(1, p1.y + slope * (1 - p1.x));
    }

    public List<Vector2Int> breshenham(Vector2Int start, Vector2Int finish)
    {
        int dx = finish.x - start.x;

        if (dx < 0)
            return breshenham(finish, start);

        int dy = finish.y - start.y;

        if (dy < 0)// negative slope
            return negativeY(breshenham(negativeY(start), negativeY(finish)));

        if (dy > dx)// slope > 1
            return swapXY(breshenham(swapXY(start), swapXY(finish)));

        int a = 2 * dy;
        int b = 2 * (dy - dx);
        int p = 2 * dy - dx;

        List<Vector2Int> outputList = new List<Vector2Int>();

        int y = start.y;
        for (int x = start.x; x <= finish.x; x++)
        {
            outputList.Add(new Vector2Int(x, y));

            if (p > 0)
            {
                y++;
                p += b;
            }

            else
            {
                p += a;
            }
        }

        return outputList;
    }

    public List<Vector2Int> negativeY(List<Vector2Int> list)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();

        foreach (Vector2Int v in list)
        {
            outputList.Add(negativeY(v));
        }

        return outputList;
    }

    public Vector2Int negativeY(Vector2Int point)
    {
        return new Vector2Int(point.x, -point.y);
    }

    public List<Vector2Int> swapXY(List<Vector2Int> list)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();

        foreach (Vector2Int v in list)
        {
            outputList.Add(swapXY(v));
        }

        return outputList;
    }

    public Vector2Int swapXY(Vector2Int point)
    {
        return new Vector2Int(point.y, point.x);
    }

    private Vector3[] MatrixTransform(
       Vector3[] meshVertices,
       Matrix4x4 transformMatrix)
    {
        Vector3[] output = new Vector3[meshVertices.Length];
        for (int i = 0; i < meshVertices.Length; i++)
            output[i] = transformMatrix *
                new Vector4(
                meshVertices[i].x,
                meshVertices[i].y,
                meshVertices[i].z,
                    1);

        return output;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutcode : MonoBehaviour
{
    Texture2D texture;

    Vector3[] cube;

    float rotationAngle;
    Vector3 startingAxis;
    Quaternion rotation;
    Vector3 scale;
    Vector3 translate;

    Matrix4x4 viewingMatrix;
    Matrix4x4 projectionMatrix;

    Matrix4x4 rotationMatrix;
    Matrix4x4 scaleMatrix;
    Matrix4x4 translateMatrix;

    Matrix4x4 transformMatrix;
    Matrix4x4 BIGMATRIX;

    Vector3[] finalImage;
    Vector2[] finalPostDivisionImage;

    Vector2 start;
    Vector2 finish;
    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height);
        GetComponent<Renderer>().material.mainTexture = texture;

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
        Vector3 cameraPosition = new Vector3(0, 0, 30);
        Vector3 cameraLookAt = new Vector3(0, 0, 0);
        Vector3 cameraUp = new Vector3(0, 1 ,0);

        Vector3 lookRotationDir = cameraLookAt - cameraPosition;

        Quaternion camRotation = Quaternion.LookRotation(lookRotationDir.normalized, cameraUp.normalized);

        viewingMatrix = Matrix4x4.TRS(-cameraPosition, camRotation, Vector3.one);

        // projection matrix
        projectionMatrix = Matrix4x4.Perspective(45, Screen.width / Screen.height, 1, 1000);

        startingAxis = new Vector3(14, 3, 3);
        startingAxis.Normalize();

        rotationAngle = -22;

        scale = new Vector3(14, 3, 3);

        translate = new Vector3(5, -3, 4);

        drawCube();
    }

    // Update is called once per frame
    void Update()
    {
        translate += (Vector3.one) * Time.deltaTime;
        drawCube();
    }

    private void drawCube()
    {
        // rotation matrix
        

       

        rotation = Quaternion.AngleAxis(rotationAngle, startingAxis);
        rotationMatrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);

        // scale matrix
        

        scaleMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, scale);

        // translation matrix
       

        translateMatrix = Matrix4x4.TRS(translate, Quaternion.identity, Vector3.one);

        // transform matrix
        transformMatrix = translateMatrix * scaleMatrix * rotationMatrix;

        // super matrix
        BIGMATRIX = projectionMatrix * viewingMatrix * transformMatrix;

        finalImage = MatrixTransform(cube, BIGMATRIX);

        finalPostDivisionImage = dividebyz(finalImage);

        start = finalPostDivisionImage[0];
        finish = finalPostDivisionImage[1];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[1];
        finish = finalPostDivisionImage[2];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[2];
        finish = finalPostDivisionImage[3];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[3];
        finish = finalPostDivisionImage[0];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[1];
        finish = finalPostDivisionImage[5];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[0];
        finish = finalPostDivisionImage[4];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[2];
        finish = finalPostDivisionImage[6];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[3];
        finish = finalPostDivisionImage[7];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[5];
        finish = finalPostDivisionImage[4];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[4];
        finish = finalPostDivisionImage[7];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[7];
        finish = finalPostDivisionImage[6];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));

        start = finalPostDivisionImage[6];
        finish = finalPostDivisionImage[5];

        if (lineClip(ref start, ref finish))
            plot(breshenham(convertToScreenSpace(start), convertToScreenSpace(finish)));
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

        if(v2 == 0)
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
        for(int x = start.x; x <= finish.x; x++)
        {
            outputList.Add(new Vector2Int(x, y));

            if(p > 0)
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

    public List<Vector2Int> negativeY (List<Vector2Int> list)
    {
        List<Vector2Int> outputList = new List<Vector2Int>();

        foreach(Vector2Int v in list)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutcode : MonoBehaviour
{
    void Start()
    {
        Texture2D texture = new Texture2D(500, 500);
        GetComponent<Renderer>().material.mainTexture = texture;

        Vector2 point1 = new Vector2(.5f, 2f);
        Vector2 point2 = new Vector2(.3f, .1f);

        Outcode a = new Outcode(point1);
        Outcode b = new Outcode(point2);

        // all false
        Outcode inViewPort = new Outcode();

        if ((a == inViewPort) && (b == inViewPort))
        {
            Debug.Log("Trivially Accept");
        }

        if(a * b != inViewPort)
        {
            Debug.Log("Trivially Rejected");
        }

        if((a + b) == inViewPort)
        {
            Debug.Log("Trivially Accept");
        }

        if(!lineClip(ref point1,ref point2))
        {
            Debug.Log(point1.x + "  ,  " + point1.y);
            Debug.Log(point2.x + "  ,  " + point2.y);
        }


        Vector2Int pixelPoint1 = new Vector2Int(500, 0);
        Vector2Int pixelPoint2 = new Vector2Int(0, 500);
        List<Vector2Int> list = breshenham(pixelPoint1, pixelPoint2);

        //foreach(Vector2Int v in list)
        //    Debug.Log(v.x + "  ,  " + v.y);
        
        foreach(Vector2Int v in list)
        {
            Color color = Color.blue;
            texture.SetPixel(v.x, v.y, color);
        }

        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
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

            return false;
        }

        if (v0.down)
        {
            v = intercept(u, v, 1);

            return false;
        }

        if (v0.left)
        {
            v = intercept(u, v, 2);

            return false;
        }

        v = intercept(u, v, 3);
        return false;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOutcode : MonoBehaviour
{
    void Start()
    {
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


        Vector2 pixelPoint1 = new Vector2(12, 15);
        Vector2 pixelPoint2 = new Vector2(2, 10);
        List<Vector2> list = breshenham(pixelPoint1, pixelPoint2);

        //foreach(Vector2 v in list)
        //    Debug.Log(v.x + "  ,  " + v.y);
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

    public List<Vector2> breshenham(Vector2 start, Vector2 finish)
    {
        int dx = (int)(finish.x - start.x);

        if (dx < 0)
            return breshenham(finish, start);

        int dy = (int)(finish.y - start.y);

        if (dy < 0)// negative slope
            return negativeY(breshenham(start, negativeY(finish)));

        if (dx > dy)// slope > 1
            return swapXY(breshenham(swapXY(start), swapXY(finish)));

        int a = 2 * dy;
        int b = 2 * (dy - dx);
        int p = 2 * dy - dx;

        List<Vector2> outputList = new List<Vector2>();

        int y = (int)start.y;
        for(int x = (int)start.x; x <= (int)finish.x; x++)
        {
            outputList.Add(new Vector2(x, y));

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

    public List<Vector2> negativeY (List<Vector2> list)
    {
        List<Vector2> outputList = new List<Vector2>();

        foreach(Vector2 v in list)
        {
            list.Add(negativeY(v));
        }

        return outputList;
    }

    public Vector2 negativeY(Vector2 point)
    {
        return new Vector2(point.x, -point.y);
    }

    public List<Vector2> swapXY(List<Vector2> list)
    {
        List<Vector2> outputList = new List<Vector2>();

        foreach (Vector2 v in list)
        {
            list.Add(swapXY(v));
        }

        return outputList;
    }

    public Vector2 swapXY(Vector2 point)
    {
        return new Vector2(point.y, point.x);
    }
}

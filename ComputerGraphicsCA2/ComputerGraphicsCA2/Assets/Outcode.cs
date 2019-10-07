using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcode
{
    public bool up;
    public bool down;
    public bool left;
    public bool right;


    public Outcode(Vector2 point)
    {
        up = point.y > 1;
        down = point.y < -1;
        left = point.x < -1;
        right = point.x > 1;
    }

    public Outcode()
    {
        up = false;
        down = false;
        left = false;
        right = false;
    }

    public Outcode(bool Up, bool Down, bool Left, bool Right)
    {
        up = Up;
        down = Down;
        left = Left;
        right = Right;
    }

    /// <summary>
    /// Logical operator EQUAL
    /// </summary>
    public static bool operator ==(Outcode a, Outcode b)
    {
        return (a.up == b.up) && (a.down == b.down) && (a.left == b.left) && (a.right == b.right);
    }

    /// <summary>
    /// Logical operator NOT EQUAL
    /// </summary>
    public static bool operator !=(Outcode a, Outcode b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Logical operator OR
    /// </summary>
    public static Outcode operator +(Outcode a, Outcode b)
    {
        return new Outcode(a.up || b.up, a.down || b.down, a.left || b.left, a.right || b.right);
    }

    /// <summary>
    /// Logical operator AND
    /// </summary>
    public static Outcode operator *(Outcode a, Outcode b)
    {
        return new Outcode(a.up && b.up, a.down && b.down, a.left && b.left, a.right && b.right);
    }

    
}

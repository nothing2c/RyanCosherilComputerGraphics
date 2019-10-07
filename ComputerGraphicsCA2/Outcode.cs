using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outcode
{
    private bool up;
    private bool down;
    private bool left;
    private bool right;


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

  
    public static bool operator ==(Outcode a, Outcode b)
    {
        return (a.up == b.up) && (a.down == b.down) && (a.left == b.left) && (a.right == b.right);
    }

    public static bool operator !=(Outcode a, Outcode b)
    {
        return !(a == b);
    }

    public static Outcode operator +(Outcode a, Outcode b) // Or operator
    {
        return new Outcode(a.up || b.up, a.down || b.down, a.left || b.left, a.right || b.right);
    }

    public static Outcode operator *(Outcode a, Outcode b) // And operator
    {
        return new Outcode(a.up && b.up, a.down && b.down, a.left && b.left, a.right && b.right);
    }
}

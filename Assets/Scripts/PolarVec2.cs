using System;
using UnityEngine;

/**
 * Class to represnt a 2D polar vector.
 * 
 * Angle is in DEGREES!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
 * */
public struct PolarVec2
{
    public static PolarVec2 up = new PolarVec2(90, 1);
    public static PolarVec2 down = new PolarVec2(270, 1);
    public static PolarVec2 left = new PolarVec2(180, 1);
    public static PolarVec2 right = new PolarVec2(0, 1);

    public float A, r;

	//angle in radians
	public float Theta
	{
		get
		{
			return A * Mathf.Deg2Rad;
		}

		set
		{
			A = Mathf.Rad2Deg * value;
		}
	}

	//2d Cartesian vector
	public Vector2 Cartesian2D
	{
		get
		{
			return new Vector2(Mathf.Cos(Theta) * r, Mathf.Sin(Theta) * r);
		}
	}

	//3d vector with the y axis as the axis of rotation
	// (the vector will be on the XZ plane)
	public Vector3 Cartesian3DHorizontal
	{
		get
		{
			return new Vector3(Mathf.Cos(Theta) * r, 0, Mathf.Sin(Theta) * r);
		}
	}

	//return a new vector with 1 radius
	public PolarVec2 normalized
	{
		get
		{
			return new PolarVec2(this.A, 1);
		}
	}


	public PolarVec2 (float A, float r)
	{
		this.A = A;
		this.r = r;
	}


	public static PolarVec2 FromCartesian(float x, float y)
	{
		PolarVec2 vector = new PolarVec2();

		vector.r = Mathf.Sqrt(x*x + y*y);
		vector.Theta = Mathf.Atan2(y, x);

		return vector;
	}

	public static PolarVec2 FromCartesian(Vector2 cartesian)
	{
		return FromCartesian(cartesian.x, cartesian.y);
	}

    public override string ToString()
    {
        return "PolarVec2: A=" + A + "deg, r=" + r;
    }

    public static PolarVec2 operator+(PolarVec2 a, PolarVec2 b)
    {

        PolarVec2 result = new PolarVec2();

		float angleDRad = (180 - a.A + b.A) * Mathf.Deg2Rad;

        //law of cosines
		result.r = Mathf.Sqrt(Mathf.Pow(a.r, 2) + Mathf.Pow(b.r, 2) - 2 * a.r * b.r * Mathf.Cos(angleDRad));

		result.A = a.A - Mathf.Asin(b.r * Mathf.Sin(angleDRad) / result.r) * Mathf.Rad2Deg;        
		
		//Debug.Log("A: " + result.A + " EA: " + (a.A + b.A) + "a.r: " + a.r + " b.r: " + b.r + " r: " + result.r);

        return result;
    }

    public static PolarVec2 operator-(PolarVec2 a, PolarVec2 b)
    {
        if(b.r == 0)
        {
            return a;
        }

        PolarVec2 result = new PolarVec2();

        //law of sines
        result.A = 90 - Mathf.Rad2Deg * Mathf.Asin((a.r / b.r) * Mathf.Sin(a.Theta - b.Theta)) + b.A;

        //law of cosines
		result.r = Mathf.Sqrt(Mathf.Pow(a.r, 2) + Mathf.Pow(b.r, 2) - 2 * a.r * b.r * Mathf.Cos(a.Theta - b.Theta)) * Math.Sign(a.A - b.A);

		//fix corner case with very small differences
		if(float.IsNaN(result.r))
		{
			result.r = 0;
		}
		
        return result;
    }

    // Multiply radius of vector by a scalar.
    public static PolarVec2 operator*(PolarVec2 vec, float number)
    {
        vec.r *= number;
        return vec;
    }

    // Divide radius of vector by a scalar.
    public static PolarVec2 operator/(PolarVec2 vec, float number)
    {
        vec.r /= number;
        return vec;
    }

    /*
     Convert the rotation to a euler (pronounced oiler) angle of rotation around the supplied axis.
    */
    public Vector3 ToOiler(Axis rotationAxis)
    {
        switch(rotationAxis)
        {
            case Axis.X:
                return new Vector3(A, 0, 0);
            case Axis.Y:
                return new Vector3(0, A, 0);
            default:
                return new Vector3(0, 0, A);
        } 
    }
}



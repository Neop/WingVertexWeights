using System;

namespace BatWings
{
 
    /**
     * Just a basic vector implementation
     */
    public class Vector
    {
        public double x, y;

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector operator- (Vector a, Vector b)
        {
            return new Vector(a.x - b.x, a.y - b.y);
        }

        public static Vector operator/(Vector a, double b)
        {
            return new Vector(a.x / b, a.y / b);
        }

        public double Magnitude()
        {
            return Math.Sqrt(x * x + y * y);
        }

        public Vector Norm()
        {
            double mag = Magnitude();
            return new Vector(x / mag, y / mag);
        }

        public static double Dot(Vector a, Vector b)
        {
            return a.x * b.x + a.y * b.y;
        }
    }

    /*
     * Bone with distance method
     */
    public class Bone
    {
        public string Name;
        public Vector A, B;

        public Bone(string name, Vector a, Vector b)
        {
            Name = name;
            A = a;
            B = b;
        }

        public double Dist(Vector x)
        {
            double d_ab = (A - B).Magnitude();
            double d_ax = (A - x).Magnitude();
            double d_bx = (B - x).Magnitude();

            double dist;
            if(Vector.Dot(A-B, x-B) * Vector.Dot(B-A, x-A) >= 0)
            {
                double det = A.x * B.y + B.x * x.y + x.x * A.y - x.x * B.y - B.x * A.y - A.x * x.y;
                dist = Math.Abs(det) / d_ab;
            }
            else
            {
                dist = Math.Min(d_ax, d_bx);
            }
            return dist;
        }
    }

}

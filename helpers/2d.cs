namespace AoC2022;

public class Point2D : IEquatable<Point2D>
{
    public int x { get; init; }
    public int y { get; init; }

    public Point2D(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(Point2D? other)
    {
        return other is not null && x == other.x && y == other.y;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Point2D other) return Equals(other);
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Tuple.Create(x, y).GetHashCode();
    }

    public double tan()
    {
        return (double)y / (double)x;
    }

    public static bool operator ==(Point2D a, Point2D b)
    {
        return a?.Equals(b) ?? b is null;
    }
    public static bool operator !=(Point2D? a, Point2D? b)
    {
        return !(a?.Equals(b) ?? b is null);
    }

    public static Point2D operator -(Point2D a, Point2D b)
    {
        return new Point2D(a.x - b.x, a.y - b.y);
    }
    public static Point2D operator +(Point2D a, Point2D b)
    {
        return new Point2D(a.x + b.x, a.y + b.y);
    }

    public static Point2D operator *(Point2D a, int b)
    {
        return new Point2D(a.x * b, a.y * b);
    }
    public static Point2D operator *(int b, Point2D a)
    {
        return new Point2D(a.x * b, a.y * b);
    }

    public static Point2D Min(Point2D a, Point2D b)
    {
        return new Point2D(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
    }

    public static Point2D Max(Point2D a, Point2D b)
    {
        return new Point2D(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
    }
    public static Point2D Sign(Point2D a)
    {
        return new Point2D(Math.Sign(a.x), Math.Sign(a.y));
    }

    public int Manhattan(Point2D? other = null)
    {
        other ??= zero;
        return Math.Abs(x - other.x) + Math.Abs(y - other.y);
    }

    public override string ToString()
    {
        return String.Format("Point2D({0},{1})", x, y);
    }


    public static readonly Point2D zero = new Point2D(0, 0);

    public static readonly Dictionary<Facing2D, Point2D> FacingToPointVector = new Dictionary<Facing2D, Point2D>
    {
        { Facing2D.Up, new Point2D(0, -1) },
        { Facing2D.Right, new Point2D(1, 0) },
        { Facing2D.Down, new Point2D(0, 1) },
        { Facing2D.Left, new Point2D(-1, 0) },
    };

    public static Point2D Facing(Facing2D F) => FacingToPointVector[F];
}

public enum Facing2D
{
    Up, Right, Down, Left
};


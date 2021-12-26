namespace AoC2019;

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
}
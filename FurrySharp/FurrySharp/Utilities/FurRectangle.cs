using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;

namespace FurrySharp.Utilities;

[DataContract]
[DebuggerDisplay("{DebugDisplayString,nq}")]
public struct FurRectangle : IEquatable<FurRectangle>
{
    private const float TOLERANCE = 0.01f;
    private static FurRectangle emptyRectangle;

    [DataMember]
    public float X;

    [DataMember]
    public float Y;

    [DataMember]
    public float Width;

    [DataMember]
    public float Height;

    public static FurRectangle Empty => FurRectangle.emptyRectangle;

    public float Left => this.X;

    public float Right => this.X + this.Width;

    public float Top => this.Y;

    public float Bottom => this.Y + this.Height;

    public bool IsEmpty => this.Width == 0 && this.Height == 0 && this.X == 0 && this.Y == 0;

    public Vector2 Location
    {
        get => new Vector2(this.X, this.Y);
        set
        {
            this.X = value.X;
            this.Y = value.Y;
        }
    }

    public Vector2 Size
    {
        get => new Vector2(this.Width, this.Height);
        set
        {
            this.Width = value.X;
            this.Height = value.Y;
        }
    }

    public Vector2 Center => new Vector2(this.X + this.Width / 2f, this.Y + this.Height / 2f);

    internal string DebugDisplayString => this.X + "  " + this.Y + "  " + this.Width + "  " + this.Height;

    public FurRectangle(float x, float y, float width, float height)
    {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
    }

    public FurRectangle(Vector2 location, Vector2 size)
    {
        this.X = location.X;
        this.Y = location.Y;
        this.Width = size.X;
        this.Height = size.Y;
    }

    public static bool operator ==(FurRectangle a, FurRectangle b) => Math.Abs(a.X - b.X) < TOLERANCE && Math.Abs(a.Y - b.Y) < TOLERANCE && Math.Abs(a.Width - b.Width) < TOLERANCE && Math.Abs(a.Height - b.Height) < TOLERANCE;

    public static bool operator !=(FurRectangle a, FurRectangle b) => !(a == b);

    // public bool Contains(int x, int y) => this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;

    // public bool Contains(float x, float y) => (double)this.X <= (double)x && (double)x < (double)(this.X + this.Width) && (double)this.Y <= (double)y && (double)y < (double)(this.Y + this.Height);

    // public bool Contains(Point value) => this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height;

    // public void Contains(ref Point value, out bool result) => result = this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height;

    // public bool Contains(Vector2 value) => (double)this.X <= (double)value.X && (double)value.X < (double)(this.X + this.Width) && (double)this.Y <= (double)value.Y && (double)value.Y < (double)(this.Y + this.Height);

    // public void Contains(ref Vector2 value, out bool result) => result = (double)this.X <= (double)value.X && (double)value.X < (double)(this.X + this.Width) && (double)this.Y <= (double)value.Y && (double)value.Y < (double)(this.Y + this.Height);

    // public bool Contains(Rectangle value) => this.X <= value.X && value.X + value.Width <= this.X + this.Width && this.Y <= value.Y && value.Y + value.Height <= this.Y + this.Height;

    // public void Contains(ref Rectangle value, out bool result) => result = this.X <= value.X && value.X + value.Width <= this.X + this.Width && this.Y <= value.Y && value.Y + value.Height <= this.Y + this.Height;

    public override bool Equals(object obj) => obj is FurRectangle rectangle && this == rectangle;

    public bool Equals(FurRectangle other) => this == other;

    public override int GetHashCode() => (((17 * 23 + this.X.GetHashCode()) * 23 + this.Y.GetHashCode()) * 23 + this.Width.GetHashCode()) * 23 + this.Height.GetHashCode();

    public void Inflate(int horizontalAmount, int verticalAmount)
    {
        this.X -= horizontalAmount;
        this.Y -= verticalAmount;
        this.Width += horizontalAmount * 2;
        this.Height += verticalAmount * 2;
    }

    public void Inflate(float horizontalAmount, float verticalAmount)
    {
        this.X -= (int)horizontalAmount;
        this.Y -= (int)verticalAmount;
        this.Width += (int)horizontalAmount * 2;
        this.Height += (int)verticalAmount * 2;
    }

    public bool Intersects(FurRectangle value, float epsilon = 0.01f)
    {
        var b1 = value.Left + epsilon < this.Right - epsilon;
        var b2 = this.Left + epsilon < value.Right - epsilon;
        var b3 = value.Top + epsilon < this.Bottom - epsilon;
        var b4 = this.Top + epsilon < value.Bottom - epsilon;
        return b1 && b2 && b3 && b4;
    }

    public void Intersects(ref FurRectangle value, out bool result) => result = value.Left < this.Right && this.Left < value.Right && value.Top < this.Bottom && this.Top < value.Bottom;

    public static FurRectangle Intersect(FurRectangle value1, FurRectangle value2)
    {
        FurRectangle result;
        FurRectangle.Intersect(ref value1, ref value2, out result);
        return result;
    }

    public static void Intersect(ref FurRectangle value1, ref FurRectangle value2, out FurRectangle result)
    {
        if (value1.Intersects(value2))
        {
            float num1 = Math.Min(value1.X + value1.Width, value2.X + value2.Width);
            float x = Math.Max(value1.X, value2.X);
            float y = Math.Max(value1.Y, value2.Y);
            float num2 = Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);
            result = new FurRectangle(x, y, num1 - x, num2 - y);
        }
        else
            result = new FurRectangle(0, 0, 0, 0);
    }

    public void Offset(int offsetX, int offsetY)
    {
        this.X += offsetX;
        this.Y += offsetY;
    }

    public void Offset(float offsetX, float offsetY)
    {
        this.X += offsetX;
        this.Y += offsetY;
    }

    public void Offset(Point amount)
    {
        this.X += amount.X;
        this.Y += amount.Y;
    }

    public void Offset(Vector2 amount)
    {
        this.X += amount.X;
        this.Y += amount.Y;
    }

    public override string ToString() => "{X:" + this.X.ToString() + " Y:" + this.Y.ToString() + " Width:" + this.Width.ToString() + " Height:" + this.Height.ToString() + "}";

    public static FurRectangle Union(FurRectangle value1, FurRectangle value2)
    {
        float x = Math.Min(value1.X, value2.X);
        float y = Math.Min(value1.Y, value2.Y);
        return new FurRectangle(x, y, Math.Max(value1.Right, value2.Right) - x, Math.Max(value1.Bottom, value2.Bottom) - y);
    }

    public static void Union(ref FurRectangle value1, ref FurRectangle value2, out FurRectangle result)
    {
        result.X = Math.Min(value1.X, value2.X);
        result.Y = Math.Min(value1.Y, value2.Y);
        result.Width = Math.Max(value1.Right, value2.Right) - result.X;
        result.Height = Math.Max(value1.Bottom, value2.Bottom) - result.Y;
    }

    public void Deconstruct(out float x, out float y, out float width, out float height)
    {
        x = this.X;
        y = this.Y;
        width = this.Width;
        height = this.Height;
    }
}
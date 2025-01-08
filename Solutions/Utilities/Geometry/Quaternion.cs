using System;

namespace AoC.Utilities.Geometry;

/// <summary>
///     A simple readonly quaternion value type used for transforming <see cref="Vec2D" /> and <see cref="Vec3D" />
///     instances.
/// </summary>
/// <param name="W">The scalar part</param>
/// <param name="X">The <b>i</b> component of the vector part</param>
/// <param name="Y">The <b>j</b> component of the vector part</param>
/// <param name="Z">The <b>k</b> component of the vector part</param>
public readonly record struct Quaternion(double W, double X, double Y, double Z)
{
    public enum Axis
    {
        X,
        Y,
        Z,
        W
    }

    private const int N90 = -90, P90 = 90, P180 = 180;

    public static readonly Quaternion Identity = new(1, 0, 0, 0);

    public static readonly Quaternion N90X = FromAxisAngle(Axis.X, N90);
    public static readonly Quaternion P90X = FromAxisAngle(Axis.X, P90);
    public static readonly Quaternion P180X = FromAxisAngle(Axis.X, P180);

    public static readonly Quaternion N90Y = FromAxisAngle(Axis.Y, N90);
    public static readonly Quaternion P90Y = FromAxisAngle(Axis.Y, P90);
    public static readonly Quaternion P180Y = FromAxisAngle(Axis.Y, P180);

    /// <summary>When used on a <see cref="Vec2D" />, -90° is a clockwise rotation (i.e. turn right)</summary>
    public static readonly Quaternion N90Z = FromAxisAngle(Axis.Z, N90);

    /// <summary>When used on a <see cref="Vec2D" />, +90° is a counter-clockwise rotation (i.e. turn left)</summary>
    public static readonly Quaternion P90Z = FromAxisAngle(Axis.Z, P90);

    public static readonly Quaternion P180Z = FromAxisAngle(Axis.Z, P180);

    private Quaternion(Vec3D vec) : this(0, vec.X, vec.Y, vec.Z)
    {
    }

    private Quaternion(Vec2D vec) : this(0, vec.X, vec.Y, 0)
    {
    }

    private Quaternion(Vec2DLong vec) : this(0, vec.X, vec.Y, 0)
    {
    }

    /// <summary>
    ///     Build a <see cref="Quaternion" /> representing a 3D rotation using the specified <see cref="Axis" />
    ///     and amount.
    /// </summary>
    /// <param name="axis">The axis of rotation</param>
    /// <param name="angleDeg">The angle of rotation, in degrees</param>
    /// <returns>A <see cref="Quaternion" /> representing the rotation</returns>
    /// <exception cref="ArgumentOutOfRangeException">An invalid <see cref="Axis" /> was specified</exception>
    public static Quaternion FromAxisAngle(Axis axis, int angleDeg) =>
        FromAxisAngle(axis, angleDeg * double.Pi / P180);

    public static Quaternion FromAxisAngle(Axis axis, double angleRad) => axis switch
    {
        Axis.X => FromAxisAngle(1, 0, 0, angleRad),
        Axis.Y => FromAxisAngle(0, 1, 0, angleRad),
        Axis.Z => FromAxisAngle(0, 0, 1, angleRad),
        _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, $"Invalid axis [{axis}]")
    };

    public static Quaternion FromAxisAngle(double x, double y, double z, int angleDeg) =>
        FromAxisAngle(x, y, z, angleDeg * double.Pi / P180);

    public static Quaternion FromAxisAngle(double x, double y, double z, double angleRad)
    {
        var norm = Math.Sqrt(x * x + y * y + z * z);
        if (norm == 0) throw new ArithmeticException($"Axis cannot be zero. Provided axis: <{x}, {y}, {z}>");
        var halfAngle = angleRad / 2;
        var downscale = Math.Sin(halfAngle) / norm;
        return new Quaternion(Math.Cos(halfAngle), x * downscale, y * downscale, z * downscale);
    }

    /// <summary>
    ///     Negates the vector part of a quaternion while keeping the scalar part the same. For a normalized
    ///     quaternion, this is the same as the Inverse.
    /// </summary>
    /// <returns>The conjugated quaternion</returns>
    public Quaternion Conjugate() => new(W, -X, -Y, -Z);

    /// <summary>
    ///     Negates the vector part of a quaternion and ensure it's normalized. For a quaternion that is already
    ///     normalized, this is the same as the Conjugate.
    /// </summary>
    /// <returns>The inverse of the quaternion</returns>
    public Quaternion Inverse() => Normalize().Conjugate();

    /// <summary>
    ///     Create a quaternion with the same orientation but with a magnitude of 1.
    /// </summary>
    /// <returns>A normalized quaternion</returns>
    public Quaternion Normalize()
    {
        var w = Math.Clamp(W, -1, 1);
        var scaleFactor = Math.Sqrt(1 - w * w) / Math.Sqrt(X * X + Y * Y + Z * Z);
        return new Quaternion(w, X * scaleFactor, Y * scaleFactor, Z * scaleFactor);
    }

    /// <summary>
    ///     Converts a rotation to angle-axis representation (angles in degrees).
    /// </summary>
    public void ToAngleAxis(out double angle, out double x, out double y, out double z)
    {
        angle = Math.Acos(W) * 360 / double.Pi;
        var norm = Math.Sqrt(X * X + Y * Y + Z * Z);
        x = X / norm;
        y = Y / norm;
        z = Z / norm;
    }

    public static Quaternion operator *(Quaternion a, Quaternion b) => new(
        a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z,
        a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y,
        a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X,
        a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W
    );

    public static Vec2D operator *(Quaternion a, Vec2D v)
    {
        var result = a * new Quaternion(v) * a.Conjugate();
        return new Vec2D((int)Math.Round(result.X), (int)Math.Round(result.Y));
    }

    public static Vec2DLong operator *(Quaternion a, Vec2DLong v)
    {
        var result = a * new Quaternion(v) * a.Conjugate();
        return new Vec2DLong((int)Math.Round(result.X), (int)Math.Round(result.Y));
    }

    public static Vec3D operator *(Quaternion a, Vec3D v)
    {
        var result = a * new Quaternion(v) * a.Conjugate();
        return new Vec3D((int)Math.Round(result.X), (int)Math.Round(result.Y), (int)Math.Round(result.Z));
    }

    public override string ToString() => $"{W} + <{X}i,{Y}j,{Z}k>";
}
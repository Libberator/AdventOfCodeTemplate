using System;
using AoC.Utilities.Extensions;

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
    public static readonly Quaternion Identity = new(1, 0, 0, 0);
    public static readonly Quaternion N90X = FromAxisAngle(Axis.X, Angle.N90);
    public static readonly Quaternion P90X = FromAxisAngle(Axis.X, Angle.P90);
    public static readonly Quaternion P180X = FromAxisAngle(Axis.X, Angle.P180);
    public static readonly Quaternion N90Y = FromAxisAngle(Axis.Y, Angle.N90);
    public static readonly Quaternion P90Y = FromAxisAngle(Axis.Y, Angle.P90);
    public static readonly Quaternion P180Y = FromAxisAngle(Axis.Y, Angle.P180);
    public static readonly Quaternion N90Z = FromAxisAngle(Axis.Z, Angle.N90);
    public static readonly Quaternion P90Z = FromAxisAngle(Axis.Z, Angle.P90);
    public static readonly Quaternion P180Z = FromAxisAngle(Axis.Z, Angle.P180);

    private Quaternion(Vec2D vec) : this(0, vec.X, vec.Y, 0)
    {
    }

    private Quaternion(Vec2DLong vec) : this(0, vec.X, vec.Y, 0)
    {
    }

    private Quaternion(Vec3D vec) : this(0, vec.X, vec.Y, vec.Z)
    {
    }

    private Quaternion(Vec3DLong vec) : this(0, vec.X, vec.Y, vec.Z)
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
        FromAxisAngle(axis, angleDeg * Angle.Deg2Rad);

    public static Quaternion FromAxisAngle(Axis axis, double angleRad) => axis switch
    {
        Axis.X => FromAxisAngle(1, 0, 0, angleRad),
        Axis.Y => FromAxisAngle(0, 1, 0, angleRad),
        Axis.Z => FromAxisAngle(0, 0, 1, angleRad),
        _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, $"Invalid axis [{axis}]")
    };

    public static Quaternion FromAxisAngle(Vec3D axis, int angleDeg) => FromAxisAngle(axis.X, axis.Y, axis.Z, angleDeg);

    public static Quaternion FromAxisAngle(Vec3DLong axis, int angleDeg) =>
        FromAxisAngle(axis.X, axis.Y, axis.Z, angleDeg);

    public static Quaternion FromAxisAngle(double x, double y, double z, int angleDeg) =>
        FromAxisAngle(x, y, z, angleDeg * Angle.Deg2Rad);

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
        var norm = Math.Sqrt(X * X + Y * Y + Z * Z).RemoveError();
        if (norm == 0) return new Quaternion(W < 0 ? -1 : 1, 0, 0, 0);
        var w = Math.Clamp(W, -1, 1).RemoveError();
        var scaleFactor = Math.Sqrt(1 - w * w) / norm;
        return new Quaternion(w,
            (X * scaleFactor).RemoveError(),
            (Y * scaleFactor).RemoveError(),
            (Z * scaleFactor).RemoveError());
    }

    /// <summary>
    ///     Converts a rotation to angle-axis representation (angles in degrees).
    /// </summary>
    public void ToAngleAxis(out double angle, out double x, out double y, out double z)
    {
        angle = 2 * Math.Acos(W) * Angle.Rad2Deg;
        var norm = Math.Sqrt(X * X + Y * Y + Z * Z).RemoveError();
        x = norm == 0 ? 0 : X / norm;
        y = norm == 0 ? 0 : Y / norm;
        z = norm == 0 ? 0 : Z / norm;
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
        return new Vec2DLong((long)Math.Round(result.X), (long)Math.Round(result.Y));
    }

    public static Vec3D operator *(Quaternion a, Vec3D v)
    {
        var result = a * new Quaternion(v) * a.Conjugate();
        return new Vec3D((int)Math.Round(result.X), (int)Math.Round(result.Y), (int)Math.Round(result.Z));
    }

    public static Vec3DLong operator *(Quaternion a, Vec3DLong v)
    {
        var result = a * new Quaternion(v) * a.Conjugate();
        return new Vec3DLong((long)Math.Round(result.X), (long)Math.Round(result.Y), (long)Math.Round(result.Z));
    }

    public override string ToString() => $"{W:g2} + <{X:g2}i,{Y:g2}j,{Z:g2}k>";
}
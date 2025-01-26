namespace AoC.Utilities.Geometry;

public static class Angle
{
    public const int N270 = -270;
    public const int N225 = -225;
    public const int N180 = -180;
    public const int N135 = -135;
    public const int N90 = -90;
    public const int N45 = -45;
    public const int Zero = 0;
    public const int P45 = 45;
    public const int P90 = 90;
    public const int P135 = 135;
    public const int P180 = 180;
    public const int P225 = 225;
    public const int P270 = 270;

    public const double Deg2Rad = double.Pi / 180;
    public const double Rad2Deg = 180 / double.Pi;

    /// <summary>Returns the four cardinal angles: 0, -90, +90, +180.</summary>
    public static readonly int[] Cardinals = [Zero, N90, P90, P180];

    /// <summary>Returns the four ordinal angles: -45, +45, -135, +135.</summary>
    public static readonly int[] Ordinals = [N45, P45, N135, P135];
}
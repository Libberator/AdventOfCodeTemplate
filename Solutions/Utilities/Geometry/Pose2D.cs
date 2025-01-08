namespace AoC.Utilities.Geometry;

/// <summary>
///     A readonly value type representing a 2D pose (Position and Direction vectors).
/// </summary>
public readonly record struct Pose2D(Vec2D Pos, Vec2D Dir)
{
    public Vec2D Ahead => Pos + Dir;
    public Vec2D Behind => Pos - Dir;
    public Vec2D Right => Pos + Dir.RotatedRight();
    public Vec2D Left => Pos + Dir.RotatedLeft();

    public Pose2D Step(int amount = 1) => this with { Pos = Pos + amount * Dir };
    public Pose2D TurnRight() => this with { Dir = Dir.RotatedRight() };
    public Pose2D TurnLeft() => this with { Dir = Dir.RotatedLeft() };
}
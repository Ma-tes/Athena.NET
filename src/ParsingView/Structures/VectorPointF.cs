namespace Athena.NET.ParsingView.Structures;
public struct VectorPointF
{
    public float X { get; internal set; }
    public float Y { get; internal set; }

    public static VectorPointF Zero { get; } = new(0, 0);

    public VectorPointF(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static VectorPointF operator +(VectorPointF firstVector, VectorPointF secondVector) =>
        new(firstVector.X + secondVector.X, firstVector.Y + secondVector.Y);
    public static VectorPointF operator -(VectorPointF firstVector, VectorPointF secondVector) =>
        new(firstVector.X - secondVector.X, firstVector.Y - secondVector.Y);
}

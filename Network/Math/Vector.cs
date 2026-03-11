using System.Collections;

record Vector(float[] Values) : IEnumerable<float>
{
    private static readonly Random random = new((int)DateTimeOffset.Now.ToUnixTimeSeconds());

    public static Vector operator +(Vector left, Vector right) => new([.. left.Zip(right, (x, y) => x + y)]);

    public static Vector operator -(Vector left, Vector right) => new([.. left.Zip(right, (x, y) => x - y)]);

    public static Vector operator *(Vector vector, float scalar) => new([..vector.Select(x => x * scalar)]);
    public static Vector Random(int size)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
        {
            w[i] = random.NextSingle() * 2 - 1;
        }

        return new Vector(w);
    }

    public Vector ElementProduct(Vector other) => new([.. this.Zip(other, (x, y) => x * y)]);
    public Matrix CrossProduct(Vector other)
    {
        var matrix = new float[other.Values.Length][];
        for (int i = 0; i < matrix.Length;i ++)
        {
            matrix[i] = (this * other.Values[i]).Values;
        }
        return new Matrix(matrix);
    }

    public IEnumerator<float> GetEnumerator() => ((IEnumerable<float>)Values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();
}
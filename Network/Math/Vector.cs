using System.Collections;

record Vector(float[] Values) : IEnumerable<float>
{
    private static readonly Random random = new((int)DateTimeOffset.Now.ToUnixTimeSeconds());

    public static Vector operator +(Vector left, Vector right) => new([.. left.Zip(right, (x, y) => x + y)]);

    public static Vector operator -(Vector left, Vector right) => new([.. left.Zip(right, (x, y) => x - y)]);

    public static Vector operator *(Vector left, Vector right) => new([.. left.Zip(right, (x, y) => x * y)]);

    public static Vector Random(int size)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
        {
            w[i] = random.NextSingle() * 2 - 1;
        }

        return new Vector(w);
    }

    public IEnumerator<float> GetEnumerator() => ((IEnumerable<float>)Values).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Values.GetEnumerator();
}
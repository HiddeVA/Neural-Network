record Matrix(float[][] Values)
{
    public Matrix Transpose()
    {
        var size = Values[0].Length;
        var transposed = new float[size][];
        for (int i = 0; i < size; i++)
        {
            transposed[i] = new float[Values.Length];
            for (int j = 0; j < Values.Length; j++)
            {
                transposed[i][j] = Values[j][i];
            }
        }
        return new Matrix(transposed);
    }

    public static Vector operator*(Matrix matrix, Vector vector)
    {
        return new Vector([.. matrix.Values.Select((w, i) => vector.Zip(w, (x, y) => x * y).Sum())]);
    }

    public static Matrix Random(int sizeInput, int sizeOutput)
    {
        var matrix = new float[sizeOutput][];
        for (int i = 0; i < sizeOutput; i++)
        {
            matrix[i] = Vector.Random(sizeInput).Values;
        }

        return new Matrix(matrix);
    }
}

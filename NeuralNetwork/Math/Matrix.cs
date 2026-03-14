namespace KnowledgeNight.NeuralNetwork.Math;

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

    public static Matrix operator+(Matrix left, Matrix right)
    {
        var sum = new float[left.Values.Length][];
        for (int i = 0; i < left.Values.Length; i++)
        {
            sum[i] = new float[left.Values[i].Length];
            for (int j = 0; j < left.Values[i].Length; j++)
            {
                sum[i][j] = left.Values[i][j] + right.Values[i][j];
            }
        }
        return new Matrix(sum);
    }

    public static Matrix operator*(Matrix matrix, float scalar)
    {
        var scaled = new float[matrix.Values.Length][];
        for (int i = 0; i < matrix.Values.Length; i++)
        {
            scaled[i] = new float[matrix.Values[i].Length];
            for (int j = 0; j < matrix.Values[i].Length; j++)
            {
                scaled[i][j] = matrix.Values[i][j] * scalar;
            }
        }
        return new Matrix(scaled);
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

    public static Matrix Average(IEnumerable<Matrix> matrices)
    {
        var first = matrices.First().Values;
        var average = new float[first.Length][];
        for (int i = 0; i < first.Length; i++)
        {
            average[i] = new float[first[i].Length];
            for (int j = 0; j < first[i].Length; j++)
            {
                float sum = 0;
                foreach (var m in matrices)
                {
                    sum += m.Values[i][j];
                }
                average[i][j] = sum / matrices.Count();
            }
        }

        return new Matrix(average);
    }
}

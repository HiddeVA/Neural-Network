class NeuralNetwork
{
    private static readonly Random random = new((int)DateTimeOffset.Now.ToUnixTimeSeconds());

    public static NeuralNetwork CreateRandom(int[] layerSizes)
    {        
        var weights = new List<float[][]>(capacity: layerSizes.Length - 1);
        var biases = new List<float[]>(capacity: layerSizes.Length - 1);
        int sizeInput = layerSizes[0];
        foreach (var sizeOutput in layerSizes.Skip(1))
        {
            weights.Add(GenerateRandomMatrix(sizeInput, sizeOutput));
            biases.Add(GenerateRandomVector(sizeOutput));
            sizeInput = sizeOutput;
        }

        return new NeuralNetwork(weights, biases);
    }

    private static float[][] GenerateRandomMatrix(int sizeInput, int sizeOutput)
    {
        var matrix = new float[sizeOutput][];
        for (int i = 0; i < sizeOutput; i++)
        {
            matrix[i] = GenerateRandomVector(sizeInput);
        }

        return matrix;
    }

    private static float[] GenerateRandomVector(int size)
    {
        var w = new float[size];
        for (int i = 0; i < size; i++)
        {
            w[i] = random.NextSingle() * 2 - 1;
        }

        return w;
    }

    private int numberOfLayers => _weights.Count;
    private readonly List<float[][]> _weights;
    private readonly List<float[]> _biases;
    public NeuralNetwork(List<float[][]> weights, List<float[]> biases)
    {
        _weights = weights;
        _biases = biases;
    }

    public int Classify(float[] input)
    {
        return FeedForward(input).Index().MaxBy(x => x.Item).Index;
    }

    public float[] FeedForward(float[] input)
    {
        var vector = input;
        for (int i = 0; i < numberOfLayers; i++)
        {
            vector = Activations(vector, _weights[i], _biases[i]);
        }
        return vector;
    }

    public float Cost(float[] input, float[] expected)
    {
        var output = FeedForward(input);
        return output.Zip(expected).Aggregate(0f, (cost, pair) =>
        {
            var diff = pair.First - pair.Second;
            return cost + diff * diff;
        });
    }

    private static float[] Activations(float[] vector, float[][] weights, float[] bias)
    {
        var weighted = weights.Select((w, i) => vector.Zip(w, (x, y) => x * y).Sum() + bias[i]);
        return [.. weighted.Select(Sigmoid)];
    }

    private static float Sigmoid(float input) => 1 / (1 + MathF.Exp(-input));

    private static float SigmoidDeriv(float input) => Sigmoid(input) * (1 - Sigmoid(input));
}
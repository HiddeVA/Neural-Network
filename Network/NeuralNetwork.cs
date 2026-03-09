class NeuralNetwork
{    
    public static NeuralNetwork CreateRandom(int[] layerSizes)
    {        
        var weights = new List<Matrix>(capacity: layerSizes.Length - 1);
        var biases = new List<Vector>(capacity: layerSizes.Length - 1);
        int sizeInput = layerSizes[0];
        foreach (var sizeOutput in layerSizes.Skip(1))
        {
            weights.Add(Matrix.Random(sizeInput, sizeOutput));
            biases.Add(Vector.Random(sizeOutput));
            sizeInput = sizeOutput;
        }

        return new NeuralNetwork(weights, biases);
    }

    private int NumberOfLayers => _weights.Count;
    private readonly List<Matrix> _weights;
    private readonly List<Vector> _biases;
    public NeuralNetwork(List<Matrix> weights, List<Vector> biases)
    {
        _weights = weights;
        _biases = biases;
    }

    private void BackPropagate(Vector input, Vector desired)
    {
        // Save Z values for each layer
        var zValues = new List<Vector>();
        var activations = input;
        for (int layer = 0; layer < NumberOfLayers; layer++)
        {
            var weights = _weights[layer];
            var biases = _biases[layer];
            var weighted = weights * activations + biases;
            zValues.Add(weighted); // Save for back propagation
            activations = Sigmoid(weighted);
        }

        var cost = activations.Zip(desired, (x, y) => (x - y) * (x - y)).Sum() / 2;

        // And now........back
        // TODO: Convert to loop format
        var errorsOutputLayer = (activations - desired) * SigmoidDeriv(zValues[1]);
        var errorsMiddleLayer = _weights[1].Transpose() * errorsOutputLayer * SigmoidDeriv(zValues[0]);
    }

    private static Vector Sigmoid(Vector input) => new([..input.Select(Sigmoid)]);
    private static float Sigmoid(float input) => 1 / (1 + MathF.Exp(-input));
    private static Vector SigmoidDeriv(Vector input) => new([..input.Select(SigmoidDeriv)]);
    private static float SigmoidDeriv(float input) => Sigmoid(input) * (1 - Sigmoid(input));
}
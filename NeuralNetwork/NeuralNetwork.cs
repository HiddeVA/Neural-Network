using KnowledgeNight.NeuralNetwork.Math;

namespace KnowledgeNight.NeuralNetwork;

public class NeuralNetwork
{    
    public const float LearningRate = 0.15f;
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

    public Vector FeedForward(Vector input)
    {        
        var activation = input;
        for (int layer = 0; layer < NumberOfLayers; layer++)
        {
            var weights = _weights[layer];
            var biases = _biases[layer];
            var weighted = weights * activation + biases;
            activation = Sigmoid(weighted);
        }
        return activation;
    }

    public float BackPropagate(ICollection<(Vector input, int expected)> inputs)
    {
        var costs = new List<float>();
        var weightErrors = new List<List<Matrix>>();
        var biasErrors = new List<List<Vector>>();
        foreach (var (input, expected) in inputs)
        {
            var desired = new Vector(new float[10]);
            desired.Values[expected] = 1;

            var (weights, bias, cost) = BackPropagate(input, desired);
            weightErrors.Add(weights);
            biasErrors.Add(bias);
            costs.Add(cost);
        }

        for (int layer = 0; layer < NumberOfLayers; layer++)
        {
            var weightAdjust = Matrix.Average(weightErrors.Select(e => e[layer])) * -LearningRate;
            var biasAdjust = Vector.Average(biasErrors.Select(e => e[layer])) * -LearningRate;

            _weights[layer] = _weights[layer] + weightAdjust;
            _biases[layer] = _biases[layer] + biasAdjust;
        }

        return costs.Average();
    }

    public (List<Matrix> weights, List<Vector> biases) Export()
    {
        return (_weights, _biases);
    }

    private (List<Matrix> weights, List<Vector> bias, float cost) BackPropagate(Vector input, Vector desired)
    {
        // Save Z values for each layer
        var zValues = new List<Vector>();
        List<Vector> activations = [input];
        var activation = input;
        for (int layer = 0; layer < NumberOfLayers; layer++)
        {
            var weights = _weights[layer];
            var biases = _biases[layer];
            var weighted = weights * activation + biases;
            zValues.Add(weighted); // Save for back propagation
            activation = Sigmoid(weighted);
            activations.Add(activation);
        }

        var cost = activation.Zip(desired, (x, y) => (x - y) * (x - y)).Sum() / 2;

        // And now........back
        var weightErrors = new List<Matrix>();
        var biasErrors = new List<Vector>();

        // Calculate errors in final layer
        var delta = (activations[NumberOfLayers] - desired)
            .ElementProduct(SigmoidDeriv(zValues[NumberOfLayers - 1]));
        biasErrors.Add(delta);
        weightErrors.Add(activations[NumberOfLayers - 1].CrossProduct(delta));

        // Calculate all other errors
        for (int i = NumberOfLayers - 2; i >= 0 ; i--)
        {
            delta = (_weights[i + 1].Transpose() * delta)
                .ElementProduct(SigmoidDeriv(zValues[i]));
            biasErrors.Add(delta);
            weightErrors.Add(activations[i].CrossProduct(delta));
        }

        // Errors were added in reverse other
        biasErrors.Reverse();
        weightErrors.Reverse();

        return (weightErrors, biasErrors, cost);
    }

    private static Vector Sigmoid(Vector input) => new([..input.Select(Sigmoid)]);
    private static float Sigmoid(float input) => 1 / (1 + MathF.Exp(-input));
    private static Vector SigmoidDeriv(Vector input) => new([..input.Select(SigmoidDeriv)]);
    private static float SigmoidDeriv(float input) => Sigmoid(input) * (1 - Sigmoid(input));
}
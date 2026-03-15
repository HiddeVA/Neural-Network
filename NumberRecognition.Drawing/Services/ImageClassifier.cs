using System.Text.Json;
using KnowledgeNight.NeuralNetwork.Math;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace KnowledgeNight.NumberRecognition.Drawing;

class ImageClassifier
{
    private readonly string samplesRoot = "/home/hidde-van-abbema/git/knowledge-night/samples";
    private readonly NeuralNetwork.NeuralNetwork _network;

    public ImageClassifier()
    {        
        // Setup network
        var parametersFile = "/home/hidde-van-abbema/git/knowledge-night/values.json";
        var parameters = JsonSerializer.Deserialize<NetworkParameters>(File.OpenRead(parametersFile))!;
        _network = new(
            parameters.Weights.Select(w => new Matrix(w)).ToList(),
            parameters.Biases.Select(b => new Vector(b)).ToList()
        );

    }
    public (int value, float confidence) Classify(IFormFile input)
    {
        var file = $"{samplesRoot}/{Guid.NewGuid()}";
        var imageBase = Image.Load(input.OpenReadStream());
        var image = imageBase.CloneAs<L8>();
        image.Mutate(ctx =>
        {
            ctx.Resize(new Size(28, 28));
            ctx.Invert();
        });
        var bytes = ToVector(image);

        var result = _network.FeedForward(bytes);
        var value = result.Index().MaxBy(p => p.Item);

        return (value.Index, value.Item);
    }

    private static Vector ToVector(Image<L8> image)
    {        
        var list = new List<float>();
        for (int row = 0; row < image.Height; row++)
        {
            for (int col = 0; col < image.Width; col++)
            {
                var pixel = image[col, row].PackedValue;
                list.Add(pixel / 255f);
            }
        }
        return new Vector(list.ToArray());
    }
}

public class NetworkParameters
{
    public required List<float[][]> Weights { get; set; } 
    public required List<float[]> Biases {get; set;}
}
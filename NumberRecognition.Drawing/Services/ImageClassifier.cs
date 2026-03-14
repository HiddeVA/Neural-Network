using System.Text.Json;
using KnowledgeNight.NeuralNetwork.Math;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace KnowledgeNight.NumberRecognition.Drawing;

class ImageClassifier
{
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
    public int Classify(IFormFile input)
    {
        var image = Image.Load(input.OpenReadStream()).CloneAs<L8>();
        image.Mutate(ctx =>
        {
            ctx.Resize(new Size(28, 28));
        });
        var bytes = ToVector(image);

        var result = _network.FeedForward(bytes);
        return 0;
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
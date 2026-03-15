
using System.Text.Json;
using KnowledgeNight.NeuralNetwork;
using KnowledgeNight.NeuralNetwork.Math;
using Parquet;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

Console.WriteLine("Starting...");

Console.WriteLine("Creating network with 3 layers");


const string projectRoot = "/home/hidde-van-abbema/git/knowledge-night";

// var network = NeuralNetwork.CreateRandom([28 * 28, 16, 10]);
var parametersFile = $"{projectRoot}/values.json";
var parameters = JsonSerializer.Deserialize<NetworkParameters>(File.OpenRead(parametersFile))!;
var network = new NeuralNetwork(
    [.. parameters.Weights.Select(w => new Matrix(w))],
    [.. parameters.Biases.Select(b => new Vector(b))]
);

using var stream = File.OpenRead($"{projectRoot}/data/train-00000-of-00001.parquet");
using var reader = await ParquetReader.CreateAsync(stream);

var counter = 0;
for (int group = 0; group < reader.RowGroupCount; group++)
{
    var groupReader = reader.OpenRowGroupReader(group);
    var schema = reader.Schema;
    var rowGroup = groupReader.RowGroup;
    var bytes = (await groupReader.ReadColumnAsync(schema.DataFields[0])).Data;
    var labels = (await groupReader.ReadColumnAsync(schema.DataFields[2])).Data;

    var partition = new List<(Vector image, int expected)>();
    for (int i = 0; i < groupReader.RowCount; i++)
    {        
        var item = bytes.GetValue(i) as byte[];
        var label = (int)(long)labels.GetValue(i)!;

        var image = Image.Load<L8>(item.AsSpan());
        var list = new List<float>();
        for (int row = 0; row < image.Height; row++)
        {
            for (int col = 0; col < image.Width; col++)
            {
                var pixel = image[col, row].PackedValue;
                list.Add(pixel / 255f);
            }
        }

        partition.Add((new Vector([.. list]), label));
    }

    var shuffled = partition.OrderBy(_ => Random.Shared.Next()).ToList();
    var costs = new List<float>();
    foreach (var sample in shuffled.Chunk(10))
    {
        costs.Add(network.BackPropagate(partition));
    }

    Console.WriteLine($"Iteration {counter++ * 10}. Average cost of last {rowGroup.NumRows} items: {costs.Average()}");
    partition.Clear();
}

Console.WriteLine("Exporting values...");
var (weights, biases) = network.Export();
var json = new NetworkParameters
{
    Weights = [..weights.Select(m => m.Values)],
    Biases = [..biases.Select(v => v.Values)],
};

var filePath = $"{projectRoot}/values.json";
var settings = new JsonSerializerOptions { WriteIndented = true };
File.WriteAllText(filePath, JsonSerializer.Serialize(json, settings));

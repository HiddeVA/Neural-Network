
using Parquet;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

Console.WriteLine("Starting...");

Console.WriteLine("Creating network with 3 layers");

var network = NeuralNetwork.CreateRandom([28 * 28, 16, 10]);

var dataRoot = "/home/hidde-van-abbema/git/knowledge night/data";

using var stream = File.OpenRead($"{dataRoot}/train-00000-of-00001.parquet");
using var reader = await ParquetReader.CreateAsync(stream);

var schema = reader.Schema;
var rowGroup = reader.RowGroups[0];
var bytes = (await rowGroup.ReadColumnAsync(schema.DataFields[0])).Data;
var labels = (await rowGroup.ReadColumnAsync(schema.DataFields[2])).Data;

for (int i = 0; i < rowGroup.RowCount; i++)
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
    var input = new Vector([.. list]);

    var expected = new float[10];
    expected[label] = 1f;
    Console.WriteLine($"Loaded image {i}");

    var cost = network.BackPropagate(input, new Vector(expected));
    foreach (var layer in cost)
    {
        Console.WriteLine($"Errors in layer: {string.Join(" ", layer)}");
    }

    break;
}




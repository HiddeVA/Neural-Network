using Microsoft.AspNetCore.Mvc;
using KnowledgeNight.NumberRecognition.Drawing;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapStaticAssets();
app.UseDefaultFiles();

var imageClassifier = new ImageClassifier();
app.MapPost("/", ([FromForm(Name = "image")] IFormFile file) =>
{
    var (value, confidence) = imageClassifier.Classify(file);
    return Results.Ok(new
    {
        Confidence = confidence,
        Value = value
    });
}).DisableAntiforgery();

app.Run();

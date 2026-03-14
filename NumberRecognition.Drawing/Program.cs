using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NumberRecognition.Drawing;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var imageClassifier = new ImageClassifier();
app.UseStaticFiles();
app.MapGet("/", () => "Hello World!");
app.MapPost("/", ([FromForm(Name = "image")] IFormFile file) =>
{
    var result = imageClassifier.Classify(file);
    return Results.Ok(result);
});

app.Run();

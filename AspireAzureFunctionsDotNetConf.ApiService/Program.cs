using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddAzureBlobClient("app-blobs");

// Add services to the container.
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapPost("/upload", async (BlobServiceClient blobServiceClient, [FromForm] IFormFile image) =>
{
    if (image is null)
    {
        return Results.BadRequest("No image was uploaded.");
    }

    var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");
    await blobContainerClient.CreateIfNotExistsAsync();

    var blobClient = blobContainerClient.GetBlobClient(image.FileName);
    await blobClient.UploadAsync(image.OpenReadStream(), true);

    return TypedResults.Ok();
});

app.MapGet("/", () => Results.Content("""
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Image Upload Form</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }
        .upload-form {
            background-color: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
        }
        .upload-form h1 {
            margin-top: 0;
        }
        .upload-form label {
            display: block;
            margin-bottom: 8px;
        }
        .upload-form input[type="file"] {
            margin-bottom: 20px;
        }
        .upload-form button {
            background-color: #007bff;
            color: #fff;
            border: none;
            padding: 10px 20px;
            border-radius: 4px;
            cursor: pointer;
        }
        .upload-form button:hover {
            background-color: #0056b3;
        }
        .success-message {
            display: none;
            color: green;
            margin-top: 20px;
        }
    </style>
</head>
<body>
    <div class="upload-form">
        <h1>Upload an image to resize</h1>
        <form id="imageUploadForm" action="/upload" method="post" enctype="multipart/form-data">
            <label for="image">Choose an image to upload:</label>
            <input type="file" id="image" name="image" accept="image/*" required>
            <br>
            <button type="submit">Upload Image</button>
        </form>
        <div class="success-message" id="successMessage">Image uploaded successfully!</div>
    </div>

    <script>
        document.getElementById('imageUploadForm').addEventListener('submit', function(event) {
            event.preventDefault();
            // Simulate a successful upload
            setTimeout(function() {
                document.getElementById('successMessage').style.display = 'block';
            }, 1000);
        });
    </script>
</body>
</html>
""", "text/html"));

app.MapDefaultEndpoints();

app.Run();

var builder = DistributedApplication.CreateBuilder(args);

var blobs = builder.AddAzureStorage("app-storage")
    .RunAsEmulator()
    .AddBlobs("app-blobs");

builder.AddProject<Projects.AspireAzureFunctionsDotNetConf_ApiService>("apiservice")
    .WithReference(blobs);

builder.Build().Run();

# Elmahdi.Phone

[![Nuget downloads](https://img.shields.io/nuget/v/Elmahdi.ImageSharp.Web.Providers.S3)](https://www.nuget.org/packages/Elmahdi.ImageSharp.Web.Providers.S3/)
[![GitHub license](https://img.shields.io/github/license/khaledelmahdi/image-sharp-web-providers-s3)](https://github.com/khaledelmahdi/image-sharp-web-providers-s3)

An Image provider for [Six Labors ImageSharp.Web](https://docs.sixlabors.com/articles/imagesharp)

This provider allows the processing and serving of image files from 
Amazon S3 or DigitalOcean Spaces and is available as an external package 
installable via [NuGet](https://www.nuget.org/packages/Elmahdi.ImageSharp.Web.Providers.S3/).

## Installation

```
PM> Install-Package Elmahdi.ImageSharp.Web.Providers.S3 -Version VERSION_NUMBER
```

```
> dotnet add package Elmahdi.ImageSharp.Web.Providers.S3 --version VERSION_NUMBER
```

## Usage

Once installed the provider AzureBlobContainerClientOptions can be configured as follows:

```c#
using Elmahdi.ImageSharp.Web.Providers.S3.Providers;
using SixLabors.ImageSharp.Web.DependencyInjection;

public void ConfigureServices(IServiceCollection services)
{
    ...
    services
        .AddImageSharp()
        .Configure<S3StorageImageProviderOptions>(options => { 
            options.S3Containers.Add(new S3ClientOptions { 
                AccessKeyId = "", // Access Key Id
                SecretAccessKey = "", // Secret Access Key
                BucketName = "", // Bucket Name, 
                EndpointUrl = "" // Endpoint Url            
            }); 
        })
        .ClearProviders()
        .AddProvider<S3StorageImageProvider>();
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseImageSharp(); // Add this BEFORE app.UseStaticFiles();
    ...
}
```

Url requests are matched in accordance to the following rule:
```
/{BucketName}/{FileName}
```

## To do 

- [ ] Add tests
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace BlogApp.Infrastructure.ExternalServices.Minio;

public class MinioStartupService : IHostedService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public MinioStartupService(
        IMinioClient minioClient, IOptions<MinioOptions> options
        )
    {
        _minioClient = minioClient;
        _options = options.Value;
    }
    
    // helper public bucket
    private static string PublicReadPolicy(string bucket) =>
        $@"{{
  ""Version"": ""2012-10-17"",
  ""Statement"": [
    {{
      ""Effect"": ""Allow"",
      ""Principal"": {{ ""AWS"": [""*""] }},
      ""Action"": [""s3:GetObject""],
      ""Resource"": [""arn:aws:s3:::{bucket}/*""]
    }}
  ]
}}";

    // create bucket 
    private async Task EnsureBucketAsync(
        string bucket,
        bool isPublic,
        CancellationToken ct)
    {
        var exists = await _minioClient.BucketExistsAsync(
            new BucketExistsArgs().WithBucket(bucket),
            ct);

        if (!exists)
        {
            await _minioClient.MakeBucketAsync(
                new MakeBucketArgs().WithBucket(bucket),
                ct);
        }

        if (isPublic)
        {
            await _minioClient.SetPolicyAsync(
                new SetPolicyArgs()
                    .WithBucket(bucket)
                    .WithPolicy(PublicReadPolicy(bucket)),
                ct);
        }
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var blogBucket = _options.Buckets["Blogs"];
        var avatarBucket = _options.Buckets["Avatars"];
        var attachmentBucket = _options.Buckets["Attachments"];

        // Blog images -> PUBLIC
        await EnsureBucketAsync(blogBucket, isPublic: true, cancellationToken);

        // Avatars -> PUBLIC
        await EnsureBucketAsync(avatarBucket, isPublic: true, cancellationToken);

        // Attachments -> PRIVATE
        await EnsureBucketAsync(attachmentBucket, isPublic: false, cancellationToken);
    }


    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
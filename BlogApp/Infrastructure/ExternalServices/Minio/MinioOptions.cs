namespace BlogApp.Infrastructure.ExternalServices.Minio;

public class MinioOptions
{
    public string Endpoint { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public bool UseSSL { get; set; }

    public Dictionary<string, string> Buckets { get; set; } = new();
}

using System.Collections.Generic;

namespace Elmahdi.ImageSharp.Web.Providers.S3.Providers
{
    /// <summary>
    /// Configuration options for the <see cref="S3StorageImageProvider"/> provider.
    /// </summary>
    public class S3StorageImageProviderOptions
    {
        /// <summary>
        /// Gets or sets the collection of AWS S3 container client options.
        /// </summary>
        public ICollection<S3ClientOptions> S3Containers { get; set; } = new HashSet<S3ClientOptions>();
    }

    /// <summary>
    /// Represents a single S3 Storage bucket.
    /// </summary>
    public class S3ClientOptions
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string EndpointUrl { get; set; }
        public string BucketName { get; set; }
    }
}

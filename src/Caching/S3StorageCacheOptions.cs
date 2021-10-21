namespace Elmahdi.ImageSharp.Web.Providers.S3.Caching
{
    /// <summary>
    /// Configuration options for the <see cref="S3StorageCache"/>.
    /// </summary>
    public class S3StorageCacheOptions
    {
        /// <summary>
        /// AWS Access Key ID
        /// </summary>
        public string AccessKeyId { get; set; }

        /// <summary>
        /// AWS Secret Access Key
        /// </summary>
        public string SecretAccessKey { get; set; }

        /// <summary>
        /// The constant used to lookup in the region hash the endpoint.
        /// </summary>
        public string EndpointUrl { get; set; }

        /// <summary>
        /// The bucket name
        /// </summary>
        public string BucketName { get; set; }
    }
}

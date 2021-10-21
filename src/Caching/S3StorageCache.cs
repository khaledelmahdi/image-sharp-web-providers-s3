using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Transfer;
using Elmahdi.ImageSharp.Web.Providers.S3.Resolvers;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.Resolvers;

namespace Elmahdi.ImageSharp.Web.Providers.S3.Caching
{
    public class S3StorageCache : IImageCache
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly S3StorageCacheOptions _options;

        public S3StorageCache(IOptions<S3StorageCacheOptions> cacheOptions)
        {
            _options = cacheOptions.Value;
            var s3ClientConfig = new AmazonS3Config { ServiceURL = $"https://{_options.EndpointUrl}" };

            _amazonS3 = new AmazonS3Client(_options.AccessKeyId, _options.SecretAccessKey, s3ClientConfig);
        }

        /// <inheritdoc/>
        public async Task<IImageCacheResolver> GetAsync(string key)
        {
            var meta = await _amazonS3.GetObjectMetadataAsync(_options.BucketName, key);

            return meta == null
                ? null
                : new S3StorageCacheResolver(_amazonS3, _options.BucketName, key);
        }

        /// <inheritdoc/>
        public async Task SetAsync(string key, Stream stream, ImageCacheMetadata metadata)
        {
            var fileTransferUtility = new TransferUtility(_amazonS3);
            var fileTransferUtilityRequest = new TransferUtilityUploadRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                CannedACL = S3CannedACL.PublicRead,
                InputStream = stream,
                ContentType = metadata.ContentType
            };

            await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
        }
    }
}

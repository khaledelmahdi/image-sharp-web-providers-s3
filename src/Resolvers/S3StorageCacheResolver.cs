using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Elmahdi.ImageSharp.Web.Providers.S3.Caching;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Resolvers;

namespace Elmahdi.ImageSharp.Web.Providers.S3.Resolvers
{
    /// <summary>
    /// Provides means to manage image buffers within the <see cref="S3StorageCache"/>.
    /// </summary>
    public class S3StorageCacheResolver : IImageCacheResolver
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly string _bucketName;
        private readonly string _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3StorageCacheResolver"/> class.
        /// </summary>
        /// <param name="amazonS3">The S3 storage.</param>
        /// <param name="bucketName">The bucket name containing the object.  When using this action with an access point, you must direct requests to the access point hostname. The access point hostname takes the form <i>AccessPointName</i>-<i>AccountId</i>.s3-accesspoint.<i>Region</i>.amazonaws.com. When using this action with an access point through the Amazon Web Services SDKs, you provide the access point ARN in place of the bucket name. For more information about access point ARNs, see <a href="https://docs.aws.amazon.com/AmazonS3/latest/userguide/using-access-points.html">Using access points</a> in the <i>Amazon S3 User Guide</i>. When using an Object Lambda access point the hostname takes the form <i>AccessPointName</i>-<i>AccountId</i>.s3-object-lambda.<i>Region</i>.amazonaws.com. When using this action with Amazon S3 on Outposts, you must direct requests to the S3 on Outposts hostname. The S3 on Outposts hostname takes the form <i>AccessPointName</i>-<i>AccountId</i>.<i>outpostID</i>.s3-outposts.<i>Region</i>.amazonaws.com. When using this action using S3 on Outposts through the Amazon Web Services SDKs, you provide the Outposts bucket ARN in place of the bucket name. For more information about S3 on Outposts ARNs, see <a href="https://docs.aws.amazon.com/AmazonS3/latest/userguide/S3onOutposts.html">Using S3 on Outposts</a> in the <i>Amazon S3 User Guide</i>.</param>
        /// <param name="key">Key of the object to get.</param>
        public S3StorageCacheResolver(IAmazonS3 amazonS3, string bucketName, string key)
        {
            _amazonS3 = amazonS3;
            _bucketName = bucketName;
            _key = key;
        }

        /// <inheritdoc/>
        public async Task<ImageCacheMetadata> GetMetaDataAsync()
        {
            var metadata = await _amazonS3.GetObjectMetadataAsync(_bucketName, _key);


            return ImageCacheMetadata.FromDictionary(ConvertMetadataToDictionary(metadata.Metadata));
        }

        /// <inheritdoc/>
        public async Task<Stream> OpenReadAsync()
        {
            var s3Object = await _amazonS3.GetObjectAsync(_bucketName, _key);
            return s3Object.ResponseStream;
        }

        /// <summary>
        /// Convert S3 object MetadataCollection to Dictionary
        /// </summary>
        /// <param name="metadataMetadata">The meta data for an S3 object</param>
        /// <returns>The dictionary to parse</returns>
        private IDictionary<string, string> ConvertMetadataToDictionary(MetadataCollection metadataMetadata)
        {
            IDictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var key in metadataMetadata.Keys)
            {
                dictionary.Add(key, metadataMetadata["key"]);
            }

            return dictionary;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.S3;
using Elmahdi.ImageSharp.Web.Providers.S3.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Providers;
using SixLabors.ImageSharp.Web.Resolvers;

namespace Elmahdi.ImageSharp.Web.Providers.S3.Providers
{
    /// <summary>
    /// Returns images stored in Digital Ocean Spaces Storage.
    /// </summary>
    public class S3StorageImageProvider : IImageProvider
    {
        /// <summary>
        /// Character array to remove from paths.
        /// </summary>
        private static readonly char[] SlashChars = { '\\', '/' };

        /// <summary>
        /// The containers for the AWS S3 services.
        /// </summary>
        private readonly Dictionary<string, IAmazonS3> _containers = new Dictionary<string, IAmazonS3>();

        /// <summary>
        /// The blob storage options.
        /// </summary>
        private readonly S3StorageImageProviderOptions _storageOptions;

        /// <summary>
        /// Contains various helper methods based on the current configuration.
        /// </summary>
        private readonly FormatUtilities _formatUtilities;

        /// <summary>
        /// A match function used by the resolver to identify itself as the correct resolver to use.
        /// </summary>
        private Func<HttpContext, bool> _match;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3StorageImageProvider"/> class.
        /// </summary>
        /// <param name="storageOptions">The S3 storage options.</param>
        /// <param name="formatUtilities">Contains various format helper methods based on the current configuration.</param>
        public S3StorageImageProvider(IOptions<S3StorageImageProviderOptions> storageOptions, FormatUtilities formatUtilities)
        {
            _storageOptions = storageOptions?.Value ?? throw new ArgumentNullException(nameof(storageOptions));
            _formatUtilities = formatUtilities;

            foreach (var container in _storageOptions.S3Containers)
            {
                var s3ClientConfig = new AmazonS3Config { ServiceURL = $"https://{container.EndpointUrl}" };

                _containers.Add(
                    container.BucketName,
                    new AmazonS3Client(container.AccessKeyId, container.SecretAccessKey, s3ClientConfig)
                );
            }
        }

        /// <inheritdoc/>
        public ProcessingBehavior ProcessingBehavior { get; } = ProcessingBehavior.All;

        /// <inheritdoc/>
        public Func<HttpContext, bool> Match
        {
            get => _match ?? this.IsMatch;
            set => _match = value;
        }

        /// <inheritdoc/>
        public async Task<IImageResolver> GetAsync(HttpContext context)
        {
            // Strip the leading slash and container name from the HTTP request path and treat
            // the remaining path string as the blob name.
            // Path has already been correctly parsed before here.
            var bucketName = string.Empty;
            IAmazonS3 container = null;

            // We want an exact match here to ensure that container names starting with
            // the same prefix are not mixed up.
            var path = context.Request.Path.Value.TrimStart(SlashChars);
            var index = path.IndexOfAny(SlashChars);
            var nameToMatch = index != -1 ? path.Substring(0, index) : path;

            foreach (var key in _containers.Keys)
            {
                if (nameToMatch.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    bucketName = key;
                    container = _containers[key];
                    break;
                }
            }

            // Something has gone horribly wrong for this to happen but check anyway.
            if (container is null)
            {
                return null;
            }

            // file name should be the remaining path string.
            var file = path.Substring(bucketName.Length).TrimStart(SlashChars);

            if (string.IsNullOrWhiteSpace(file))
            {
                return null;
            }

            var obj = await container.GetObjectMetadataAsync(bucketName, file);

            return obj == null
                ? null
                : new S3StorageImageResolver(container, bucketName, file);
        }

        /// <inheritdoc/>
        public bool IsValidRequest(HttpContext context)
            => _formatUtilities.GetExtensionFromUri(context.Request.GetDisplayUrl()) != null;

        private bool IsMatch(HttpContext context)
        {
            // Only match loosly here for performance.
            // Path matching conflicts should be dealt with by configuration.
            var path = context.Request.Path.Value.TrimStart(SlashChars);
            foreach (string container in _containers.Keys)
            {
                if (path.StartsWith(container, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

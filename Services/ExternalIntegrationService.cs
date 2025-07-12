using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Services
{
    public class ExternalIntegrationService : IExternalIntegrationService
    {
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;
        private readonly string _projectRef;
        private readonly string _publicBaseUrl;

        public ExternalIntegrationService(IConfiguration configuration)
        {
            _accessKey = configuration["SupabaseSettings:AccessKey"]!;
            _secretKey = configuration["SupabaseSettings:SecretKey"]!;
            _bucketName = configuration["SupabaseSettings:BucketName"]!;
            _projectRef = configuration["SupabaseSettings:ProjectRef"]!;
            _publicBaseUrl = $"https://{_projectRef}.supabase.co/storage/v1/object/public";
        }

        public async Task<string> UploadImageStreamAsync(Stream stream, string keyNameInBucket, string contentType = "image/jpeg")
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.APSoutheast1,
                ServiceURL = $"https://{_projectRef}.supabase.co/storage/v1/s3",
                ForcePathStyle = true
            };

            using var client = new AmazonS3Client(_accessKey, _secretKey, config);
            using var transferUtility = new TransferUtility(client);

            var request = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = keyNameInBucket,
                BucketName = _bucketName,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await transferUtility.UploadAsync(request);
            return $"{_publicBaseUrl}/{_bucketName}/{keyNameInBucket}";
        }
    }
}

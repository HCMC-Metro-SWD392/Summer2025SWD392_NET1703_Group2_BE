using Amazon;
using Amazon.S3.Model;
using Amazon.S3;
using MetroTicketBE.Application.IService;
using Microsoft.Extensions.Configuration;
using MetroTicketBE.Domain.DTO.Auth;

namespace MetroTicketBE.Application.Service
{
    public class S3Service : IS3Service
    {
        private readonly string _bucketName;

        public S3Service(IConfiguration configuration)
        {
            _bucketName = configuration["AWS_S3:BucketName"] ?? throw new ArgumentNullException("AWS_S3:BucketName is not configured.");
        }

        public ResponseDTO GenerateDownloadUrl(string objectKey)
        {
            var s3Client = new AmazonS3Client(RegionEndpoint.APSoutheast1);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            var url = s3Client.GetPreSignedURL(request);

            return new ResponseDTO
            {
                StatusCode = 200,
                Message = "Tạo đường dẫn kết nối s3 thành công",
                Result = new
                {
                    Url = url,
                    ObjectKey = objectKey
                }
            };
        }

        public ResponseDTO GenerateUploadUrl(string objectKey, string contentType)
        {
            var s3Client = new AmazonS3Client(RegionEndpoint.APSoutheast1);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = objectKey,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ContentType = contentType
            };

            var url = s3Client.GetPreSignedURL(request);

            return new ResponseDTO
            {
                StatusCode = 200,
                Message = "Tạo đường dẫn kết nối s3 thành công",
                Result = new
                {
                    Url = url,
                    ObjectKey = objectKey
                }
            };
        }
    }
}

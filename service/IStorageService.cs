using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Aws.Media.Convert.Api.Controllers.model;

namespace Aws.Media.Convert.Api.Controllers.service
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAysnc(S3Object obj, BasicAWSCredentials credentials, AmazonS3Config regionConfig, AmazonS3Client client);
    }
}
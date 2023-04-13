using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Aws.Media.Convert.Api.Model;

namespace Aws.Media.Convert.Api.Services
{
    public interface IStorageService
    {
        Task<S3ResponseDto> UploadFileAysnc(CS3Object obj, BasicAWSCredentials credentials, AmazonS3Client client);
    }
}
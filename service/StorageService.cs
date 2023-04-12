using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Aws.Media.Convert.Api.Controllers;
using Aws.Media.Convert.Api.Controllers.model;
using Aws.Media.Convert.Api.Controllers.service;

namespace Aws.Media.Convert.Api.service
{
    public class StorageService : IStorageService
    {
        public StorageService(IConfiguration config){}

        public async Task<S3ResponseDto> UploadFileAysnc(S3Object obj, BasicAWSCredentials credentials, AmazonS3Config regionConfig, AmazonS3Client client)
        {
            var response = new S3ResponseDto();
            try
            {
                var uploadRequest = new TransferUtilityUploadRequest(){
                    InputStream = obj.InputStream,
                    Key = obj.Name,
                    BucketName = obj.BucketName,
                    CannedACL = S3CannedACL.NoACL
                };
                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(uploadRequest);
                response.StatusCode = 201;
                response.Message = $"{obj.Name} has been uploaded sucessfully";
            }
            catch (AmazonS3Exception s3Exeption)
            {
                response.StatusCode = (int)s3Exeption.StatusCode;
                response.Message = s3Exeption.Message;
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
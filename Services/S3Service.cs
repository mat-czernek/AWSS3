using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.Util;


namespace AWSS3
{   
    /// <summary>
    /// Class provides basic operations on AWS S3 buckets
    /// </summary>
    public class S3Service : IS3Service
    {
        /// <summary>
        /// S3 client implemented in S3 SDK
        /// </summary>
        private AmazonS3Client _client;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="accessKey">IAM user access key</param>
        /// <param name="secretKey">IAM user secret key</param>
        /// <param name="endpoint">Service region endpoint</param>
        public S3Service(string accessKey, string secretKey, RegionEndpoint endpoint)
        {
            _client = new AmazonS3Client(accessKey, secretKey, endpoint);           
        }

        /// <summary>
        /// Method downloads specific version of the file from S3 bucket.
        /// </summary>
        /// <param name="bucketName">Name of the S3 bucket</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="versionId">Version of the file</param>
        public void GetFile(string bucketName, string fileName, string versionId)
        {
            if(string.IsNullOrEmpty(bucketName))
                throw new ArgumentNullException(nameof(bucketName));

            if(string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if(string.IsNullOrEmpty(versionId))
                throw new ArgumentNullException(nameof(versionId));

            try
            {
                using(Task<GetObjectResponse> s3GetObjectResponse = _client.GetObjectAsync(bucketName, fileName, versionId))
                {
                    if(s3GetObjectResponse.Result.ResponseStream != null)
                    {               
                        _responseStreamToFile(s3GetObjectResponse.Result.ResponseStream, fileName); 
                        Console.WriteLine("Success.");
                    }
                    else
                    {
                        Console.WriteLine("Failure");
                        Console.WriteLine($"HTTP status code : {s3GetObjectResponse.Result.HttpStatusCode}");
                    }
                }
            }
            catch(System.AggregateException ex)
            {
                if(ex.InnerException != null)
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
            }
        }

        /// <summary>
        /// Method downloads the latest version of the file from the S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="fileName">Name of the file</param>
        public void GetFile(string bucketName, string fileName)
        {
            if(string.IsNullOrEmpty(bucketName))
                throw new ArgumentNullException(nameof(bucketName));

            if(string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));


            try
            {
                using(Task<GetObjectResponse> s3GetObjectResponse = _client.GetObjectAsync(bucketName, fileName))
                {
                    if(s3GetObjectResponse.Result.ResponseStream != null)
                    {               
                        _responseStreamToFile(s3GetObjectResponse.Result.ResponseStream, fileName); 
                        Console.WriteLine("Success.");
                    }
                    else
                    {
                        Console.WriteLine("Failure");
                        Console.WriteLine($"HTTP status code : {s3GetObjectResponse.Result.HttpStatusCode}");
                    }
                }
            }
            catch(System.AggregateException ex)
            {
                if(ex.InnerException != null)
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
            }
        }

        /// <summary>
        /// Method verifies whenever the bucket exist in S3
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <returns>True if bucket exist, false in other case</returns>
        public bool IsBucketExist(string bucketName)
        {
            if(string.IsNullOrEmpty(bucketName))
                throw new ArgumentNullException(nameof(bucketName));

            try
            {
                using(Task<bool> isBucketAlreadyExist = AmazonS3Util.DoesS3BucketExistAsync(_client, bucketName))
                {
                    if(isBucketAlreadyExist.Result == true)
                    {
                        Console.WriteLine("Bucket name already exist.");
                        return true;
                    }

                    return false;
                }
            }
            catch(System.AggregateException ex)
            {
                if(ex.InnerException != null)
                    Console.WriteLine($"Message: {ex.InnerException.Message}");

                return false;
            }
        }


        /// <summary>
        /// Method gets version id's of a file in S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="fileName">Name of the file</param>
        /// <returns>List of strings with file versions id's or empty list in case of failure</returns>
        public List<string> ListFileVersions(string bucketName, string fileName)
        {
            if(string.IsNullOrEmpty(bucketName))
                throw new ArgumentNullException(nameof(bucketName));

            if(string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            List<string> versionsList = new List<string>();

            try
            {
                using(Task<ListVersionsResponse> s3ListVersionsResponse = _client.ListVersionsAsync(bucketName, fileName))
                {
                    foreach(S3ObjectVersion objectVersion in s3ListVersionsResponse.Result.Versions)
                    {
                        Console.WriteLine("Version ID: {0}", objectVersion.VersionId);

                        versionsList.Add(objectVersion.VersionId);
                    }
                }
            }
            catch(System.AggregateException ex)
            {
                if(ex.InnerException != null)
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
            }

            return versionsList;
        }


        /// <summary>
        /// Method saves stream returned from the S3 into the file stream
        /// </summary>
        /// <param name="responseStream">Response stream from the S3</param>
        /// <param name="fileName">Name of the target file</param>
        private void _responseStreamToFile(Stream responseStream, string fileName)
        {
            var bufferSize = 1024;
            var buffer = new byte[bufferSize];
            int bytesRead = 0;
            
            using(FileStream fileStream = File.Create(fileName))
            {
                while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    fileStream.Write(buffer, 0, bytesRead);
                    fileStream.Flush();
                    buffer = new byte[bufferSize];
                } 

                fileStream.Close();
            }
            
        }

        /// <summary>
        /// Method puts the file in S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the file</param>
        /// <param name="filePath">Full path to file</param>
        public void PutFile(string bucketName, string filePath)
        {
            PutObjectRequest s3PutObjectRequest = new PutObjectRequest();
            s3PutObjectRequest.BucketName = bucketName;
            s3PutObjectRequest.FilePath = filePath;
            s3PutObjectRequest.StorageClass = S3StorageClass.Standard;

            try
            {
                using(Task<PutObjectResponse> s3PutObjectResponse = _client.PutObjectAsync(s3PutObjectRequest))
                {
                    Console.WriteLine($"HTTP status code : {s3PutObjectResponse.Result.HttpStatusCode}");
                }
            }
            catch(System.AggregateException ex)
            {
                if(ex.InnerException != null)
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
            }
        }

        /// <summary>
        /// Method removes all version of the file in S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="fileName">File name</param>
        public void DeleteFile(string bucketName, string fileName)
        {
            try
            {
                DeleteObjectRequest s3DeleteObjectRequest = new DeleteObjectRequest();
                s3DeleteObjectRequest.BucketName = bucketName;
                s3DeleteObjectRequest.Key = fileName;

                using(Task<DeleteObjectResponse> s3DeleteObjectResponse = _client.DeleteObjectAsync(s3DeleteObjectRequest))
                {
                    Console.WriteLine($"HTTP status code : {s3DeleteObjectResponse.Result.HttpStatusCode}");
                }
            }
            catch(System.AggregateException ex)
            {
                if(ex.InnerException != null)
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
            }
        }



        /// <summary>
        /// Method removes only a specific version of the file in S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="fileName">File name</param>
        /// <param name="versionId">File version</param>
        public void DeleteFile(string bucketName, string fileName, string versionId)
        {
            try
            {
                DeleteObjectRequest s3DeleteObjectRequest = new DeleteObjectRequest();
                s3DeleteObjectRequest.BucketName = bucketName;
                s3DeleteObjectRequest.Key = fileName;
                s3DeleteObjectRequest.VersionId = versionId;

                using(Task<DeleteObjectResponse> s3DeleteObjectResponse = _client.DeleteObjectAsync(s3DeleteObjectRequest))
                {
                    Console.WriteLine($"HTTP status code : {s3DeleteObjectResponse.Result.HttpStatusCode}");
                }
            }
            catch(System.AggregateException ex)
            {
                if(ex.InnerException != null)
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
            }
        }

        /// <summary>
        /// Method creates new bucket in S3
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        public void CreateBucket(string bucketName)
        {
            
            PutBucketRequest s3PutBucketRequest = new PutBucketRequest();
            s3PutBucketRequest.BucketName = bucketName;
            s3PutBucketRequest.UseClientRegion = true;

            try
            {
                
                using(Task<PutBucketResponse> s3PutBucketResponse = _client.PutBucketAsync(s3PutBucketRequest))
                {
                    Console.WriteLine($"HTTP status code : {s3PutBucketResponse.Result.HttpStatusCode}");

                    PublicAccessBlockConfiguration s3PublicAccessBlockConfiguration = new PublicAccessBlockConfiguration();
                    s3PublicAccessBlockConfiguration.BlockPublicAcls = true;
                    s3PublicAccessBlockConfiguration.BlockPublicPolicy = true;
                    s3PublicAccessBlockConfiguration.IgnorePublicAcls = true;
                    s3PublicAccessBlockConfiguration.RestrictPublicBuckets = true;

                    PutPublicAccessBlockRequest s3PublicAccessBlockRequest = new PutPublicAccessBlockRequest();
                    s3PublicAccessBlockRequest.BucketName = bucketName;
                    s3PublicAccessBlockRequest.PublicAccessBlockConfiguration = s3PublicAccessBlockConfiguration;
                    

                    using(Task<PutPublicAccessBlockResponse> s3PutPublicAccessBlockResponse = _client.PutPublicAccessBlockAsync(s3PublicAccessBlockRequest))
                    {
                        Console.WriteLine($"HTTP status code : {s3PutPublicAccessBlockResponse.Result.HttpStatusCode}");
                    }
                }
  
            }
            catch(System.AggregateException ex)
            {
                if(ex.InnerException != null)
                    Console.WriteLine($"Message: {ex.InnerException.Message}");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Util;


namespace AWSS3
{   
    public class S3AWSService
    {
        private AmazonS3Client _client;

        public S3AWSService(string accessKey, string secretKey)
        {
            _client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.EUCentral1);   
        }

        public void GetFileByVersion(string bucketName, string fileName, string versionId)
        {
            Task<GetObjectResponse> objectResponse = _client.GetObjectAsync(bucketName, fileName, versionId);

            Console.WriteLine("HTTP status code: {0}", objectResponse.Result.HttpStatusCode);
            Console.WriteLine("FLast version ID: {0}", objectResponse.Result.VersionId);

            _responseStreamToFile(objectResponse.Result.ResponseStream, fileName); 
        }

        public void GetFileLastVersion(string bucketName, string fileName)
        {
            Task<ListBucketsResponse> response = _client.ListBucketsAsync();

            S3Bucket bucket = response.Result.Buckets.SingleOrDefault(b => b.BucketName == bucketName);
        
            if(bucket != null)
            {
                Console.WriteLine("Found target bucket: {0}", bucket.BucketName);
                
                Task<GetObjectResponse> objectResponse = _client.GetObjectAsync(bucketName, fileName);
        

                Console.WriteLine("HTTP status code: {0}", objectResponse.Result.HttpStatusCode);
                Console.WriteLine("FLast version ID: {0}", objectResponse.Result.VersionId);

                _responseStreamToFile(objectResponse.Result.ResponseStream, fileName);       
            } 
        }


        public List<string> GetFileVersions(string bucketName, string fileName)
        {
            List<string> versionsList = new List<string>();

            Task<ListVersionsResponse> listVersionsResponse = _client.ListVersionsAsync("herkbucket", fileName);

            foreach(S3ObjectVersion version in listVersionsResponse.Result.Versions)
            {
                Console.WriteLine("Version ID: {0}", version.VersionId);

                versionsList.Add(version.VersionId);
            }

            return versionsList;
        }


        private void _responseStreamToFile(Stream responseStream, string fileName)
        {
            var bufferSize = 1024;
            var buffer = new byte[bufferSize];
            int bytesRead;

            FileStream fileStream = File.Create(fileName);

            while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
                fileStream.Flush();
                buffer = new byte[bufferSize];
            } 

            fileStream.Close();
        }
    }
}
using System;


namespace AWSS3
{
    class Program
    {
        static void Main(string[] args)
        {
            S3AWSConfiguration s3AWSConfiguration = new S3AWSConfiguration();

            S3AWSService s3AWSApi = new S3AWSService(s3AWSConfiguration.AuthenticationConfiguration.AccessKey, s3AWSConfiguration.AuthenticationConfiguration.SecretKey);

        }
    }
}

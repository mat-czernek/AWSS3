using System;
using System.IO;
using Amazon;
using Microsoft.Extensions.Configuration;

namespace AWSS3
{
    class Program
    {
        private static IConfigurationRoot configuration;

        private static AuthenticationModel AuthenticationConfiguration;

        static void Main(string[] args)
        {
            
            SetupEnvironment();

            IS3Service s3Service = new S3Service(
                AuthenticationConfiguration.AccessKey, 
                AuthenticationConfiguration.SecretKey,
                RegionEndpoint.EUCentral1);
        }

        private static void SetupEnvironment()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            configuration = builder.Build();

            AuthenticationConfiguration = new AuthenticationModel();

            configuration.GetSection("Authentication").Bind(AuthenticationConfiguration);

            if(AuthenticationConfiguration.AccessKey == null)
                throw new ArgumentException(nameof(AuthenticationConfiguration.AccessKey));

            if(AuthenticationConfiguration.SecretKey == null)
                throw new ArgumentException(nameof(AuthenticationConfiguration.SecretKey));
        }
    }
}

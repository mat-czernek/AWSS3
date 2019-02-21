using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace AWSS3
{        
    public class S3AWSConfiguration
    {
        IConfigurationRoot configuration;

        public AuthenticationModel AuthenticationConfiguration;

        public S3AWSConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();

            AuthenticationConfiguration = new AuthenticationModel();

            configuration.GetSection("Authentication").Bind(AuthenticationConfiguration);

            if(string.IsNullOrEmpty(AuthenticationConfiguration.AccessKey) || string.IsNullOrEmpty(AuthenticationConfiguration.SecretKey))
            {
                throw new ArgumentException("Check configuration file and setup the access and secret key,");
            }
        }

        
    } 
}
using System.Security;

namespace AWSS3
{        
    /// <summary>
    /// AWS S3 authentication model
    /// </summary>
    public class AuthenticationModel
    {
        public string AccessKey { get; set; }

        public string SecretKey { get; set; }
    }
}
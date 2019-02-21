using System.Collections.Generic;

namespace AWSS3
{
    /// <summary>
    /// Basic operations on S3 bucket
    /// </summary>
    public interface IS3Service
    {
        /// <summary>
        /// Method downloads specific version of the file from S3 bucket.
        /// </summary>
        /// <param name="bucketName">Name of the S3 bucket</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="versionId">Version of the file</param>
        void GetFile(string bucketName, string fileName, string versionId);


        /// <summary>
        /// Method downloads the latest version of the file from the S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="fileName">Name of the file</param>
        void GetFile(string bucketName, string fileName);


        /// <summary>
        /// Method verifies whenever the bucket exist in S3
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <returns>True if bucket exist, false in other case</returns>
        bool IsBucketExist(string bucketName);


        /// <summary>
        /// Method gets version id's of a file in S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="fileName">Name of the file</param>
        /// <returns>List of strings with file versions id's or empty list in case of failure</returns>
        List<string> ListFileVersions(string bucketName, string fileName);


        /// <summary>
        /// Method puts the file in S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the file</param>
        /// <param name="filePath">Full path to file</param>
        void PutFile(string bucketName, string filePath);


        /// <summary>
        /// Method removes all version of the file in S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="fileName">File name</param>
        void DeleteFile(string bucketName, string fileName);

        /// <summary>
        /// Method removes only a specific version of the file in S3 bucket
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        /// <param name="fileName">File name</param>
        /// <param name="versionId">File version</param>
        void DeleteFile(string bucketName, string fileName, string versionId);

        /// <summary>
        /// Method creates new bucket in S3 with no public access
        /// </summary>
        /// <param name="bucketName">Name of the bucket</param>
        void CreateBucket(string bucketName);
    }
}
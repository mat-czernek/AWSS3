# Overview

Simple project to present basic operations on Amazon Web Services S3.

# Current state

List of the operations on S3 service included into this project:

* Get last version of the file
* Get file by version
* Check whenever the bucket already exist
* List version of the file
* Put new file or new version of file
* Delete file
* Delete file by version
* Create none public bucket (blocking public policies and ACLs)

# Authentication

Create new IAM user in AWS and save access key and secret key. Create new group and setup policies. Note that for operations like put, delete, create you need to have full control permissions on the bucket.

Open appsettings.json file and setup the values with your IAM user credentials.
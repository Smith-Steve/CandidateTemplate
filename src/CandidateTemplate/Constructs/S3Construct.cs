using Constructs;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.IAM;
using CandidateTemplate.Globals;
using Amazon.CDK;
using Amazon.CDK.AWS.S3.Deployment;
using System.Threading.Tasks.Dataflow;

namespace CandidateTemplate.Constructs
{
    public class S3Construct : Construct
    {
        public S3Construct(Construct scope, string nameId) : base(scope, nameId)
        {
            var bucketRole = new Role(this, "CandidateTemplateS3Role", new RoleProps
            {
                AssumedBy = new ServicePrincipal("s3.amazonaws.com")
            });

            var bucket = new Bucket(this, "CandidateTemplateS3", new BucketProps
            {
                BucketName = "None",
                Versioned = false,
                Encryption = BucketEncryption.S3_MANAGED,
                WebsiteIndexDocument = "index.html",
                ObjectLockEnabled = false,
                RemovalPolicy = RemovalPolicy.DESTROY,
                AutoDeleteObjects = true,
                //web
                PublicReadAccess = true,
                BlockPublicAccess = new BlockPublicAccess(new BlockPublicAccessOptions
                {
                    BlockPublicAcls = false,
                    BlockPublicPolicy = false,
                    IgnorePublicAcls = false,
                    RestrictPublicBuckets = false
                }),
                BucketKeyEnabled = false
            });

            bucket.AddCorsRule(new CorsRule
            {
                AllowedHeaders = new[] { "*" },
                AllowedMethods = new[] { HttpMethods.GET, HttpMethods.POST, HttpMethods.PUT, HttpMethods.DELETE },
                AllowedOrigins = new[] { "*" },
                MaxAge = 100
            });
            
                        string bucketArnString = bucket.BucketArn + "/*";

            //Adding Resource Policy
            bucket.AddToResourcePolicy(
                new PolicyStatement(new PolicyStatementProps{
                    Sid = "PublicReadForGetBucketObjects",
                    Effect = Effect.ALLOW,
                    //AWS discourages the use of 'StarPrinicipal'
                    Principals = new [] {new StarPrincipal()},
                    Actions = new [] {"s3:GetObject"},
                    Resources = new [] {bucketArnString}
                })
            );
        }
    }
}
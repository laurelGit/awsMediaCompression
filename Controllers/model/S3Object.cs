using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aws.Media.Convert.Api.Controllers.model
{
    public class S3Object
    {
        public string Name {get; set;} = null!;
        public MemoryStream InputStream {get; set;} = null!;
        public string BucketName {get; set;} = null!;
    }
}
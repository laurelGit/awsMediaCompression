using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aws.Media.Convert.Api.Model
{
    public class S3ResponseDto
    {
        public int StatusCode {get; set;}
        public string Message {get; set;} = null!;
    }
}
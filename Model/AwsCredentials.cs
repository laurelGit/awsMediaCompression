using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aws.Media.Convert.Api.Model
{
    public class AwsCredentials
    {
        public string AccessKey {get; set;} = "";
        public string SecretKey {get; set;} = "";
    }
}
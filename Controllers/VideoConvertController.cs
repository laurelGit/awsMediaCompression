using Amazon;
using Amazon.MediaConvert;
using Amazon.MediaConvert.Model;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Aws.Media.Convert.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Aws.Media.Convert.Api.Services;

namespace Aws.Media.Convert.Api.Controllers
{
    [ApiController]
    [Route("api/video")]
    public class VideoConvertController : ControllerBase
    {
        // private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _config;
        private readonly IAmazonS3 _s3Client;
        private readonly RegionEndpoint region;
        private readonly BasicAWSCredentials credentials;
        private readonly AwsCredentials cred;
        private readonly AmazonS3Config regionConfig;

        public VideoConvertController(IConfiguration config){
            _config = config;
            cred = new AwsCredentials(){
                AccessKey = _config["AwsConfiguration:AWSAccessKey"],
                SecretKey = _config["AwsConfiguration:AWSSecretKey"]
            };
            regionConfig = new AmazonS3Config()
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            };
            credentials = new BasicAWSCredentials(cred.AccessKey, cred.SecretKey);
            _s3Client = new AmazonS3Client(credentials, regionConfig);
            region = RegionEndpoint.USEast1;
        }

        [HttpPost]
        public async Task<ActionResult> ConvertAsync(string filename)
        {

            // Console.WriteLine($" AccessKeyId :{cred.AccessKey} SecretKey :{cred.SecretKey} ");
            String mediaConvertRole = "arn:aws:iam::820582469945:role/MediaConcertRoleSdk";
            String inputBucket = "s3://us-video-vod-input";
            String outputBucket = "s3://us-video-vod-output/_720X500";
            String mediaConvertEndpoint = "";

            // var filename = args[0];
            var prefix = filename.Substring(0, filename.LastIndexOf("."));

            // Obtain the customer-specific MediaConvert endpoint and create MediaConvert client
            AmazonMediaConvertClient client = new AmazonMediaConvertClient(credentials, region);
            DescribeEndpointsRequest describeRequest = new DescribeEndpointsRequest();
            DescribeEndpointsResponse describeResponse = await client.DescribeEndpointsAsync(describeRequest);
            mediaConvertEndpoint = describeResponse.Endpoints[0].Url;
            Console.WriteLine($"MediaConvert endpoint: {mediaConvertEndpoint}");
            client = new AmazonMediaConvertClient(credentials, new AmazonMediaConvertConfig { ServiceURL = mediaConvertEndpoint});

            // Create job request
            CreateJobRequest createJobRequest = new CreateJobRequest();
            createJobRequest.Role = mediaConvertRole;
            createJobRequest.UserMetadata.Add("Customer", "Amazon");

            JobSettings jobSettings = new JobSettings();
            jobSettings.AdAvailOffset = 0;
            jobSettings.TimecodeConfig = new TimecodeConfig();
            jobSettings.TimecodeConfig.Source = TimecodeSource.EMBEDDED;
            createJobRequest.Settings = jobSettings;

            OutputGroup ofg = new OutputGroup();
            ofg.Name = "File Group";
            ofg.OutputGroupSettings = new OutputGroupSettings();
            ofg.OutputGroupSettings.Type = OutputGroupType.FILE_GROUP_SETTINGS;
            ofg.OutputGroupSettings.FileGroupSettings = new FileGroupSettings();
            ofg.OutputGroupSettings.FileGroupSettings.Destination = outputBucket + "/" + prefix;

            #region Video description
            VideoDescription vdes = new VideoDescription();
            vdes.ScalingBehavior = ScalingBehavior.DEFAULT;
            vdes.TimecodeInsertion = VideoTimecodeInsertion.DISABLED;
            vdes.AntiAlias = AntiAlias.ENABLED;
            vdes.Sharpness = 50;
            vdes.Height = 500;
            vdes.Width = 720;
            vdes.AfdSignaling = AfdSignaling.NONE;
            vdes.DropFrameTimecode = DropFrameTimecode.ENABLED;
            vdes.RespondToAfd = RespondToAfd.NONE;
            vdes.ColorMetadata = ColorMetadata.INSERT;
            vdes.CodecSettings = new VideoCodecSettings();
            vdes.CodecSettings.Codec = VideoCodec.H_264;
            H264Settings h264 = new H264Settings();
            h264.InterlaceMode = H264InterlaceMode.PROGRESSIVE;
            h264.NumberReferenceFrames = 3;
            h264.Syntax = H264Syntax.DEFAULT;
            h264.Softness = 0;
            h264.GopClosedCadence = 1;
            h264.GopSize = 90;
            h264.Slices = 1;
            h264.GopBReference = H264GopBReference.DISABLED;
            h264.SlowPal = H264SlowPal.DISABLED;
            h264.SpatialAdaptiveQuantization = H264SpatialAdaptiveQuantization.ENABLED;
            h264.TemporalAdaptiveQuantization = H264TemporalAdaptiveQuantization.ENABLED;
            h264.FlickerAdaptiveQuantization = H264FlickerAdaptiveQuantization.DISABLED;
            h264.EntropyEncoding = H264EntropyEncoding.CABAC;
            h264.Bitrate = 1000000;
            h264.FramerateControl = H264FramerateControl.SPECIFIED;
            h264.RateControlMode = H264RateControlMode.CBR;
            h264.CodecProfile = H264CodecProfile.MAIN;
            h264.Telecine = H264Telecine.NONE;
            h264.MinIInterval = 0;
            h264.AdaptiveQuantization = H264AdaptiveQuantization.HIGH;
            h264.CodecLevel = H264CodecLevel.AUTO;
            h264.FieldEncoding = H264FieldEncoding.PAFF;
            h264.SceneChangeDetect = H264SceneChangeDetect.ENABLED;
            h264.QualityTuningLevel = H264QualityTuningLevel.SINGLE_PASS;
            h264.FramerateConversionAlgorithm = H264FramerateConversionAlgorithm.DUPLICATE_DROP;
            h264.UnregisteredSeiTimecode = H264UnregisteredSeiTimecode.DISABLED;
            h264.GopSizeUnits = H264GopSizeUnits.FRAMES;
            h264.ParControl = H264ParControl.SPECIFIED;
            h264.NumberBFramesBetweenReferenceFrames = 2;
            h264.RepeatPps = H264RepeatPps.DISABLED;
            h264.FramerateNumerator = 30;
            h264.FramerateDenominator = 1;
            h264.ParNumerator = 1;
            h264.ParDenominator = 1;
            #endregion VideoDescription

            #region Audio description
            AudioDescription ades = new AudioDescription();
            ades.LanguageCodeControl = AudioLanguageCodeControl.FOLLOW_INPUT;
            // This name matches one specified in the Inputs below
            ades.AudioSourceName = "Audio Selector 1";
            ades.CodecSettings = new AudioCodecSettings();
            ades.CodecSettings.Codec = AudioCodec.AAC;
            AacSettings aac = new AacSettings();
            aac.AudioDescriptionBroadcasterMix = AacAudioDescriptionBroadcasterMix.NORMAL;
            aac.RateControlMode = AacRateControlMode.CBR;
            aac.CodecProfile = AacCodecProfile.LC;
            aac.CodingMode = AacCodingMode.CODING_MODE_2_0;
            aac.RawFormat = AacRawFormat.NONE;
            aac.SampleRate = 48000;
            aac.Specification = AacSpecification.MPEG4;
            aac.Bitrate = 64000;
            ades.CodecSettings.AacSettings = aac;

            #endregion AudioDescription

            #region Output group

            Output output = new Output()
            {
                NameModifier = "_1_720X500",
                VideoDescription = vdes,
                AudioDescriptions = new List<AudioDescription>(),
                ContainerSettings = new ContainerSettings()
                {
                    Container = ContainerType.MP4,
                    Mp4Settings = new Mp4Settings()
                    {
                        CslgAtom = Mp4CslgAtom.INCLUDE,
                        FreeSpaceBox = Mp4FreeSpaceBox.EXCLUDE,
                        MoovPlacement = Mp4MoovPlacement.PROGRESSIVE_DOWNLOAD
                    }
                }
            };
            output.VideoDescription.CodecSettings.H264Settings = h264;
            output.AudioDescriptions.Add(ades);
            ofg.Outputs.Add(output);

            Output output2 = new Output()
            {
                NameModifier = "_2_720X500",
                AudioDescriptions = new List<AudioDescription>(),
                ContainerSettings = new ContainerSettings()
                {
                    Container = ContainerType.MOV,
                    MovSettings = new MovSettings()
                    {
                        CslgAtom = MovCslgAtom.INCLUDE,
                    }
                }
            };
            output2.AudioDescriptions.Add(ades);
            output2.VideoDescription = vdes;
            ofg.Outputs.Add(output2);
            createJobRequest.Settings.OutputGroups.Add(ofg);
            #endregion Output group

            #region Input
            Input input = new Input();
            input.FilterEnable = InputFilterEnable.AUTO;
            input.PsiControl = InputPsiControl.USE_PSI;
            input.FilterStrength = 0;
            input.DeblockFilter = InputDeblockFilter.DISABLED;
            input.DenoiseFilter = InputDenoiseFilter.DISABLED;
            input.TimecodeSource = InputTimecodeSource.EMBEDDED;
            input.FileInput = inputBucket + "/" + filename;

            AudioSelector audsel = new AudioSelector();
            audsel.Offset = 0;
            audsel.DefaultSelection = AudioDefaultSelection.NOT_DEFAULT;
            audsel.ProgramSelection = 1;
            audsel.SelectorType = AudioSelectorType.TRACK;
            audsel.Tracks.Add(1);
            input.AudioSelectors.Add("Audio Selector 1", audsel);

            input.VideoSelector = new VideoSelector();
            input.VideoSelector.ColorSpace = ColorSpace.FOLLOW;

            createJobRequest.Settings.Inputs.Add(input);
            #endregion Input

            // Create job

            try
            {
                Console.WriteLine("Creating MediaConvert job");
                CreateJobResponse createJobResponse = await client.CreateJobAsync(createJobRequest);
                Console.WriteLine("Job Id: {0}", createJobResponse.Job.Id);
            }
            catch (BadRequestException bre)
            {
                Console.WriteLine($"BadRequestException: {bre.Message}");
                // If the endpoint was bad
                if (bre.Message.StartsWith("You must use the customer-"))
                {
                    // The exception contains the correct endpoint; extract it
                    mediaConvertEndpoint = bre.Message.Split('\'')[1];
                }
            }

            return Ok("Sucessfull");
        }

        
        /// <summary>
        /// Shows how to create a new Amazon S3 bucket.
        /// </summary>
        /// <param name="client">An initialized Amazon S3 client object.</param>
        /// <param name="bucketName">The name of the bucket to create.</param>
        /// <returns>A boolean value representing the success or failure of
        /// the bucket creation process.</returns>
        [HttpPost("create")]
        public async Task<bool> CreateBucketAsync(string bucketName)
        {
            try
            {
                var request = new PutBucketRequest
                {
                    BucketName = bucketName,
                    UseClientRegion = true,
                };

                var response = await _s3Client.PutBucketAsync(request);
                return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error creating bucket: '{ex.Message}'");
                return false;
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllBucketAsync()
        {
            var client = new AmazonS3Client(credentials, regionConfig);
            var data = await _s3Client.ListBucketsAsync();
            var buckets = data.Buckets.Select(b => { return b.BucketName; });
            return Ok(buckets);
        }

        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            // Process file
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            var fileExt = Path.GetExtension(file.FileName);
            var docName = $"{Guid.NewGuid}.{fileExt}";
            // call server

            var s3Obj = new CS3Object() {
                BucketName = "us-video-vod-output",
                InputStream = memoryStream,
                Name = docName
            };
            var client = new AmazonS3Client(credentials, regionConfig);
            var _storageService = new StorageService();
            var result = await _storageService.UploadFileAysnc(s3Obj, credentials, client);
            // 
            return Ok(result);

        }



    }
}
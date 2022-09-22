using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using Helpers;

namespace cloudautomation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Loading Azure configuration files
            var config = LoadAppSettings();
            if (config == null)
            {
                Console.WriteLine("Invalid appsettings.json file.");
                return;
            }

            // Ask for authentication
            var client = GetAuthenticatedGraphClient(config);
            var profileResponse = client.Me.Request().GetAsync().Result;
            Console.WriteLine("Hello " + profileResponse.DisplayName);

            // Prepare to upload
            var fileName = "dumpsIndemetal.zip";
            var currentFolder = System.IO.Directory.GetCurrentDirectory();
            var filePath = Path.Combine(currentFolder, fileName);

            // Load resource as a stream
            using (Stream fileStream = new FileStream(filePath, FileMode.Open))
            {
                GraphServiceClient graphClient = GetAuthenticatedGraphClient(config);
                var uploadSession = graphClient.Me.Drive.Root
                                               .ItemWithPath(fileName)
                                               .CreateUploadSession()
                                               .Request()
                                               .PostAsync()
                                               .Result;
                // Create upload task
                var maxChunkSize = 2000 * 1024; // 2GB max size
                var largeUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, fileStream, maxChunkSize);

                // Create upload progress reporter
                IProgress<long> uploadProgress = new Progress<long>(uploadBytes =>
                {
                    Console.WriteLine($"Uploaded {uploadBytes} bytes of {fileStream.Length} bytes");
                });

                // Upload file
                UploadResult<DriveItem> uploadResult = largeUploadTask.UploadAsync(uploadProgress).Result;
                if (uploadResult.UploadSucceeded)
                {
                    Console.WriteLine("File uploaded to user's OneDrive root folder.");
                }
            }
        }

        private static IConfigurationRoot? LoadAppSettings()
        {
            try
            {
                var config = new ConfigurationBuilder()
                                  .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", false, true)
                                  .Build();

                if (string.IsNullOrEmpty(config["applicationId"]) ||
                    string.IsNullOrEmpty(config["tenantId"]))
                {
                    return null;
                }

                return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }

        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config)
        {
            var clientId = config["applicationId"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .WithDefaultRedirectUri()
                                                    .Build();
            return MsalAuthenticationProvider.GetInstance(cca, scopes.ToArray());
        }

        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config)
        {
            var authenticationProvider = CreateAuthorizationProvider(config);
            var graphClient = new GraphServiceClient(authenticationProvider);
            return graphClient;
        }
    }
}
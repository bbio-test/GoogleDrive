using Blackbird.Applications.Sdk.Common.Authentication;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.GoogleDrive
{
    public class GoogleDriveClient : DriveService
    {
        private static Initializer GetInitializer(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var serviceAccountConfString = authenticationCredentialsProviders.First(p => p.KeyName == "serviceAccountConfString").Value;
            string[] scopes = { DriveService.Scope.Drive };
            ServiceAccountCredential? credential = GoogleCredential.FromJson(serviceAccountConfString)
                                                  .CreateScoped(scopes)
                                                  .UnderlyingCredential as ServiceAccountCredential;
            return new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Blackbird"
            };
        }

        public GoogleDriveClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) : base (GetInitializer(authenticationCredentialsProviders)) { }
    }
}

using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using Microsoft.AspNetCore.WebUtilities;

namespace Apps.GoogleDrive.Auth.OAuth2
{
    public class OAuth2AuthorizeService : IOAuth2AuthorizeService
    {
        public string GetAuthorizationUrl(Dictionary<string, string> values)
        {
            const string oauthUrl = "https://accounts.google.com/o/oauth2/v2/auth";
            var parameters = new Dictionary<string, string>
            {
                { "client_id", ApplicationConstants.ClientId },
                { "redirect_uri", ApplicationConstants.RedirectUri },
                { "response_type", "code" },
                { "scope", ApplicationConstants.Scope },
                { "state", values["state"] },
                { "access_type", "offline" }
            };
            return QueryHelpers.AddQueryString(oauthUrl, parameters);
        }
    }
}

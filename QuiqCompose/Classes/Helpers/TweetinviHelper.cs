using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using SDSK.QuiqCompose.WinDesktop.Classes.Helpers.Security;
using Tweetinvi;
using Tweetinvi.Core.Public.Parameters;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace SDSK.QuiqCompose.WinDesktop.Classes.Helpers {
    public static class TweetinviHelper {
        public static void SetCredentialsWithTokens(SecureString accessToken, SecureString accessTokenSecret, bool applicationWide = true)
            => SetCredentialsWithTokens(accessToken.ReadAsString(), accessTokenSecret.ReadAsString(), applicationWide);

        public static void SetCredentialsWithTokens(string accessToken, string accessTokenSecret, bool applicationWide = true) {
            if(applicationWide) {
                Auth.ApplicationCredentials = Auth.CreateCredentials(ApplicationData.Instance.TwitterApp.ConsumerKey.ReadAsString(),
                    ApplicationData.Instance.TwitterApp.ConsumerSecretKey.ReadAsString(),
                    accessToken,
                    accessTokenSecret);
            } else {
                Auth.Credentials = Auth.CreateCredentials(ApplicationData.Instance.TwitterApp.ConsumerKey.ReadAsString(),
                    ApplicationData.Instance.TwitterApp.ConsumerSecretKey.ReadAsString(),
                    accessToken,
                    accessTokenSecret);
            }
        }

        public static Tuple<bool, IAuthenticatedUser> SetCredentialsWithPin(IAuthenticationContext context, string pin, bool applicationWide = true) {
            var userCreds = AuthFlow.CreateCredentialsFromVerifierCode(pin, context);

            if(userCreds != null) {
                var authedUser = User.GetAuthenticatedUser(userCreds);
                if(applicationWide) {
                    Auth.ApplicationCredentials = userCreds;

                    if(Auth.ApplicationCredentials.AccessToken == userCreds.AccessToken &&
                        Auth.ApplicationCredentials.AccessTokenSecret == userCreds.AccessTokenSecret) {
                        return Tuple.Create(true, authedUser);
                    }
                } else {
                    Auth.Credentials = userCreds;

                    if(Auth.Credentials.AccessToken == userCreds.AccessToken &&
                        Auth.Credentials.AccessTokenSecret == userCreds.AccessTokenSecret) {
                        return Tuple.Create(true, authedUser);
                    }
                }
            }

            return Tuple.Create<bool, IAuthenticatedUser>(false, null);
        }

        public static ITwitterCredentials GetCredentials(bool applicationWide = true)
            => applicationWide ? Auth.CredentialsAccessor.ApplicationCredentials : Auth.CredentialsAccessor.CurrentThreadCredentials;

        public static IAuthenticatedUser GetAuthenticatedUser(bool applicationWide = true)
            => User.GetAuthenticatedUser(GetCredentials(applicationWide));

        public static IAuthenticatedUser GetAuthedUserById(long accId) {
            var decryptedAccessTokens = ConfigurationManager.ConfigurationInstance.DecryptAccessTokens(accId);

            return User.GetAuthenticatedUser(Auth.CreateCredentials(ApplicationData.Instance.TwitterApp.ConsumerKey.ReadAsString(),
                ApplicationData.Instance.TwitterApp.ConsumerSecretKey.ReadAsString(),
                decryptedAccessTokens.Item1.ReadAsString(),
                decryptedAccessTokens.Item2.ReadAsString()));
        }

        public static Task<bool> PublishTweetAsync(IPublishTweetParameters optionalParameters = null)
            => Task.Factory.StartNew(() => {
                Auth.SetCredentials(Auth.ApplicationCredentials);

                if(Tweet.PublishTweet(optionalParameters) != null) {
                    return true;
                }

                return false;
            });

        public static Task<IMedia> UploadImageAsync(byte[] imageByte, IUploadOptionalParameters optionalParameters = null)
            => Task.Run(async () => {
                Auth.SetCredentials(Auth.ApplicationCredentials);

                if(await CheckImageCanBeUploaded(imageByte)) {
                    return Upload.UploadBinary(imageByte, optionalParameters);
                }

                return null;
            });

        public static async Task<bool> CheckImageCanBeUploaded(byte[] imageByte) {
            byte[] headerBytes = null;

            using(MemoryStream stream = new MemoryStream(imageByte)) {
                if(stream.CanRead && stream.CanSeek) {
                    headerBytes = new byte[8];
                    await stream.ReadAsync(headerBytes, 0, headerBytes.Length);
                }
            }

            if(headerBytes != null) {
                string headerHexString = string.Concat(headerBytes.Select(v => v.ToString("X2")).ToArray());

                if(headerHexString.StartsWith(ImageCacheHelper.ImageHeaderInfo.JPG)
                    || headerHexString.StartsWith(ImageCacheHelper.ImageHeaderInfo.PNG)
                    || headerHexString.StartsWith(ImageCacheHelper.ImageHeaderInfo.WEBP)) {
                    return true;
                }
            }

            return false;
        }
    }
}

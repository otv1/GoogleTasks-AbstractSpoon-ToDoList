using System.Configuration;

namespace SyncGoogleTasksAbstractSpoon
{
    internal static class GoogleApiSettings
    {
        /// <summary>
        /// The Google OAuth2.0 Client ID of your project.
        /// </summary>
        public static string ClientId
        {
            get { return ConfigurationManager.AppSettings["ClientId"]; }

        }

        /// <summary>
        /// The Google OAuth2.0 Client secret of your project.
        /// </summary>
        public static string ClientSecret
        {
            get { return ConfigurationManager.AppSettings["ClientSecret"]; }

        }
    }
}

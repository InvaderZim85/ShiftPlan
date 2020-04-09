using System.IO;
using System.Net;
using ShiftPlan.Core.DataObjects;

namespace ShiftPlan.Core.Business
{
    /// <summary>
    /// Provides the functions for the interaction with the ftp server
    /// </summary>
    public static class FtpManager
    {
        /// <summary>
        /// Uploads the given file
        /// </summary>
        /// <param name="filepath">The file which should be uploaded</param>
        /// <param name="settings">The settings of the ftp server</param>
        public static void Upload(FileInfo filepath, FtpSettings settings)
        {
            ServiceLogger.Info($"Upload '{filepath.Name}'");
            var destination = CreateDestinationName(settings.Server);

            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(settings.User, settings.Password);
                client.UploadFile(destination, WebRequestMethods.Ftp.UploadFile, filepath.FullName);
            }
        }

        /// <summary>
        /// Creates the path for the destination (ftp server + index.html)
        /// </summary>
        /// <param name="server">The path of the server</param>
        /// <returns>The destination name</returns>
        private static string CreateDestinationName(string server)
        {
            if (!server.EndsWith("/"))
                server += "/";

            return $"{server}index.html";
        }
    }
}

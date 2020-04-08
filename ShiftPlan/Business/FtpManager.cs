using System.IO;
using System.Net;
using ShiftPlan.Global;

namespace ShiftPlan.Business
{
    /// <summary>
    /// Provides the functions for the interaction with the ftp server
    /// </summary>
    internal static class FtpManager
    {
        /// <summary>
        /// Uploads the given file
        /// </summary>
        /// <param name="filepath">The file which should be uploaded</param>
        public static void Upload(FileInfo filepath)
        {
            ServiceLogger.Info($"Upload '{filepath.Name}'");
            var destination = CreateDestinationName();

            using (var client = new WebClient())
            {
                client.Credentials = new NetworkCredential(Helper.Settings.Ftp.User, Helper.Settings.Ftp.Password);
                client.UploadFile(destination, WebRequestMethods.Ftp.UploadFile, filepath.FullName);
            }
        }

        /// <summary>
        /// Creates the path for the destination (ftp server + index.html)
        /// </summary>
        /// <returns>The destination name</returns>
        private static string CreateDestinationName()
        {
            var server = Helper.Settings.Ftp.Server;
            if (!server.EndsWith("/"))
                server += "/";

            return $"{server}index.html";
        }
    }
}

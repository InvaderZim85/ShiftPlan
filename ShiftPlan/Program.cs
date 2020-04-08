using System;
using System.Linq;
using ShiftPlan.Business;
using ShiftPlan.Global;
using Topshelf;
using ZimLabs.Utility.Extensions;

namespace ShiftPlan
{
    /// <summary>
    /// Provides the main entry point of the program
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point of the program
        /// </summary>
        /// <param name="args">The command line arguments</param>
        private static void Main(string[] args)
        {
            try
            {
                if (args.Any(a => a.ContainsIgnoreCase("debug")))
                {
                    var manager = new ServiceManager();
                    manager.Start();

                    Console.WriteLine("Press enter to exit.");
                    Console.ReadLine();

                    manager.Stop();
                }
                else
                {
                    HostFactory.Run(c =>
                    {
                        c.Service<ServiceManager>(s =>
                        {
                            s.ConstructUsing(sf => new ServiceManager());
                            s.WhenStarted(sa => sa.Start());
                            s.WhenStopped(sa => sa.Stop());
                        });

                        c.UseNLog();
                        c.RunAsLocalSystem();
                        c.SetServiceName(Helper.Settings.Service.ServiceName);
                        c.SetDisplayName(Helper.Settings.Service.DisplayName);
                        c.SetDescription(Helper.Settings.Service.Description);
                    });
                }
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(
                    "An error has occured. Please check exception message for more information.",
                    ex);
                Console.ReadLine();
            }
        }
    }
}

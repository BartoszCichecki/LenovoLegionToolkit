using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class UpdateChecker
    {
        public static async Task CheckUpdates()
        {
            try
            {
                var githubClient = new GitHubClient(new ProductHeaderValue("LenovoLegionToolkit-UpdateChecker"));
                var releases = await githubClient.Repository.Release.GetAll("BartoszCichecki", "LenovoLegionToolkit");

                var newestRelease = releases
                    .Select(r => Version.Parse(r.TagName))
                    .OrderByDescending(r => r)
                    .FirstOrDefault();

                var thisRelease = Assembly.GetEntryAssembly().GetName().Version;

                if (thisRelease < newestRelease)
                    Console.WriteLine("TADA!");
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Octokit;

namespace LenovoLegionToolkit.Lib.Utils
{
    public static class Updates
    {
        public static async Task<bool> Check()
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

                return thisRelease < newestRelease;
            }
            catch
            {
                return false;
            }
        }
    }
}

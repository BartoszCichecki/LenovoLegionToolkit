using Octokit;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LenovoLegionToolkit.Lib.Utils
{
    public class UpdateChecker
    {
        public async Task<bool> CheckUpdates()
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

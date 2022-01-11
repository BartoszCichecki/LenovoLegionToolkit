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
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Checking...");

                var githubClient = new GitHubClient(new ProductHeaderValue("LenovoLegionToolkit-UpdateChecker"));
                var releases = await githubClient.Repository.Release.GetAll("BartoszCichecki", "LenovoLegionToolkit").ConfigureAwait(false);

                var newestRelease = releases
                    .Select(r => Version.Parse(r.TagName))
                    .OrderByDescending(r => r)
                    .FirstOrDefault();

                var thisRelease = Assembly.GetEntryAssembly()?.GetName().Version;

                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Checked [thisRelease={thisRelease}, newestRelease={newestRelease}]");

                return thisRelease < newestRelease;
            }
            catch (Exception ex)
            {
                if (Log.Instance.IsTraceEnabled)
                    Log.Instance.Trace($"Error: {ex}");
                return false;
            }
        }
    }
}

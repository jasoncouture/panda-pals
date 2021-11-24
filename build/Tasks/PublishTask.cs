using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.GitVersion;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Publish")]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestTask))]
public class PublishTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.PackageRelease;

    public override void Run(BuildContext context)
    {
        context.Information("Deleting existing publish output for {0} (if any)", context.RuntimeIdentifier);
        context.EnsureDirectoryDoesNotExist(System.IO.Path.Combine(context.WorkingDirectory, "output", context.RuntimeIdentifier));
        var gitVersion = context.CurrentGitVersionData;
        context.Information("Publishing for {0}: {1}", context.MsBuildConfiguration, gitVersion.FullBuildMetaData);
        context.DotNetPublish(context.Project, context.GetDotNetPublishSettings());
    }
}
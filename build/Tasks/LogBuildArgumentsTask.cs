using Cake.Common.Diagnostics;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("LogBuildArguments")]
public class LogBuildArgumentsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.Information("Configuration - {0}", context.MsBuildConfiguration);
        context.Information("OS Target - {0}", context.RuntimeIdentifier);
        context.Information("Running tests? {0}", context.RunTests ? "Yes" : "No");
        context.Information("Restore Packages? {0}", context.Restore ? "Yes" : "No");
        context.Information("Working Directory: {0}", context.WorkingDirectory);
        context.Information("Project: {0}", context.Project);
    }
}
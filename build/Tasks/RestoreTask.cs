using Cake.Common.Tools.DotNet;
using Cake.Common.Tools.DotNet.Tool;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("RestoreTools")]
public class RestoreToolsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        context.DotNetTool("restore", new DotNetToolSettings()
        {
            WorkingDirectory = context.WorkingDirectory
        });
    }
}

[TaskName("Restore")]
[IsDependentOn(typeof(RestoreToolsTask))]
public class RestoreTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.Restore;

    public override void Run(BuildContext context)
    {
        context.DotNetRestore(context.GetDotNetRestoreSettings());
    }
}
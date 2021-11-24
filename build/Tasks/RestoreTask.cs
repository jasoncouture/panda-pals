using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

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
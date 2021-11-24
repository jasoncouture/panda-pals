using Cake.Common.Tools.DotNet;
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
        context.DotNetPublish(context.Project, context.GetDotNetPublishSettings());
    }
}
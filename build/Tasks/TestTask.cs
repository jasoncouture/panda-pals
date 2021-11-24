using Cake.Common.Tools.DotNet;
using Cake.Frosting;

namespace Build.Tasks;

[TaskName("Test")]
[IsDependentOn(typeof(BuildTask))]
public class TestTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.RunTests;

    public override void Run(BuildContext context)
    {
        context.DotNetTest(context.Project, context.GetDotNetTestSettings());
    }
}
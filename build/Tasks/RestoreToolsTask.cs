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
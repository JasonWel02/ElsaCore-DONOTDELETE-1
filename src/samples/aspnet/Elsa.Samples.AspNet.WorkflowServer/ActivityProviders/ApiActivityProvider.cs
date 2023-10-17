using System.Reflection;
using System.Text;
using System.Text.Json;
using Elsa.Expressions.Models;
using Elsa.Extensions;
using Elsa.Http;
using Elsa.Samples.AspNet.WorkflowServer;
using Elsa.Workflows.Core;
using Elsa.Testing.Shared;
using Elsa.Workflows.Core.Contracts;
using Elsa.Workflows.Core.Models;
using Elsa.Workflows.Management;
using Humanizer;

namespace Elsa.Samples.AspNet.WorkflowServer.ActivityProviders;

/// <summary>
/// Provides activities based on API descriptions (see /Data/apis.json).
/// </summary>
public class ApiActivityProvider : IActivityProvider
{
    private readonly IActivityRegistry _activityRegistry;
    private readonly IActivityDescriber _activityDescriber;

    public ApiActivityProvider(IActivityRegistry activityRegistry, IActivityDescriber activityDescriber)
    {
        _activityRegistry = activityRegistry;
        _activityDescriber = activityDescriber;
    }

    public async ValueTask<IEnumerable<ActivityDescriptor>> GetDescriptorsAsync(CancellationToken cancellationToken = default)
    {

        //var services = new ServiceCollection();

        List<ActivityDescriptor> activities = new List<ActivityDescriptor>();
        string[] pluginPaths = new string[]
        {
            "C:\\Users\\jmw\\Desktop\\ELSACOREDONOTDELETE\\elsa-core-3\\LIMM.Custom.Activities\\bin\\Debug\\net7.0\\LIMM.Custom.Activities.dll",
        };

        //// Build service container.
        //var serviceProvider = services.BuildServiceProvider();


        //// Populate registries. This is only necessary for applications  that are not using hosted services.
        //await serviceProvider.PopulateRegistriesAsync();

        var activityDescriptorTasks = pluginPaths.Select(async pluginPaths =>
        {
            var pluginAssembly = LoadPlugin(pluginPaths);
            return await CreateActivityDescription(pluginAssembly, _activityDescriber);
        }).ToList();

        var activityDescriptors = (await Task.WhenAll(activityDescriptorTasks)).SelectMany(x => x).ToList();


        foreach (ActivityDescriptor actdesc in activityDescriptors)
        {

            _activityRegistry.Register(actdesc);
        }
        return activities;
    }


    // Load Assembly
    static Assembly LoadPlugin(string relativePath)
    {
        // Navigate up to the solution root
        string root = Path.GetFullPath(Path.Combine(
            Path.GetDirectoryName(
                Path.GetDirectoryName(
                    Path.GetDirectoryName(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));
        string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        Console.WriteLine($"Loading commands from: {pluginLocation}");
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }
     

    // Create ActivityDescriptor from Assembly
    public async Task<IEnumerable<ActivityDescriptor>> CreateActivityDescription(Assembly assembly, IActivityDescriber describer)
    {

        var descriptors = new List<ActivityDescriptor>();

        foreach (Type type in assembly.GetTypes())
        {
            if (typeof(IActivity).IsAssignableFrom(type))
            {

                var ad = await _activityDescriber.DescribeActivityAsync(type);

                descriptors.Add(ad);
            }
        }

        if (!descriptors.Any())
        {
            string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
            throw new ApplicationException(
                $"Can't find any type which implement IActivity in {assembly} from {assembly.Location}.\n" +
                $"Available types: {availableTypes}");
        }

        return descriptors;
    }

}
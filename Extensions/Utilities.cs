using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Extensions;

public class Utilities
{
    public static AssetBundle GetAssetBundleFromResources(string filename)
    {
        var execAssembly = Assembly.GetExecutingAssembly();
        var resourceName = execAssembly.GetManifestResourceNames()
            .Single(str => str.EndsWith(filename));

        using (var stream = execAssembly.GetManifestResourceStream(resourceName))
        {
            return AssetBundle.LoadFromStream(stream);
        }
    }
}
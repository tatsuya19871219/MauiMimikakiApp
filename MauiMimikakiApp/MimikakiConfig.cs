using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiMimikakiApp;

internal class MimikakiConfig
{
    static Func<string, string> configPath;
    static readonly Assembly assembly;

    static public MimikakiConfig Current;

    public int Dx { get; private set; }
    public int Dy { get; private set; }
    [JsonInclude] public int dt { get; private set; }

    static MimikakiConfig()
    {
        configPath = (filename) =>  $"MauiMimikakiApp.Resources.Raw.{filename}";
        assembly = typeof(MimikakiConfig).GetTypeInfo().Assembly;
    }

    static internal MimikakiConfig Load(string filename)
    {
        using var stream = assembly.GetManifestResourceStream( configPath(filename) );

        if (stream is null) throw new Exception($"The config file ({filename}) is not found");

        using var reader = new StreamReader(stream);

        var contents = reader.ReadToEnd();

        return Current = JsonSerializer.Deserialize<MimikakiConfig>(contents);
    }

}

using System.Reflection;
using System.Text.Json;

namespace MauiMimikakiApp;

internal class MimikakiConfig
{
    static Func<string, string> configPath;
    static readonly Assembly assembly;

    static public MimikakiConfig Current;

    // Properties in JSON config file
    public string KakiSoundFilename { get; init; }
    public double SEcutoffVelocity { get; init; }
    public int TrackerUpdateInterval { get; init; }
    public int GraphicsUpdateInterval { get; init; }
    public ModelParams Params { get; init; }

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

namespace MauiMimikakiApp;

record ModelParams
{
    public TwoDimensionalVector RegionRoughness { get; init; }
    public MimiHairConfig MimiHair { get; init; }
    public double MimiHairDensity { get; init; }
    public MimiDirtConfig MimiDirt { get; init; }
    public RegionProbabilities MimiDirtGenerationRate { get; init; }
    public int TouchPointRadius { get; init; }
}

record TwoDimensionalVector
{
    public int DX { get; init; }
    public int DY { get; init; }
}

record MimiHairConfig
{
    public double Thinness { get; init; }
    public double SpringConst { get; init; }
}

record MimiDirtConfig
{
    public double Size { get; init; }
    public double Hardness { get; init; }
    public string ColorName { get; init; }
}

record RegionProbabilities
{
    public double Outer { get; init; }
    public double Inner { get; init; }
    public double Hole { get; init; }
}

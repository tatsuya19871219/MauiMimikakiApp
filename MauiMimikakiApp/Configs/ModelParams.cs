namespace MauiMimikakiApp;

record ModelParams
{
    public TwoDimensionalVector RegionRoughness { get; init; }
    public MimiHairConfig MimiHair { get; init; }
    public double MimiHairDensity { get; init; }
    public MimiDirtConfig MimiDirt { get; init; }
    public RegionProbabilities MimiDirtGenerationProbs { get; init; }
    public int TouchPointRadius { get; init; }
}

record TwoDimensionalVector(int dx, int dy);
record MimiHairConfig(double thinness, double springConst, string colorName);
record MimiDirtConfig(double size, double hardness, string colorName);
record RegionProbabilities(double outer, double inner, double hole);

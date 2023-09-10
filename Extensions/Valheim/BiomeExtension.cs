namespace Extensions.Valheim;

public static class BiomeExtension
{
    public static string GetLocalizationKey(this Heightmap.Biome biome)
    {
        return "$biome_" + biome.ToString().ToLower();
    }
}
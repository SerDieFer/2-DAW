namespace Vestigio.Utilities
{
    public static class LevelsNaming
    {
        private static readonly Dictionary<int, string> Levels = new()
        {
            { 1, "Common" },
            { 2, "Uncommon" },
            { 3, "Rare" },
            { 4, "Epic" },
            { 5, "Legendary" },
            { 6, "Mythical" },
            { 7, "Divine" },
            { 8, "Ethereal" },
            { 9, "Transcendent" },
            { 10, "Unique" }
        };

        public static string GetLevelName(int level)
        {
            return Levels.TryGetValue(level, out string name) ? name : "Unknown";
        }

        public static int GetExpRequiredForLevel(int level)
        {
            return level switch
            {
                1 => 100,
                2 => 200,
                3 => 300,
                4 => 400,
                5 => 500,
                6 => 600,
                7 => 700,
                8 => 800,
                9 => 900,
                10 => 1000,
                _ => 1000 * (int)Math.Pow(level, 2)
            };
        }
    }
}

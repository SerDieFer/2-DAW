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
                1 => 0,
                2 => 100,
                3 => 200,
                4 => 300,
                5 => 400,
                6 => 500,
                7 => 600,
                8 => 700,
                9 => 800,
                10 => 900,
                _ => 100 * (level - 1)
            };
        }
    }
}

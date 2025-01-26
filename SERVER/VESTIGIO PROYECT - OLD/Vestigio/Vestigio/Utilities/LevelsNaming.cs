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
    }
}

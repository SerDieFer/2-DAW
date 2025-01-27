using System.Collections.Generic;

namespace Vestigio.Utilities
{
    public static class ClothingSizes
    {
        // DICTIONARY FOR UNISEX CLOTHING SIZES (EU STANDARD)
        public static readonly Dictionary<string, string> Sizes = new Dictionary<string, string>
        {
            { "XS", "Extra Small (EU 32-34)" },
            { "S", "Small (EU 36-38)" },
            { "M", "Medium (EU 40-42)" },
            { "L", "Large (EU 44-46)" },
            { "XL", "Extra Large (EU 48-50)" },
            { "XXL", "Double Extra Large (EU 52-54)" },
            { "XXXL", "Triple Extra Large (EU 56-58)" }
        };

        // METHOD TO GET SIZE DESCRIPTION
        public static string GetSizeDescription(string size)
        {
            if (Sizes.ContainsKey(size))
                return Sizes[size];

            return "Unknown Size"; // DEFAULT: UNKNOWN SIZE
        }

        // METHOD TO GET ALL SIZES
        public static Dictionary<string, string> GetAllSizes()
        {
            return Sizes;
        }
    }
}
using Vestigio.Models;

namespace Vestigio.DTO_s
{
    public class ChallengeResponseDto
    {
        public IEnumerable<ChallengeDto> Challenges { get; set; }
        public int Count { get; set; }
        public int UserLevel { get; set; }
    }
    
}

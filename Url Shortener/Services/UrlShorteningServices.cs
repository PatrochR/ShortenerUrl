using Microsoft.EntityFrameworkCore;

namespace Url_Shortener.Services
{
    public class UrlShorteningServices
    {
        public const int NumberOfCharsInShortLink = 7;
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private readonly Random _random = new Random();
        private readonly ApplicationContext _context;

        public UrlShorteningServices(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateUniqueCode()
        {
            var codeChars = new char[NumberOfCharsInShortLink];

            while (true)
            {
                for (int i = 0; i < NumberOfCharsInShortLink; i++)
                {
                    var alphabetIndex = _random.Next(Alphabet.Length - 1);
                    codeChars[i] = Alphabet[alphabetIndex];
                }

                string code = new string(codeChars);

                if (!await _context.ShortenedUrls.AnyAsync(s => s.Code == code))
                {
                    return code;
                }

            }
        }
    }
}

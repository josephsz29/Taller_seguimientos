using System.Security.Cryptography;
using MemoryGame.Data;
using MemoryGame.Models;
using Microsoft.EntityFrameworkCore;

namespace MemoryGame.Services
{
    public class GameService : IGameService
    {
        private readonly AppDbContext _context;

        public GameService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Game> StartAsync(Difficulty difficulty)
        {
            var settings = DifficultySettings.Get(difficulty);

            // 1) Elegimos al azar los símbolos de esta partida
            var symbolIds = Enumerable.Range(0, CardSymbols.All.Count).ToList();
            Shuffle(symbolIds);

            // 2) Cada símbolo se repite dos veces (una pareja)
            var cards = symbolIds
                .Take(settings.Pairs)
                .SelectMany(id => new[] { id, id })
                .ToList();

            // 3) Mezclamos todas las cartas
            Shuffle(cards);

            var game = new Game
            {
                Id = Guid.NewGuid(),
                Difficulty = difficulty,
                Layout = string.Join(",", cards),
                MatchedCards = new string('0', cards.Count),
                CreatedAt = DateTime.Now
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            return game;
        }

        public async Task<Game?> GetAsync(Guid id)
        {
            return await _context.Games.FindAsync(id);
        }

        public async Task<FlipResult> FlipAsync(Guid id, int cardIndex)
        {
            var game = await _context.Games.FindAsync(id);

            if (game == null)
            {
                return FlipResult.Fail("Partida no encontrada.");
            }

            if (game.FinishedAt.HasValue)
            {
                return FlipResult.Fail("La partida ya terminó.");
            }

            var symbolIds = game.SymbolIds;

            // El servidor valida todo: nunca confiamos en lo que envía el navegador
            if (cardIndex < 0 || cardIndex >= symbolIds.Length)
            {
                return FlipResult.Fail("Carta no válida.");
            }

            if (game.MatchedCards[cardIndex] == '1')
            {
                return FlipResult.Fail("Esa carta ya está emparejada.");
            }

            if (game.PendingCardIndex == cardIndex)
            {
                return FlipResult.Fail("Esa carta ya está volteada.");
            }

            var now = DateTime.Now;

            // El tiempo empieza con la primera carta volteada
            game.StartedAt ??= now;

            var result = new FlipResult
            {
                Success = true,
                CardIndex = cardIndex,
                Symbol = CardSymbols.Get(symbolIds[cardIndex])
            };

            if (game.PendingCardIndex == null)
            {
                // Primera carta del intento: queda esperando a la segunda
                game.PendingCardIndex = cardIndex;
            }
            else
            {
                // Segunda carta: se completa un movimiento
                var firstIndex = game.PendingCardIndex.Value;
                game.PendingCardIndex = null;
                game.Moves++;

                result.IsSecondFlip = true;
                result.FirstCardIndex = firstIndex;
                result.IsMatch = symbolIds[firstIndex] == symbolIds[cardIndex];

                if (result.IsMatch)
                {
                    var matched = game.MatchedCards.ToCharArray();
                    matched[firstIndex] = '1';
                    matched[cardIndex] = '1';
                    game.MatchedCards = new string(matched);

                    // Si ya no quedan cartas sin emparejar, la partida terminó
                    if (!game.MatchedCards.Contains('0'))
                    {
                        game.FinishedAt = now;
                        game.Score = CalculateScore(game);

                        result.IsFinished = true;
                        result.Score = game.Score;
                        result.DurationLabel = game.DurationLabel;
                    }
                }
            }

            result.Moves = game.Moves;
            result.MatchedPairs = game.MatchedCards.Count(c => c == '1') / 2;

            await _context.SaveChangesAsync();
            return result;
        }

        public async Task<bool> SaveScoreAsync(Guid id, string playerName)
        {
            var game = await _context.Games.FindAsync(id);

            // Solo se puede guardar el puntaje de una partida terminada y que aún no se guardó
            if (game == null || !game.FinishedAt.HasValue || game.PlayerName != null)
            {
                return false;
            }

            game.PlayerName = playerName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Game>> GetLeaderboardAsync(Difficulty difficulty, int count)
        {
            // Mejor puntaje primero; si hay empate, gana quien usó menos movimientos
            return await _context.Games
                .Where(g => g.Difficulty == difficulty && g.PlayerName != null && g.Score != null)
                .OrderByDescending(g => g.Score)
                .ThenBy(g => g.Moves)
                .Take(count)
                .ToListAsync();
        }

        // Puntaje = parejas x 100, menos 10 por cada movimiento de más, menos 1 por segundo (mínimo 0)
        private static int CalculateScore(Game game)
        {
            var pairs = DifficultySettings.Get(game.Difficulty).Pairs;
            var extraMoves = Math.Max(0, game.Moves - pairs);
            var seconds = game.DurationSeconds ?? 0;

            return Math.Max(0, pairs * 100 - extraMoves * 10 - seconds);
        }

        // Mezcla (Fisher-Yates) con un generador criptográficamente seguro
        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}

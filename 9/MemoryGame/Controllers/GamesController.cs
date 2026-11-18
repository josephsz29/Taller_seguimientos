using MemoryGame.Models;
using MemoryGame.Services;
using Microsoft.AspNetCore.Mvc;

namespace MemoryGame.Controllers
{
    public class GamesController : Controller
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService;
        }

        // GET: /Games  -> elegir el nivel de dificultad
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Games/Start  -> crea una partida nueva y abre el tablero
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(Difficulty difficulty)
        {
            if (!ModelState.IsValid || !Enum.IsDefined(typeof(Difficulty), difficulty))
            {
                return RedirectToAction(nameof(Index));
            }

            var game = await _gameService.StartAsync(difficulty);
            return RedirectToAction(nameof(Play), new { id = game.Id });
        }

        // GET: /Games/Play/{id}  -> tablero (si se recarga la página, la partida continúa donde iba)
        [HttpGet]
        public async Task<IActionResult> Play(Guid id)
        {
            var game = await _gameService.GetAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            return View(BuildBoard(game));
        }

        // POST: /Games/Flip  -> el navegador pide voltear una carta y recibe el resultado en JSON
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Flip(Guid id, int cardIndex)
        {
            var result = await _gameService.FlipAsync(id, cardIndex);
            return Json(result);
        }

        // POST: /Games/SaveScore  -> guarda el puntaje en el ranking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveScore(Guid id, string? playerName)
        {
            var name = playerName?.Trim();

            if (string.IsNullOrEmpty(name) || name.Length > 30)
            {
                TempData["Error"] = "Escribe tu nombre (máximo 30 caracteres).";
                return RedirectToAction(nameof(Play), new { id });
            }

            var game = await _gameService.GetAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            var saved = await _gameService.SaveScoreAsync(id, name);

            if (!saved)
            {
                TempData["Error"] = "No se pudo guardar el puntaje.";
                return RedirectToAction(nameof(Play), new { id });
            }

            TempData["Success"] = "¡Puntaje guardado!";
            return RedirectToAction(nameof(Leaderboard), new { difficulty = game.Difficulty });
        }

        // GET: /Games/Leaderboard?difficulty=Easy  -> mejores puntajes de un nivel
        [HttpGet]
        public async Task<IActionResult> Leaderboard(Difficulty? difficulty)
        {
            var selected = difficulty ?? Difficulty.Easy;

            var model = new LeaderboardViewModel
            {
                Difficulty = selected,
                Entries = await _gameService.GetLeaderboardAsync(selected, 10)
            };

            return View(model);
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }

        // Convierte la partida en lo que se envía al navegador.
        // Solo se incluye el símbolo de las cartas reveladas: las demás permanecen ocultas.
        private static GameBoardViewModel BuildBoard(Game game)
        {
            var settings = DifficultySettings.Get(game.Difficulty);
            var symbolIds = game.SymbolIds;
            var cards = new List<BoardCard>();

            for (int i = 0; i < symbolIds.Length; i++)
            {
                var isMatched = game.MatchedCards[i] == '1';
                var isRevealed = isMatched || game.PendingCardIndex == i;

                cards.Add(new BoardCard
                {
                    Index = i,
                    IsMatched = isMatched,
                    IsRevealed = isRevealed,
                    Symbol = isRevealed ? CardSymbols.Get(symbolIds[i]) : null
                });
            }

            // Segundos transcurridos (para que el cronómetro continúe si se recarga la página)
            var elapsedSeconds = 0;
            if (game.StartedAt.HasValue)
            {
                var end = game.FinishedAt ?? DateTime.Now;
                elapsedSeconds = (int)Math.Round((end - game.StartedAt.Value).TotalSeconds);
            }

            return new GameBoardViewModel
            {
                GameId = game.Id,
                Difficulty = game.Difficulty,
                DifficultyLabel = settings.Label,
                Columns = settings.Columns,
                TotalPairs = settings.Pairs,
                MatchedPairs = game.MatchedCards.Count(c => c == '1') / 2,
                Moves = game.Moves,
                IsStarted = game.StartedAt.HasValue,
                IsFinished = game.FinishedAt.HasValue,
                ElapsedSeconds = elapsedSeconds,
                Score = game.Score,
                DurationLabel = game.DurationLabel,
                PlayerName = game.PlayerName,
                Cards = cards
            };
        }
    }
}

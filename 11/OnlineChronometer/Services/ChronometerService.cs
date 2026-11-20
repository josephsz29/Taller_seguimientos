using Microsoft.EntityFrameworkCore;
using OnlineChronometer.Data;
using OnlineChronometer.Models;

namespace OnlineChronometer.Services
{
    public class ChronometerService : IChronometerService
    {
        private readonly AppDbContext _context;

        public ChronometerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChronometerSummary>> GetSummariesAsync()
        {
            // La base de datos trae los datos y cuenta las vueltas (COUNT)
            var rows = await _context.Chronometers
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.AccumulatedMilliseconds,
                    c.RunningSince,
                    c.CreatedAt,
                    LapCount = c.Laps.Count
                })
                .ToListAsync();

            var nowUtc = DateTime.UtcNow;

            // El tiempo transcurrido se calcula aquí, con la misma hora para todos
            return rows.Select(r =>
            {
                var elapsed = r.AccumulatedMilliseconds;
                if (r.RunningSince.HasValue)
                {
                    elapsed += Math.Max(0, (long)Math.Round((nowUtc - r.RunningSince.Value).TotalMilliseconds));
                }

                return new ChronometerSummary
                {
                    Id = r.Id,
                    Name = r.Name,
                    IsRunning = r.RunningSince.HasValue,
                    ElapsedMilliseconds = elapsed,
                    LapCount = r.LapCount,
                    CreatedAt = r.CreatedAt
                };
            }).ToList();
        }

        public async Task<Chronometer> CreateAsync(string? name)
        {
            // Si no se escribe nombre, usamos "Cronómetro 1", "Cronómetro 2"...
            var finalName = string.IsNullOrWhiteSpace(name)
                ? "Cronómetro " + (await _context.Chronometers.CountAsync() + 1)
                : name.Trim();

            var chronometer = new Chronometer
            {
                Id = Guid.NewGuid(),
                Name = finalName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Chronometers.Add(chronometer);
            await _context.SaveChangesAsync();

            return chronometer;
        }

        public async Task DeleteAsync(Guid id)
        {
            var chronometer = await _context.Chronometers.FindAsync(id);
            if (chronometer != null)
            {
                // Las vueltas se borran en cascada
                _context.Chronometers.Remove(chronometer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ChronometerState?> GetStateAsync(Guid id)
        {
            var chronometer = await _context.Chronometers.FindAsync(id);
            if (chronometer == null)
            {
                return null;
            }

            return await BuildStateAsync(chronometer);
        }

        public async Task<ChronometerState?> StartAsync(Guid id)
        {
            var chronometer = await _context.Chronometers.FindAsync(id);
            if (chronometer == null)
            {
                return null;
            }

            // Si ya está corriendo no hacemos nada (así un doble clic no reinicia el tramo)
            if (!chronometer.IsRunning)
            {
                chronometer.RunningSince = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return await BuildStateAsync(chronometer);
        }

        public async Task<ChronometerState?> PauseAsync(Guid id)
        {
            var chronometer = await _context.Chronometers.FindAsync(id);
            if (chronometer == null)
            {
                return null;
            }

            if (chronometer.IsRunning)
            {
                // Lo que había corrido en este tramo pasa a lo acumulado
                chronometer.AccumulatedMilliseconds = chronometer.GetElapsedMilliseconds(DateTime.UtcNow);
                chronometer.RunningSince = null;
                await _context.SaveChangesAsync();
            }

            return await BuildStateAsync(chronometer);
        }

        public async Task<ChronometerState?> ResetAsync(Guid id)
        {
            var chronometer = await _context.Chronometers.FindAsync(id);
            if (chronometer == null)
            {
                return null;
            }

            chronometer.AccumulatedMilliseconds = 0;
            chronometer.RunningSince = null;
            await _context.SaveChangesAsync();

            // Borra todas las vueltas con un solo DELETE en la base de datos
            await _context.ChronometerLaps
                .Where(l => l.ChronometerId == id)
                .ExecuteDeleteAsync();

            return await BuildStateAsync(chronometer);
        }

        public async Task<ChronometerState?> LapAsync(Guid id)
        {
            var chronometer = await _context.Chronometers.FindAsync(id);
            if (chronometer == null)
            {
                return null;
            }

            // Solo se puede marcar una vuelta mientras el cronómetro corre
            if (chronometer.IsRunning)
            {
                var totalMilliseconds = chronometer.GetElapsedMilliseconds(DateTime.UtcNow);

                var lastNumber = await _context.ChronometerLaps
                    .Where(l => l.ChronometerId == id)
                    .MaxAsync(l => (int?)l.Number) ?? 0;

                var lastTotal = await _context.ChronometerLaps
                    .Where(l => l.ChronometerId == id)
                    .MaxAsync(l => (long?)l.TotalMilliseconds) ?? 0;

                _context.ChronometerLaps.Add(new ChronometerLap
                {
                    ChronometerId = id,
                    Number = lastNumber + 1,
                    TotalMilliseconds = totalMilliseconds,
                    LapMilliseconds = totalMilliseconds - lastTotal,
                    CreatedAt = DateTime.UtcNow
                });

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    // Dos vueltas simultáneas pueden chocar con el índice único (mismo número): ignoramos la repetida
                    _context.ChangeTracker.Clear();
                    var current = await _context.Chronometers.FindAsync(id);
                    return current == null ? null : await BuildStateAsync(current);
                }
            }

            return await BuildStateAsync(chronometer);
        }

        // Arma el estado que se envía al navegador, con el tiempo calculado en este instante
        private async Task<ChronometerState> BuildStateAsync(Chronometer chronometer)
        {
            var nowUtc = DateTime.UtcNow;

            var laps = await _context.ChronometerLaps
                .Where(l => l.ChronometerId == chronometer.Id)
                .OrderBy(l => l.Number)
                .ToListAsync();

            var state = new ChronometerState
            {
                Id = chronometer.Id,
                Name = chronometer.Name,
                IsRunning = chronometer.IsRunning,
                ElapsedMilliseconds = chronometer.GetElapsedMilliseconds(nowUtc),
                Laps = laps.Select(l => new LapState
                {
                    Number = l.Number,
                    LapMilliseconds = l.LapMilliseconds,
                    TotalMilliseconds = l.TotalMilliseconds
                }).ToList()
            };

            // Vuelta más rápida y más lenta (solo tiene sentido comparar con 2 o más vueltas)
            if (laps.Count >= 2)
            {
                var best = laps.MinBy(l => l.LapMilliseconds)!;
                var worst = laps.MaxBy(l => l.LapMilliseconds)!;

                if (best.LapMilliseconds != worst.LapMilliseconds)
                {
                    state.BestLapNumber = best.Number;
                    state.WorstLapNumber = worst.Number;
                }
            }

            return state;
        }
    }
}

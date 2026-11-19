using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SurveyPlatform.Data;
using SurveyPlatform.Models;

namespace SurveyPlatform.Services
{
    public class SurveyService : ISurveyService
    {
        // Sin caracteres ambiguos (I, O, 0, 1) para que el código sea fácil de leer
        private const string CodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private const int CodeLength = 8;

        private readonly AppDbContext _context;

        public SurveyService(AppDbContext context)
        {
            _context = context;
        }

        // ---------- Consultas ----------

        public async Task<List<SurveySummary>> GetSummariesAsync()
        {
            // Los totales se calculan dentro de la misma consulta SQL (COUNT)
            return await _context.Surveys
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SurveySummary
                {
                    Survey = s,
                    QuestionCount = s.Questions.Count,
                    ResponseCount = s.Responses.Count
                })
                .ToListAsync();
        }

        public async Task<Survey?> GetByIdAsync(int id)
        {
            return await _context.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Survey?> GetByCodeAsync(string code)
        {
            var normalizedCode = code.Trim().ToUpperInvariant();

            return await _context.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.ShareCode == normalizedCode);
        }

        public async Task<int> GetResponseCountAsync(int surveyId)
        {
            return await _context.SurveyResponses.CountAsync(r => r.SurveyId == surveyId);
        }

        // ---------- Encuestas ----------

        public async Task<Survey> CreateAsync(string title, string? description)
        {
            var survey = new Survey
            {
                Title = title,
                Description = description,
                ShareCode = await GenerateUniqueCodeAsync(),
                IsOpen = true,
                CreatedAt = DateTime.Now
            };

            _context.Surveys.Add(survey);
            await _context.SaveChangesAsync();

            return survey;
        }

        public async Task<bool> UpdateAsync(int id, string title, string? description)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return false;
            }

            survey.Title = title;
            survey.Description = description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleOpenAsync(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
            {
                return false;
            }

            survey.IsOpen = !survey.IsOpen;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task DeleteAsync(int id)
        {
            // Primero borramos las respuestas con un solo DELETE en la base de datos
            // (las preguntas y opciones se borran luego en cascada junto con la encuesta)
            await _context.Answers
                .Where(a => a.SurveyResponse.SurveyId == id)
                .ExecuteDeleteAsync();

            var survey = await _context.Surveys.FindAsync(id);
            if (survey != null)
            {
                _context.Surveys.Remove(survey);
                await _context.SaveChangesAsync();
            }
        }

        // ---------- Preguntas ----------

        public async Task<bool> AddQuestionAsync(int surveyId, string text, QuestionType type, bool isRequired, List<string> options)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
            {
                return false;
            }

            // Con respuestas guardadas no se pueden cambiar las preguntas
            if (await _context.SurveyResponses.AnyAsync(r => r.SurveyId == surveyId))
            {
                return false;
            }

            var question = new Question
            {
                SurveyId = surveyId,
                Text = text,
                Type = type,
                IsRequired = isRequired,
                Position = survey.Questions.Count == 0 ? 1 : survey.Questions.Max(q => q.Position) + 1
            };

            if (type.HasOptions())
            {
                question.Options = options
                    .Select((optionText, index) => new QuestionOption { Text = optionText, Position = index + 1 })
                    .ToList();
            }

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question == null)
            {
                return false;
            }

            if (await _context.SurveyResponses.AnyAsync(r => r.SurveyId == question.SurveyId))
            {
                return false;
            }

            // Las opciones se borran en cascada
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        // ---------- Responder ----------

        // Valida lo que envió el usuario contra las preguntas REALES de la base de datos y guarda la respuesta.
        // Devuelve los errores por pregunta (clave 0 = error general). Sin errores = se guardó.
        public async Task<Dictionary<int, string>> SubmitResponseAsync(int surveyId, IEnumerable<QuestionAnswerInput> posted)
        {
            var errors = new Dictionary<int, string>();

            var survey = await GetByIdAsync(surveyId);
            if (survey == null || !survey.IsOpen)
            {
                errors[0] = "Esta encuesta no está disponible.";
                return errors;
            }

            var postedList = posted.ToList();
            var response = new SurveyResponse { SurveyId = survey.Id, SubmittedAt = DateTime.Now };

            foreach (var question in survey.Questions.OrderBy(q => q.Position))
            {
                var input = postedList.FirstOrDefault(p => p.QuestionId == question.Id)
                            ?? new QuestionAnswerInput { QuestionId = question.Id };

                switch (question.Type)
                {
                    case QuestionType.SingleChoice:
                    {
                        if (input.SelectedOptionId == null)
                        {
                            if (question.IsRequired) errors[question.Id] = "Selecciona una opción.";
                        }
                        else if (!question.Options.Any(o => o.Id == input.SelectedOptionId.Value))
                        {
                            errors[question.Id] = "La opción seleccionada no es válida.";
                        }
                        else
                        {
                            response.Answers.Add(new Answer { QuestionId = question.Id, OptionId = input.SelectedOptionId });
                        }
                        break;
                    }

                    case QuestionType.MultipleChoice:
                    {
                        var optionIds = input.SelectedOptionIds.Distinct().ToList();

                        if (optionIds.Count == 0)
                        {
                            if (question.IsRequired) errors[question.Id] = "Selecciona al menos una opción.";
                        }
                        else if (optionIds.Any(optionId => !question.Options.Any(o => o.Id == optionId)))
                        {
                            errors[question.Id] = "Alguna opción seleccionada no es válida.";
                        }
                        else
                        {
                            // Una fila por cada opción marcada
                            foreach (var optionId in optionIds)
                            {
                                response.Answers.Add(new Answer { QuestionId = question.Id, OptionId = optionId });
                            }
                        }
                        break;
                    }

                    case QuestionType.Rating:
                    {
                        if (input.RatingValue == null)
                        {
                            if (question.IsRequired) errors[question.Id] = "Elige una calificación.";
                        }
                        else if (input.RatingValue < 1 || input.RatingValue > 5)
                        {
                            errors[question.Id] = "La calificación debe estar entre 1 y 5.";
                        }
                        else
                        {
                            response.Answers.Add(new Answer { QuestionId = question.Id, RatingValue = input.RatingValue });
                        }
                        break;
                    }

                    default:
                    {
                        var text = input.TextValue?.Trim();

                        if (string.IsNullOrEmpty(text))
                        {
                            if (question.IsRequired) errors[question.Id] = "Escribe tu respuesta.";
                        }
                        else if (text.Length > 1000)
                        {
                            errors[question.Id] = "La respuesta no puede superar 1000 caracteres.";
                        }
                        else
                        {
                            response.Answers.Add(new Answer { QuestionId = question.Id, TextValue = text });
                        }
                        break;
                    }
                }
            }

            if (errors.Count > 0)
            {
                return errors;
            }

            if (response.Answers.Count == 0)
            {
                errors[0] = "Responde al menos una pregunta.";
                return errors;
            }

            _context.SurveyResponses.Add(response);
            await _context.SaveChangesAsync();

            return errors;
        }

        // ---------- Resultados ----------

        public async Task<SurveyResults?> GetResultsAsync(int surveyId)
        {
            var survey = await GetByIdAsync(surveyId);
            if (survey == null)
            {
                return null;
            }

            var results = new SurveyResults
            {
                Survey = survey,
                TotalResponses = await GetResponseCountAsync(surveyId)
            };

            foreach (var question in survey.Questions.OrderBy(q => q.Position))
            {
                var questionResult = new QuestionResult
                {
                    QuestionId = question.Id,
                    Text = question.Text,
                    Type = question.Type
                };

                var answers = _context.Answers.Where(a => a.QuestionId == question.Id);

                // Cuántas personas respondieron esta pregunta (en opción múltiple una persona marca varias opciones)
                questionResult.AnsweredCount = await answers
                    .Select(a => a.SurveyResponseId)
                    .Distinct()
                    .CountAsync();

                if (question.Type.HasOptions())
                {
                    // La base de datos cuenta las respuestas de cada opción (GROUP BY)
                    var counts = await answers
                        .Where(a => a.OptionId != null)
                        .GroupBy(a => a.OptionId)
                        .Select(g => new { OptionId = g.Key, Count = g.Count() })
                        .ToListAsync();

                    foreach (var option in question.Options.OrderBy(o => o.Position))
                    {
                        var count = counts.FirstOrDefault(c => c.OptionId == option.Id)?.Count ?? 0;

                        questionResult.Options.Add(new OptionResult
                        {
                            Label = option.Text,
                            Count = count,
                            Percentage = CalculatePercentage(count, questionResult.AnsweredCount)
                        });
                    }
                }
                else if (question.Type == QuestionType.Rating)
                {
                    var counts = await answers
                        .Where(a => a.RatingValue != null)
                        .GroupBy(a => a.RatingValue)
                        .Select(g => new { Value = g.Key, Count = g.Count() })
                        .ToListAsync();

                    var totalRatings = counts.Sum(c => c.Count);

                    // Siempre se muestran las 5 calificaciones, aunque nadie haya elegido alguna
                    for (int value = 1; value <= 5; value++)
                    {
                        var count = counts.FirstOrDefault(c => c.Value == value)?.Count ?? 0;

                        questionResult.Options.Add(new OptionResult
                        {
                            Label = value.ToString(),
                            Count = count,
                            Percentage = CalculatePercentage(count, totalRatings)
                        });
                    }

                    // Promedio = suma de (calificación x cantidad) / total de calificaciones
                    if (totalRatings > 0)
                    {
                        var sum = counts.Sum(c => (decimal)c.Value!.Value * c.Count);
                        questionResult.RatingAverage = Math.Round(sum / totalRatings, 2);
                    }
                }
                else
                {
                    // Texto libre: las 50 respuestas más recientes
                    questionResult.TextAnswers = await answers
                        .Where(a => a.TextValue != null)
                        .OrderByDescending(a => a.Id)
                        .Select(a => a.TextValue!)
                        .Take(50)
                        .ToListAsync();
                }

                results.Questions.Add(questionResult);
            }

            return results;
        }

        // Genera el contenido de un archivo CSV (se abre en Excel): una fila por persona, una columna por pregunta
        public async Task<string?> BuildCsvAsync(int surveyId)
        {
            var survey = await GetByIdAsync(surveyId);
            if (survey == null)
            {
                return null;
            }

            var questions = survey.Questions.OrderBy(q => q.Position).ToList();

            var responses = await _context.SurveyResponses
                .Where(r => r.SurveyId == surveyId)
                .Include(r => r.Answers)
                .OrderBy(r => r.SubmittedAt)
                .ToListAsync();

            var builder = new StringBuilder();

            // Encabezado
            var header = new List<string> { "Fecha" };
            header.AddRange(questions.Select(q => CsvEscape(q.Text)));
            builder.AppendLine(string.Join(",", header));

            // Una fila por respuesta
            foreach (var response in responses)
            {
                var cells = new List<string>
                {
                    response.SubmittedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)
                };

                foreach (var question in questions)
                {
                    var answers = response.Answers.Where(a => a.QuestionId == question.Id).ToList();
                    string cell;

                    if (question.Type.HasOptions())
                    {
                        cell = string.Join("; ", answers.Select(a =>
                            question.Options.FirstOrDefault(o => o.Id == a.OptionId)?.Text ?? string.Empty));
                    }
                    else if (question.Type == QuestionType.Rating)
                    {
                        cell = answers.FirstOrDefault()?.RatingValue?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
                    }
                    else
                    {
                        cell = answers.FirstOrDefault()?.TextValue ?? string.Empty;
                    }

                    cells.Add(CsvEscape(cell));
                }

                builder.AppendLine(string.Join(",", cells));
            }

            return builder.ToString();
        }

        // ---------- Ayudas ----------

        private static decimal CalculatePercentage(int count, int total)
        {
            return total > 0 ? Math.Round(count * 100m / total, 1) : 0m;
        }

        // Prepara un texto para una celda CSV
        private static string CsvEscape(string value)
        {
            // Seguridad: Excel ejecuta como fórmula lo que empieza con = + - @ (inyección de fórmulas).
            // Anteponer una comilla simple lo convierte en texto normal.
            if (value.Length > 0 && "=+-@\t\r".Contains(value[0]))
            {
                value = "'" + value;
            }

            // Si tiene comas, comillas o saltos de línea, va entre comillas (y las comillas se duplican)
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            {
                value = "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }

        private async Task<string> GenerateUniqueCodeAsync()
        {
            while (true)
            {
                var chars = new char[CodeLength];
                for (int i = 0; i < CodeLength; i++)
                {
                    // RandomNumberGenerator es criptográficamente seguro (el código no debe ser adivinable)
                    chars[i] = CodeAlphabet[RandomNumberGenerator.GetInt32(CodeAlphabet.Length)];
                }

                var code = new string(chars);

                if (!await _context.Surveys.AnyAsync(s => s.ShareCode == code))
                {
                    return code;
                }
            }
        }
    }
}

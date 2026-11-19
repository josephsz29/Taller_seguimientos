using System.Text;
using Microsoft.AspNetCore.Mvc;
using SurveyPlatform.Models;
using SurveyPlatform.Services;

namespace SurveyPlatform.Controllers
{
    public class SurveysController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // ================= Administración (quien crea las encuestas) =================

        // GET: /Surveys  -> lista de encuestas
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var summaries = await _surveyService.GetSummariesAsync();
            return View(summaries);
        }

        // GET: /Surveys/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("Form", new SurveyFormViewModel());
        }

        // POST: /Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var survey = await _surveyService.CreateAsync(model.Title.Trim(), CleanDescription(model.Description));

            TempData["Success"] = "Encuesta creada. Ahora agrega las preguntas.";
            return RedirectToAction(nameof(Builder), new { id = survey.Id });
        }

        // GET: /Surveys/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _surveyService.GetByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            return View("Form", new SurveyFormViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description
            });
        }

        // POST: /Surveys/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SurveyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Form", model);
            }

            var updated = await _surveyService.UpdateAsync(model.Id, model.Title.Trim(), CleanDescription(model.Description));
            if (!updated)
            {
                return NotFound();
            }

            TempData["Success"] = "Encuesta actualizada.";
            return RedirectToAction(nameof(Builder), new { id = model.Id });
        }

        // POST: /Surveys/ToggleOpen/5  -> abrir o cerrar la encuesta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleOpen(int id)
        {
            await _surveyService.ToggleOpenAsync(id);
            return RedirectToAction(nameof(Builder), new { id });
        }

        // POST: /Surveys/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _surveyService.DeleteAsync(id);

            TempData["Success"] = "Encuesta eliminada.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Surveys/Builder/5  -> preguntas de la encuesta
        [HttpGet]
        public async Task<IActionResult> Builder(int id)
        {
            var model = await BuildBuilderAsync(id, null);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: /Surveys/AddQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion([Bind(Prefix = "NewQuestion")] QuestionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var builderModel = await BuildBuilderAsync(model.SurveyId, model);
                if (builderModel == null)
                {
                    return NotFound();
                }

                return View(nameof(Builder), builderModel);
            }

            var added = await _surveyService.AddQuestionAsync(
                model.SurveyId,
                model.Text.Trim(),
                model.Type,
                model.IsRequired,
                model.GetOptionLines());

            if (added)
            {
                TempData["Success"] = "Pregunta agregada.";
            }
            else
            {
                TempData["Error"] = "No se pudo agregar la pregunta: la encuesta ya tiene respuestas.";
            }

            return RedirectToAction(nameof(Builder), new { id = model.SurveyId });
        }

        // POST: /Surveys/DeleteQuestion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id, int surveyId)
        {
            var deleted = await _surveyService.DeleteQuestionAsync(id);

            if (deleted)
            {
                TempData["Success"] = "Pregunta eliminada.";
            }
            else
            {
                TempData["Error"] = "No se pudo eliminar la pregunta: la encuesta ya tiene respuestas.";
            }

            return RedirectToAction(nameof(Builder), new { id = surveyId });
        }

        // ================= Responder (enlace público) =================

        // GET: /Surveys/Answer?code=AB12CD34
        [HttpGet]
        public async Task<IActionResult> Answer(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return NotFound();
            }

            var survey = await _surveyService.GetByCodeAsync(code);
            if (survey == null)
            {
                return NotFound();
            }

            if (!survey.IsOpen)
            {
                return View("Closed", survey);
            }

            return View(BuildAnswerModel(survey, null, null));
        }

        // POST: /Surveys/Answer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(AnswerSurveyViewModel posted)
        {
            var survey = await _surveyService.GetByCodeAsync(posted.Code ?? string.Empty);
            if (survey == null)
            {
                return NotFound();
            }

            if (!survey.IsOpen)
            {
                return View("Closed", survey);
            }

            var errors = await _surveyService.SubmitResponseAsync(survey.Id, posted.Questions);

            if (errors.Count == 0)
            {
                return RedirectToAction(nameof(Thanks), new { code = survey.ShareCode });
            }

            // Hubo errores: volvemos a mostrar el formulario con lo que escribió el usuario
            return View(BuildAnswerModel(survey, posted.Questions, errors));
        }

        // GET: /Surveys/Thanks?code=AB12CD34
        [HttpGet]
        public async Task<IActionResult> Thanks(string? code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return NotFound();
            }

            var survey = await _surveyService.GetByCodeAsync(code);
            if (survey == null)
            {
                return NotFound();
            }

            return View(survey);
        }

        // ================= Resultados e informes =================

        // GET: /Surveys/Results/5  -> análisis con gráficos
        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            var results = await _surveyService.GetResultsAsync(id);
            if (results == null)
            {
                return NotFound();
            }

            return View(results);
        }

        // GET: /Surveys/ExportCsv/5  -> descarga los datos para abrirlos en Excel
        [HttpGet]
        public async Task<IActionResult> ExportCsv(int id)
        {
            var csv = await _surveyService.BuildCsvAsync(id);
            if (csv == null)
            {
                return NotFound();
            }

            // El "preámbulo" (BOM) hace que Excel muestre bien las tildes y la ñ
            var encoding = new UTF8Encoding(true);
            var bytes = encoding.GetPreamble().Concat(encoding.GetBytes(csv)).ToArray();

            return File(bytes, "text/csv", "encuesta-" + id + "-resultados.csv");
        }

        public IActionResult Error()
        {
            return Content("Ocurrió un error inesperado.");
        }

        // ================= Ayudas =================

        private static string? CleanDescription(string? description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        private async Task<SurveyBuilderViewModel?> BuildBuilderAsync(int id, QuestionFormViewModel? form)
        {
            var survey = await _surveyService.GetByIdAsync(id);
            if (survey == null)
            {
                return null;
            }

            return new SurveyBuilderViewModel
            {
                Survey = survey,
                ResponseCount = await _surveyService.GetResponseCountAsync(id),
                NewQuestion = form ?? new QuestionFormViewModel { SurveyId = id }
            };
        }

        // Arma el formulario público con las preguntas REALES de la base de datos.
        // Si hay respuestas enviadas, se vuelven a poner para que el usuario no tenga que escribir todo otra vez.
        private static AnswerSurveyViewModel BuildAnswerModel(
            Survey survey,
            IEnumerable<QuestionAnswerInput>? posted,
            Dictionary<int, string>? errors)
        {
            var postedList = posted?.ToList() ?? new List<QuestionAnswerInput>();
            var inputs = new List<QuestionAnswerInput>();

            foreach (var question in survey.Questions.OrderBy(q => q.Position))
            {
                var answer = postedList.FirstOrDefault(p => p.QuestionId == question.Id);

                inputs.Add(new QuestionAnswerInput
                {
                    QuestionId = question.Id,
                    Text = question.Text,
                    Type = question.Type,
                    IsRequired = question.IsRequired,
                    Options = question.Options.OrderBy(o => o.Position).ToList(),
                    Error = errors != null && errors.TryGetValue(question.Id, out var message) ? message : null,
                    SelectedOptionId = answer?.SelectedOptionId,
                    SelectedOptionIds = answer?.SelectedOptionIds ?? new List<int>(),
                    RatingValue = answer?.RatingValue,
                    TextValue = answer?.TextValue
                });
            }

            return new AnswerSurveyViewModel
            {
                Code = survey.ShareCode,
                Title = survey.Title,
                Description = survey.Description,
                GeneralError = errors != null && errors.TryGetValue(0, out var generalError) ? generalError : null,
                Questions = inputs
            };
        }
    }
}

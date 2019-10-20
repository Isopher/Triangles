using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Traingles.DomainModel.TriangleFinder;
using Triangles.Domain.TriangleFinder;

namespace Triangles.Web.Controllers
{
    public class TriangleFinderController : Controller
    {
        private readonly IMediator _mediator;

        public TriangleFinderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new Exception("Mediator is required");
        }

        public ActionResult Index()
        {
            return View("TriangleFinder", new RequestedTriangle());
        }

        public async Task<ActionResult> FindByCoordinates(RequestedTriangle triangle)
        {
            var query = new FindTriangleByCoordinates.Query(triangle);
            var result = await _mediator.Send(query);
            if (!query.IsValid)
            {
                foreach (var error in query.ValidationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("TriangleFinder", query);
            }

            return View("TriangleFinder", result);
        }

        public async Task<ActionResult> FindByVertices(RequestedTriangle triangle)
        {
            var query = new FindTriangleByVertices.Query(triangle);
            var result = await _mediator.Send(query);
            if (!query.IsValid)
            {
                foreach (var error in query.ValidationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View("TriangleFinder", query);
            }

            return View("TriangleFinder", result);
        }
    }
}
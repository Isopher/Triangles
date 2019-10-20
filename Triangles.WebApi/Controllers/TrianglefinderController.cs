using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using MediatR;
using Traingles.DomainModel.TriangleFinder;
using Triangles.Domain.TriangleFinder;

namespace Triangles.WebApi.Controllers
{
    public class TriangleFinderController : ApiController
    {
        private readonly IMediator _mediator;

        public TriangleFinderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new Exception("Mediator is required");
        }

        public async Task<JsonResult<RequestedTriangle>> FindByCoordinates(string row, int column)
        {
            var query = new FindTriangleByCoordinates.Query(row, column);
            var result = await _mediator.Send(query);

            return Json(result);
        }

        public async Task<JsonResult<RequestedTriangle>> FindByVertices(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            var query = new FindTriangleByVertices.Query
            {
                X1 = x1, Y1 = y1,
                X2 = x2, Y2 = y2,
                X3 = x3, Y3 = y3
            };
            var result = await _mediator.Send(query);

            return Json(result);
        }
    }
}
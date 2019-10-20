using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Traingles.DomainModel.TriangleFinder;
using Triangles.Domain.Shared;

namespace Triangles.Domain.TriangleFinder
{
    public class FindTriangleByCoordinates
    {
        public class Query : FindTriangleRequest, IRequest<RequestedTriangle>
        {
            public Query(RequestedTriangle triangle)
            {
                Column = triangle.Column;
                if (Enum.TryParse(triangle.sRow?.ToUpper() ?? "", out RowEnum parsedValue)) Row = parsedValue;
            }
        }

        public class Handler : IRequestHandler<Query, RequestedTriangle>
        {
            private readonly IValidator<Query> _validator;

            public Handler(IValidator<Query> validator)
            {
                _validator = validator ?? throw new Exception("Validator Required");
            }

            public async Task<RequestedTriangle> Handle(Query request, CancellationToken cancellationToken)
            {
                await request.Validate(_validator);
                if (!request.IsValid) return null;

                var triangle = new RequestedTriangle
                {
                    Column = request.Column,
                    Row = request.Row
                };

                var bottomX = (int) request.Row * 10;
                var topX = bottomX - 10;
                var rightY = CalculateRightY(request.Column);
                var leftY = rightY - 10;

                if (request.Column % 2 == 0)
                {
                    // Even Column has 2 vertices on top
                    // vertical is right aligned
                    triangle.X1 = topX;
                    triangle.Y1 = leftY;

                    triangle.X2 = topX;
                    triangle.Y2 = rightY;

                    triangle.X3 = bottomX;
                    triangle.Y3 = rightY;
                }
                else
                {
                    // Odd Column has 2 vertices on bottom,
                    // vertical is left aligned
                    triangle.X1 = topX;
                    triangle.Y1 = leftY;

                    triangle.X2 = bottomX;
                    triangle.Y2 = leftY;

                    triangle.X3 = bottomX;
                    triangle.Y3 = rightY;
                }

                return triangle;
            }

            public int CalculateRightY(int column)
            {
                var rightY = column;
                if (rightY % 2 == 1) rightY += 1;

                return rightY / 2 * 10;
            }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.Row)
                    .NotNull().WithMessage("Please Specify Row")
                    .IsInEnum().WithMessage("Specified Row Does Not Exist");
                RuleFor(x => x.Column)
                    .GreaterThan(0).WithMessage("Column must be between 1 and 12")
                    .LessThanOrEqualTo(12).WithMessage("Column must be between 1 and 12");
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Traingles.DomainModel.TriangleFinder;
using Triangles.Domain.Shared;

namespace Triangles.Domain.TriangleFinder
{
    public class FindTriangleByVertices
    {
        public class Query : FindTriangleRequest, IRequest<RequestedTriangle>
        {
            public Query(RequestedTriangle triangle)
            {
                X1 = triangle.X1;
                X2 = triangle.X2;
                X3 = triangle.X3;
                Y1 = triangle.Y1;
                Y2 = triangle.Y2;
                Y3 = triangle.Y3;
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
                    X1 = request.X1, Y1 = request.Y1,
                    X2 = request.X2, Y2 = request.Y2,
                    X3 = request.X3, Y3 = request.Y3,
                    IsEstimated = CheckVertexAlignment(request)
                };

                if (!CheckValidTriangle(request))
                    request.ValidationResult.Errors.Add(
                        new ValidationFailure("Summary",
                            "Entered Triangle Vertices Are Invalid and no triangle could be inferred.")
                    );

                triangle.Row = IdentifyRow(request);
                triangle.sRow = triangle.Row.ToString();
                triangle.Column = IdentifyColumn(request);

                return triangle;
            }

            public bool CheckVertexAlignment(Query request)
            {
                var estimated = false;
                if (request.X1 % 10 != 0)
                {
                    request.X1 = SnapToNearestVertex(request.X1);
                    estimated = true;
                }

                if (request.X2 % 10 != 0)
                {
                    request.X2 = SnapToNearestVertex(request.X2);
                    estimated = true;
                }

                if (request.X3 % 10 != 0)
                {
                    request.X3 = SnapToNearestVertex(request.X3);
                    estimated = true;
                }

                if (request.Y1 % 10 != 0)
                {
                    request.Y1 = SnapToNearestVertex(request.Y1);
                    estimated = true;
                }

                if (request.Y2 % 10 != 0)
                {
                    request.Y2 = SnapToNearestVertex(request.Y2);
                    estimated = true;
                }

                if (request.Y3 % 10 != 0)
                {
                    request.Y3 = SnapToNearestVertex(request.Y3);
                    estimated = true;
                }

                return estimated;
            }

            public int SnapToNearestVertex(int value)
            {
                var offset = value % 10;
                var retVal = value;

                retVal = offset > 5 ? retVal + (10 - offset) : retVal - offset;

                return retVal;
            }

            public bool CheckValidTriangle(Query request)
            {
                return ValidatePointRange(request.X1, request.X2, request.X3)
                       && ValidatePointRange(request.Y1, request.Y2, request.Y3);
            }

            public bool ValidatePointRange(int v1, int v2, int v3)
            {
                // Exactly Two of the three values should match
                if (v1 != v2 && v2 != v3 && v1 != v3 || v1 == v2 && v2 == v3) return false;

                if (v1 == v2 || v2 == v3)
                    return ValidatePointSpread(v1, v3);
                return ValidatePointSpread(v2, v3);
            }

            public bool ValidatePointSpread(int v1, int v2)
            {
                return v1 > v2 ? v1 - v2 == 10 : v2 - v1 == 10;
            }

            public RowEnum IdentifyRow(Query request)
            {
                var val = new[] {request.X1, request.X2, request.X3}.Max();
                return (RowEnum) (val / 10);
            }

            public int IdentifyColumn(Query request)
            {
                int singlePoint;
                int doublePoint;

                if (request.Y1 == request.Y2)
                {
                    singlePoint = request.Y3;
                    doublePoint = request.Y1;
                }
                else if (request.Y2 == request.Y3)
                {
                    singlePoint = request.Y1;
                    doublePoint = request.Y2;
                }
                else
                {
                    singlePoint = request.Y2;
                    doublePoint = request.Y1;
                }

                return doublePoint > singlePoint
                    ? IdentifyEvenColumn(doublePoint)
                    : IdentifyOddColumn(doublePoint);
            }

            public int IdentifyEvenColumn(int doublePoint)
            {
                return doublePoint / 10 * 2;
            }

            public int IdentifyOddColumn(int doublePoint)
            {
                var adjustedPoint = doublePoint / 10;
                return adjustedPoint * 2 + 1;
            }
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.X1)
                    .GreaterThanOrEqualTo(0).WithMessage("Value Cannot Be Below 0")
                    .LessThanOrEqualTo(60).WithMessage("Value Cannot Exceed 60");
                RuleFor(x => x.Y1)
                    .GreaterThanOrEqualTo(0).WithMessage("Value Cannot Be Below 0")
                    .LessThanOrEqualTo(60).WithMessage("Value Cannot Exceed 60");
                RuleFor(x => x.X2)
                    .GreaterThanOrEqualTo(0).WithMessage("Value Cannot Be Below 0")
                    .LessThanOrEqualTo(60).WithMessage("Value Cannot Exceed 60");
                RuleFor(x => x.Y2)
                    .GreaterThanOrEqualTo(0).WithMessage("Value Cannot Be Below 0")
                    .LessThanOrEqualTo(60).WithMessage("Value Cannot Exceed 60");
                RuleFor(x => x.X3)
                    .GreaterThanOrEqualTo(0).WithMessage("Value Cannot Be Below 0")
                    .LessThanOrEqualTo(60).WithMessage("Value Cannot Exceed 60");
                RuleFor(x => x.Y3)
                    .GreaterThanOrEqualTo(0).WithMessage("Value Cannot Be Below 0")
                    .LessThanOrEqualTo(60).WithMessage("Value Cannot Exceed 60");
            }
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Traingles.DomainModel.TriangleFinder;

namespace Triangles.Domain.Shared
{
    public class FindTriangleRequest : RequestedTriangle
    {
        public bool IsValid => ValidationResult != null && ValidationResult.Errors.All(e => e.Severity != Severity.Error);

        public ValidationResult ValidationResult { get; set; }

        public async Task Validate(IValidator validator)
        {
            var validationResult = await validator.ValidateAsync(this);
            ValidationResult = validationResult;
        }
    }
}
using MediatR;
using Sudan_Train.Core.Bases;

namespace Sudan_Train.Core.Features.Users.Queries.GetCustomerLookup
{
    // Powers the staff counter-booking flow. Staff types a phone / email /
    // ID number / username and we return up to 10 matches.
    public class GetCustomerLookupQuery : IRequest<Response<List<CustomerSummaryDto>>>
    {
        public string Query { get; set; } = string.Empty;
    }

    public class CustomerSummaryDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = default!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }
        // Last-seen passenger ID number from any prior booking — helps disambiguate.
        public string? IdNumber { get; set; }
    }
}

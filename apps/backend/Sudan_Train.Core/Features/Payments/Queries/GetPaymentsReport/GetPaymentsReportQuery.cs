using MediatR;
using Sudan_Train.Core.Bases;
using Sudan_Train.Data.DTOs.Payment;

namespace Sudan_Train.Core.Features.Payments.Queries.GetPaymentsReport
{
    public class GetPaymentsReportQuery : IRequest<Response<PaymentsReportDto>>
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Method { get; set; }
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

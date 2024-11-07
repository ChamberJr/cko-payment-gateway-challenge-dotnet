using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentsRepository _paymentsRepository;

    public PaymentsController(PaymentsRepository paymentsRepository)
    {
        _paymentsRepository = paymentsRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentDetails>> SubmitPayment(SubmitPaymentRequest request)
    {

    }

    [HttpGet("{id:guid}")]
    public ActionResult<PaymentDetails> GetPayment(Guid id)
    {
        return _paymentsRepository.TryGetPaymentDetails(id, out var paymentDetails)
            ? Ok(paymentDetails)
            : NotFound();
    }
}
using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Dal.Database;
using PaymentGateway.Api.Logic;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(PaymentsRepository paymentsRepository, IPaymentSubmitter paymentSubmitter) : ControllerBase
{

    [HttpPost]
    public async Task<ActionResult<PaymentDetails>> SubmitPayment(SubmitPaymentRequest request)
    {
        var paymentSubmissionResult = await paymentSubmitter.SubmitPayment(request);

        return paymentSubmissionResult.Successful
            ? CreatedAtAction(nameof(GetPayment), paymentSubmissionResult.PaymentDetails)
            : BadRequest(paymentSubmissionResult.ValidationErrorMessage);
    }

    [HttpGet("{id:guid}")]
    public ActionResult<PaymentDetails> GetPayment(Guid id)
    {
        return paymentsRepository.TryGetPaymentDetails(id, out var paymentDetails)
            ? Ok(paymentDetails)
            : NotFound();
    }
}
<h1>Design Decisions</h1>

* General
    * Adding interfaces for easier mocking and extensibility
    * In a larger solution I would consider splitting the solution into Web Layer/Data Access Layer/Logic Layer for lower coupling and dependency inversion. However, given that the scope of this solution is very small, this would complicate things for little benefit.
    * Attempting to make class purpose singular
    * Running on IIS Express for local setup, as I don't know any required server configuration.
 
* SubmitPaymentRequest
    * Using Microsoft's built in validation to reduce use of bespoke logic wherever possible. Swagger also recognises some of these attributes to make slightly better default field values. For validation not conveniently validatable (FutureMonth and CurrencyCode), using custom validation.
    * CardNumber should be a string as the first digit can be 0. This would make length validation not make sense if reduced to an int. Also, the maximum int and long values are 19 digits or fewer.
    * CardNumber validated using my own validation instead of the built-in CreditCard validation attribute to ensure I can match validation rules specified as closely as possible.
    * Making PostPaymentRequest and PaymentDetails non-mutable for maintainability.
    * Amount is a long as even though we wouldn't support £1,000,000 transactions by card, some currencies may have smaller denominations, e.g. Yuan or Yen
    * Amount can be 0 as we may just be interested in card validation rather than payment.
    * Cvv is a string as it can start with 0.
   
* PaymentSubmissionResult
    * Using a Success/Failure pattern with a private constructor to prevent illogical result states occurring.

* PaymentDetails
    * CardNumberLastFour is a string as it could start with 0.

* PaymentDetailsCreator
    * Creating our own Guid for IDing instead of using AuthorizationCode, as I think it's worth storing Declined payments for auditing purposes, but Declined payments don't have an AuthorizationCode.

* PaymentsRepository
    * Modifying retrieving payments to a TryGet to make success/failure results more explicit.
    * Making Payments private to prevent outside mutation.
    * Making Payments a dictionary to improve performance of id lookup.

* PaymentsController
    * Returning 404 when no payment found as a more standard response than 200 with a null response
    * Changing from Controller to ControllerBase as no view needed
    * Making 'Rejected' status BadRequest returning error message for invalid requests to make it easier to consumers to integrate with API
    * Making 'Authorized' and 'Declined' statuses Created responses pointing at the URI to retrieve information

* PaymentStatus
    * Removing Rejected from PaymentStatus as it's never the status of a real payment
    * Stating enum numbers explicitly to avoid Authorized being the default enum value of 0

* IValidator
    * Using in keyword to allow contravariance on type T
    * Allowing for extension of validation on any datatypes

* FutureMonthValidator
    * Injecting a delegate to figure out current DateTime to make unit tests independent of real time

* BankPaymentSubmitter
    * Injecting an IHttpClientFactory in to allow unit testing.

* Program
    * I've suppressed the automatic model validation filter as we have extra validation we want to perform, so we can do it all at once.

* ServiceRegistration
    * Pulling service registration out of Program to split responsibilities
    * Adding two ways to register services to aid testing
    * Injecting URL as string - ideally it would be profiled config but that's not worth it for a single use application.
    * I've reused a lot of interfaces - the benefit is this ties more classes together conceptually and is more extensible and configurable, but the cost is DI is more explicitly managed.

* PaymentsControllerTests (AcceptanceTests)
    * Tests using WireMock to mock out the Bank API to test whole system to test entire business operations.


<h1>Things I would like to do:</h1>

* Write auto-generated randomised tests to test the validation logic and high load capabilities of the API
* There's some linting issues and theoretical places where things could be nullable - the only reason they won't be null is due to the current code state. I'd make this more robust if spending longer on this.
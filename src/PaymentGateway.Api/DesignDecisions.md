* General
** Adding interfaces for easier mocking and extensibility
** In a larger solution I would consider splitting the solution into Web Layer/Data Access Layer/Logic Layer for lower coupling and dependency inversion. However, given that the scope of this solution is very small, this would complicate things for little benefit.
** Could make Validation more general by using an AggregateValidator, but probably not worth it for a single use and only 2 validations.
** Attempting to make class purpose singular
 
* PostPaymentRequest
** CardNumber should be a string as the first digit can be 0. This would make length validation not make sense if reduced to an int. Also, the maximum int and long values are 19 digits or fewer.
** CardNumber validated using my own validation instead of the built-in CreditCard validation attribute to ensure I can match validation rules specified as closely as possible.
** Making PostPaymentRequest and PaymentDetails non-mutable for maintainability.
** Amount is a long as even though we would support £10,000 transactions by card, some currencies may have smaller denominations, e.g. Yuan or Yen
** Amount can be 0 as we may just be interested in card validation rather than payment.
** Cvv is a string as it can start with 0.

* PaymentDetails
** CardNumberLastFour is a string as it could start with 0.

* PaymentsRepository
** Modifying retriving payments to a TryGet to make success/failure results more explicit.
** Making Payments private to prevent outside mutation.
** Making Payments a dictionary to improve performance of id lookup.

* PaymentsController
** Returning 404 when no payment found as a more standard response than 200 with a null response
** Changing from Controller to ControllerBase as no view needed
** Making 'Rejected' status BadRequest returning error message for invalid requests to make it easier to consumers to integrate with API
** Making 'Authorized' and 'Declined' statuses Created responses pointing at the URI to retrieve information

* PaymentStatus
** Removing Rejected from PaymentStatus as it's never the status of a real payment
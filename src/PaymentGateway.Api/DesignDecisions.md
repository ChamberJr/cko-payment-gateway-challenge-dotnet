* General
** Adding interfaces for easier mocking and extensibility
** In a larger solution I would consider splitting the solution into Web Layer/Data Access Layer/Logic Layer for lower coupling and dependency inversion. However, given that the scope of this solution is very small, this would complicate things for little benefit.
 
 
* PostPaymentRequest
** CardNumber should be a string as the first digit can be 0. This would make length validation not make sense if reduced to an int. Also, the maximum int and long values are 19 digits or fewer.
** CardNumber validated using my own validation instead of the built-in CreditCard validation attribute to ensure I can match validation rules specified as closely as possible.
** Making PostPaymentRequest and PaymentDetails non-mutable for maintainability.
** Amount is a long as even though we would support £10,000 transactions by card, some currencies may have smaller denominations, e.g. Yuan or Yen
** Amount can be 0 as we may just be interested in card validation rather than payment.

* PaymentsRepository
** Modifying retriving payments to a TryGet to make success/failure results more explicit.
** Making Payments private to prevent outside mutation.
** Making Payments a dictionary to improve performance of id lookup.

* PaymentsController
** Returning 404 when no payment found as a more standard response than 200 with a null response
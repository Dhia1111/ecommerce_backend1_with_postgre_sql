

//////////////
///
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe;

using Ecommerce1;
using Microsoft.AspNetCore.Http.HttpResults;
using Stripe.V2;
using Microsoft.Extensions.Logging;
using Stripe.Events;

public static class Events
{
    public const string PaymentIntentSucceeded = "payment_intent.succeeded";
    public const string PaymentIntentPaymentFailed = "payment_intent.payment_failed";
    // …etc
}

[Route("api/Ecommerce/clsStripeHookControlerAPIs")]
[ApiController]


public class clsStripeHookControlerAPIs : ControllerBase
{



  
    [HttpPost("handlingStripHooks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleStripeHooks()

    {
        string Status;
        Guid? TransactionGUID;
        clsTransaction? Transaction ;
        // 1) Read the raw body
        var json = await new StreamReader(Request.Body).ReadToEndAsync();

        var ErrorObject = clsTransaction.GetLastPaymentError(json);

       
       
        (string? TokenTransactionGUID,string? status)=clsTransaction.ParseStripePaymentEvent(json);


        if (string.IsNullOrEmpty(TokenTransactionGUID) || string.IsNullOrEmpty(status))
        {
            return BadRequest(new DTOGeneralResponse("thie Data is not Valaid",400,"Parsing and validation error"));
        }else
        {
            Status = status;
            TransactionGUID=clsGlobale.ExtractGuidIDFromToken(TokenTransactionGUID);

            if (!TransactionGUID.HasValue) {
                return BadRequest(new DTOGeneralResponse("thie Data is not Valaid", 400, "Parsing and validation error"));

            }
            else
            {
                Transaction =await clsTransaction.Find(TransactionGUID.Value);
                if (Transaction == null) {

                    return BadRequest(new DTOGeneralResponse("thie Data is not Valaid, ( the TransactionGUID )", 400, "Parsing and validation error"));

              }
            }

        }
            

            var stripeSignature = Request.Headers["Stripe-Signature"];

        Stripe.Event stripeEvent;
        try
        {
            // 2) Verify & construct the event
            stripeEvent = EventUtility.ConstructEvent(

                json,

                stripeSignature,
             

                clsGlobale.StripeWebhookSecret(),


           
             throwOnApiVersionMismatch: false // Allow version differences // Allow version differences



            );
        }
        catch (StripeException e)
        {
            Console.WriteLine($"⚠️ Webhook signature verification failed: {e.Message}");

            return BadRequest(); // signature verification failed

        }

        try
        {
            switch (Status.ToLower())
            {
                case "succeeded":

                    Transaction.State = DTOTransaction.enState.Succeeded;
                    Transaction.TransactionGUID = Guid.NewGuid();

                    await Transaction.Save();

                    break;

                case "failed":

                    Transaction.State = DTOTransaction.enState.Failed;
                    Transaction.TransactionGUID = Guid.NewGuid();
                    await Transaction.Save();

                    break;

                case "requires_payment_method":

                    if (ErrorObject.Code!=null)
                    {
                        Transaction.State = DTOTransaction.enState.Failed;
                        Transaction.TransactionGUID = Guid.NewGuid();
                        await Transaction.Save();

                    }

                    break;

                case "requires_action":

                            
                    break;

                case "processing":
                    break;

                default:
                    Console.WriteLine($"Unhandled status: {Status}");
                    break;
            }



        }

        catch

        {



        }
        finally
        {
  
        }

        // 4) Return 200 to acknowledge receipt
        return Ok();
    }
}




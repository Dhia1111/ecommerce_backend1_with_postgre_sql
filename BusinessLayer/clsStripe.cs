using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public  class clsStripe
    {
        public static (string? Code, string? Message, string? PaymentMethodId) GetLastPaymentError(string json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json))
                    return (null, null, null);
                var objJson = JObject.Parse(json);
                var Data = objJson["data"];
                var ObjectsHolder = Data["object"];
                var paymentError = ObjectsHolder["last_payment_error"];

                if (paymentError == null || paymentError.Type == JTokenType.Null)
                    return (null, null, null);

                return (
                    Code: paymentError["code"]?.ToString(),
                    Message: paymentError["message"]?.ToString(),
                    PaymentMethodId: paymentError["payment_method"]?["id"]?.ToString()
                );
            }
            catch
            {
                // Handle parsing errors silently
                return (null, null, null);
            }
        }


        public static (string? TransactionGuid, string? PaymentStatus) ParseStripePaymentEvent(string json)
        {
            try
            {
                // Check for empty/null JSON
                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new ArgumentException("JSON input cannot be null or empty");
                }

                // Parse the JSON

                var jsonObject = JObject.Parse(json);
                var DataObject = jsonObject["data"];
                var paymentObject = DataObject["object"];

                // Check if the object exists
                if (paymentObject == null)
                {
                    throw new KeyNotFoundException("'object' property not found in JSON");
                }

                // Extract TransactionGUID from metadata
                string? transactionGuid = null;
                var metadata = paymentObject["metadata"];
                if (metadata != null)
                {
                    transactionGuid = metadata["TransactionGUID"]?.ToString();

                    // Alternative case-insensitive check
                    if (transactionGuid == null)
                    {
                        transactionGuid = metadata.Children<JProperty>()
                            .FirstOrDefault(x => x.Name.Equals("TransactionGUID", StringComparison.OrdinalIgnoreCase))?
                            .Value.ToString();
                    }
                }

                // Extract payment status
                var paymentStatus = paymentObject["status"]?.ToString();

                // Validate required fields
                if (paymentStatus == null)
                {
                    throw new KeyNotFoundException("Payment status not found in JSON");
                }

                return (transactionGuid, paymentStatus);
            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"Invalid JSON format: {ex.Message}");
                return (null, null);
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is KeyNotFoundException)
            {
                Console.WriteLine($"Data parsing error: {ex.Message}");
                return (null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return (null, null);
            }
        }


    }
}

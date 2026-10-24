using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

public class PaymentRequest
{
    public string GetPaymentUrl(PaymentRequestDto paymentRequestDto)
    {
        StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"].ToString();
        var domain = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
        var domainPath = domain.GetLeftPart(UriPartial.Authority);
        var options = new SessionCreateOptions
        {

            LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    // Provide the exact Price ID (for example, pr_1234) of the product you want to sell
                    Price = paymentRequestDto.PriceId,
                    Quantity = 1,
                  },
                },
            Mode = "payment",
            SuccessUrl = domainPath + "/payment/Success.aspx?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = domainPath + "/payment/Cancel.aspx",
            ClientReferenceId = DateTime.Now.ToFileTimeUtc().ToString(),
            CustomerEmail = paymentRequestDto.EmailId,
            Metadata = new Dictionary<string, string>()
                {
                    {
                        "firstName" , paymentRequestDto.FirstName
                    },
                    {
                        "lastName", paymentRequestDto.LastName
                    },
                    {
                        "emailId", paymentRequestDto.EmailId
                    },
                    {
                        "phoneNo", paymentRequestDto.PhoneNo
                    },
                    {
                        "mobileNo", paymentRequestDto.MobileNo
                    },
                    {
                        "fileName", paymentRequestDto.FileName
                    },
                    {
                        "priceId", paymentRequestDto.PriceId
                    },
                    {
                        "productId", paymentRequestDto.ProductId
                    },
                    {
                        "empId", paymentRequestDto.EmpId
                    },
                    {
                        "source", paymentRequestDto.Source
                    },
                    {"planName", "" },
                    {
                        "planStartDate", ""
                    },
                    {
                        "planEndDate", ""
                    },
                    {
                        "users", ""
                    },
                    {
                        "business", ""
                    }
                }
        };
        var service = new SessionService();
        Session session = service.Create(options);
        HttpContext.Current.Response.Headers.Add("Location", session.Url);
        return session.Url;
    }

    public StripeList<Product> FetchStripPlans(int limit)
    {
        StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"].ToString();
        var options = new ProductListOptions
        {
            Limit = limit,
        };
        var service = new ProductService();
        StripeList<Product> products = service.List(
          options);
        products.ToList().ForEach(product =>
        {
            if (product.DefaultPriceId != null)
                product.DefaultPrice = new PriceService().Get(product.DefaultPriceId);
        });

        return products;
    }

    public string CreatePaymentLink(PaymentRequestDto payment)
    {
        var domain = new Uri(HttpContext.Current.Request.Url.AbsoluteUri);
        var domainPath = domain.GetLeftPart(UriPartial.Authority);
        StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeSecretKey"].ToString();
        var options = new ProductCreateOptions
        {
            Name = payment.PlanName,

            DefaultPriceData = new ProductDefaultPriceDataOptions
            {
                UnitAmount = (long)(Convert.ToDouble(payment.Amount) * 100),
                Currency = payment.Currency,

            },
            Expand = new List<string> { "default_price" },
        };
        var service = new ProductService();
        var productDetails = service.Create(options);

        var priceOptions = new PriceCreateOptions
        {
            Currency = payment.Currency,
            UnitAmount = (long)(Convert.ToDouble(payment.Amount) * 100),
            Product = productDetails.Id,
        };
        var priceService = new PriceService();
        var priceDetails = priceService.Create(priceOptions);

        var linkOptions = new PaymentLinkCreateOptions
        {
            LineItems = new List<PaymentLinkLineItemOptions>
            {
                new PaymentLinkLineItemOptions { Price = priceDetails.Id, Quantity = 1 },
            },
            AfterCompletion = new PaymentLinkAfterCompletionOptions
            {
                Type = "redirect",
                Redirect = new PaymentLinkAfterCompletionRedirectOptions
                {
                    Url = domainPath + "/payment/Success.aspx?session_id={CHECKOUT_SESSION_ID}",
                },
            },
            Metadata = new Dictionary<string, string>
            {
                {"emailId", payment.EmailId },
                {
                    "priceId", priceDetails.Id
                },
                {
                    "productId", productDetails.Id
                },
                {"planName", payment.PlanName },
                {
                    "empId", payment.EmpId
                },
                {
                    "planStartDate", payment.PlanStartDate
                },
                {
                    "planEndDate", payment.PlanEndDate
                },
                {
                    "users", payment.NoOfUsers
                },
                {
                    "business", payment.NoOfBusiness
                }
            }
        };
        var linkService = new PaymentLinkService();
        var linkDetails = linkService.Create(linkOptions);
        return linkDetails.Url;
    }
}

public class PaymentRequestDto
{
    public string EmpId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailId { get; set; }
    public string PhoneNo { get; set; }
    public string MobileNo { get; set; }
    public string PriceId { get; set; }
    public string ProductId { get; set; }
    public string FileName { get; set; }
    public string Source { get; set; }
    public string PlanName { get; set; }
    public string PlanStartDate { get; set; }
    public string PlanEndDate { get; set; }
    public string NoOfUsers { get; set; }
    public string NoOfBusiness { get; set; }
    public string Amount { get; set; }
    public string Currency { get; set; }
}
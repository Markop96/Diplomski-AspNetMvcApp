using Microsoft.Extensions.Options;
using Stripe;
using System.Threading.Tasks;

public class StripeService
{
    private readonly StripeSettings _stripeSettings;

    public StripeService(IOptions<StripeSettings> stripeSettings)
    {
        _stripeSettings = stripeSettings.Value;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public string GetPublishableKey()
    {
        return _stripeSettings.PublishableKey;
    }

    public async Task<Charge> CreateCharge(decimal amount, string currency, string source, string description)
    {
        var options = new ChargeCreateOptions
        {
            Amount = (long)(amount * 100), // Amount in cents
            Currency = currency,
            Source = source,
            Description = description,
        };

        var service = new ChargeService();
        return await service.CreateAsync(options);
    }
}

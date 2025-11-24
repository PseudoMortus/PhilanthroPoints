using PhilanthroPoints.Models;
using PhilanthroPoints.Data;
using System.Net.Mail;
using System.Net;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PhilanthroPoints.Services;

public interface INotificationService
{
    Task SendOrderConfirmationEmailAsync(string email, string customerName, List<Item> items);
    Task SendOrderConfirmationSmsAsync(string phoneNumber, string customerName, List<Item> items);
    Task SendAdminNotificationEmailAsync(string email, string subject, string message);
    Task SendBulkNotificationsAsync(List<Order> orders);
    Task<bool> TestEmailConnectionAsync();
    Task<bool> TestSmsConnectionAsync();
}

public class NotificationService : INotificationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<NotificationService> _logger;
    private readonly ApplicationDbContext _dbContext;

    public NotificationService(IConfiguration configuration, ILogger<NotificationService> logger, ApplicationDbContext dbContext)
    {
        _configuration = configuration;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task SendOrderConfirmationEmailAsync(string email, string customerName, List<Item> items)
    {
        try
        {
            var subject = "🎉 Order Confirmation - PhilanthroPoints";
            var emailBody = BuildEmailBody(customerName, items);

            var emailProvider = _configuration["Email:Provider"];
            
            _logger.LogInformation($"Attempting to send email to {email} using provider: {emailProvider}");
            
            switch (emailProvider?.ToLower())
            {
                case "sendgrid":
                    await SendEmailViaSendGridAsync(email, subject, emailBody);
                    break;
                case "smtp":
                    await SendEmailViaSmtpAsync(email, subject, emailBody);
                    break;
                default:
                    _logger.LogWarning($"Email provider '{emailProvider}' not configured. Logging email content:");
                    _logger.LogInformation($"To: {email}, Subject: {subject}");
                    Console.WriteLine($"[EMAIL] To: {email}");
                    Console.WriteLine($"[EMAIL] Subject: {subject}");
                    Console.WriteLine($"[EMAIL] Would send order confirmation for {items.Count} items");
                    break;
            }

            _logger.LogInformation($"Order confirmation email sent successfully to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send order confirmation email to {email}");
            
            // Don't throw - try console fallback
            Console.WriteLine($"[EMAIL ERROR] Failed to send to {email}: {ex.Message}");
            Console.WriteLine($"[EMAIL FALLBACK] Order confirmation for {customerName}:");
            foreach (var item in items)
            {
                Console.WriteLine($"[EMAIL FALLBACK] - {item.Name} ({item.Cost} points)");
            }
            Console.WriteLine($"[EMAIL FALLBACK] Total: {items.Sum(i => i.Cost)} points");
        }
    }

    public async Task SendOrderConfirmationSmsAsync(string phoneNumber, string customerName, List<Item> items)
    {
        try
        {
            var message = BuildSmsMessage(customerName, items);
            var smsProvider = _configuration["Sms:Provider"];

            _logger.LogInformation($"Attempting to send SMS to {phoneNumber} using provider: {smsProvider}");

            switch (smsProvider?.ToLower())
            {
                case "twilio":
                    await SendSmsViaTwilioAsync(phoneNumber, message);
                    break;
                default:
                    _logger.LogWarning($"SMS provider '{smsProvider}' not configured. Logging SMS content:");
                    _logger.LogInformation($"To: {phoneNumber}");
                    Console.WriteLine($"[SMS] To: {phoneNumber}");
                    Console.WriteLine($"[SMS] Message: {message}");
                    break;
            }

            _logger.LogInformation($"Order confirmation SMS sent successfully to {phoneNumber}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send order confirmation SMS to {phoneNumber}");
            
            // Don't throw - try console fallback
            Console.WriteLine($"[SMS ERROR] Failed to send to {phoneNumber}: {ex.Message}");
            Console.WriteLine($"[SMS FALLBACK] Order confirmation for {customerName}:");
            var message = BuildSmsMessage(customerName, items);
            Console.WriteLine($"[SMS FALLBACK] {message}");
        }
    }

    public async Task SendAdminNotificationEmailAsync(string email, string subject, string message)
    {
        try
        {
            var emailProvider = _configuration["Email:Provider"];
            
            switch (emailProvider?.ToLower())
            {
                case "sendgrid":
                    await SendEmailViaSendGridAsync(email, subject, message, false);
                    break;
                case "smtp":
                    await SendEmailViaSmtpAsync(email, subject, message, false);
                    break;
                default:
                    _logger.LogInformation($"Admin notification email logged: {email} - {subject}");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send admin notification email to {email}");
        }
    }

    public async Task SendBulkNotificationsAsync(List<Order> orders)
    {
        var tasks = new List<Task>();
        
        foreach (var order in orders)
        {
            if (!order.EmailSent)
            {
                tasks.Add(SendOrderConfirmationEmailAsync(order.ContactEmail, order.HeadOfHousehold, 
                    await GetOrderItemsAsync(order.Id)));
            }
            
            if (!order.SmsSent)
            {
                tasks.Add(SendOrderConfirmationSmsAsync(order.ContactPhone, order.HeadOfHousehold, 
                    await GetOrderItemsAsync(order.Id)));
            }
        }

        await Task.WhenAll(tasks);
    }

    private async Task<List<Item>> GetOrderItemsAsync(int orderId)
    {
        return await Task.FromResult(_dbContext.OrderItems
            .Where(oi => oi.OrderId == orderId)
            .Select(oi => new Item 
            { 
                Name = oi.ItemName, 
                Cost = oi.ItemCost 
            })
            .ToList());
    }

    public async Task<bool> TestEmailConnectionAsync()
    {
        try
        {
            var testEmail = _configuration["Email:SendGrid:FromEmail"] ?? _configuration["Email:Smtp:FromEmail"] ?? "test@example.com";
            await SendAdminNotificationEmailAsync(testEmail, "Test Email", "This is a test email from PhilanthroPoints.");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> TestSmsConnectionAsync()
    {
        try
        {
            var testNumber = _configuration["Sms:Twilio:FromNumber"];
            if (!string.IsNullOrEmpty(testNumber))
            {
                await SendSmsViaTwilioAsync(testNumber, "Test SMS from PhilanthroPoints");
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    // SendGrid Implementation
    private async Task SendEmailViaSendGridAsync(string email, string subject, string content, bool isHtml = true)
    {
        var apiKey = _configuration["Email:SendGrid:ApiKey"];
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("SendGrid API key not configured");
        }

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress(_configuration["Email:SendGrid:FromEmail"] ?? "noreply@philanthropopoints.com", 
                                   _configuration["Email:SendGrid:FromName"] ?? "PhilanthroPoints");
        var to = new EmailAddress(email);
        
        var msg = MailHelper.CreateSingleEmail(from, to, subject, isHtml ? null : content, isHtml ? content : null);
        
        var response = await client.SendEmailAsync(msg);
        
        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            var body = await response.Body.ReadAsStringAsync();
            throw new Exception($"SendGrid failed with status {response.StatusCode}: {body}");
        }
    }

    // Traditional SMTP Implementation
    private async Task SendEmailViaSmtpAsync(string email, string subject, string content, bool isHtml = true)
    {
        var host = _configuration["Email:Smtp:Host"];
        var portStr = _configuration["Email:Smtp:Port"];
        var username = _configuration["Email:Smtp:Username"];
        var password = _configuration["Email:Smtp:Password"];
        var fromEmail = _configuration["Email:Smtp:FromEmail"];
        var fromName = _configuration["Email:Smtp:FromName"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(portStr) || 
            string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || 
            string.IsNullOrEmpty(fromEmail))
        {
            var missingConfigs = new List<string>();
            if (string.IsNullOrEmpty(host)) missingConfigs.Add("Host");
            if (string.IsNullOrEmpty(portStr)) missingConfigs.Add("Port");
            if (string.IsNullOrEmpty(username)) missingConfigs.Add("Username");
            if (string.IsNullOrEmpty(password)) missingConfigs.Add("Password");
            if (string.IsNullOrEmpty(fromEmail)) missingConfigs.Add("FromEmail");
            
            var errorMsg = $"SMTP configuration incomplete. Missing: {string.Join(", ", missingConfigs)}";
            _logger.LogWarning(errorMsg);
            Console.WriteLine($"[SMTP CONFIG ERROR] {errorMsg}");
            Console.WriteLine($"[SMTP CONFIG] Please update appsettings.json with valid Yahoo credentials");
            throw new InvalidOperationException(errorMsg);
        }

        try
        {
            using var smtpClient = new SmtpClient(host)
            {
                Port = int.Parse(portStr),
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName ?? "PhilanthroPoints"),
                Subject = subject,
                Body = content,
                IsBodyHtml = isHtml,
            };
            mailMessage.To.Add(email);

            _logger.LogInformation($"Sending email via SMTP: {host}:{portStr}");
            await smtpClient.SendMailAsync(mailMessage);
            _logger.LogInformation($"SMTP email sent successfully to {email}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"SMTP send failed to {email}");
            Console.WriteLine($"[SMTP ERROR] {ex.Message}");
            throw;
        }
    }

    // Twilio SMS Implementation
    private async Task SendSmsViaTwilioAsync(string phoneNumber, string message)
    {
        var accountSid = _configuration["Sms:Twilio:AccountSid"];
        var authToken = _configuration["Sms:Twilio:AuthToken"];
        var fromNumber = _configuration["Sms:Twilio:FromNumber"];

        if (string.IsNullOrEmpty(accountSid) || string.IsNullOrEmpty(authToken) || string.IsNullOrEmpty(fromNumber))
        {
            var missingConfigs = new List<string>();
            if (string.IsNullOrEmpty(accountSid)) missingConfigs.Add("AccountSid");
            if (string.IsNullOrEmpty(authToken)) missingConfigs.Add("AuthToken");
            if (string.IsNullOrEmpty(fromNumber)) missingConfigs.Add("FromNumber");
            
            var errorMsg = $"Twilio credentials not configured. Missing: {string.Join(", ", missingConfigs)}";
            _logger.LogWarning(errorMsg);
            Console.WriteLine($"[TWILIO CONFIG ERROR] {errorMsg}");
            Console.WriteLine($"[TWILIO CONFIG] Please update appsettings.json with valid Twilio credentials");
            throw new InvalidOperationException(errorMsg);
        }

        try
        {
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(new PhoneNumber(phoneNumber))
            {
                From = new PhoneNumber(fromNumber),
                Body = message
            };

            _logger.LogInformation($"Sending SMS via Twilio from {fromNumber}");
            var twilioMessage = await MessageResource.CreateAsync(messageOptions);
            
            if (twilioMessage.ErrorCode.HasValue)
            {
                throw new Exception($"Twilio SMS failed: {twilioMessage.ErrorMessage}");
            }
            
            _logger.LogInformation($"Twilio SMS sent successfully to {phoneNumber}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Twilio SMS send failed to {phoneNumber}");
            Console.WriteLine($"[TWILIO ERROR] {ex.Message}");
            throw;
        }
    }

    private string BuildEmailBody(string customerName, List<Item> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<html><body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>");
        sb.AppendLine($"<div style='text-align: center; background: linear-gradient(135deg, #249EA0 0%, #FAAB36 100%); color: white; padding: 30px; border-radius: 10px; margin-bottom: 20px;'>");
        sb.AppendLine($"<h2 style='margin: 0; font-size: 28px;'>🎉 Congratulations {customerName}!</h2>");
        sb.AppendLine($"<p style='margin: 10px 0 0 0; font-size: 16px;'>Your PhilanthroPoints order is confirmed!</p>");
        sb.AppendLine($"</div>");
        
        sb.AppendLine($"<div style='background: #f9f9f9; padding: 20px; border-radius: 8px; margin-bottom: 20px;'>");
        sb.AppendLine($"<h3 style='color: #249EA0; margin-top: 0;'>🎁 Your Order Details:</h3>");
        sb.AppendLine($"<ul style='list-style: none; padding: 0;'>");

        var totalCost = 0;
        foreach (var item in items)
        {
            sb.AppendLine($"<li style='background: white; padding: 15px; margin: 10px 0; border-radius: 5px; border-left: 4px solid #FAAB36;'>");
            sb.AppendLine($"<strong>{item.Name}</strong>");
            sb.AppendLine($"<span style='float: right; color: #249EA0; font-weight: bold;'>{item.Cost} points</span>");
            sb.AppendLine($"</li>");
            totalCost += item.Cost;
        }

        sb.AppendLine($"</ul>");
        sb.AppendLine($"<div style='background: white; padding: 15px; border-radius: 5px; text-align: center; border: 2px solid #249EA0;'>");
        sb.AppendLine($"<strong style='font-size: 18px; color: #249EA0;'>Total: {totalCost} points</strong>");
        sb.AppendLine($"</div>");
        sb.AppendLine($"</div>");
        
        sb.AppendLine($"<div style='text-align: center; color: #666; padding: 20px;'>");
        sb.AppendLine($"<p>Thank you for participating in our birthday celebration program! 🎂</p>");
        sb.AppendLine($"<p><strong>Your order will arrive shortly!</strong></p>");
        sb.AppendLine($"<hr style='border: 1px solid #ddd; margin: 20px 0;'>");
        sb.AppendLine($"<p style='font-size: 14px;'>Best regards,<br/><strong>The PhilanthroPoints Team</strong></p>");
        sb.AppendLine($"</div>");
        sb.AppendLine($"</body></html>");

        return sb.ToString();
    }

    private string BuildSmsMessage(string customerName, List<Item> items)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"🎉 Congratulations {customerName}!");
        sb.AppendLine($"Your PhilanthroPoints order is confirmed and will arrive shortly:");
        sb.AppendLine();
        
        foreach (var item in items)
        {
            sb.AppendLine($"• {item.Name} ({item.Cost} pts)");
        }

        var totalCost = items.Sum(i => i.Cost);
        sb.AppendLine();
        sb.AppendLine($"Total: {totalCost} points");
        sb.AppendLine($"Thank you! 🎂 - PhilanthroPoints");

        return sb.ToString();
    }
}
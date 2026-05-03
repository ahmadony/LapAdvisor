using LapAdvisor.Model;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

public class ContactController : Controller
{
    [HttpPost]
    public IActionResult SendMessage(VMContactUs model)
    {
        try
        {
            var mail = new MailMessage();
            mail.From = new MailAddress("ahmadalsmadi2004@gmail.com");
            mail.To.Add("ahmadalsmadi2004@gmail.com"); // الإيميل اللي بدك توصله الرسائل
            mail.Subject = "New Contact Message - LapAdvisor";

            mail.Body =
                $"Name: {model.Name}\n" +
                $"Email: {model.Email}\n\n" +
                $"Message:\n{model.Message}";

            var smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential(
                "ahmadalsmadi2004@gmail.com",
                "ujbp letc kjmy ijiu"
            );
            smtp.EnableSsl = true;
            smtp.Send(mail);

            TempData["Success"] = "Your message has been sent successfully.";
        }
        catch
        {
            TempData["Error"] = "Something went wrong. Please try again.";
        }

        // رجوع لنفس صفحة Contact
        return RedirectToAction("Page", "Pages", new { pageId = 11 });
    }
}

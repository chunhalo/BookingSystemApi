using AnnouncementLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BookingSystemApi.Models
{
    public class EmailHelper
    {
        public bool SendEmailPasswordReset(string userEmail, Uri link)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("safechatproject8@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Password Reset";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = link.ToString();

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("safechatproject8@gmail.com", "cstmiwzlmxugjezv");
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            try
            {

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        public bool SendEmail(string userEmail, Uri confirmationlink)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("safechatproject8@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Email Confirmation";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = confirmationlink.ToString();

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("safechatproject8@gmail.com", "cstmiwzlmxugjezv");
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            try
            {

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        public bool SendAnnouncementEmail(string userEmail, AnnouncementClass announcement)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("safechatproject8@gmail.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = announcement.Title;
            mailMessage.IsBodyHtml = true;

            //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(announcement.Description + "<image src=cid:HDIImage>", null, "text/html");
            ////LinkedResource imageResource = new LinkedResource(Server.MapPath("~/Path/To/YourImage.png"));
            ////LinkedResource imageResource = new LinkedResource("https://localhost:44325/AnnouncementImages/download.jfif");
            //imageResource.ContentId = "HDIImage";
            //htmlView.LinkedResources.Add(imageResource);


            //mailMessage.AlternateViews.Add(htmlView);
            //mailMessage.BodyEncoding = Encoding.Default;
            // mailMessage.Body = announcement.Description+ "<br /><img src='https://localhost:44325/AnnouncementImages/"+announcement.Image+"' alt='Image' width='200' height='200' />";
            mailMessage.Body = announcement.Description;
            /*+ "<br /><img src='https://localhost:44325/AnnouncementImages/download.jfif' alt='Image' width='200' height='200' />"*/
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("safechatproject8@gmail.com", "cstmiwzlmxugjezv");
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            try
            {

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }



        public bool SendUserBookingEmail(BookingClass booking)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("safechatproject8@gmail.com");
            mailMessage.To.Add(new MailAddress(booking.Email));

            mailMessage.Subject = "Booking Pending";
            mailMessage.IsBodyHtml = true;

            //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(booking.Res.Name+ "<image src=cid:HDIImage>", null, "text/html");

            //LinkedResource imageResource = new LinkedResource(Server.MapPath("~/wwwroot/images/Announcement/download(1).png"));
            //imageResource.ContentId = "HDIImage";
            //htmlView.LinkedResources.Add(imageResource);


            //mailMessage.AlternateViews.Add(htmlView);
            mailMessage.Body = "Booking ID: " + booking.BookId + " <br /> Your Booking on " + booking.ResName + " from " + booking.StartDate + " to " + booking.EndDate + " with Table No "
                + booking.TableNo + " has been placed on pending. You will receive an email notification when admin approve the booking.";

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("safechatproject8@gmail.com", "cstmiwzlmxugjezv");
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            try
            {

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }

        public bool SendConfirmBookingEmail(BookingClass booking)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("safechatproject8@gmail.com");
            mailMessage.To.Add(new MailAddress(booking.Email));

            mailMessage.Subject = "Booking Confirmed";
            mailMessage.IsBodyHtml = true;

            //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(booking.Res.Name+ "<image src=cid:HDIImage>", null, "text/html");

            //LinkedResource imageResource = new LinkedResource(Server.MapPath("~/wwwroot/images/Announcement/download(1).png"));
            //imageResource.ContentId = "HDIImage";
            //htmlView.LinkedResources.Add(imageResource);


            //mailMessage.AlternateViews.Add(htmlView);
            mailMessage.Body = "Booking ID: " + booking.BookId + " <br /> Your Booking on " + booking.ResName + " from " + booking.StartDate + " to " + booking.EndDate + " with Table No "
                + booking.TableNo + " has been approved.";

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("safechatproject8@gmail.com", "cstmiwzlmxugjezv");
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            try
            {

                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }
    }

}

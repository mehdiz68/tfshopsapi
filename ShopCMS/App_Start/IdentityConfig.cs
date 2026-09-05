using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Domain;
using DataLayer;
using System.Net.Mail;
using CoreLib.Infrastructure.SMS;
using Kavenegar;
using RestSharp;
using UnitOfWork;

namespace TfShop
{

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            // Credentials:
            var credentialUserName = System.Configuration.ConfigurationManager.AppSettings["credentialUserName"];
            var sentFrom = System.Configuration.ConfigurationManager.AppSettings["siteName"];
            var pwd = System.Configuration.ConfigurationManager.AppSettings["pwd"];

            // Configure the client:
            System.Net.Mail.SmtpClient client =
                new System.Net.Mail.SmtpClient(System.Configuration.ConfigurationManager.AppSettings["smtp"]);

            client.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Create the credentials:
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = false;
            client.Credentials = credentials;

            // Create the message:
            var mail =
                new System.Net.Mail.MailMessage(credentialUserName, message.Destination);

            mail.Subject = message.Subject;
            mail.Body = message.Body;

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            // Send:
            return client.SendMailAsync(mail);

        }

        public Task SendMultiDestinationAsync(string Subject, string Body, List<string> Destinations)
        {
            // Plug in your email service here to send an email.
            // Credentials:
            var credentialUserName = System.Configuration.ConfigurationManager.AppSettings["credentialUserName"];
            var sentFrom = System.Configuration.ConfigurationManager.AppSettings["siteName"];
            var pwd = System.Configuration.ConfigurationManager.AppSettings["pwd"];

            // Configure the client:
            System.Net.Mail.SmtpClient client =
                new System.Net.Mail.SmtpClient(System.Configuration.ConfigurationManager.AppSettings["smtp"]);

            client.Port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Port"]);
            client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Create the credentials:
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(credentialUserName, pwd);

            client.EnableSsl = false;
            client.Credentials = credentials;

            // Create the message:
            var mail = new System.Net.Mail.MailMessage(credentialUserName, Destinations.First());
            foreach (var item in Destinations.Skip(1))
            {
                mail.To.Add(item);
            }

            mail.Subject = Subject;
            mail.Body = Body;

            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            // Send:
            return client.SendMailAsync(mail);

        }
    }

    public class SmsService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {

            await Task.FromResult(0);
        }
        public async Task SendSMSAsync(IdentityMessage message, string template = null, string token = null, string token2 = null, string token3 = null, string token10 = null, string token20 = null, bool Public = false, bool unsent = false)
        {
            // Credentials:
            string SMSUsername, SMSPassword, SMSSenderNumber, SMSDomainName, SMSCompany, siteName = "";
            if (Public == false)
            {
                SMSUsername = System.Configuration.ConfigurationManager.AppSettings["SMSUsername"];
                SMSPassword = System.Configuration.ConfigurationManager.AppSettings["SMSPassword"];
                SMSSenderNumber = System.Configuration.ConfigurationManager.AppSettings["SMSSenderNumber"];
                SMSDomainName = System.Configuration.ConfigurationManager.AppSettings["SMSDomainName"];
                SMSCompany = System.Configuration.ConfigurationManager.AppSettings["SMSCompany"];
                siteName = System.Configuration.ConfigurationManager.AppSettings["siteName"];
            }
            else
            {
                SMSUsername = System.Configuration.ConfigurationManager.AppSettings["SMSUsername2"];
                SMSPassword = System.Configuration.ConfigurationManager.AppSettings["SMSPassword2"];
                SMSSenderNumber = System.Configuration.ConfigurationManager.AppSettings["SMSSenderNumber2"];
                SMSDomainName = System.Configuration.ConfigurationManager.AppSettings["SMSDomainName2"];
                SMSCompany = System.Configuration.ConfigurationManager.AppSettings["SMSCompany2"];
                siteName = System.Configuration.ConfigurationManager.AppSettings["siteName2"];
            }

            //if (NikSmsManager.SingleSms(message.Body, message.Destination, SMSUsername, SMSPassword, SMSSenderNumber) != "")
            //{
            //    var api = new KavenegarApi("7145682F7961563461476B36596D5854424767322B385543736D70704C4B346F");
            //    if (template == "NewLetmeKnowExist" || template == "NewLetmeKnowOff")
            //        api.VerifyLookup(message.Destination, token, null, null, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
            //    else if (token != null && token2 != null && token3 != null && token10 != null && token20 != null)
            //        api.VerifyLookup(message.Destination, token, token2, token3, token10, token20, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
            //    else if (token != null && token3 != null && token10 != null)
            //        api.VerifyLookup(message.Destination, token, null, token3, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
            //    else if (token != null && token2 != null && token3 == null)
            //        api.VerifyLookup(message.Destination, token, token2, null, null, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
            //    else if (token != null && token10 != null && token20 == null)
            //        api.VerifyLookup(message.Destination, token, null, null, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
            //    else if (token10 != null)
            //        api.VerifyLookup(message.Destination, token, null, null, token10, token20, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
            //    else
            //        api.VerifyLookup(message.Destination, token, token2, token3, template);
            //}

            if (SMSCompany.ToString() == "1")//NikSms
            {
                //using (UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass())
                //{
                //    uow.UnSentMessageRepository.Insert(new UnSentMessage() { InsertDate = DateTime.Now, IsActive = true, Mobile = message.Destination, state = false, Text = message.Body });
                //    uow.Save();
                //}
                var result = await NikSmsManager.ss(message.Body, message.Destination, SMSUsername, SMSPassword, SMSSenderNumber);
                if (result.ReturnType == false)
                {
                    using (UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass())
                    {
                        if (unsent == false && !uow.UnSentMessageRepository.Any(x => x.Id, x => x.Text == message.Body && x.Mobile == message.Destination && x.state == false && x.InUpdate == false && x.IsActive == true))
                        {
                            TfShop.Infrastructure.EventLog.Logger.Add(5, "SendSmsJob", "Index", true, 500, "عدم ارسال sms به " + message.Destination + " کد خطا :" + result.status, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
                            uow.UnSentMessageRepository.Insert(new UnSentMessage() { InsertDate = DateTime.Now, IsActive = true, Mobile = message.Destination, state = false, Text = message.Body });
                            uow.Save();

                        }
                    }
                }
                else
                    TfShop.Infrastructure.EventLog.Logger.Add(1, "SendSmsJob", "Index", true, 200, "ارسال sms صحیح به " + message.Destination + "، " + message.Body.ToString() + " کد خطا :" + result.status, DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");
            }
            else if (SMSCompany.ToString() == "2")//payamakPanel
            {
                PanelPayamakManager.SingleSms(message.Body, message.Destination, SMSUsername, SMSPassword, SMSSenderNumber);
            }
            else//kavenegar
            {
                var api = new KavenegarApi("7145682F7961563461476B36596D5854424767322B385543736D70704C4B346F");
                if (template == "NewLetmeKnowExist" || template == "NewLetmeKnowOff")
                    api.VerifyLookup(message.Destination, token, null, null, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token != null && token2 != null && token3 != null && token10 != null && token20 != null)
                    api.VerifyLookup(message.Destination, token, token2, token3, token10, token20, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token != null && token3 != null && token10 != null)
                    api.VerifyLookup(message.Destination, token, null, token3, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token != null && token2 != null && token3 == null)
                    api.VerifyLookup(message.Destination, token, token2, null, null, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token != null && token10 != null && token20 == null)
                    api.VerifyLookup(message.Destination, token, null, null, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token10 != null)
                    api.VerifyLookup(message.Destination, token, null, null, token10, token20, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else
                    api.VerifyLookup(message.Destination, token, token2, token3, template);

            }

            // Plug in your SMS service here to send a text message.
            await Task.FromResult(0);
        }
        public async Task SendSMSAsyncWithDelay(IdentityMessage message, string template = null, string token = null, string token2 = null, string token3 = null, string token10 = null, string token20 = null, bool Public = false, bool unsent = false)
        {
            // Credentials:
            string SMSUsername, SMSPassword, SMSSenderNumber, SMSDomainName, SMSCompany, siteName = "";
            if (Public == false)
            {
                SMSUsername = System.Configuration.ConfigurationManager.AppSettings["SMSUsername"];
                SMSPassword = System.Configuration.ConfigurationManager.AppSettings["SMSPassword"];
                SMSSenderNumber = System.Configuration.ConfigurationManager.AppSettings["SMSSenderNumber"];
                SMSDomainName = System.Configuration.ConfigurationManager.AppSettings["SMSDomainName"];
                SMSCompany = System.Configuration.ConfigurationManager.AppSettings["SMSCompany"];
                siteName = System.Configuration.ConfigurationManager.AppSettings["siteName"];
            }
            else
            {
                SMSUsername = System.Configuration.ConfigurationManager.AppSettings["SMSUsername2"];
                SMSPassword = System.Configuration.ConfigurationManager.AppSettings["SMSPassword2"];
                SMSSenderNumber = System.Configuration.ConfigurationManager.AppSettings["SMSSenderNumber2"];
                SMSDomainName = System.Configuration.ConfigurationManager.AppSettings["SMSDomainName2"];
                SMSCompany = System.Configuration.ConfigurationManager.AppSettings["SMSCompany2"];
                siteName = System.Configuration.ConfigurationManager.AppSettings["siteName2"];
            }



            if (SMSCompany.ToString() == "1")//NikSms
            {
                using (UnitOfWork.UnitOfWorkClass uow = new UnitOfWork.UnitOfWorkClass())
                {
                    uow.UnSentMessageRepository.Insert(new UnSentMessage() { InsertDate = DateTime.Now, IsActive = true, Mobile = message.Destination, state = false, Text = message.Body });
                    uow.Save();
                }

            }
            else if (SMSCompany.ToString() == "2")//payamakPanel
            {
                PanelPayamakManager.SingleSms(message.Body, message.Destination, SMSUsername, SMSPassword, SMSSenderNumber);
            }
            else//kavenegar
            {
                var api = new KavenegarApi("7145682F7961563461476B36596D5854424767322B385543736D70704C4B346F");
                if (template == "NewLetmeKnowExist" || template == "NewLetmeKnowOff")
                    api.VerifyLookup(message.Destination, token, null, null, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token != null && token2 != null && token3 != null && token10 != null && token20 != null)
                    api.VerifyLookup(message.Destination, token, token2, token3, token10, token20, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token != null && token3 != null && token10 != null)
                    api.VerifyLookup(message.Destination, token, null, token3, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token != null && token2 != null && token3 == null)
                    api.VerifyLookup(message.Destination, token, token2, null, null, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token != null && token10 != null && token20 == null)
                    api.VerifyLookup(message.Destination, token, null, null, token10, null, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else if (token10 != null)
                    api.VerifyLookup(message.Destination, token, null, null, token10, token20, template, Kavenegar.Models.Enums.VerifyLookupType.Sms);
                else
                    api.VerifyLookup(message.Destination, token, token2, token3, template);

            }

            // Plug in your SMS service here to send a text message.
            await Task.FromResult(0);
        }

        public async Task SendMultiDestinationAsync(string message, List<string> Destinations, string template = null, List<string> token = null, List<string> token2 = null, List<string> token3 = null, bool Public = false)
        {
            string SMSUsername, SMSPassword, SMSSenderNumber, SMSDomainName, SMSCompany, siteName = "";
            if (Public == false)
            {
                SMSUsername = System.Configuration.ConfigurationManager.AppSettings["SMSUsername"];
                SMSPassword = System.Configuration.ConfigurationManager.AppSettings["SMSPassword"];
                SMSSenderNumber = System.Configuration.ConfigurationManager.AppSettings["SMSSenderNumber"];
                SMSDomainName = System.Configuration.ConfigurationManager.AppSettings["SMSDomainName"];
                SMSCompany = System.Configuration.ConfigurationManager.AppSettings["SMSCompany"];
                siteName = System.Configuration.ConfigurationManager.AppSettings["siteName"];
            }
            else
            {
                SMSUsername = System.Configuration.ConfigurationManager.AppSettings["SMSUsername2"];
                SMSPassword = System.Configuration.ConfigurationManager.AppSettings["SMSPassword2"];
                SMSSenderNumber = System.Configuration.ConfigurationManager.AppSettings["SMSSenderNumber2"];
                SMSDomainName = System.Configuration.ConfigurationManager.AppSettings["SMSDomainName2"];
                SMSCompany = System.Configuration.ConfigurationManager.AppSettings["SMSCompany2"];
                siteName = System.Configuration.ConfigurationManager.AppSettings["siteName2"];
            }
            if (SMSCompany.ToString() == "1")//NikSms
            {
                //CoreLib.Infrastructure.SMS.HeroSMSManager.sendMulti(Destinations, message);
                await NikSmsManager.GroupSms(message + siteName, Destinations.ToArray(), SMSUsername, SMSPassword, SMSSenderNumber);
            }
            else if (SMSCompany.ToString() == "2")//payamakPanel
            {
                PanelPayamakManager.GroupSms(message + siteName, Destinations.ToArray(), SMSUsername, SMSPassword, SMSSenderNumber);
            }
            else//kavenegar
            {
                var api = new KavenegarApi("7145682F7961563461476B36596D5854424767322B385543736D70704C4B346F");
                int i = 0;
                foreach (var item in Destinations)
                {

                    api.VerifyLookup(item, token[i], token2[i], token3[i], template);
                    i++;
                }
            }


            // Plug in your SMS service here to send a text message.
            await Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }
        public override async Task<IdentityResult> UpdateAsync(ApplicationUser user)
        {
            Infrastructure.EventLog.Logger.Add(1, "IdentityConfig", "UpdateAsync", true, 200, " start", DateTime.Now, "b11c3d7b-4d2d-4b58-85f1-2e8e0c8d6d84");

            bool? oldValue = null;

            using (var db = new TfShopDbContext())
            {
                var oldUser = await db.Users
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(x => x.Id == user.Id);

                if (oldUser != null)
                    oldValue = oldUser.PhoneNumberConfirmed;
            }

            if (oldValue != null && oldValue != user.PhoneNumberConfirmed)
            {
                using (UnitOfWorkClass uow = new UnitOfWorkClass())
                {
                    PhoneChangeLog phoneChangeLog = new PhoneChangeLog()
                    {
                        ChangeDate = DateTime.Now,
                        NewValue = user.PhoneNumberConfirmed,
                        OldValue = oldValue.Value,
                        UserId = user.Id
                    };

                    uow.PhoneChangeLogRepository.Insert(phoneChangeLog);
                    uow.Save();
                }
            }

            return await base.UpdateAsync(user);
        }


        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<TfShopDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = false
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(10);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}

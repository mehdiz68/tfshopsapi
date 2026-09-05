using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TfShop.ViewModels.Api.HeaderVM;
using UnitOfWork;

namespace TfShop.Infrastructure.Sms
{
    public static class Pattern
    {
        public static bool checkBlackListNumber(string phonenumber, out bool sendsms, out string msg)
        {
            using (UnitOfWorkClass uow = new UnitOfWorkClass())
            {
                if (uow.BlackListNumberRepository.Any(x => x.Id, x => x.PhoneNumber == phonenumber))
                {
                    var blc = uow.BlackListNumberRepository.Get(x => x, x => x.PhoneNumber == phonenumber &&  x.SendSmsBlackList == false).First();
                    sendsms = blc.SendSmsBlackList;
                    msg = blc.MessageBlackList;
                    blc.SendSmsBlackList = true;
                    uow.BlackListNumberRepository.Update(blc);
                    uow.Save();
                    return true;
                }
                else if (uow.UserRepository.Any(x => x.Id, x => x.PhoneNumber == phonenumber && x.BlackList == true))
                {
                    var usr = uow.UserRepository.Get(x => x, x => x.PhoneNumber == phonenumber && x.BlackList == true).First();
                    sendsms = usr.SendSmsBlackList;
                    msg = usr.MessageBlackList;
                    usr.SendSmsBlackList = true;
                    uow.UserRepository.Update(usr);
                    uow.Save();
                    return true;
                }
                else
                {
                    sendsms = false;
                    msg = "";
                    return false;
                }
            }
        }
        public static string GetSmsBody(Domain.SmsPatternType smsPatternType, string UserId, string code = null, string link = null, string Ratelink = null, string Trackingcode = null, string ActivationCode = null, string Sendway = null, string OrderState = null, string OrderPrice = null, string OrderCity = null, string OrderPayTime = null, string ProfileLink = null, string OrderType = null, string NewDeliverDate = null, string NewDeliverTime = null, string reas = null, string DTime = null, string DDate = null, string DAdress = null,string DLink=null, string OrderGift = null,string sharePayTime=null,string payway=null)
        {
            UnitOfWorkClass uow = new UnitOfWorkClass();
            var SmsPattern = uow.SmsPatternRepository.Get(x => x, x => x.smsPatternType == smsPatternType).FirstOrDefault();
            if (SmsPattern != null)
            {
                string pattern = SmsPattern.Text;
                if (!string.IsNullOrEmpty(UserId))
                {
                    var user = uow.UserRepository.GetByID(UserId);
                    if (pattern.Contains("%Name"))
                        pattern = pattern.Replace("%Name", user.FirstName);
                    if (pattern.Contains("%FullName"))
                        pattern = pattern.Replace("%FullName", user.LastName);
                }
                if (pattern.Contains("%code") && !String.IsNullOrEmpty(code))
                    pattern = pattern.Replace("%code", code);
                if (pattern.Contains("%link") && !String.IsNullOrEmpty(link))
                    pattern = pattern.Replace("%link", link);
                if (pattern.Contains("%Ratelink") && !String.IsNullOrEmpty(Ratelink))
                    pattern = pattern.Replace("%Ratelink", Ratelink);
                if (pattern.Contains("%Trackingcode") && !String.IsNullOrEmpty(Trackingcode))
                    pattern = pattern.Replace("%Trackingcode", Trackingcode);
                if (pattern.Contains("%ActivationCode") && !String.IsNullOrEmpty(ActivationCode))
                    pattern = pattern.Replace("%ActivationCode", ActivationCode);
                if (pattern.Contains("%Sendway") && !String.IsNullOrEmpty(Sendway))
                    pattern = pattern.Replace("%Sendway", Sendway);
                if (pattern.Contains("%OrderState") && !String.IsNullOrEmpty(OrderState))
                    pattern = pattern.Replace("%OrderState", OrderState);
                if (pattern.Contains("%OrderPrice") && !String.IsNullOrEmpty(OrderPrice))
                    //pattern = pattern.Replace("%OrderPrice", string.Format("{0:n0}", Convert.ToInt64(OrderPrice)));
                    pattern = pattern.Replace("%OrderPrice", OrderPrice);
                if (pattern.Contains("%OrderCity") && !String.IsNullOrEmpty(OrderCity))
                    pattern = pattern.Replace("%OrderCity", OrderCity);
                if (pattern.Contains("%OrderPayTime") && !String.IsNullOrEmpty(OrderPayTime))
                    pattern = pattern.Replace("%OrderPayTime", OrderPayTime);
                if (pattern.Contains("%ProfileLink") && !String.IsNullOrEmpty(ProfileLink))
                    pattern = pattern.Replace("%ProfileLink", ProfileLink);
                if (pattern.Contains("%OrderType") && !String.IsNullOrEmpty(OrderType))
                    pattern = pattern.Replace("%OrderType", OrderType);
                if (pattern.Contains("%NewDeliverDate") && !String.IsNullOrEmpty(NewDeliverDate))
                    pattern = pattern.Replace("%NewDeliverDate", NewDeliverDate);
                if (pattern.Contains("%reas") && !String.IsNullOrEmpty(reas))
                    pattern = pattern.Replace("%reas", reas);
                if (pattern.Contains("%CurrentDate"))
                    pattern = pattern.Replace("%CurrentDate", CoreLib.Infrastructure.DateTime.DateTimeConverter.ChangeMiladiToShamsi(DateTime.Now));
                if (pattern.Contains("%NewDeliverTime") && !String.IsNullOrEmpty(NewDeliverTime))
                    pattern = pattern.Replace("%NewDeliverTime", NewDeliverTime);
                if (pattern.Contains("%DTime") && !String.IsNullOrEmpty(DTime))
                    pattern = pattern.Replace("%DTime", DTime);
                if (pattern.Contains("%DDate") && !String.IsNullOrEmpty(DDate))
                    pattern = pattern.Replace("%DDate", DDate);
                if (pattern.Contains("%DAdress") && !String.IsNullOrEmpty(DAdress))
                    pattern = pattern.Replace("%DAdress", DAdress);
                if (pattern.Contains("%DLinkPay") && !String.IsNullOrEmpty(DLink))
                    pattern = pattern.Replace("%DLinkPay", DLink);
                if (pattern.Contains("%OrderGift") && !String.IsNullOrEmpty(OrderGift))
                    pattern = pattern.Replace("%OrderGift", OrderGift);
                if (pattern.Contains("%sharePayTime") && !String.IsNullOrEmpty(sharePayTime))
                    pattern = pattern.Replace("%sharePayTime", sharePayTime);
                if (pattern.Contains("%payway") && !String.IsNullOrEmpty(payway))
                    pattern = pattern.Replace("%payway", payway);

                return pattern;
            }
            else
                return "";
        }
    }
}
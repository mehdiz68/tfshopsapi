using Domain;
using Domain.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Domain.ViewModels
{
    public class SMSPattern
    {
        public SMSPattern()
        {

        }
        public IPagedList<UserOfferMessage> userOfferMessages { get; set; }
        public IPagedList<UserGroupMessage> userGroupMessages { get; set; }
        public IEnumerable<letmeKnowMessage> letmeKnowMessages { get; set; }
        public IEnumerable<CancelOrderMenssage> cancelOrderMenssages { get; set; }
        public IEnumerable<OrderReminderMenssage> OrderReminderMenssages { get; set; }
        public IEnumerable<SmsPattern> SmsPatterns { get; set; }

    }

}

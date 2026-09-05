namespace TfShop.Controllers
{
    // این نسخه‌ی کوچک‌شده‌ی ProfileController.cس اصلیه - از پروژه‌ی apiv1-only حذف شده چون فقط
    // یه صفحه‌سازِ MVC بود و apiv1 هیچ‌کدوم از اکشن‌هاش رو صدا نمی‌زنه. تنها چیزی که ApiV1Controller
    // واقعاً از این کلاس لازم داره (از طریق `using static TfShop.Controllers.ProfileController;`)
    // همین enum تودرتوئه؛ عمداً از Controller ارث‌بری نمی‌کنه تا هیچ روتِ MVC ای بهش نرسه (فقط یه
    // نگه‌دارنده‌ی enum ئه، نه یه کنترلرِ واقعی/routable).
    public class ProfileController
    {
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            SendConfirmEmail,
            SendConfirmPhone,
            Error
        }
    }
}

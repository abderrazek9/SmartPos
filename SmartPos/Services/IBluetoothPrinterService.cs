using System.Threading.Tasks;

namespace SmartPos.Services
{
    /// <summary>
    /// واجهة لتعريف دوال الطباعة عبر البلوتوث.
    /// من خلالها نضمن أن أي خدمة (حقيقية أو محاكية) ستطبق نفس الدوال.
    /// </summary>
    public interface IBluetoothPrinterService
    {
        /// <summary>
        /// يحاول إنشاء اتصال مع الطابعة عبر البلوتوث.
        /// </summary>
        Task<bool> ConnectAsync();

        /// <summary>
        /// يرسل المحتوى للطباعة.
        /// </summary>
        Task<bool> PrintAsync(string content);

        /// <summary>
        /// يفصل الاتصال بالطابعة.
        /// </summary>
        Task DisconnectAsync();
    }
}

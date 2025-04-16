using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SmartPos.Services
{
    /// <summary>
    /// خدمة محاكاة للطباعة عبر البلوتوث لا تتصل بطابعة حقيقية، بل تكتب في Debug.
    /// </summary>
    public class DummyBluetoothPrinterService : IBluetoothPrinterService
    {
        public async Task<bool> ConnectAsync()
        {
            // محاكاة الاتصال الناجح
            await Task.Delay(300);
            Debug.WriteLine("Dummy: تم الاتصال الافتراضي بالطابعة عبر البلوتوث.");
            return true;
        }

        public async Task<bool> PrintAsync(string content)
        {
            // محاكاة عملية الطباعة عن طريق كتابة النص في Debug
            await Task.Delay(500);
            Debug.WriteLine("Dummy: تمت عملية الطباعة الافتراضية.");
            Debug.WriteLine($"Dummy Printing content:\n{content}");
            return true;
        }

        public async Task DisconnectAsync()
        {
            // محاكاة عملية قطع الاتصال
            await Task.Delay(300);
            Debug.WriteLine("Dummy: تم قطع الاتصال الافتراضي بالطابعة.");
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace SmartPos.Services
{
    /// <summary>
    /// خدمة للطباعة عبر البلوتوث باستخدام مكتبة Plugin.BLE.
    /// تنفذ واجهة IBluetoothPrinterService.
    /// </summary>
    public class BluetoothPrinterService : IBluetoothPrinterService
    {
        private readonly IBluetoothLE _bluetoothLE;
        private readonly IAdapter _adapter;

        public BluetoothPrinterService()
        {
            _bluetoothLE = CrossBluetoothLE.Current;
            _adapter = CrossBluetoothLE.Current.Adapter;
        }

        /// <summary>
        /// يحاول الاتصال بالبلوتوث (تحقق بسيط).
        /// يمكنك تطوير المنطق للاقتران المسبق أو اختيار الجهاز المستهدف.
        /// </summary>
        public async Task<bool> ConnectAsync()
        {
            if (_bluetoothLE.State != BluetoothState.On)
                throw new Exception("البلوتوث غير مفعل. الرجاء تفعيله من الإعدادات.");

            // هنا يمكن إضافة أي إجراءات أخرى للاتصال، مثلًا البحث عن طابعة محددة أو التأكد من الإعدادات
            // سنكتفي بالتحقق من أن البلوتوث "On"
            await Task.Delay(200); // تأخير بسيط لمحاكاة أي إعدادات
            return true;
        }

        /// <summary>
        /// يرسل المحتوى للطباعة عبر البلوتوث.
        /// في هذا المثال يتم البحث عن أي جهاز بلوتوث اسمه غير فارغ (قد تحتاج لتعديله حسب نوع طابعتك).
        /// </summary>
        public async Task<bool> PrintAsync(string content)
        {
            if (_bluetoothLE.State != BluetoothState.On)
                throw new Exception("البلوتوث غير مفعل. الرجاء تفعيله من الإعدادات.");

            var discoveredDevices = new List<IDevice>();

            _adapter.DeviceDiscovered += (s, a) =>
            {
                // نضيف الجهاز إلى القائمة إن لم يكن موجودًا مسبقًا واسمه ليس فارغًا
                if (!discoveredDevices.Any(d => d.Id == a.Device.Id) && !string.IsNullOrWhiteSpace(a.Device.Name))
                {
                    discoveredDevices.Add(a.Device);
                }
            };

            // بدء البحث عن أجهزة بلوتوث
            await _adapter.StartScanningForDevicesAsync();
            // ننتظر 10 ثواني (يمكن تقليلها أو زيادتها حسب الحاجة)
            await Task.Delay(10000);
            // إيقاف البحث
            await _adapter.StopScanningForDevicesAsync();

            // نختار أول جهاز اسمه غير فارغ
            var printer = discoveredDevices.FirstOrDefault();
            if (printer == null)
                throw new Exception("لم يتم العثور على أي طابعة بلوتوث.");

            // محاولة الاتصال بالجهاز
            await _adapter.ConnectToDeviceAsync(printer);

            // هذا الـ GUID خاص بخدمة SPP (Serial Port Profile) المستخدمة في أغلب طابعات البلوتوث
            var serviceGuid = new Guid("00001101-0000-1000-8000-00805F9B34FB");

            // نجلب الخدمات المتاحة من الجهاز
            var services = await printer.GetServicesAsync();
            var targetService = services.FirstOrDefault(s => s.Id == serviceGuid);
            if (targetService == null)
                throw new Exception("لم يتم العثور على خدمة الطابعة المطلوبة.");

            // نجلب الخصائص (Characteristics) ونبحث عن واحدة قابلة للكتابة
            var characteristics = await targetService.GetCharacteristicsAsync();
            var writeCharacteristic = characteristics.FirstOrDefault(c => c.CanWrite);
            if (writeCharacteristic == null)
                throw new Exception("لا توجد خاصية قابلة للكتابة في الطابعة.");

            // تحويل النص إلى بايتات وإرسالها
            var data = Encoding.UTF8.GetBytes(content);
            await writeCharacteristic.WriteAsync(data);

            // أنهينا الإرسال، (قد تحتاج لإرسال رموز تحكم بالطابعة إن كانتESC/POS مثلاً)
            // فصل الاتصال
            await _adapter.DisconnectDeviceAsync(printer);

            return true;
        }

        /// <summary>
        /// قطع الاتصال بالطابعة (إن لزم).
        /// حاليا لا نحتفظ بجهاز متصل هنا، يتم الفصل مباشرة بعد الطباعة.
        /// لكن لو أردت الاحتفاظ باتصال مفتوح، يمكنك تخزين الجهاز المتصل في متغير لتتحكم به.
        /// </summary>
        public async Task DisconnectAsync()
        {
            await Task.CompletedTask;
        }
    }
}

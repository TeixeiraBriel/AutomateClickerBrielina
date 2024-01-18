using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AutomateClickerBrielina.Util
{
    public static class Timer
    {
        private static DispatcherTimer _timer = new DispatcherTimer
        {
            IsEnabled = false,
            Interval = new TimeSpan(0, 0, 0, 1, 0),
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Pode ser utilizado em um contexto específico.")]
        static Timer()
        {
            _timer.Tick += (sender, e) =>
            {
                _timer.Stop();
                Tick?.Invoke();
                _timer.Start();
            };
        }

        public static event Action Tick;

        internal static void Inicia() => _timer.IsEnabled = true;

        internal static void Para() => _timer.IsEnabled = false;
    }
}

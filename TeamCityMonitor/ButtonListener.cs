using System;
using Windows.Devices.Gpio;

namespace TeamCityMonitor
{
    public class ButtonListener
    {
        private readonly GpioPin _gpio;

        public ButtonListener(GpioController controller, int pin)
        {
            _gpio = controller.OpenPin(pin);

            var driveMode = _gpio.IsDriveModeSupported(GpioPinDriveMode.InputPullUp)
                ? GpioPinDriveMode.InputPullUp
                : GpioPinDriveMode.Input;
            _gpio.SetDriveMode(driveMode);

            _gpio.DebounceTimeout = TimeSpan.FromMilliseconds(50);

            _gpio.ValueChanged += GpioOnValueChanged;
        }

        private void GpioOnValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.FallingEdge)
            {
                Pressed?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler Pressed;
    }
}

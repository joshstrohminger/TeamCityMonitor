using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI;
using System.ComponentModel;

namespace BlinkStickUniversal
{
    public interface IBlinkStick : INotifyPropertyChanged
    {
        event BlinkStick.ErrorHandler OnError;

        ObservableCollection<byte> CurrentColors { get; }

        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        bool Connected { get; }

        /// <summary>
        /// Gets or sets the name of the device (InfoBlock1).
        /// </summary>
        /// <value>String value of InfoBlock1</value>
        string InfoBlock1 { get; set; }

        /// <summary>
        /// Gets or sets the data of the device (InfoBlock2).
        /// </summary>
        /// <value>String value of InfoBlock2</value>
        string InfoBlock2 { get; set; }

        int SetColorDelay { get; set; }

        /// <summary>
        /// Gets or sets the mode of BlinkStick device.
        /// </summary>
        /// <value>The mode to set or get.</value>
        int Mode { get; set; }

        /// <summary>
        /// Attempts to connect to a BlinkStick device.
        /// 
        /// After a successful connection, a DeviceAttached event will normally be sent.
        /// </summary>
        /// <returns>True if a Blinkstick device is connected, False otherwise.</returns>
        Task<bool> OpenDeviceAsync();

        /// <summary>
        /// Closes the connection to the device.
        /// </summary>
        void CloseDevice();

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format</param>
        Task SetColorAsync(string color);

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Color as RgbColor class.</param>
        Task SetColorAsync(Color color);

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        Task SetColorAsync(byte r, byte g, byte b);

        /// <summary>
        /// Gets the color of the led.
        /// </summary>
        /// <returns><c>true</c>, if led color was received, <c>false</c> otherwise.</returns>
        Task<(byte R, byte G, byte B)> GetColorAsync();

        /// <summary>
        /// Turn BlinkStick off.
        /// </summary>
        Task TurnOffAsync();

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        Task SetColorAsync(byte channel, byte index, byte r, byte g, byte b);

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        Task SetColorAsync(byte channel, byte index, string color);

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        Task SetColorAsync(byte channel, byte index, Color color);

        /// <summary>
        /// Send a packet of data to LEDs
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="colorData">Report data must be a byte array in the following format: [g0, r0, b0, g1, r1, b1, g2, r2, b2 ...]</param>
        Task SetColorsAsync(byte channel, byte[] colorData);

        /// <summary>
        /// Gets led data.
        /// </summary>
        /// <returns><c>true</c>, if led data was received, <c>false</c> otherwise.</returns>
        Task<byte[]> GetColorsAsync();

        /// <summary>
        /// Gets the color of the led.
        /// </summary>
        /// <returns><c>true</c>, if led color was received, <c>false</c> otherwise.</returns>
        Task<(byte R, byte G, byte B)> GetColorAsync(byte index);

        void Stop();
        void Enable();

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        Task BlinkAsync(byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int delay = 500);

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        Task BlinkAsync(byte channel, byte index, Color color, int repeats = 1, int delay = 500);

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        Task BlinkAsync(byte channel, byte index, string color, int repeats = 1, int delay = 500);

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        Task BlinkAsync(byte r, byte g, byte b, int repeats = 1, int delay = 500);

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        Task BlinkAsync(Color color, int repeats = 1, int delay = 500);

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        Task BlinkAsync(string color, int repeats = 1, int delay = 500);

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task MorphAsync(byte channel, byte index, byte r, byte g, byte b, int duration = 1000, int steps = 50);

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task MorphAsync(byte channel, byte index, Color color, int duration = 1000, int steps = 50);

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task MorphAsync(byte channel, byte index, string color, int duration = 1000, int steps = 50);

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task MorphAsync(byte r, byte g, byte b, int duration = 1000, int steps = 50);

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task MorphAsync(Color color, int duration = 1000, int steps = 50);

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task MorphAsync(string color, int duration = 1000, int steps = 50);

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task PulseAsync(byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50);

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task PulseAsync(byte channel, byte index, Color color, int repeats = 1, int duration = 1000, int steps = 50);

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task PulseAsync(byte channel, byte index, string color, int repeats = 1, int duration = 1000, int steps = 50);

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task PulseAsync(byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50);

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task PulseAsync(Color color, int repeats = 1, int duration = 1000, int steps = 50);

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        Task PulseAsync(string color, int repeats = 1, int duration = 1000, int steps = 50);
    }
}
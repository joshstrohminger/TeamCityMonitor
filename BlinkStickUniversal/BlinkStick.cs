using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage.Streams;

namespace BlinkStickUniversal
{
    public class BlinkStick
    {
        #region Fields

        protected const int UsagePage = 0xFF00;
        protected const int UsageId = 0x0001;
        protected const int VendorId = 0x20A0;
        protected const int ProductId = 0x41E5;

        private readonly DeviceInformation _info;
        private HidDevice _device;
        private bool _stopped;

        #endregion

        #region Events

        public delegate void ErrorHandler(string msg, Exception e);
        public event ErrorHandler OnError;

        #endregion Events

        #region Device Properties

        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool Connected => _device != null;

        /// <summary>
        /// Gets the device type
        /// </summary>
        /// <value>Returns the device type.</value>
        public BlinkStickDeviceEnum BlinkStickDevice { get; private set; }

        private string _infoBlock1;
        /// <summary>
        /// Gets or sets the name of the device (InfoBlock1).
        /// </summary>
        /// <value>String value of InfoBlock1</value>
        public string InfoBlock1
        {
            get
            {
                if (_infoBlock1 != null) return _infoBlock1;
                var getInfoBlockTask = Task.Run(async () => await GetInfoBlockAsStringAsync(2));
                getInfoBlockTask.Wait();
                if (getInfoBlockTask.IsFaulted)
                {
                    _infoBlock1 = null;
                    OnError?.Invoke($"Failed to get {nameof(InfoBlock1)}", getInfoBlockTask.Exception);
                }
                else
                {
                    _infoBlock1 = getInfoBlockTask.Result;
                }
                return _infoBlock1;
            }
            set
            {
                if (_infoBlock1 == value) return;
                var previousValue = _infoBlock1;
                _infoBlock1 = value;
                Task.Run(async () =>
                {
                    await SetInfoBlockAsync(2, _infoBlock1);
                }).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        OnError?.Invoke($"Failed to set {nameof(InfoBlock1)}, resetting to {previousValue}", task.Exception);
                        _infoBlock1 = previousValue;
                    }
                });
            }
        }

        private string _infoBlock2;
        /// <summary>
        /// Gets or sets the data of the device (InfoBlock2).
        /// </summary>
        /// <value>String value of InfoBlock2</value>
        public string InfoBlock2
        {
            get
            {
                if (_infoBlock2 != null) return _infoBlock2;
                var getInfoBlockTask = Task.Run(async () => await GetInfoBlockAsStringAsync(3));
                getInfoBlockTask.Wait();
                if (getInfoBlockTask.IsFaulted)
                {
                    _infoBlock2 = null;
                    OnError?.Invoke($"Failed to get {nameof(InfoBlock2)}", getInfoBlockTask.Exception);
                }
                else
                {
                    _infoBlock2 = getInfoBlockTask.Result;
                }
                return _infoBlock2;
            }
            set
            {
                if (_infoBlock2 == value) return;
                var previousValue = _infoBlock2;
                _infoBlock2 = value;
                Task.Run(async () => {
                    await SetInfoBlockAsync(3, _infoBlock2);
                }).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        OnError?.Invoke($"Failed to set {nameof(InfoBlock2)}, resetting to {previousValue}", task.Exception);
                        _infoBlock2 = previousValue;
                    }
                });
            }
        }

        public int SetColorDelay { get; set; }

        private int _mode = -1;

        /// <summary>
        /// Gets or sets the mode of BlinkStick device.
        /// </summary>
        /// <value>The mode to set or get.</value>
        public int Mode
        {
            get
            {
                if (_mode != -1) return _mode;
                var getModeTask = Task.Run(async () => {
                    var result = await GetModeAsync();
                    return result;
                });

                _mode = getModeTask.Result;
                return _mode;
            }
            set
            {
                if (_mode == value) return;
                var previousValue = _mode;
                _mode = value;
                Task.Run(async () => {
                    await SetModeAsync((byte)_mode);
                }).ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        OnError?.Invoke($"Failed to set {nameof(Mode)}, resetting to {previousValue}", task.Exception);
                        _mode = previousValue;
                    }
                });
            }
        }
        #endregion
        
        #region Device Open/Close functions

        public BlinkStick(DeviceInformation info)
        {
            _info = info;
        }

        /// <summary>
        /// Attempts to connect to a BlinkStick device.
        /// 
        /// After a successful connection, a DeviceAttached event will normally be sent.
        /// </summary>
        /// <returns>True if a Blinkstick device is connected, False otherwise.</returns>
        public async Task<bool> OpenDeviceAsync(BlinkStickDeviceEnum blinkStickDevice)
        {
            BlinkStickDevice = blinkStickDevice;
            var result = false;

            if (_info != null)
            {
                result = await OpenCurrentDevice();
            }

            return result;
        }

        /// <summary>
        /// Opens the current device.
        /// </summary>
        /// <returns><c>true</c>, if current device was opened, <c>false</c> otherwise.</returns>
        private async Task<bool> OpenCurrentDevice()
        {
            _device = await HidDevice.FromIdAsync(_info.Id, Windows.Storage.FileAccessMode.ReadWrite);
            return Connected;
        }

        /// <summary>
        /// Closes the connection to the device.
        /// </summary>
        public void CloseDevice()
        {
            _device.Dispose();
            _device = null;
        }
        #endregion

        #region Helper functions for InfoBlocks

        /// <summary>
        /// Sets the info block.
        /// </summary>
        /// <param name="id">2 - InfoBlock1, 3 - InfoBlock2</param>
        /// <param name="data">Maximum 32 bytes of data</param>
        private async Task SetInfoBlockAsync(byte id, string data)
        {
            await SetInfoBlockAsync(id, Encoding.ASCII.GetBytes(data));
        }

        private async Task<string> GetInfoBlockAsStringAsync(byte id)
        {
            var dataBytes = await GetInfoBlockAsync(id);

            for (var i = 1; i < dataBytes.Length; i++)
            {
                if (dataBytes[i] != 0) continue;
                Array.Resize(ref dataBytes, i);
                break;
            }

            return Encoding.ASCII.GetString(dataBytes, 1, dataBytes.Length - 1);
        }

        protected async Task SetInfoBlockAsync(byte id, byte[] data)
        {
            if (id == 2 || id == 3)
            {
                if (data.Length > 32)
                {
                    Array.Resize(ref data, 32);
                }
                else if (data.Length < 32)
                {
                    int size = data.Length;

                    Array.Resize(ref data, 32);

                    //pad with zeros
                    for (var i = size; i < 32; i++)
                    {
                        data[i] = 0;
                    }
                }

                Array.Resize(ref data, 33);


                for (var i = 32; i > 0; i--)
                {
                    data[i] = data[i - 1];
                }

                data[0] = id;

                await SetFeatureAsync(data);
            }
            else
            {
                throw new Exception("Invalid info block id");
            }
        }

        /// <summary>
        /// Gets the info block.
        /// </summary>
        /// <returns><c>true</c>, if info block was received, <c>false</c> otherwise.</returns>
        /// <param name="id">Identifier.</param>
        public async Task<byte[]> GetInfoBlockAsync(byte id)
        {
            if (!Connected) throw new Exception("Not connected");
            if (id == 2 || id == 3)
            {
                return await GetFeatureAsync(id);
            }
            throw new Exception("Invalid info block id");
        }
        #endregion

        #region Color manipulation functions
        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format</param>
        public async Task SetColorAsync(string color)
        {
            await SetColorAsync(RgbColor.FromString(color));
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="color">Color as RgbColor class.</param>
        public async Task SetColorAsync(RgbColor color)
        {
            await SetColorAsync(color.R, color.G, color.B);
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public async Task SetColorAsync(byte r, byte g, byte b)
        {
            if (Connected)
            {
                await SetFeatureAsync(new byte[] { 1, r, g, b });
            }
        }

        /// <summary>
        /// Gets the color of the led.
        /// </summary>
        /// <returns><c>true</c>, if led color was received, <c>false</c> otherwise.</returns>
        public async Task<Tuple<byte, byte, byte>> GetColorAsync()
        {
            if (!Connected) throw new Exception("Not connected");
            var report = await GetFeatureAsync(0x01);
            return new Tuple<byte, byte, byte>(report[1], report[2], report[3]);
        }

        /// <summary>
        /// Turn BlinkStick off.
        /// </summary>
        public async Task TurnOff()
        {
            await SetColorAsync(0, 0, 0);
        }
        #endregion

        #region Color manipulation functions for BlinkStick Pro
        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public async Task SetColorAsync(byte channel, byte index, byte r, byte g, byte b)
        {
            if (Connected)
            {
                await SetFeatureAsync(new byte[] { 5, channel, index, r, g, b });
            }
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        public async Task SetColorAsync(byte channel, byte index, string color)
        {
            await SetColorAsync(channel, index, RgbColor.FromString(color));
        }

        /// <summary>
        /// Sets the color of the led.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        public async Task SetColorAsync(byte channel, byte index, RgbColor color)
        {
            await SetColorAsync(channel, index, color.R, color.G, color.B);
        }

        /// <summary>
        /// Send a packet of data to LEDs
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="colorData">Report data must be a byte array in the following format: [g0, r0, b0, g1, r1, b1, g2, r2, b2 ...]</param>
        public async Task SetColorsAsync(byte channel, byte[] colorData)
        {
            var maxLeds = 64;
            byte reportId = 9;

            //Automatically determine the correct report id to send the data to
            if (colorData.Length <= 8 * 3)
            {
                maxLeds = 8;
                reportId = 6;
            }
            else if (colorData.Length <= 16 * 3)
            {
                maxLeds = 16;
                reportId = 7;
            }
            else if (colorData.Length <= 32 * 3)
            {
                maxLeds = 32;
                reportId = 8;
            }
            else if (colorData.Length <= 64 * 3)
            {
                maxLeds = 64;
                reportId = 9;
            }
            else if (colorData.Length <= 128 * 3)
            {
                maxLeds = 64;
                reportId = 10;
            }

            var data = new byte[maxLeds * 3 + 2];
            data[0] = reportId;
            data[1] = channel; // chanel index

            for (var i = 0; i < Math.Min(colorData.Length, data.Length - 2); i++)
            {
                data[i + 2] = colorData[i];
            }

            for (var i = colorData.Length + 2; i < data.Length; i++)
            {
                data[i] = 0;
            }

            await SetFeatureAsync(data);

            if (reportId == 10)
            {
                for (var i = 0; i < Math.Min(data.Length - 2, colorData.Length - 64 * 3); i++)
                {
                    data[i + 2] = colorData[64 * 3 + i];
                }

                for (var i = colorData.Length + 2 - 64 * 3; i < data.Length; i++)
                {
                    data[i] = 0;
                }

                data[0] = (byte)(reportId + 1);

                await SetFeatureAsync(data);
            }
        }

        /// <summary>
        /// Gets led data.
        /// </summary>
        /// <returns><c>true</c>, if led data was received, <c>false</c> otherwise.</returns>
        public async Task<byte[]> GetColors()
        {
            if (!Connected) throw new Exception("Not connected");
            var data = await GetFeatureAsync(0x09);

            var colorData = new byte[3 * 8 * 8];
            Array.Copy(data, 2, colorData, 0, colorData.Length);

            return colorData;
        }


        /// <summary>
        /// Gets the color of the led.
        /// </summary>
        /// <returns><c>true</c>, if led color was received, <c>false</c> otherwise.</returns>
        public async Task<Tuple<byte, byte, byte>> GetColorAsync(byte index)
        {
            if (index == 0)
            {
                return await GetColorAsync();
            }

            var colors = await GetColors();

            if (colors.Length >= (index + 1) * 3)
            {
                return new Tuple<byte, byte, byte>(colors[index * 3 + 1], colors[index * 3], colors[index * 3 + 2]);
            }
            throw new Exception("Unable to retreive LED data for index=" + index);
        }
        #endregion

        #region BlinkStick Pro mode selection
        /// <summary>
        /// Sets the mode for BlinkStick Pro.
        /// </summary>
        /// <param name="mode">0 - Normal, 1 - Inverse, 2 - WS2812</param>
        public async Task SetModeAsync(byte mode)
        {
            if (Connected)
            {
                await SetFeatureAsync(new byte[] { 4, mode });
            }
        }

        /// <summary>
        /// Gets the mode on BlinkStick Pro.
        /// </summary>
        public async Task<int> GetModeAsync()
        {
            if (!Connected) return -1;
            var data = await GetFeatureAsync(0x04);
            return data[1];
        }
        #endregion

        #region BlinkStick Flex features
        public async Task SetLedCount(byte count)
        {
            if (Connected)
            {
                await SetFeatureAsync(new byte[] { 0x81, count });
            }
        }

        public async Task<int> GetLedCount()
        {
            if (!Connected) return -1;
            var data = await GetFeatureAsync(0x81);
            return data[1];
        }
        #endregion

        #region Animation Control
        public void Stop()
        {
            _stopped = true;
        }

        public void Enable()
        {
            _stopped = false;
        }
        #endregion

        #region Blink Animation
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
        public async Task BlinkAsync(byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int delay = 500)
        {
            for (var i = 0; i < repeats; i++)
            {
                await InternalSetColorAsync(channel, index, r, g, b);
                await Task.Delay(delay);
                await InternalSetColorAsync(channel, index, 0, 0, 0);
                await Task.Delay(delay);
            }
        }

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public async Task BlinkAsync(byte channel, byte index, RgbColor color, int repeats = 1, int delay = 500)
        {
            await BlinkAsync(channel, index, color.R, color.G, color.B, repeats, delay);
        }

        /// <summary>
        /// Blink the LED on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public async Task BlinkAsync(byte channel, byte index, string color, int repeats = 1, int delay = 500)
        {
            await BlinkAsync(channel, index, RgbColor.FromString(color), repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public async Task BlinkAsync(byte r, byte g, byte b, int repeats = 1, int delay = 500)
        {
            await BlinkAsync(0, 0, r, g, b, repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public async Task BlinkAsync(RgbColor color, int repeats = 1, int delay = 500)
        {
            await BlinkAsync(0, 0, color, repeats, delay);
        }

        /// <summary>
        /// Blink the LED.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">How many times to repeat (default 1)</param>
        /// <param name="delay">Delay delay between on/off sequences (default 500)</param>
        public async Task BlinkAsync(string color, int repeats = 1, int delay = 500)
        {
            await BlinkAsync(0, 0, color, repeats, delay);
        }
        #endregion

        #region Morph Animation
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
        public async Task MorphAsync(byte channel, byte index, byte r, byte g, byte b, int duration = 1000, int steps = 50)
        {
            if (_stopped)
                return;

            var color = await GetColorAsync(index);

            for (var i = 1; i <= steps; i++)
            {
                await InternalSetColorAsync(channel, index,
                    (byte)(1.0 * color.Item1 + (r - color.Item1) / 1.0 / steps * i),
                    (byte)(1.0 * color.Item2 + (g - color.Item2) / 1.0 / steps * i),
                    (byte)(1.0 * color.Item3 + (b - color.Item3) / 1.0 / steps * i));

                await Task.Delay(duration / steps);
            }
        }

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task MorphAsync(byte channel, byte index, RgbColor color, int duration = 1000, int steps = 50)
        {
            await MorphAsync(channel, index, color.R, color.G, color.B, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task MorphAsync(byte channel, byte index, string color, int duration = 1000, int steps = 50)
        {
            await MorphAsync(channel, index, RgbColor.FromString(color), duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task MorphAsync(byte r, byte g, byte b, int duration = 1000, int steps = 50)
        {
            await MorphAsync(0, 0, r, g, b, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task MorphAsync(RgbColor color, int duration = 1000, int steps = 50)
        {
            await MorphAsync(0, 0, color, duration, steps);
        }

        /// <summary>
        /// Morph from current color to new color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task MorphAsync(string color, int duration = 1000, int steps = 50)
        {
            await MorphAsync(0, 0, color, duration, steps);
        }
        #endregion

        #region Pulse Animation

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
        public async Task PulseAsync(byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50)
        {
            await InternalSetColorAsync(channel, index, 0, 0, 0);

            if (SetColorDelay > 0)
            {
                await Task.Delay(SetColorDelay);
            }

            for (var i = 0; i < repeats; i++)
            {
                if (_stopped)
                    break;

                await MorphAsync(channel, index, r, g, b, duration, steps);

                if (_stopped)
                    break;

                await MorphAsync(channel, index, 0, 0, 0, duration, steps);
            }
        }

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Color parameter as RgbColor class instance</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task PulseAsync(byte channel, byte index, RgbColor color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            await PulseAsync(channel, index, color.R, color.G, color.B, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color on BlinkStick Pro.
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task PulseAsync(byte channel, byte index, string color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            await PulseAsync(channel, index, RgbColor.FromString(color), repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task PulseAsync(byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50)
        {
            await PulseAsync(0, 0, r, g, b, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task PulseAsync(RgbColor color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            await PulseAsync(0, 0, color, repeats, duration, steps);
        }

        /// <summary>
        /// Pulse specified color.
        /// </summary>
        /// <param name="color">Must be in #rrggbb format or named color ("red", "green", "blue")</param>
        /// <param name="repeats">Number of times to repeat.</param>
        /// <param name="duration">How long should the morph last</param>
        /// <param name="steps">How many steps for color changes</param>
        public async Task PulseAsync(string color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            await PulseAsync(0, 0, color, repeats, duration, steps);
        }
        #endregion

        #region Static Functions to initialize BlinkSticks
        
        /// <summary>
        /// Find all BlinkStick devices.
        /// </summary>
        /// <returns>An array of BlinkStick devices</returns>
        public static async Task<BlinkStick[]> FindAllAsync()
        {
            var selector = HidDevice.GetDeviceSelector(UsagePage, UsageId, VendorId, ProductId);
            var deviceInformationList = await DeviceInformation.FindAllAsync(selector);
            return deviceInformationList.Select(info => new BlinkStick(info)).ToArray();
        }

        /// <summary>
        /// Find first BlinkStick.
        /// </summary>
        /// <returns>BlinkStick device if found, otherwise null if no devices found</returns>
        public static async Task<BlinkStick> FindFirstAsync()
        {
            var devices = await FindAllAsync();
            return devices.FirstOrDefault();
        }
        #endregion

        #region Misc helper functions

        /// <summary>
        /// Automatically sets the color of the device using either BlinkStick or BlinkStick Pro API
        /// </summary>
        /// <param name="channel">Channel (0 - R, 1 - G, 2 - B)</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        private async Task InternalSetColorAsync(byte channel, byte index, byte r, byte g, byte b)
        {
            if (channel == 0 && index == 0)
            {
                await SetColorAsync(r, g, b);
            }
            else
            {
                await SetColorAsync(channel, index, r, g, b);
            }
        }

        #endregion

        #region Misc helper functions

        private async Task SetFeatureAsync(byte[] buffer)
        {
            var featureReport = _device.CreateFeatureReport(buffer[0]);

            var dataWriter = new DataWriter();

            dataWriter.WriteBytes(buffer);

            for (var i = 0; i < featureReport.Data.Length - buffer.Length; i++)
            {
                dataWriter.WriteByte(0);
            }

            featureReport.Data = dataWriter.DetachBuffer();

            var attempt = 0;
            while (attempt < 5)
            {
                attempt++;
                try
                {
                    await _device.SendFeatureReportAsync(featureReport);
                    break;
                }
                catch (Exception)
                {
                    if (attempt == 5)
                        throw;
                }
            }
        }

        private async Task<byte[]> GetFeatureAsync(ushort reportId)
        {
            var featureReport = await _device.GetFeatureReportAsync(reportId);
            var dataReader = DataReader.FromBuffer(featureReport.Data);
            var result = new byte[featureReport.Data.Length];
            dataReader.ReadBytes(result);
            return result;
        }

        #endregion
    }

    public enum BlinkStickDeviceEnum
    {
        Unknown,
        BlinkStick,
        BlinkStickPro,
        BlinkStickStrip,
        BlinkStickSquare,
        BlinkStickNano,
        BlinkStickFlex
    }
}

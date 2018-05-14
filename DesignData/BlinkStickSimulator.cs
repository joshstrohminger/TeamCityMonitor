using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using BlinkStickUniversal;
using Interfaces;
using MVVM;

namespace DesignData
{
    public class BlinkStickSimulator : ObservableObject, IBlinkStick
    {
        private bool _connected;
        private const int Leds = 8;
        public event BlinkStick.ErrorHandler OnError;

        public ObservableCollection<byte> CurrentColors { get; } = new ObservableCollection<byte>(Enumerable.Repeat((byte)0, Leds * 3));
        public bool Connected
        {
            get => _connected;
            private set => UpdateOnPropertyChanged(ref _connected, value);
        }

        public string InfoBlock1 { get; set; } = nameof(InfoBlock1);
        public string InfoBlock2 { get; set; } = nameof(InfoBlock2);
        public int SetColorDelay { get; set; }
        public int Mode { get; set; }
        public async Task<bool> OpenDeviceAsync()
        {
            Connected = true;
            return await Task.FromResult(Connected);
        }

        public void CloseDevice()
        {
            Connected = false;
        }

        public async Task SetColorAsync(string color)
        {
            await SetColorAsync(ColorExtensions.FromString(color));
        }

        public async Task SetColorAsync(Color color)
        {
            await SetColorAsync(color.R, color.G, color.B);
        }

        public async Task SetColorAsync(byte r, byte g, byte b)
        {
            if (Connected)
            {
                var bytes = new[] {r, g, b};
                for (var i = 0; i < CurrentColors.Count; i++)
                {
                    CurrentColors[i] = bytes[i % 3];
                }
                await Task.Delay(0);
            }
        }

        public async Task<(byte R, byte G, byte B)> GetColorAsync()
        {
            return await Task.FromResult((CurrentColors[0], CurrentColors[1], CurrentColors[2]));
        }

        public async Task TurnOffAsync()
        {
            await SetColorAsync(0, 0, 0);
        }

        public async Task SetColorAsync(byte channel, byte index, byte r, byte g, byte b)
        {
            CurrentColors[index * 3] = r;
            CurrentColors[index * 3 + 1] = g;
            CurrentColors[index * 3 + 2] = b;
            await Task.Delay(0);
        }

        public async Task SetColorAsync(byte channel, byte index, string color)
        {
            await SetColorAsync(channel, index, ColorExtensions.FromString(color));
        }

        public async Task SetColorAsync(byte channel, byte index, Color color)
        {
            await SetColorAsync(channel, index, color.R, color.G, color.B);
        }

        public async Task SetColorsAsync(byte channel, byte[] colorData)
        {
            for (var i = 0; i < colorData.Length; i++)
            {
                CurrentColors[i] = colorData[i];
            }
            await Task.Delay(1);
        }

        public async Task<byte[]> GetColorsAsync()
        {
            return await Task.FromResult(CurrentColors.ToArray());
        }

        public async Task<(byte R, byte G, byte B)> GetColorAsync(byte index)
        {
            return await Task.FromResult((CurrentColors[index * 3], CurrentColors[index * 3 + 1], CurrentColors[index * 3 + 2]));
        }

        public void Stop()
        {
            Connected = false;
        }

        public void Enable()
        {
            Connected = true;
        }

        public Task BlinkAsync(byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int delay = 500)
        {
            throw new NotImplementedException();
        }

        public Task BlinkAsync(byte channel, byte index, Color color, int repeats = 1, int delay = 500)
        {
            throw new NotImplementedException();
        }

        public Task BlinkAsync(byte channel, byte index, string color, int repeats = 1, int delay = 500)
        {
            throw new NotImplementedException();
        }

        public Task BlinkAsync(byte r, byte g, byte b, int repeats = 1, int delay = 500)
        {
            throw new NotImplementedException();
        }

        public Task BlinkAsync(Color color, int repeats = 1, int delay = 500)
        {
            throw new NotImplementedException();
        }

        public Task BlinkAsync(string color, int repeats = 1, int delay = 500)
        {
            throw new NotImplementedException();
        }

        public Task MorphAsync(byte channel, byte index, byte r, byte g, byte b, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task MorphAsync(byte channel, byte index, Color color, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task MorphAsync(byte channel, byte index, string color, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task MorphAsync(byte r, byte g, byte b, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task MorphAsync(Color color, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task MorphAsync(string color, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task PulseAsync(byte channel, byte index, byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task PulseAsync(byte channel, byte index, Color color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task PulseAsync(byte channel, byte index, string color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task PulseAsync(byte r, byte g, byte b, int repeats = 1, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task PulseAsync(Color color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }

        public Task PulseAsync(string color, int repeats = 1, int duration = 1000, int steps = 50)
        {
            throw new NotImplementedException();
        }
    }
}

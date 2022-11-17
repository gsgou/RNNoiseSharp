using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using RNNoiseSharp;
using Xamarin.Essentials;
using Xamarin.Forms;
using Sample.Helpers;

namespace Sample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await TestRNNoiseSharp();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        async Task TestRNNoiseSharp()
        {
            using (var denoiser = new Denoiser())
            {
                var pcm = await ResourceHelpers.GetBytesAsync("babble_15dB.pcm");

                float[] data = Convert16BitToFloat(pcm);

                Debug.WriteLine(denoiser.Denoise(data.AsSpan()));

                byte[] pcmDenoised = ConvertFloatTo16Bit(data);

                var fn = "babble_15dB_denoised.pcm";
                var file = Path.Combine(FileSystem.CacheDirectory, fn);
                File.WriteAllBytes(file, pcmDenoised);

                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = Title,
                    File = new ShareFile(file)
                });
            }
        }

        float[] Convert16BitToFloat(byte[] input)
        {
            // 16 bit input, so 2 bytes per sample
            int inputSamples = input.Length / 2;
            float[] output = new float[inputSamples];
            int outputIndex = 0;
            for (int n = 0; n < inputSamples; n++)
            {
                short sample = BitConverter.ToInt16(input, n * 2);
                output[outputIndex++] = sample / 32768f;
            }
            return output;
        }

        byte[] ConvertFloatTo16Bit(float[] samples)
        {
            int samplesCount = samples.Length;
            var pcm = new byte[samplesCount * 2];
            int sampleIndex = 0, pcmIndex = 0;

            while (sampleIndex < samplesCount)
            {
                var outsample = (short)(samples[sampleIndex] * short.MaxValue);
                pcm[pcmIndex] = (byte)(outsample & 0xff);
                pcm[pcmIndex + 1] = (byte)((outsample >> 8) & 0xff);

                sampleIndex++;
                pcmIndex += 2;
            }

            return pcm;
        }
    }
}
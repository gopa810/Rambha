using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

using NAudio;
using NAudio.Wave;

using Rambha.Document;

namespace SlideViewer
{
    public class AudioPlayer: IDisposable
    {
        MNReferencedSound sound = null;
        WaveStream p_audioFileReader = null;
        MemoryStream p_memoryStream = null;
        DirectSoundOut p_waveOut = null;
        WaveChannel32 p_waveChannel = null;
        public bool Playing { get; set; }

        Timer timer1 = null;

        public AudioPlayer()
        {
            if (timer1 == null) timer1 = new Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            Playing = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1 != null) timer1.Stop();
            if (p_waveOut != null)
            {
                //Debugger.Log(0, "", "Audio Player stoped.\n");
                DisposeAll();
            }
            Playing = false;

        }

        public void DisposeAll()
        {
            if (p_memoryStream != null)
            {
                p_memoryStream.Dispose();
                p_memoryStream = null;
            }

            if (timer1 != null)
            {
                timer1.Dispose();
                timer1 = null;
            }
            DisposeAudios();
        }

        public void DisposeAudios()
        {
            if (p_waveOut != null)
            {
                p_waveOut.Stop();
                p_waveOut.Dispose();
                p_waveOut = null;
            }

            if (p_waveChannel != null)
            {
                p_waveChannel.Dispose();
                p_waveChannel = null;
            }

            if (p_audioFileReader != null)
            {
                p_audioFileReader.Dispose();
                p_audioFileReader = null;
            }
        }


        public void SetSound(MNReferencedSound s)
        {
            sound = s;
        }

        private WaveStream GetAudioStream()
        {
            switch (sound.AudioType)
            {
                case ".mp3":
                    return new Mp3FileReader(p_memoryStream);
                case ".wav":
                    return new WaveFileReader(p_memoryStream);
                case ".aiff":
                    return new AiffFileReader(p_memoryStream);
                default:
                    return null;
            }
        }

        public void Stop()
        {
            if (timer1 != null) timer1.Stop();
            DisposeAll();
        }

        public void Play()
        {
            ErrorCatcher.currentOperation = "PlaySound ";
            try
            {
                if (sound != null && sound.GetData() != null)
                {
                    ErrorCatcher.currentOperation = "PlaySound " + sound.Name;
                    if (Playing == true)
                    {
                        Stop();
                    }

                    if (p_memoryStream == null)
                    {
                        p_memoryStream = new MemoryStream(sound.GetData());
                    }

                    if (p_audioFileReader != null)
                    {
                        p_audioFileReader.Dispose();
                        p_audioFileReader = null;
                    }

                    if (p_audioFileReader == null)
                    {
                        p_audioFileReader = GetAudioStream();
                    }

                    if (p_audioFileReader != null)
                    {
                        TimeSpan length = p_audioFileReader.TotalTime;
                        if (timer1 == null) timer1 = new Timer();
                        timer1.Interval = Convert.ToInt32(length.TotalMilliseconds);
                        timer1.Start();

                        if (p_waveChannel == null)
                            p_waveChannel = new WaveChannel32(p_audioFileReader);

                        if (p_waveOut == null)
                        {
                            p_waveOut = new DirectSoundOut();
                            p_waveOut.Init(p_waveChannel);
                        }

                        if (p_waveOut != null)
                        {
                            Debugger.Log(0, "", "Audio Player start.\n");
                            p_waveOut.Play();
                            Playing = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorCatcher.Add("Exception: {0}\n\nStackTrace: {1}\n\n\n", ex.Message, ex.StackTrace);
            }
            finally
            {
                ErrorCatcher.currentOperation = "";
            }
        }

        public void Dispose()
        {
            DisposeAll();
        }
    }
}

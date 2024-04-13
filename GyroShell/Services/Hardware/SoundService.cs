#region Copyright (License GPLv3)
// GyroShell - A modern, extensible, fast, and customizable shell platform.
// Copyright (C) 2022-2024  Pdawg
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using CoreAudio;
using GyroShell.Library.Services.Hardware;
using System;

namespace GyroShell.Services.Hardware
{
    internal class SoundService : ISoundService
    {
        private MMDevice _audioDevice;
        private MMDeviceEnumerator _deviceEnumerator;

        public int Volume 
        {
            get => Math.Clamp((int)(_audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar * 100), 0, 100);
            set => _audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Clamp(value / 100f, 0f, 1f);
        }
        public bool IsMuted 
        {
            get => _audioDevice.AudioEndpointVolume.Mute;
            set => _audioDevice.AudioEndpointVolume.Mute = value;
        }

        public event EventHandler OnVolumeChanged;

        public SoundService()
        {
            _deviceEnumerator = new MMDeviceEnumerator(Guid.NewGuid());
            _audioDevice = _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            _audioDevice.AudioEndpointVolume.OnVolumeNotification += new AudioEndpointVolumeNotificationDelegate(AudioEndpointVolume_OnVolumeNotification);
        }

        private void AudioEndpointVolume_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            OnVolumeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

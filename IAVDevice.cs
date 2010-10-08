﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSource.UPnP
{
    public interface IAVDevice
    {
        void Play();
        void Play(string Location);
        void Play(string Location, bool AddToQueue);
        void Pause();
        void Stop();

        void Next();
        void Previous();
        void Seek(string RawTime);
        void Seek(int Hour, int Minute, int Second);
        void Seek(System.TimeSpan SeekTime);

        void Queue(string Location);

        System.Uri Uri { get; }
        string UniqueDeviceName { get; }
        OpenSource.UPnP.UPnPDevice Device { get; set; }
        OpenSource.UPnP.AV.CpAVTransport Transport { get; }
        string FriendlyName { get; }

        DateTime FirstSeen { get; set; }
        DateTime LastSeen { get; set; }
        bool IsActive { get; }
    
    }
}
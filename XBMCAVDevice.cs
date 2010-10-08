using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSource.UPnP;
using OpenSource.UPnP.AV;


namespace OpenSource.XBMC
{
    public class XBMCAVDevice : OpenSource.UPnP.IAVDevice
    {
        public XBMCAVDevice()
        {
            this.FirstSeen = DateTime.Now;
            this.LastSeen = DateTime.Now;
        }

        #region IAVDevice Members

        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public bool IsActive
        {
            get
            {
                System.TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - LastSeen.Ticks);
                if (ts.Minutes > KeepAliveMax)
                {
                    return false;
                }
                return true;
            }
        }


        static int KeepAliveMax = 5; //minutes


        uint instanceID = 0;
        public void Play()
        {
            Transport.Play(instanceID, UPnP.AV.CpAVTransport.Enum_TransportPlaySpeed._1);
        }
        public void Play(string Location)
        {
            Transport.SetAVTransportURI(instanceID, Location, null);
        }
        public void Play(string Location, bool AddToQueue)
        {
            if (AddToQueue)
                Queue(Location);
            else
                Transport.SetAVTransportURI(instanceID, Location, null);
        }
        public void Stop()
        {
            Transport.Stop(instanceID);
           
        }

        public void Next()
        {
            Transport.Next(instanceID);
        }
        public void Previous()
        {
            Transport.Previous(instanceID);
        }
        public void Seek(string RawTime)
        {
            TimeSpan ts;
            if (TimeSpan.TryParse(RawTime, out ts))
                Seek(ts);
            else
                Transport.Seek(instanceID, CpAVTransport.Enum_A_ARG_TYPE_SeekMode.REL_TIME, RawTime);

        }
        public void Seek(int Hour, int Minute, int Second)
        {
            System.TimeSpan ts = new TimeSpan(Hour, Minute, Second);
            Seek(ts);
        }
        public void Seek(System.TimeSpan SeekTime)
        {
            Transport.Seek(instanceID, CpAVTransport.Enum_A_ARG_TYPE_SeekMode.REL_TIME, SeekTime.ToString());
        }


        public void Queue(string Location)
        {
            Transport.SetNextAVTransportURI(instanceID, Location, null);

        }
        

               

        OpenSource.UPnP.UPnPDevice device;
        public OpenSource.UPnP.UPnPDevice Device {
            get { return device; }
            set { device = value; }
        }
        public void Pause()
        {
            Transport.Pause(instanceID);
        }

        public System.Uri Uri
        {
            get { return Transport.GetUPnPService().ParentDevice.BaseURL; }
        }
        public string UniqueDeviceName
        {
            get { return Transport.GetUPnPService().ParentDevice.UniqueDeviceName; }
        }

        UPnP.AV.CpAVTransport transport;
        object _lock = new object();
        public UPnP.AV.CpAVTransport Transport
        {
            get
            {
                lock (_lock)
                {
                    if (transport != null) return transport;
                    OpenSource.UPnP.UPnPService svc = device.GetService(AudioVideoDevices.AVServiceID);
                    transport = new OpenSource.UPnP.AV.CpAVTransport(svc);
                    return transport;
                }

            }
        }

        public string FriendlyName
        {
            get
            {
                return Device.FriendlyName;
            }
        }

        #endregion
    }
}

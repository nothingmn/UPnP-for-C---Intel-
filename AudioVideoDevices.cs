using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenSource.UPnP
{
    public class AudioVideoDevices
    {

        static object _lock = new object();
        static AudioVideoDevices instance = null;
        List<OpenSource.UPnP.IAVDevice> aVTransportList = new List<OpenSource.UPnP.IAVDevice>();
        List<string> listIndex = new List<string>();
        public List<OpenSource.UPnP.IAVDevice> Devices { get { return aVTransportList; } }

        public static AudioVideoDevices Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance != null) return instance;
                    AudioVideoDevices d = new AudioVideoDevices();
                    instance = d;
                    return instance;
                }
            }
        }

        private AudioVideoDevices()
        {
            OpenSource.UPnP.Sniffer s = new OpenSource.UPnP.Sniffer();
            s.OnUPnPDeviceFound += new OpenSource.UPnP.Sniffer.UPnPDeviceFound(s_OnUPnPDeviceFound);
            s.Start(OpenSource.UPnP.SearchTypes.MediaRenderer);

        }
        void s_OnUPnPDeviceFound(object Sender, string Packet, System.Net.IPEndPoint Local, System.Net.IPEndPoint From, string PType, string NT, string USN, string UUID, string Location, string FromHostName)
        {
            OpenSource.UPnP.UPnPDeviceFactory df = new OpenSource.UPnP.UPnPDeviceFactory();
            df.OnDevice += new OpenSource.UPnP.UPnPDeviceFactory.UPnPDeviceHandler(df_OnDevice);
            df.OnFailed += new OpenSource.UPnP.UPnPDeviceFactory.UPnPDeviceFailedHandler(df_OnFailed);
            System.Uri u;
            if (System.Uri.TryCreate(Location, UriKind.RelativeOrAbsolute, out u))
            {
                Console.WriteLine(FromHostName + ":"+ Packet);
                lock (_lock)
                {
                    if (!listIndex.Contains(Location))
                    {
                        listIndex.Add(Location);
                        df.CreateDevice(u, 30000, Local.Address, null);
                    }
                    else
                    {
                        foreach (IAVDevice d in Devices)
                        {
                            if (d.Uri.ToString() == Location)
                            {
                                d.LastSeen = DateTime.Now;
                                break;
                            }
                        }
                    }
                }
            }
        }

        public static string AVServiceID = "AVTransport";
        void df_OnFailed(OpenSource.UPnP.UPnPDeviceFactory sender, Uri URL, Exception e, string URN)
        {
        }

        void df_OnDevice(OpenSource.UPnP.UPnPDeviceFactory sender, OpenSource.UPnP.UPnPDevice device, Uri URL)
        {
            lock (_lock)
            {
                OpenSource.UPnP.UPnPService svc = device.GetService(AVServiceID);
                if (svc != null)
                {
                    UPnP.AV.CpAVTransport transport = new OpenSource.UPnP.AV.CpAVTransport(svc);
                    if (transport != null)
                    {

                    }
                }


                if (device.FriendlyName.Contains("XBMC"))
                {
                    XBMC.XBMCAVDevice dev = new XBMC.XBMCAVDevice();
                    dev.Device = device;
                    aVTransportList.Add(dev);
                }
            }
            //a.Pause((uint)0);
            //a.Play((uint)0, OpenSource.UPnP.AV.CpAVTransport.Enum_TransportPlaySpeed._1);
        }
    }
}
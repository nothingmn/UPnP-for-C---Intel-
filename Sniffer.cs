using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSource.Utilities;
using OpenSource.UPnP;
using UPnPSniffer;
using System.Net;



namespace OpenSource.UPnP
{
    public enum SearchTypes
    {
        all,
        rootdevice,
        InternetGatewayDevice,
        MediaServer,
        MediaRenderer
    }

    public class Sniffer
    {
        private string SearchTypesToString(SearchTypes T)
        {
            string type = "ssdp:all";
            switch (T)
            {
                case SearchTypes.rootdevice:
                    type = "upnp:rootdevice";
                    break;
                case SearchTypes.InternetGatewayDevice:
                    type = "urn:schemas-upnp-org:device:InternetGatewayDevice:1";
                    break;
                case SearchTypes.MediaServer:
                    type = "urn:schemas-upnp-org:device:MediaServer:1";
                    break;
                case SearchTypes.MediaRenderer:
                    type = "urn:schemas-upnp-org:device:MediaRenderer:1";
                    break;
                default:
                    break;
            }
            return type;
        }
        public delegate void UPnPDeviceFound(object Sender, string Packet, IPEndPoint Local, IPEndPoint From,string PType, string NT, string USN, string UUID, string Location, string FromHostName);
        public event UPnPDeviceFound OnUPnPDeviceFound;

        UPnPSearchSniffer SSniffer = new UPnPSearchSniffer();
        public Sniffer()
        {
            SSniffer.OnPacket += new UPnPSearchSniffer.PacketHandler(SSniffer_OnPacket);
        }
        public void Start(SearchTypes T)
        {
            SSniffer.Search(SearchTypesToString(T));
        }

        void SSniffer_OnPacket(object sender, string Packet, IPEndPoint Local, IPEndPoint From)
        {
            //System.Console.WriteLine(Local.ToString() + " / " + From.ToString());
            //if (From.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) return;
            //if (From.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) return;
            //Console.WriteLine(Packet);
            DText p = new DText();
            UTF8Encoding U = new UTF8Encoding();
            string PType = "Unknown";
            int i = Packet.IndexOf(" ");
            if (i > 0) PType = Packet.Substring(0, i);
            string NT = "Unknown";
            string USN="";
            string UUID="";
            string Location = "";

            if (PType == "NOTIFY")
            {
                int pos1 = Packet.IndexOf("\r\nNT:");
                if (pos1 == -1) pos1 = Packet.IndexOf("\r\nnt:");
                if (pos1 > 0)
                {
                    int pos2 = Packet.IndexOf("\r\n", pos1 + 5);
                    NT = Packet.Substring(pos1 + 5, pos2 - (pos1 + 5));
                }
                NT = NT.Trim();

                USN = "";
                pos1 = Packet.IndexOf("\r\nUSN:");
                if (pos1 == -1) pos1 = Packet.IndexOf("\r\nusn:");
                if (pos1 > 0)
                {
                    int pos2 = Packet.IndexOf("\r\n", pos1 + 6);
                    USN = Packet.Substring(pos1 + 6, pos2 - (pos1 + 6));
                }
                USN = USN.Trim();
                int UsnEndPos = USN.IndexOf("::");
                if (USN.StartsWith("uuid:") == true && UsnEndPos != -1)
                {
                    UUID = USN.Substring(5, UsnEndPos - 5);
                }
            }

            if (PType == "HTTP/1.1")
            {
                int pos1 = Packet.IndexOf("\r\nST:");
                if (pos1 == -1) pos1 = Packet.IndexOf("\r\nst:");
                if (pos1 > 0)
                {
                    int pos2 = Packet.IndexOf("\r\n", pos1 + 5);
                    NT = Packet.Substring(pos1 + 5, pos2 - (pos1 + 5));
                }
                NT = NT.Trim();

                USN = "";
                pos1 = Packet.IndexOf("\r\nUSN:");
                if (pos1 == -1) pos1 = Packet.IndexOf("\r\nusn:");
                if (pos1 > 0)
                {
                    int pos2 = Packet.IndexOf("\r\n", pos1 + 6);
                    USN = Packet.Substring(pos1 + 6, pos2 - (pos1 + 6));
                }
                USN = USN.Trim();

                string packetLower = Packet.ToLowerInvariant();
                Location = "";
                pos1 = packetLower.IndexOf("\r\nlocation:");
                if (pos1 > 0)
                {
                    int pos2 = Packet.IndexOf("\r\n", pos1 + 11);
                    Location = Packet.Substring(pos1 + 11, pos2 - (pos1 + 11));
                }
                Location = Location.Trim();
                


                int UsnEndPos = USN.IndexOf("::");
                if (USN.StartsWith("uuid:") == true && UsnEndPos != -1)
                {
                    UUID = USN.Substring(5, UsnEndPos - 5);
                }
            }
            string FromHostName = From.Address.ToString();
            try
            {
                FromHostName = System.Net.Dns.Resolve(From.Address.ToString()).HostName;
            }
            catch (Exception) { }
            if (OnUPnPDeviceFound != null) OnUPnPDeviceFound(sender, Packet, Local, From, PType, NT, USN, UUID, Location, FromHostName);
        }
    }
}
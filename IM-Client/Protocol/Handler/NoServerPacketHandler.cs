using IM_Client.Models;
using IM_Client.Protocol.NoServerPacket;
using IM_Client.Utils;
using System;
using System.Windows;

namespace IM_Client.Protocol.Handler
{
    public class NoServerPacketHandler
    {
        public ViewModelLocator viewModelLocator;
        public event Action<Packet> NoServerPacketHandlers;
        public static NoServerPacketHandler INSTANCE = new NoServerPacketHandler();

        public void INVOKE(Packet packet)
        {
            NoServerPacketHandlers?.Invoke(packet);
        }

        private NoServerPacketHandler()
        {
            viewModelLocator = (ViewModelLocator)Application.Current.Resources["VMLocator"];
            NoServerPacketHandlers += NoServerLoginPacketHanler;
        }
        private void NoServerLoginPacketHanler(Packet packet)
        {
            if (!(packet is NoServerLoginPacket)) return;

            Console.WriteLine("Receive NoServerLoginPacket");

            NoServerLoginPacket noServerLoginPacket = (NoServerLoginPacket)packet;

            Participant participant = new Participant();
            participant.UserName = noServerLoginPacket.UserName;
            participant.Photo = noServerLoginPacket.Avator;

            App.Current.Dispatcher.Invoke((Action)delegate ()
            {
                viewModelLocator.MainWindowVM.Participants.Add(participant);
            });


        }

        private void NoServerLogoutPacketHandler(Packet packet)
        {

        }

    }
}

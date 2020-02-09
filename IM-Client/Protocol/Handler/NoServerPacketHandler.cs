using IM_Client.Models;
using IM_Client.Protocol.NoServerPacket;
using IM_Client.Utils;
using System;
using System.Windows;
using System.IO;
using IM_Client.Services;
using System.Linq;

namespace IM_Client.Protocol.Handler
{
    public class NoServerPacketHandler
    {
        public static readonly IChatService chatService = new ChatService();
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
            NoServerPacketHandlers += NoServerLogoutPacketHandler;
        }


        private void NoServerLoginPacketHanler(Packet packet)
        {
            if (!(packet is NoServerLoginPacket)) return;

            Console.WriteLine("Receive NoServerLoginPacket");

            NoServerLoginPacket noServerLoginPacket = (NoServerLoginPacket)packet;

            var person = viewModelLocator.MainWindowVM.Participants
                .Where((p) => string.Equals(p.UserName, noServerLoginPacket.UserName))
                .FirstOrDefault();
            //用户尚未登录在表中
            if (person == null)
            {
                if (string.Equals(noServerLoginPacket.UserName, viewModelLocator.MainWindowVM.UserName)) return;

                Participant participant = new Participant();
                participant.UserName = noServerLoginPacket.UserName;
                participant.Photo = noServerLoginPacket.Avator;
                participant.IsLoggedIn = true;

                App.Current.Dispatcher.Invoke((Action)delegate ()
                {
                    viewModelLocator.MainWindowVM.Participants.Add(participant);
                });
            }
            else
            {
                App.Current.Dispatcher.Invoke(() => person.IsLoggedIn = true);
            }


            //如果收到的登录报文尚未被回复
            if (!noServerLoginPacket.IsReply)
            {
                NoServerLoginPacket responsePacket = new NoServerLoginPacket();
                responsePacket.UserName = viewModelLocator.MainWindowVM.UserName;
                responsePacket.Port = 20000;

                if (!string.IsNullOrEmpty(viewModelLocator.MainWindowVM.ProfilePic))
                    responsePacket.Avator = File.ReadAllBytes(viewModelLocator.MainWindowVM.ProfilePic);

                responsePacket.IsReply = true;

                chatService.InvokeBroadcastPacketEvent(responsePacket);
            }
        }

        private void NoServerLogoutPacketHandler(Packet packet)
        {
            if (!(packet is NoServerLogoutPacket)) return;
            Console.WriteLine("Receive NoServerLogoutPacket");
            NoServerLogoutPacket logoutPacket = (NoServerLogoutPacket)packet;

            App.Current.Dispatcher.Invoke(delegate ()
            {
                var person = viewModelLocator.MainWindowVM.Participants
                                .Where((p) => string.Equals(p.UserName, logoutPacket.UserName))
                                .FirstOrDefault();
                if (person != null) person.IsLoggedIn = false;
            });
        }
    }
}

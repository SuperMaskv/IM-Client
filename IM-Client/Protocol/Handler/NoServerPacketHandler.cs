using IM_Client.Models;
using IM_Client.Protocol.NoServerPacket;
using IM_Client.Services;
using IM_Client.Utils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace IM_Client.Protocol.Handler
{
    public class NoServerPacketHandler
    {
        public static readonly IChatService chatService = new ChatService();
        public ViewModelLocator locator;

        public event Action<Packet> NoServerPacketHandlers;

        public static NoServerPacketHandler INSTANCE = new NoServerPacketHandler();

        public void INVOKE(Packet packet)
        {
            NoServerPacketHandlers?.Invoke(packet);
        }

        private NoServerPacketHandler()
        {
            locator = (ViewModelLocator)Application.Current.Resources["VMLocator"];
            NoServerPacketHandlers += NoServerLoginPacketHanler;
            NoServerPacketHandlers += NoServerLogoutPacketHandler;
            NoServerPacketHandlers += NoServerTextMsgPacketHandler;
            NoServerPacketHandlers += NoServerPicMsgPacketHandler;
            NoServerPacketHandlers += NoServerSendFilePacketHandler;
        }

        private void NoServerLoginPacketHanler(Packet packet)
        {
            if (!(packet is NoServerLoginPacket)) return;

            Console.WriteLine("Receive NoServerLoginPacket");

            NoServerLoginPacket noServerLoginPacket = (NoServerLoginPacket)packet;

            var person = locator.MainWindowVM.Participants
                .Where((p) => string.Equals(p.UserName, noServerLoginPacket.UserName))
                .FirstOrDefault();
            //用户尚未登录在表中
            if (person == null)
            {
                if (string.Equals(noServerLoginPacket.UserName, locator.MainWindowVM.UserName)) return;

                Participant participant = new Participant();
                participant.UserName = noServerLoginPacket.UserName;
                participant.Photo = noServerLoginPacket.Avator;
                participant.IsLoggedIn = true;
                participant.Remote = new IPEndPoint(locator.MainWindowVM.REMOTE.Address, 20000);

                App.Current.Dispatcher.Invoke((Action)delegate ()
                {
                    locator.MainWindowVM.Participants.Add(participant);
                    if (locator.MainWindowVM.Participants.Count() == 1)
                    {
                        locator.MainWindowVM.SelectedParticipant = participant;
                    }
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
                responsePacket.UserName = locator.MainWindowVM.UserName;
                responsePacket.Port = 20000;

                if (!string.IsNullOrEmpty(locator.MainWindowVM.ProfilePic))
                    responsePacket.Avator = File.ReadAllBytes(locator.MainWindowVM.ProfilePic);

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
                var person = locator.MainWindowVM.Participants
                                .Where((p) => string.Equals(p.UserName, logoutPacket.UserName))
                                .FirstOrDefault();
                if (person != null) person.IsLoggedIn = false;
            });
        }

        private void NoServerTextMsgPacketHandler(Packet packet)
        {
            if (!(packet is TextMessagePacket)) return;
            Console.WriteLine("Receive Text Message");
            TextMessagePacket textMessagePacket = (TextMessagePacket)packet;

            ChatMessage chatMessage = new ChatMessage();
            chatMessage.Author = textMessagePacket.Author;
            chatMessage.Message = textMessagePacket.TextMessage;
            chatMessage.Time = DateTime.Now;
            chatMessage.IsOriginNative = false;

            App.Current.Dispatcher.Invoke(delegate ()
            {
                var person = locator.MainWindowVM.Participants
                                .Where((p) => string.Equals(p.UserName, textMessagePacket.Author))
                                .FirstOrDefault();
                if (person != null)
                {
                    person.ChatMessages.Add(chatMessage);
                    if (person.UserName != locator.MainWindowVM.SelectedParticipant.UserName)
                    {
                        person.HasSentNewMessage = true;
                    }
                }
            });
        }

        private void NoServerPicMsgPacketHandler(Packet packet)
        {
            if (!(packet is NoServerPicMsgPacket)) return;
            Console.WriteLine("Receive NoServerPicMsgPacket");

            NoServerPicMsgPacket picMsgPacket = (NoServerPicMsgPacket)packet;

            var img = new MemoryStream(picMsgPacket.Pic);
            string picName = "";
            if (img != null)
            {
                Image pic = Image.FromStream(img);
                picName = DateTime.Now.ToFileTimeUtc().ToString();
                pic.Save(picName + ".jpg", ImageFormat.Jpeg);
            }

            ChatMessage chatMessage = new ChatMessage()
            {
                Author = picMsgPacket.Author,
                Picture = Path.GetFullPath(picName + ".jpg"),
                Time = DateTime.Now,
                IsOriginNative = false
            };

            App.Current.Dispatcher.Invoke(delegate ()
            {
                var person = locator.MainWindowVM.Participants
                                .Where((p) => string.Equals(picMsgPacket.Author, p.UserName))
                                .FirstOrDefault();
                if (person != null)
                {
                    person.ChatMessages.Add(chatMessage);
                    if (person.UserName != locator.MainWindowVM.SelectedParticipant.UserName)
                    {
                        person.HasSentNewMessage = true;
                    }
                }
            });
        }

        private void NoServerSendFilePacketHandler(Packet packet)
        {
            if (!(packet is NoServerSendFilePacket)) return;
            Console.WriteLine("Receive NoServerSendFilePacket");
            NoServerSendFilePacket sendFilePacket = (NoServerSendFilePacket)packet;

            NoServerSendFilePacket responsePacket = new NoServerSendFilePacket();
            responsePacket.IsSend = false;

            //判断是请求报文还是应答报文
            if (sendFilePacket.IsSend)
            {
                //请求报文
                string messageBoxText = "用户 "
                    + sendFilePacket.Author
                    + " 想要发送给您 "
                    + sendFilePacket.FileName
                    + " 是否接受？";

                var result = Task.Run(() => MessageBox.Show(messageBoxText, "", MessageBoxButton.YesNo));

                switch (result.Result)
                {
                    case MessageBoxResult.Yes:
                        responsePacket.WillSend = true;
                        responsePacket.Author = locator.MainWindowVM.UserName;
                        responsePacket.Port = 8888;
                        break;
                    case MessageBoxResult.No:
                        responsePacket.WillSend = false;
                        break;
                }
            }
            else
            {

            }

            chatService.InvokeUnicastPacketEvent()
        }
    }
}
﻿using IM_Client.Protocol.ServerPacket;
using IM_Client.Utils;
using System;
using System.Windows;
using IM_Client.Models;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Sockets;
using System.Net;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace IM_Client.Protocol.Handler
{
    public class ServerPacketHandler
    {
        public static readonly ServerPacketHandler INSTANCE = new ServerPacketHandler();

        public ServerPacketHandlers handlers;

        private ViewModelLocator locator = (ViewModelLocator)Application.Current.Resources["VMLocator"];

        [DllImport("user32")] public static extern int FlashWindow(IntPtr hwnd, bool bInvert);

        public ServerPacketHandler()
        {
            handlers += LoginResponseHandler;
            handlers += ContactListPacketHanler;
            handlers += OnlineContactPacketHandler;
            handlers += OfflineContactPacketHandler;
            handlers += EditContactAliasPacketHandler;
            handlers += RemoveContactPacketHandler;
            handlers += ToUserMsgPacketHandler;
            handlers += FileTransferPacketHandler;
            handlers += RegisterResponseHandler;
        }

        public delegate void ServerPacketHandlers(Packet packet);
        private void ContactListPacketHanler(Packet packet)
        {
            if (!(packet is ContactListPacket)) return;
            Console.WriteLine("Receive ContactListPacket");
            ContactListPacket contactListPacket = (ContactListPacket)packet;

            //遍历联系人列表，并装载到UI中
            foreach (var contact in contactListPacket.contacts)
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    locator.MainWindowVM.Participants.Add(new Participant()
                    {
                        UserName = contact.alias == null ? contact.contactName : contact.alias,
                        TrueName = contact.contactName,
                        Photo = contact.photo,
                        IsLoggedIn = false
                    });
                });

            }
        }

        private void EditContactAliasPacketHandler(Packet packet)
        {
            if (!(packet is EditContactAliasPacket)) return;
            Console.WriteLine("Receive EditContactAliasPacket");
            EditContactAliasPacket response = (EditContactAliasPacket)packet;

            Application.Current.Dispatcher.Invoke(delegate
            {
                var person = locator.MainWindowVM.Participants
                                .Where((p) => p.TrueName.Equals(response.contactName))
                                .FirstOrDefault();
                person.UserName = response.alias;
            });
        }

        private void FileTransferPacketHandler(Packet packet)
        {
            if (!(packet is FileTransferPacket)) return;
            Console.WriteLine("Receive FileTransferPacket");

            FileTransferPacket fileTransferPacket = (FileTransferPacket)packet;
            //判断是什么报文
            if (fileTransferPacket.requestFlag)
            {
                //请求报文：让用户决定是否接收文件
                string messageBoxText = "用户 "
                                        + fileTransferPacket.fileSender
                                        + " 想要发送给您 "
                                        + fileTransferPacket.fileName
                                        + " 是否接受？";
                MessageBoxResult result = MessageBox.Show(messageBoxText, "", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        fileTransferPacket.agreeFlag = true;
                        //打开FileTransferWindow
                        locator.FileTransferWindowViewModel.SendFileMode = Enums.SendFileMode.Receive;
                        locator.FileTransferWindowViewModel.LISTEN.Start();
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            FileTransferWindow window = new FileTransferWindow();
                            window.Show();
                        });
                        break;
                    case MessageBoxResult.No:
                        fileTransferPacket.agreeFlag = false;
                        break;
                }
                fileTransferPacket.requestFlag = false;
                //发送应答报文
                var stream = locator.MainWindowVM.TcpClient.GetStream();
                var packetBytes = PacketCodec.INSTANCE.Encode(fileTransferPacket);
                if (stream.CanWrite)
                {
                    stream.Write(packetBytes, 0, packetBytes.Length);
                }
            }
            else
            {
                //应答报文：如果对方同意接收，发送文件，反之弹出提醒
                if (fileTransferPacket.agreeFlag)
                {
                    locator.FileTransferWindowViewModel.REMOTE
                        = new IPEndPoint(IPAddress.Parse(fileTransferPacket.receiverAddress.Substring(1))
                        , fileTransferPacket.receiverPort + 1);
                    locator.FileTransferWindowViewModel.SEND.Start();
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        locator.InfoDialogViewModel.Info = "对方拒绝接收文件";
                        locator.InfoDialogViewModel.OpenDialogCommand.Execute(new object());
                    });
                }
            }
        }

        private void LoginResponseHandler(Packet packet)
        {
            //判断是否为登录响应报文
            if (!(packet is ResponsePacket)) return;
            var responsePacket = (ResponsePacket)packet;
            if (responsePacket.packetType != PacketType.LOGIN) return;
            Console.WriteLine("Receive Login Response Packet");


            try
            {
                //判断是否登录成功
                if (responsePacket.status)
                {
                    //登录成功
                    //提取token
                    long token = Convert.ToInt64(responsePacket.info);
                    locator.MainWindowVM.Token = token;
                    //进入聊天界面
                    locator.MainWindowVM.UserMode = Enums.UserModes.Chat;
                }
                else
                {
                    //登录失败
                    //显示登录失败信息
                    App.Current.Dispatcher.Invoke(delegate
                    {
                        locator.InfoDialogViewModel.Info = responsePacket.info;
                        locator.InfoDialogViewModel.OpenDialogCommand.Execute(new object());
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void OfflineContactPacketHandler(Packet packet)
        {
            if (!(packet is OfflineContactPacket)) return;
            Console.WriteLine("Receive OfflineContactPacket");

            OfflineContactPacket offline = (OfflineContactPacket)packet;

            foreach (var participant in locator.MainWindowVM.Participants)
            {
                if (participant.TrueName.Equals(offline.userName))
                {
                    participant.IsLoggedIn = false;
                }
            }
        }

        private void OnlineContactPacketHandler(Packet packet)
        {
            if (!(packet is OnlineContactPacket)) return;
            Console.WriteLine("Receive OnlineContactPacket");
            OnlineContactPacket onlineContactPacket = (OnlineContactPacket)packet;

            foreach (var participant in locator.MainWindowVM.Participants)
            {
                foreach (var contact in onlineContactPacket.onlineContacts)
                {
                    if (participant.TrueName.Equals(contact))
                    {
                        participant.IsLoggedIn = true;
                    }
                }
            }
        }
        private void RemoveContactPacketHandler(Packet packet)
        {
            if (!(packet is RemoveContactPacket)) return;
            Console.WriteLine("Receive RemoveContactPacket");
            RemoveContactPacket response = (RemoveContactPacket)packet;

            Application.Current.Dispatcher.Invoke(delegate
            {
                var person = locator.MainWindowVM.Participants
                                .Where((p) => response.contactName.Equals(p.TrueName))
                                .FirstOrDefault();
                locator.MainWindowVM.SelectedParticipant = null;
                locator.MainWindowVM.Participants.Remove(person);
            });
        }

        private void ToUserMsgPacketHandler(Packet packet)
        {
            if (!(packet is ToUserMessagePacket)) return;
            Console.WriteLine("Receive ToUserMessagePacket");
            ToUserMessagePacket message = (ToUserMessagePacket)packet;

            Application.Current.Dispatcher.Invoke(delegate
            {
                var person = locator.MainWindowVM.Participants
                    .Where((p) => message.msgSender.Equals(p.TrueName))
                    .FirstOrDefault();
                //判断是文字消息还是图片消息
                if (!string.IsNullOrEmpty(message.msgContent))
                {
                    //文字消息
                    if (person != null)
                    {
                        person.ChatMessages.Add(new ChatMessage()
                        {
                            Author = message.msgSender,
                            Message = message.msgContent,
                            Time = message.sendTime,
                            IsOriginNative = false
                        });
                        if (locator.MainWindowVM.SelectedParticipant == null
                            || !person.TrueName.Equals(locator.MainWindowVM.SelectedParticipant.TrueName))
                        {

                            person.HasSentNewMessage = true;

                        }

                    }
                }
                else
                {
                    //图片消息
                    var img = new MemoryStream(message.photo);
                    string picName = "";
                    if (img != null)
                    {
                        Image pic = Image.FromStream(img);
                        picName = DateTime.Now.ToFileTimeUtc().ToString();
                        pic.Save(picName + ".jpg", ImageFormat.Jpeg);
                    }

                    if (person != null)
                    {
                        person.ChatMessages.Add(new ChatMessage()
                        {
                            Author = message.msgSender,
                            Picture = Path.GetFullPath(picName + ".jpg"),
                            Time = message.sendTime,
                            IsOriginNative = false
                        });
                        if (locator.MainWindowVM.SelectedParticipant == null
                            || !person.TrueName.Equals(locator.MainWindowVM.SelectedParticipant.TrueName))
                        {

                            person.HasSentNewMessage = true;

                        }
                    }
                }

                //任务栏闪烁
                WindowInteropHelper wih = new WindowInteropHelper(Application.Current.MainWindow);
                FlashWindow(wih.Handle, true);
            });
        }

        private void RegisterResponseHandler(Packet packet)
        {
            if (!(packet is ResponsePacket)) return;
            ResponsePacket response = (ResponsePacket)packet;
            if (response.packetType != PacketType.REGISTER) return;
            Console.WriteLine("Receive RegisterResponsePacket");

            Application.Current.Dispatcher.Invoke(delegate
            {
                locator.InfoDialogViewModel.Info = response.info;
                locator.InfoDialogViewModel.OpenDialogCommand.Execute(new object());
            });
        }
    }
}

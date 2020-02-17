using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using IM_Client.Protocol.ServerPacket;
using IM_Client.Utils;

namespace IM_Client.Protocol.Handler
{
    public class ServerPacketHandler
    {
        public delegate void ServerPacketHandlers(Packet packet);
        public ServerPacketHandlers handlers;
        public static readonly ServerPacketHandler INSTANCE = new ServerPacketHandler();
        private ViewModelLocator locator = (ViewModelLocator)Application.Current.Resources["VMLocator"];


        public ServerPacketHandler()
        {
            handlers += LoginPacketHandler;
        }

        public void LoginPacketHandler(Packet packet)
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
    }
}

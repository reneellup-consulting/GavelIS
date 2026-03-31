using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.MiddleTier;

namespace GAVELISv2.Win
{
    public class MyConnectionProvider : MiddleTierConnectionHelper
    {
        public override void RegisterDefaultChannel()
        {
            //IDictionary channelProperties = new Hashtable();
            //channelProperties["secure"] = "true";
            //channelProperties["username"] = "Anonymous";
            //channelProperties["password"] = "";
            //channelProperties["domain"] = "";
            if (ChannelServices.GetChannel("tcp") == null)
            {
                ChannelServices.RegisterChannel(new TcpChannel(), false);
            }
        }
    }
}

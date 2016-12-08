using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using Newtonsoft.Json.Converters;
namespace mqttsample
{

    //班级消息协议
    public struct ClassMsg {
        public string cmdtype;
        public string cmdstr;
        public string[] applist;
    }

    //APP维护消息
    public struct AppMsg
    {
        public string cmdtype;
        public string cmdstr;
        public string[] applist;
    }


    public partial class Form1 : Form
    {
        public string mqtthost;
        public Form1()
        {
            InitializeComponent();
            mqtthost = "192.168.199.1";
            mqtttopic = "/hitecloud/iclass/classmode";
        }
        public MqttClient mqttclt;
        public string mqtttopic;
        private void button1_Click(object sender, EventArgs e)
        {
            mqttclt = new MqttClient(mqtthost);
            
            mqttclt.MqttMsgPublishReceived += mqttmsgrecive;
            mqttclt.Connect(txb_recviceid.Text);
            mqttclt.Subscribe(new string[] { mqtttopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });


        }
        public void mqttmsgrecive(object sender, MqttMsgPublishEventArgs e) {
            tlb_msg.Text = "";
            string msgstr = Encoding.UTF8.GetString(e.Message);
            // tlb_msg.Text = msgstr;
            string msgtxt= System.Text.ASCIIEncoding.Default.GetString(Convert.FromBase64String(msgstr));
            //sender.txb_msg.Text = msgtxt;
            MessageBox.Show(msgtxt);

        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            mqtttopic = "/hitecloud/iclass/classmode";
            //发送班级消息
            ClassMsg classMsg = new ClassMsg();
            classMsg.cmdtype = "classmode";
            classMsg.cmdstr = "shangke";
            classMsg.applist =new string[] { "com.android.hht.superstudent", "com.android.hht.launcher", "com.android.hht.superstudy", "com.android.hht.superparent", "com.android.hht.superapp", "com.android.hht.mqtt" };
            string MsgStr = Newtonsoft.Json.JsonConvert.SerializeObject(classMsg);
            pubishmsg(mqtttopic, MsgStr,0,true);

        }

        private void pubishmsg(string topic, string MessageString,int QosCode,bool Retain) {
            MqttClient mqttcltp;
            mqttcltp = new MqttClient(mqtthost);

            mqttcltp.Connect(txb_publishid.Text);
            byte[] bytedata = Encoding.ASCII.GetBytes(MessageString);
            string msgsendstr = Convert.ToBase64String(bytedata, 0, bytedata.Length);
            mqttcltp.Publish(mqtttopic, Encoding.UTF8.GetBytes(msgsendstr), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
            tlb_msg.Text = "发布消息";

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string clientid = Guid.NewGuid().ToString();
            txb_publishid.Text = clientid;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            mqtttopic = "/hitecloud/iclass/cmd";
            //发送班级消息
            ClassMsg classMsg = new ClassMsg();
            classMsg.cmdtype = "classmode";
            classMsg.cmdstr = "freedom";
            classMsg.applist = new string[] {""};
            string MsgStr = Newtonsoft.Json.JsonConvert.SerializeObject(classMsg);
            pubishmsg(mqtttopic, MsgStr, 0, true);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mqtttopic = "/hitecloud/iclass/appmaintain";
            //发送班级消息
            AppMsg classMsg = new AppMsg();
            classMsg.cmdtype = "appmaintain";
            classMsg.cmdstr = "appinstall";
            classMsg.applist = new string[] { "http://192.168.199.1/test.apk" };
            string MsgStr = Newtonsoft.Json.JsonConvert.SerializeObject(classMsg);
            pubishmsg(mqtttopic, MsgStr, 0, true);
        }
    }
}

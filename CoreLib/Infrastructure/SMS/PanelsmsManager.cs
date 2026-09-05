using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Infrastructure.SMS
{
    public static class PanelsmsManager
    {
        //o0q6ujblva
        public static string SendPatternRegister(string destination, string pattern, string code)
        {
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "shemsh";
            var password = "M@HD1F@GHz1402";
            var fromNum = "3000505";
            string[] toNum = { destination };

            var patternCode = pattern;


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                //new PanelSMS.input_data_type(){ key ="name",value =name } ,
                //new PanelSMS.input_data_type(){ key ="family",value =family },
                new PanelSMS.input_data_type(){ key ="ActivationCode",value =code }
            };

            return client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
        }
        public static string SendPatternOrginality(string destination, string pattern, string name, string code)
        {
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "shemsh";
            var password = "M@HD1F@GHz1402";
            var fromNum = "3000505";
            string[] toNum = { destination };

            var patternCode = pattern;


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                new PanelSMS.input_data_type(){ key ="name",value =name } ,
                new PanelSMS.input_data_type(){ key ="code",value =code }
            };

            return client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
        }
        public static string SendPatternCardAdmin(string destination, string pattern, string ordertype, string price,string city)
        {
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "shemsh";
            var password = "M@HD1F@GHz1402";
            var fromNum = "3000505";
            string[] toNum = { destination };

            var patternCode = pattern;


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                new PanelSMS.input_data_type(){ key ="ordertype",value =ordertype } ,
                new PanelSMS.input_data_type(){ key ="price",value =price },
                new PanelSMS.input_data_type(){ key ="city",value =city }
            };

            return client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
        }
        public static string SendPatternCardUser(string destination, string pattern, string name, string orderId)
        {
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "shemsh";
            var password = "M@HD1F@GHz1402";
            var fromNum = "3000505";
            string[] toNum = { destination };

            var patternCode = pattern;


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                new PanelSMS.input_data_type(){ key ="name",value =name } ,
                new PanelSMS.input_data_type(){ key ="orderId",value =orderId }
            };

            return client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
        }

        public static string SendPatternCardUserestelam(string destination, string pattern, string name, string code,string OrderPrice)
        {
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "shemsh";
            var password = "M@HD1F@GHz1402";
            var fromNum = "3000505";
            string[] toNum = { destination };

            var patternCode = pattern;


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                new PanelSMS.input_data_type(){ key ="name",value =name } ,
                new PanelSMS.input_data_type(){ key ="code",value =code },
                new PanelSMS.input_data_type(){ key ="OrderPrice",value =OrderPrice } 
            };

            return client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
        }
        public static string SendPatternOrderAdmin(string destination, string pattern, string name,string state, string orderId)
        {
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "shemsh";
            var password = "M@HD1F@GHz1402";
            var fromNum = "3000505";
            string[] toNum = { destination };

            var patternCode = pattern;


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                new PanelSMS.input_data_type(){ key ="name",value =name } ,
                new PanelSMS.input_data_type(){ key ="state",value =state } ,
                new PanelSMS.input_data_type(){ key ="orderId",value =orderId }
            };

            return client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
        }

        public static string SendPatternOrderSend(string destination, string pattern, string name,string send, string orderId)
        {
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "shemsh";
            var password = "M@HD1F@GHz1402";
            var fromNum = "3000505";
            string[] toNum = { destination };

            var patternCode = pattern;


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                new PanelSMS.input_data_type(){ key ="name",value =name } ,
                new PanelSMS.input_data_type(){ key ="send",value =send } ,
                new PanelSMS.input_data_type(){ key ="orderId",value =orderId }
            };

            return client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
        }

        public static string SendPatternOrderPost(string destination, string pattern, string name, string send,string tracking, string orderId)
        {
            PanelSMS.smsserver client = new PanelSMS.smsserver();
            var username = "shemsh";
            var password = "M@HD1F@GHz1402";
            var fromNum = "3000505";
            string[] toNum = { destination };

            var patternCode = pattern;


            var data = new PanelSMS.input_data_type[] {
                // key is your parameter name and value is what you want to send to the receiptor 
                new PanelSMS.input_data_type(){ key ="name",value =name } ,
                new PanelSMS.input_data_type(){ key ="send",value =send } ,
                new PanelSMS.input_data_type(){ key ="tracking",value =tracking } ,
                new PanelSMS.input_data_type(){ key ="orderId",value =orderId }
            };

            return client.sendPatternSms(fromNum, toNum, username, password, patternCode, data);
        }
    }
}

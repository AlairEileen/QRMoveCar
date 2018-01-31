using System;
using System.Collections.Generic;
using System.Text;

namespace YTXDAL
{
    public class SMSModel
    {
        private string _action = "templateSms";
        public string mobile { get; set; }
        public string appid { get; set; }
        private string _templateId = "685";
        public string[] datas { get; set; }
        public string templateId { get => _templateId; set => _templateId = value; }
        public string action { get => _action; set => _action = value; }
    }


    public class TwoPhoneModel
    {
        private string _action = "callDailBack";
        public string action { get => _action; set => _action = value; }
        public string src { get; set; }
        public string dst { get; set; }
        public string appid { get; set; }
        private string _credit = "360";
        public string credit { get => _credit; set => _credit = value; }
        private string _dstclid = "01053189990";
        public string dstclid { get => _dstclid; set => _dstclid = value; }
        private string _srcclid = "01053189990";
        public string srcclid { get => _srcclid; set => _srcclid = value; }

    }

    public class MeetingPhoneModel
    {
        private string _action = "createMeeting";
        public string action { get => _action; set => _action = value; }
        public string meetingname { get; set; }
        public string creator { get; set; }
        public Party[] parties { get; set; }
        public string appid { get; set; }

        private string _dstclid = "01053189990";
        public string dstclid { get => _dstclid; set => _dstclid = value; }
        private string _srcclid = "01053189990";
        public string srcclid { get => _srcclid; set => _srcclid = value; }

        private string _shownumber = "01053189990";
        public string bookmeeting { get => _bookmeeting; set => _bookmeeting = value; }
        public string confid { get => _confid; set => _confid = value; }
        public string booktime { get => _booktime; set => _booktime = value; }
        public string shownumber { get => _shownumber; set => _shownumber = value; }

        private string _bookmeeting = "0";
        private string _confid = "";
        private string _booktime = "";


    }

    public class Party
    {
        public string name { get; set; }
        public string phone { get; set; }
    }
}

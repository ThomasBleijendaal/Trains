using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SharpOSCCore
{
    public class OscBundle : OscPacket
    {
        Timetag _timetag;
        public IPEndPoint OriginEP;

        public ulong Timetag
        {
            get { return _timetag.Tag; }
            set { _timetag.Tag = value; }
        }

        public DateTime Timestamp
        {
            get { return _timetag.Timestamp; }
            set { _timetag.Timestamp = value; }
        }

        public List<OscMessage> Messages;

        public OscBundle(ulong timetag, IPEndPoint originEP, params OscMessage[] args)
        {
            _timetag = new Timetag(timetag);
            OriginEP = originEP;

            Messages = new List<OscMessage>();
            Messages.AddRange(args);
        }

        public override byte[] GetBytes()
        {
            var bundle = "#bundle";
            var bundleTagLen = Utils.AlignedStringLength(bundle);
            var tag = SetULong(_timetag.Tag);

            var outMessages = new List<byte[]>();
            foreach (var msg in Messages)
            {
                outMessages.Add(msg.GetBytes());
            }

            var len = bundleTagLen + tag.Length + outMessages.Sum(x => x.Length + 4);

            var i = 0;
            var output = new byte[len];
            Encoding.ASCII.GetBytes(bundle).CopyTo(output, i);
            i += bundleTagLen;
            tag.CopyTo(output, i);
            i += tag.Length;

            foreach (var msg in outMessages)
            {
                var size = SetInt(msg.Length);
                size.CopyTo(output, i);
                i += size.Length;

                msg.CopyTo(output, i);
                i += msg.Length; // msg size is always a multiple of 4
            }

            return output;
        }

    }
}

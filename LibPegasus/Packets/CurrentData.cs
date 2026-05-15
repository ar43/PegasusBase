using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace LibPegasus.Packets
{
	public class CurrentData
	{
		public byte[] RecvData = new byte[1024 + 3];
		public byte[] NewData = new byte[1024];
		public byte[] UnknownData = new byte[3];

		public int UnknownDataLen = 0;

		public CurrentData() { }

		public void SetUnknownData(byte[] data, int offset, int length)
		{
			Debug.Assert(length <= 3);
			UnknownDataLen = length;
			Array.Copy(data, offset, UnknownData, 0, length);
		}

		public int Update(int NewDataLen)
		{
			Debug.Assert(UnknownDataLen <= 3 && NewDataLen <= 1024);
			if(UnknownDataLen > 0)
			{
				Serilog.Log.Debug($"Unknown len: {UnknownDataLen}");
				var newLen = NewDataLen + UnknownDataLen;
				Array.Copy(UnknownData, RecvData, UnknownDataLen);
				
				Array.Copy(NewData, 0, RecvData, UnknownDataLen, NewDataLen);
				Utils.Utility.PrintByteArray(RecvData, newLen, "Assembled from unknown");

				if (newLen >= 4)
				{
					UnknownDataLen = 0;
					//Serilog.Log.Debug("Packet should NOT be underflow");
				}
					

				return newLen;
			}
			else
			{
				Array.Copy(NewData, RecvData, NewData.Length);
				return NewDataLen;
			}
		}
	}
}

using Google.Protobuf.Protocol;
using Google.Protobuf;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ServerSession : PacketSession
{
	public void Send(IMessage packet)
	{
		string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
		MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

		// [size(2)][packetId(2)][ 데이터 ]
		ushort size = (ushort)packet.CalculateSize();
		byte[] sendBuffer = new byte[size + 4];

		//sendBuffer 0번째 인덱스에 BitConverter.GetBytes(size+4) ([size(2))를 복사
		Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));

		//sendBuffer 2번째 인덱스에 BitConverter.GetBytes(protocolId) (packetID(2))를 복사
		Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));

		//sendBuffer 4번째 인덱스에 person.ToByteArray() (데이터)를 복사
		Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

		Send(new ArraySegment<byte>(sendBuffer));
	}

	public override void OnConnected(EndPoint endPoint)
	{
		//Debug.Log($"OnConnected : {endPoint}");

		//      C_EnterRoom enterRoom = new C_EnterRoom();

		//enterRoom.PlayerID = NetworkManager.PlayerID;

		//      Send(enterRoom);

		//C_EnterRoom enterRoom = new C_EnterRoom();

		//enterRoom.PlayerID = NetworkManager.Instance.PlayerID;

		//Send(enterRoom);

		


		PacketManager.Instance.CustomHandler = (s, m, i) =>
		{
            //NetworkManager Update에서 PacketQueue데이터를 처리
            PacketQueue.Instance.Push(i, m);
		};
	}

	public override void OnDisconnected(EndPoint endPoint)
	{
		Debug.Log($"OnDisconnected : {endPoint}");
	}

	public override void OnRecvPacket(ArraySegment<byte> buffer)
	{
		PacketManager.Instance.OnRecvPacket(this, buffer);
	}

	public override void OnSend(int numOfBytes)
	{
		//Console.WriteLine($"Transferred bytes: {numOfBytes}");
	}

	public void Login(string name)
	{
        //C_Login loginPacket = new C_Login();
        //loginPacket.PlayerName = name;
        //Debug.Log(loginPacket.PlayerName);
        //NetworkManager.instance.Send(loginPacket);
    }
}
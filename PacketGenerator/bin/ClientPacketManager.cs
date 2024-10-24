using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}
			//Protocol ID, 어떤 작업을 할지
	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	//PacketHandler
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
	
	/// <summary> 유니티 메인쓰레드에서 동작시킬 패킷 데이터를 저장</summary>
	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }

	/// <summary> 패킷을 등록하는 함수 </summary>
	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SLoginSuccess, MakePacket<S_LoginSuccess>);
		_handler.Add((ushort)MsgId.SLoginSuccess, PacketHandler.S_LoginSuccessHandler);		
		_onRecv.Add((ushort)MsgId.SRoomList, MakePacket<S_RoomList>);
		_handler.Add((ushort)MsgId.SRoomList, PacketHandler.S_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.SEnterLobby, MakePacket<S_EnterLobby>);
		_handler.Add((ushort)MsgId.SEnterLobby, PacketHandler.S_EnterLobbyHandler);		
		_onRecv.Add((ushort)MsgId.SEnterRoom, MakePacket<S_EnterRoom>);
		_handler.Add((ushort)MsgId.SEnterRoom, PacketHandler.S_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveRoom, MakePacket<S_LeaveRoom>);
		_handler.Add((ushort)MsgId.SLeaveRoom, PacketHandler.S_LeaveRoomHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveGame, MakePacket<S_LeaveGame>);
		_handler.Add((ushort)MsgId.SLeaveGame, PacketHandler.S_LeaveGameHandler);		
		_onRecv.Add((ushort)MsgId.SNewHost, MakePacket<S_NewHost>);
		_handler.Add((ushort)MsgId.SNewHost, PacketHandler.S_NewHostHandler);		
		_onRecv.Add((ushort)MsgId.SEnterBattlefield, MakePacket<S_EnterBattlefield>);
		_handler.Add((ushort)MsgId.SEnterBattlefield, PacketHandler.S_EnterBattlefieldHandler);		
		_onRecv.Add((ushort)MsgId.SMove, MakePacket<S_Move>);
		_handler.Add((ushort)MsgId.SMove, PacketHandler.S_MoveHandler);		
		_onRecv.Add((ushort)MsgId.SStopMove, MakePacket<S_StopMove>);
		_handler.Add((ushort)MsgId.SStopMove, PacketHandler.S_StopMoveHandler);		
		_onRecv.Add((ushort)MsgId.STest, MakePacket<S_Test>);
		_handler.Add((ushort)MsgId.STest, PacketHandler.S_TestHandler);
	}

	/// <summary>패킷 역직렬화 함수</summary>
	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		// 패킷헤더 정의 [size(2)][packetId(2)][ 데이터 ]
		ushort count = 0;
		
		//[size(2)]
		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		
		//[packetId(2)]
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	/// <summary> 패킷을 만드는 함수 </summary>
	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		if (CustomHandler != null) //커스텀패킷 핸들이 등록이 되어 있으면(유니티 클라에서 처리 할 패킷)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else //서버에서 처리 할 패킷
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}
	
	/// <summary>
	/// 패킷의 ID로 패킷 핸들러를 호출 하는 함수
	/// </summary>
	/// <param name="id"> 패킷 ID</param>
	/// <returns> 패킷 핸들러 </returns>
	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}
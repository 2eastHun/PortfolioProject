using ServerCore;
using System;
using System.Collections.Generic;


public class PacketManager
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
    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    //PacketHandler
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
       
    public void Register()
    {
      _makeFunc.Add((ushort)PacketId.S_BroadcastEnterGame, MakePacket<S_BroadcastEnterGame>);
        _handler.Add((ushort)PacketId.S_BroadcastEnterGame, PacketHandler.S_BroadcastEnterGameHandler); //PacketHandler 호출
      _makeFunc.Add((ushort)PacketId.S_BroadcastLeaveGame, MakePacket<S_BroadcastLeaveGame>);
        _handler.Add((ushort)PacketId.S_BroadcastLeaveGame, PacketHandler.S_BroadcastLeaveGameHandler); //PacketHandler 호출
      _makeFunc.Add((ushort)PacketId.S_PlayerList, MakePacket<S_PlayerList>);
        _handler.Add((ushort)PacketId.S_PlayerList, PacketHandler.S_PlayerListHandler); //PacketHandler 호출
      _makeFunc.Add((ushort)PacketId.S_BroadcastMove, MakePacket<S_BroadcastMove>);
        _handler.Add((ushort)PacketId.S_BroadcastMove, PacketHandler.S_BroadcastMoveHandler); //PacketHandler 호출
     
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> onRecvCallback = null)
    {
        // 패킷헤더 역직렬화
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;
        if (_makeFunc.TryGetValue(id, out func))
        {
           IPacket packet = func.Invoke(session, buffer);

            if (onRecvCallback != null)
                onRecvCallback.Invoke(session, packet);
            else
                HandlePacket(session, packet);
        }
    }
    
    /// <summary>
    /// 패킷을 만드는 함수
    /// </summary>
    /// <param name="session"></param>
    /// <param name="buffer"></param>
    /// <returns></returns>
    T MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        return pkt;
    }

    /// <summary>
    /// 패킷을 호출 하는 함수
    /// </summary>
    /// <param name="session"></param>
    /// <param name="packet"></param>
    public void HandlePacket(PacketSession session, IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(packet.Protocol, out action))
            action.Invoke(session, packet);
    }
}
﻿
using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string clientRegister;
        static string serverRegister;

        static void Main(string[] args)
        {
            string file = "../../Common/protoc-3.12.3-win64/bin/Protocol.proto";
            if (args.Length >= 1)
                file = args[0];

            bool startParsing = false;
            foreach (string line in File.ReadAllLines(file))
            {
                //enum MsgId 줄 부터 파싱시작
                if (!startParsing && line.Contains("enum MsgId"))
                {
                    startParsing = true;
                    continue;
                }

                if (!startParsing)
                    continue;

                if (line.Contains("}"))
                    break;

                string[] names = line.Trim().Split(" =");
                if (names.Length == 0)
                    continue;

                string name = names[0];

                //패킷이름이 S_로 시작하면 클라의 ClientPacketManager에 파싱 - 서버에서 보낸 클라의 패킷
                if (name.StartsWith("S_"))
                {
                    string[] words = name.Split("_");

                    string msgName = "";
                    foreach (string word in words)
                        msgName += FirstCharToUpper(word);

                    string packetName = $"S_{msgName.Substring(1)}";
                    clientRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);
                }
                //패킷이름이 C_로 시작하면 서버의 ServerPacketManager에 파싱 - 클라에서 보낸 서버의 패킷
                else if (name.StartsWith("C_"))
                {
                    string[] words = name.Split("_");

                    string msgName = "";
                    foreach (string word in words)
                        msgName += FirstCharToUpper(word);

                    string packetName = $"C_{msgName.Substring(1)}";
                    serverRegister += string.Format(PacketFormat.managerRegisterFormat, msgName, packetName);

                    Console.WriteLine(serverRegister);
                }
            }

            //.exe 실행하면 .cs파일들 생성
            string clientManagerText = string.Format(PacketFormat.managerFormat, clientRegister);
            File.WriteAllText("ClientPacketManager.cs", clientManagerText);
            string serverManagerText = string.Format(PacketFormat.managerFormat, serverRegister);
            File.WriteAllText("ServerPacketManager.cs", serverManagerText);
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1).ToLower();
        }
    }
}

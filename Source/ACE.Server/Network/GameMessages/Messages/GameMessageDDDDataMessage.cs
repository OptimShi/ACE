using ACE.Common.Extensions;
using ACE.DatLoader;
using ACE.Server.WorldObjects;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ACE.Server.Network.GameMessages.Messages
{
    public class GameMessageDDDDataMessage : GameMessage
    {
        public static string UpdatePath = "D:\\Source\\ACE\\DatUpdates";
        public GameMessageDDDDataMessage(uint FileID, DatFileType FileType, uint Iteration)
            : base(GameMessageOpcode.DDD_DataMessage, GameMessageGroup.DatabaseQueue)
        {
            var enumType = typeof(DatFileType);
            var memberInfos = enumType.GetMember(FileType.ToString());
            var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
            var valueAttributes = enumValueMemberInfo.GetCustomAttributes(typeof(DatDatabaseTypeAttribute), false);
            DatDatabaseType datDbType = ((DatDatabaseTypeAttribute)valueAttributes[0]).Type;

            string datSubFolder = "";
            switch (datDbType)
            {
                case DatDatabaseType.Portal:
                    Writer.Write((uint)0); // DatFileType
                    Writer.Write((uint)1); // DatFileID
                    datSubFolder = "portal";
                    break;
                case DatDatabaseType.Cell:
                    Writer.Write((uint)1); // DatFileType
                    Writer.Write((uint)2); // DatFileID
                    datSubFolder = "cell";
                    break;
                case DatDatabaseType.Language:
                    Writer.Write((uint)1); // DatFileType
                    Writer.Write((uint)3); // DatFileID
                    datSubFolder = "language";
                    break;
            }

            Writer.Write((uint)FileType);
            Writer.Write(FileID);

            Writer.Write(Iteration); // Iteration
            Writer.Write((byte)0); // Compressed = false - For the sake of simplicity, not going to bother with this.

            var filePath = UpdatePath + "\\" + datSubFolder + "\\" + FileID.ToString("X8") + ".bin";
            var fileContents = File.ReadAllBytes(filePath);

            Writer.Write(3); // version
            Writer.Write(fileContents.Length + 4); // length + size of this message
            Writer.Write(fileContents);
        }
    }
}

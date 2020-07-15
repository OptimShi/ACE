using ACE.DatLoader;
using ACE.Entity.Enum;
using ACE.Server.Entity;
using ACE.Server.Managers;
using ACE.Server.Network.Enum;
using ACE.Server.Network.GameEvent.Events;
using ACE.Server.Network.GameMessages;
using ACE.Server.Network.GameMessages.Messages;
using System.Collections.Generic;
using System.Linq;

namespace ACE.Server.Network.Handlers
{
    public static class DDDHandler
    {
        public static IEnumerable<object> ItersWIthKeys { get; private set; }

        [GameMessage(GameMessageOpcode.DDD_InterrogationResponse, SessionState.AuthConnected)]
        public static void DDD_InterrogationResponse(ClientMessage message, Session session)
        {
            if (PropertyManager.GetBool("show_dat_warning").Item)
            {
                message.Payload.ReadUInt32(); // m_ClientLanguage

                var ItersWithKeys = CAllIterationList.Read(message.Payload);
                // var ItersWithoutKeys = CAllIterationList.Read(message.Payload); // Not seen this populated in any pcap.
                // message.Payload.ReadUInt32(); // m_dwFlags - We don't need this

                foreach (var entry in ItersWithKeys.Lists)
                {
                    switch (entry.DatFileId)
                    {
                        case 1: // PORTAL
                            if (entry.Ints[0] != DatManager.PortalDat.Iteration)
                                session.DatWarnPortal = true;
                            break;
                        case 2: // CELL
                            if (entry.Ints[0] != DatManager.CellDat.Iteration)
                                session.DatWarnCell = true;
                            break;
                        case 3: // LANGUAGE
                            if (entry.Ints[0] != DatManager.LanguageDat.Iteration)
                                session.DatWarnLanguage = true;
                            break;
                    }
                }
            }

            GameMessageDDDEndDDD patchStatusMessage = new GameMessageDDDEndDDD();
            session.Network.EnqueueSend(patchStatusMessage);
        }

        [GameMessage(GameMessageOpcode.DDD_EndDDD, SessionState.AuthConnected)]
        public static void DDD_EndDDD(ClientMessage message, Session session)
        {
            // We don't need to reply to this message.
        }

        [GameMessage(GameMessageOpcode.DDD_RequestDataMessage, SessionState.WorldConnected)]
        public static void DDD_RequestDataMessage(ClientMessage message, Session session)
        {

            DatFileType qdid_type = (DatFileType)message.Payload.ReadInt32();
            var qdid_ID = message.Payload.ReadUInt32();

            // Landblock also needs to send the LandBlockInfo (0xFFFE) file with it...
            if (qdid_type == DatFileType.LandBlock)
            {
                GameMessageDDDDataMessage landblockInfoMessage = new GameMessageDDDDataMessage(qdid_ID, qdid_type);
                session.Network.EnqueueSend(landblockInfoMessage);
            }

            GameMessageDDDDataMessage dataMessage = new GameMessageDDDDataMessage(qdid_ID, qdid_type);
            session.Network.EnqueueSend(dataMessage);
        }
    }
}

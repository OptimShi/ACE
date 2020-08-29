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
        private static bool doPatching = false;

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

            if (doPatching)
            {
                GameMessageDDDBeginDDD beginDDD = new GameMessageDDDBeginDDD();
                session.Network.EnqueueSend(beginDDD);

                List<uint> PortalPatches = new List<uint>();
                PortalPatches.Add(0x010070C2);
                PortalPatches.Add(0x020029ED);
                PortalPatches.Add(0x0300184C);
                PortalPatches.Add(0x040040AE);
                PortalPatches.Add(0x040040B1);
                PortalPatches.Add(0x05005B41);
                PortalPatches.Add(0x05005B42);
                PortalPatches.Add(0x05005B43);
                PortalPatches.Add(0x05005B44);
                PortalPatches.Add(0x05005B45);
                PortalPatches.Add(0x06009B41);
                PortalPatches.Add(0x06009B42);
                PortalPatches.Add(0x06009B43);
                PortalPatches.Add(0x06009B44);
                PortalPatches.Add(0x06009B45);
                PortalPatches.Add(0x0800382A);
                PortalPatches.Add(0x0800382B);
                PortalPatches.Add(0x0800382C);
                PortalPatches.Add(0x0800382D);
                PortalPatches.Add(0x0800382E);
                PortalPatches.Add(0x0800382F);
                PortalPatches.Add(0x08003830);
                PortalPatches.Add(0x090010B0);
                PortalPatches.Add(0x0A001463);
                PortalPatches.Add(0x0F00119B);
                PortalPatches.Add(0x100012AE);
                PortalPatches.Add(0x2000016D);
                PortalPatches.Add(0x330020D3);
                PortalPatches.Add(0x330020D4);
                PortalPatches.Add(0x330020D5);
                PortalPatches.Add(0x330020D6);
                PortalPatches.Add(0x330020D7);
                PortalPatches.Add(0x330020D8);
                PortalPatches.Add(0x330020D9);
                PortalPatches.Add(0x330020DA);
                PortalPatches.Add(0x330020DB);
                PortalPatches.Add(0x330020DC);
                PortalPatches.Add(0x330020DD);
                PortalPatches.Add(0x330020DE);
                PortalPatches.Add(0x330020DF);
                PortalPatches.Add(0x330020E0);
                PortalPatches.Add(0x330020E1);
                PortalPatches.Add(0x330020E2);
                PortalPatches.Add(0x330020E3);
                PortalPatches.Add(0x330020E4);
                PortalPatches.Add(0x330020E5);
                PortalPatches.Add(0x330020E6);
                PortalPatches.Add(0x330020E7);
                PortalPatches.Add(0x330020E8);
                PortalPatches.Add(0x330020E9);
                PortalPatches.Add(0x330020EA);
                PortalPatches.Add(0x330020EB);
                PortalPatches.Add(0x330020EC);
                PortalPatches.Add(0x330020ED);
                PortalPatches.Add(0x330020EE);
                PortalPatches.Add(0x330020EF);
                PortalPatches.Add(0x330020F0);
                PortalPatches.Add(0x330020F1);
                PortalPatches.Add(0x330020F2);
                PortalPatches.Add(0x330020F3);
                PortalPatches.Add(0x330020F4);
                PortalPatches.Add(0x330020F5);
                PortalPatches.Add(0x330020F6);
                PortalPatches.Add(0x33002233);
                PortalPatches.Add(0x3300232F);
                PortalPatches.Add(0x33002331);
                PortalPatches.Add(0x33002332);
                PortalPatches.Add(0x33002719);
                PortalPatches.Add(0x33002867);
                PortalPatches.Add(0x33002868);
                PortalPatches.Add(0x33002869);
                PortalPatches.Add(0x3300286A);
                PortalPatches.Add(0x3300286B);
                PortalPatches.Add(0x3300286C);
                PortalPatches.Add(0x33002A84);
                PortalPatches.Add(0x33002A86);
                PortalPatches.Add(0x3400018A);

                for (var i =0; i < PortalPatches.Count; i++)
                {
                    var fileType = DatManager.GetFileType(DatDatabaseType.Portal, PortalPatches[i]);
                    if (!fileType.Equals(null))
                    {
                        GameMessageDDDDataMessage portalUpdate = new GameMessageDDDDataMessage(PortalPatches[i], (DatFileType)fileType, 2073);
                        session.Network.EnqueueSend(portalUpdate);
                    }
                }
            }
            else
            {
                GameMessageDDDEndDDD patchStatusMessage = new GameMessageDDDEndDDD();
                session.Network.EnqueueSend(patchStatusMessage);
            }

            // Dummy Data Message
            //GameMessageDDDDataMessage spellTableUpdate = new GameMessageDDDDataMessage(0x0E00000E, DatFileType.DbSpellTable, 2073);
            //GameMessageDDDDataMessage lbData = new GameMessageDDDDataMessage(0x55EDFFFF, DatFileType.LandBlock);
            //session.Network.EnqueueSend(spellTableUpdate);
        }

        [GameMessage(GameMessageOpcode.DDD_EndDDD, SessionState.AuthConnected)]
        public static void DDD_EndDDD(ClientMessage message, Session session)
        {
            if (doPatching)
            {
                GameMessageDDDEndDDD patchStatusMessage = new GameMessageDDDEndDDD();
                session.Network.EnqueueSend(patchStatusMessage);
            }
        }

        [GameMessage(GameMessageOpcode.DDD_RequestDataMessage, SessionState.WorldConnected)]
        public static void DDD_RequestDataMessage(ClientMessage message, Session session)
        {

            DatFileType qdid_type = (DatFileType)message.Payload.ReadInt32();
            var qdid_ID = message.Payload.ReadUInt32();

            // Landblock also needs to send the LandBlockInfo (0xFFFE) file with it...
            if (qdid_type == DatFileType.LandBlock)
            {
                GameMessageDDDDataMessage landblockInfoMessage = new GameMessageDDDDataMessage(qdid_ID, qdid_type, 983);
                session.Network.EnqueueSend(landblockInfoMessage);
            }

            GameMessageDDDDataMessage dataMessage = new GameMessageDDDDataMessage(qdid_ID, qdid_type, 983);
            session.Network.EnqueueSend(dataMessage);
        }
    }
}

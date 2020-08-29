using System.IO;

using log4net;

namespace ACE.DatLoader
{
    public static class DatManager
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string datFile;

        private static int count;

        // End of retail Iteration versions.
        private static int ITERATION_CELL = 982;
        private static int ITERATION_PORTAL = 2072;
        private static int ITERATION_HIRES = 497;
        private static int ITERATION_LANGUAGE = 994;
        public static CellDatDatabase CellDat { get; private set; }

        public static PortalDatDatabase PortalDat { get; private set; }
        public static DatDatabase HighResDat { get; private set; }
        public static LanguageDatDatabase LanguageDat { get; private set; }

        public static void Initialize(string datFileDirectory, bool keepOpen = false, bool loadCell = true)
        {
            var datDir = Path.GetFullPath(Path.Combine(datFileDirectory));

            if (loadCell)
            {
                try
                {
                    datFile = Path.Combine(datDir, "client_cell_1.dat");
                    CellDat = new CellDatDatabase(datFile, keepOpen);
                    count = CellDat.AllFiles.Count;
                    log.Info($"Successfully opened {datFile} file, containing {count} records, iteration {CellDat.Iteration}");
                    if (CellDat.Iteration != ITERATION_CELL)
                        log.Warn($"{datFile} iteration does not match expected end-of-retail version of {ITERATION_CELL}.");
                }
                catch (FileNotFoundException ex)
                {
                    log.Error($"An exception occured while attempting to open {datFile} file!  This needs to be corrected in order for Landblocks to load!");
                    log.Error($"Exception: {ex.Message}");
                }
            }

            try
            {
                datFile = Path.Combine(datDir, "client_portal.dat");
                PortalDat = new PortalDatDatabase(datFile, keepOpen);
                PortalDat.SkillTable.AddRetiredSkills();
                count = PortalDat.AllFiles.Count;
                log.Info($"Successfully opened {datFile} file, containing {count} records, iteration {PortalDat.Iteration}");
                if (PortalDat.Iteration != ITERATION_PORTAL)
                    log.Warn($"{datFile} iteration does not match expected end-of-retail version of {ITERATION_PORTAL}.");
            }
            catch (FileNotFoundException ex)
            {
                log.Error($"An exception occured while attempting to open {datFile} file!\n\n *** Please check your 'DatFilesDirectory' setting in the config.js file. ***\n *** ACE will not run properly without this properly configured! ***\n");
                log.Error($"Exception: {ex.Message}");
            }

            // Load the client_highres.dat file. This is not required for ACE operation, so no exception needs to be generated.
            datFile = Path.Combine(datDir, "client_highres.dat");
            if (File.Exists(datFile))
            {
                HighResDat = new DatDatabase(datFile, keepOpen);
                count = HighResDat.AllFiles.Count;
                log.Info($"Successfully opened {datFile} file, containing {count} records, iteration {HighResDat.Iteration}");
                if (HighResDat.Iteration != ITERATION_HIRES)
                    log.Warn($"{datFile} iteration does not match expected end-of-retail version of {ITERATION_HIRES}.");
            }

            try
            {
                datFile = Path.Combine(datDir, "client_local_English.dat");
                LanguageDat = new LanguageDatDatabase(datFile, keepOpen);
                count = LanguageDat.AllFiles.Count;
                log.Info($"Successfully opened {datFile} file, containing {count} records, iteration {LanguageDat.Iteration}");
                if(LanguageDat.Iteration != ITERATION_LANGUAGE)
                    log.Warn($"{datFile} iteration does not match expected end-of-retail version of {ITERATION_LANGUAGE}.");
            }
            catch (FileNotFoundException ex)
            {
                log.Error($"An exception occured while attempting to open {datFile} file!\n\n *** Please check your 'DatFilesDirectory' setting in the config.json file. ***\n *** ACE will not run properly without this properly configured! ***\n");
                log.Error($"Exception: {ex.Message}");
            }
        }

        public static DatFileType? GetFileType(DatDatabaseType datDatabaseType, uint ObjectId)
        {
            if (datDatabaseType == DatDatabaseType.Cell)
            {
                if ((ObjectId & 0xFFFF) == 0xFFFF)
                    return DatFileType.LandBlock;
                else if ((ObjectId & 0xFFFF) == 0xFFFE)
                    return DatFileType.LandBlockInfo;
                else
                    return DatFileType.EnvCell;
            }
            switch (ObjectId >> 24)
            {
                case 0x01:
                    return DatFileType.GraphicsObject;
                case 0x02:
                    return DatFileType.Setup;
                case 0x03:
                    return DatFileType.Animation;
                case 0x04:
                    return DatFileType.Palette;
                case 0x05:
                    return DatFileType.SurfaceTexture;
                case 0x06:
                    return DatFileType.Texture;
                case 0x08:
                    return DatFileType.Surface;
                case 0x09:
                    return DatFileType.MotionTable;
                case 0x0A:
                    return DatFileType.Wave;
                case 0x0D:
                    return DatFileType.Environment;
                case 0x0F:
                    return DatFileType.PaletteSet;
                case 0x10:
                    return DatFileType.Clothing;
                case 0x11:
                    return DatFileType.DegradeInfo;
                case 0x12:
                    return DatFileType.Scene;
                case 0x13:
                    return DatFileType.Region;
                case 0x20:
                    return DatFileType.SoundTable;
                case 0x22:
                    return DatFileType.EnumMapper;
                case 0x23:
                    return DatFileType.StringTable;
                case 0x25:
                    return DatFileType.DidMapper;
                case 0x27:
                    return DatFileType.DualDidMapper;
                case 0x30:
                    return DatFileType.CombatTable;
                case 0x32:
                    return DatFileType.ParticleEmitter;
                case 0x33:
                    return DatFileType.PhysicsScript;
                case 0x34:
                    return DatFileType.PhysicsScriptTable;
                case 0x40:
                    return DatFileType.Font;
            }

            if (ObjectId == 0x0E000002)
                return DatFileType.CharacterGenerator;
            else if (ObjectId == 0x0E000007)
                return DatFileType.ChatPoseTable;
            else if (ObjectId == 0x0E00000D)
                return DatFileType.ObjectHierarchy;
            else if (ObjectId == 0xE00001A)
                return DatFileType.BadData;
            else if (ObjectId == 0x0E00001E)
                return DatFileType.TabooTable;
            else if (ObjectId == 0x0E00001F)
                return DatFileType.FileToId;
            else if (ObjectId == 0x0E000020)
                return DatFileType.NameFilterTable;
            else if (ObjectId == 0x0E020000)
                return DatFileType.MonitoredProperties;

            //Console.WriteLine($"Unknown file type: {ObjectId:X8}");
            return null;
        }
    }
}

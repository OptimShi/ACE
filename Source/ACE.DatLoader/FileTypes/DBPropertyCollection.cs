using ACE.DatLoader.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ACE.DatLoader.FileTypes
{
    /// <summary>
    /// These are file types 0x78 in the client_Portal.dat, and there are two of them.
    /// Both are related to floating windows (chat?) and their Opacity (Active and Inactive)
    /// </summary>
    [DatFileType(DatFileType.DbProperties)]
    public class DBPropertyCollection : FileType
    {
        public Dictionary<uint, BaseProperty> Properties { get; } = new Dictionary<uint, BaseProperty>();

        public override void Unpack(BinaryReader reader)
        {
            Id = reader.ReadUInt32();
            reader.ReadByte();
            var totalObjects = reader.ReadByte();
            for (int i = 0; i < totalObjects; i++)
            {
                var key = reader.ReadUInt32();

                var item = new BaseProperty();
                item.Unpack(reader);
                Properties.Add(key, item);
            }
        }

    }
}

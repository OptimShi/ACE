using System.Collections.Generic;

namespace ACE.Common
{
    public class UpdateData
    {
        public List<uint> FilesToAdd = new List<uint>();
        public List<uint> FilesToPurge = new List<uint>();

        public UpdateData()
        {

        }
    }
}

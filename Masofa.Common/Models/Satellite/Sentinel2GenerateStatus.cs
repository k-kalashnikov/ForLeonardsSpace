using System.ComponentModel.DataAnnotations.Schema;

namespace Masofa.Common.Models.Satellite
{
    public class Sentinel2GenerateIndexStatus
    {
        public Guid Id { get; set; }
        public Guid Sentinel2ProductQueue { get; set; }
        public bool ArviTiff { get; set; } = false;
        public bool EviTiff { get; set; } = false;
        public bool GndviTiff { get; set; } = false;
        public bool MndwiTiff { get; set; } = false;
        public bool NdmiTiff { get; set; } = false;
        public bool NdviTiff { get; set; } = false;
        public bool NdwiTiff { get; set; } = false;
        public bool OrviTiff { get; set; } = false;
        public bool OsaviTiff { get; set; } = false;
        public bool ArviDb { get; set; } = false;
        public bool EviDb { get; set; } = false;
        public bool GndviDb { get; set; } = false;
        public bool MndwiDb { get; set; } = false;
        public bool NdmiDb { get; set; } = false;
        public bool NdviDb { get; set; } = false;
        public bool NdwiDb { get; set; } = false;
        public bool OrviDb { get; set; } = false;
        public bool OsaviDb { get; set; } = false;

        [NotMapped]
        public bool IsTiffComplite
        {
            get
            {
                return ArviTiff && EviTiff && GndviTiff && MndwiTiff && NdmiTiff && NdviTiff && NdwiTiff && OrviTiff && OsaviTiff;
            }
        }

        [NotMapped]
        public bool IsDbComplite
        {
            get
            {
                return ArviDb && EviDb && GndviDb && MndwiDb && NdmiDb && NdviDb && NdwiDb && OrviDb && OsaviDb;
            }
        }
    }

    public class Sentinel2GenerateIndexStatusHistory : BaseHistoryEntity<Sentinel2GenerateIndexStatus> { }
}

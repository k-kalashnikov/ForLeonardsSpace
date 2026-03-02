using Masofa.Common.Models.Satellite.Indices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Masofa.DataAccess
{
    public class MasofaIndicesDbContext : DbContext
    {
        public MasofaIndicesDbContext(DbContextOptions<MasofaIndicesDbContext> options) : base(options)
        {
            ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<NdviPoint> NdviPoints { get; set; }
        public DbSet<NdviPolygon> NdviPolygons { get; set; }
        public DbSet<NdviPolygonRelation> NdviPolygonRelations { get; set; }

        public DbSet<EviPoint> EviPoints { get; set; }
        public DbSet<EviPolygon> EviPolygons { get; set; }
        public DbSet<EviPolygonRelation> EviPolygonRelations { get; set; }

        public DbSet<GndviPoint> GndviPoints { get; set; }
        public DbSet<GndviPolygon> GndviPolygons { get; set; }
        public DbSet<GndviPolygonRelation> GndviPolygonRelations { get; set; }

        public DbSet<MndwiPoint> MndwiPoints { get; set; }
        public DbSet<MndwiPolygon> MndwiPolygons { get; set; }
        public DbSet<MndwiPolygonRelation> MndwiPolygonRelations { get; set; }

        public DbSet<NdmiPoint> NdmiPoints { get; set; }
        public DbSet<NdmiPolygon> NdmiPolygons { get; set; }
        public DbSet<NdmiPolygonRelation> NdmiPolygonRelations { get; set; }

        public DbSet<OrviPoint> OrviPoints { get; set; }
        public DbSet<OrviPolygon> OrviPolygons { get; set; }
        public DbSet<OrviPolygonRelation> OrviPolygonRelations { get; set; }

        public DbSet<OsaviPoint> OsaviPoints { get; set; }
        public DbSet<OsaviPolygon> OsaviPolygons { get; set; }
        public DbSet<OsaviPolygonRelation> OsaviPolygonRelations { get; set; }

        public DbSet<ArviPoint> ArviPoints { get; set; }
        public DbSet<ArviPolygon> ArviPolygons { get; set; }
        public DbSet<ArviPolygonRelation> ArviPolygonRelations { get; set; }

        public DbSet<NdwiPoint> NdwiPoints { get; set; }
        public DbSet<NdwiPolygon> NdwiPolygons { get; set; }
        public DbSet<NdwiPolygonRelation> NdwiPolygonRelations { get; set; }

        public DbSet<AnomalyPoint> AnomalyPoints { get; set; }
        public DbSet<AnomalyPolygon> AnomalyPolygons { get; set; }

        public DbSet<ArviSeasonReport> ArviSeasonReports { get; set; }
        public DbSet<EviSeasonReport> EviSeasonReports { get; set; }
        public DbSet<GndviSeasonReport> GndviSeasonReports { get; set; }
        public DbSet<MndwiSeasonReport> MndwiSeasonReports { get; set; }
        public DbSet<NdmiSeasonReport> NdmiSeasonReports { get; set; }
        public DbSet<NdviSeasonReport> NdviSeasonReports { get; set; }
        public DbSet<OrviSeasonReport> OrviSeasonReports { get; set; }
        public DbSet<OsaviSeasonReport> OsaviSeasonReports { get; set; }
        public DbSet<NdwiSeasonReport> NdwiSeasonReports { get; set; }

        public DbSet<ArviSharedReport> ArviSharedReports { get; set; }
        public DbSet<EviSharedReport> EviSharedReports { get; set; }
        public DbSet<GndviSharedReport> GndviSharedReports { get; set; }
        public DbSet<MndwiSharedReport> MndwiSharedReports { get; set; }
        public DbSet<NdmiSharedReport> NdmiSharedReports { get; set; }
        public DbSet<NdviSharedReport> NdviSharedReports { get; set; }
        public DbSet<OrviSharedReport> OrviSharedReports { get; set; }
        public DbSet<OsaviSharedReport> OsaviSharedReports { get; set; }
        public DbSet<NdwiSharedReport> NdwiSharedReports { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            this.ApplyLocalizationStringSettings(builder);

        }
    }
}

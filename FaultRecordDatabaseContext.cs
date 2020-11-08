using System;
using Microsoft.EntityFrameworkCore;

namespace faultnet_demo_api {
    public class FaultRecordDatabaseContext : DbContext {
        public FaultRecordDatabaseContext(DbContextOptions<FaultRecordDatabaseContext> options) : base(options) {
            // TODO
        }

        public DbSet<FaultRecord> FaultRecords { get; set; }
    }
}
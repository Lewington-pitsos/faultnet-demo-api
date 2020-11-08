using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace faultnet_demo_api {

    [ApiController]
    public class FaultRecordApiController : ControllerBase {
        private readonly ILogger<FaultRecordApiController> logger;
        private readonly FaultRecordDatabaseContext databaseContext;
        private readonly IGlobalTimer timer;

        public FaultRecordApiController(FaultRecordDatabaseContext context, IGlobalTimer timer, ILogger<FaultRecordApiController> logger) {
            this.databaseContext = context;
            this.timer = timer;
            this.logger = logger;
        }

        [HttpGet("/allfaults")]
        public async Task<ActionResult<IEnumerable<FaultRecord>>> GetAllRecords() {
            return await databaseContext.FaultRecords.ToListAsync();
        }

        [HttpGet("/realtimefaults")]
        public async Task<ActionResult<IEnumerable<FaultRecord>>> GetFaultNetRecords() {
            var faults = await databaseContext.FaultRecords.ToListAsync();
            return faults.GetRange(0, Math.Min(faults.Count, timer.PollTriggerCounter()));
        }
        
        [HttpPost("/addfaults")]
        public async Task<IActionResult> AddNewFaults([FromBody] IList<FaultRecord> newFaults) {
            int count = await CountRecords();
            await AddNewRecords(newFaults);
            timer.ReadAndRecordTime(Math.Min(timer.PollTriggerCounter(), count));
            return Ok(new {
                faults = newFaults
            });
        }

        [HttpPost("/overwritefaults")]
        public async Task<IActionResult> OverwriteFaults([FromBody] IList<FaultRecord> newFaults) {
            await VaporiseCurrentRecords();
            await AddNewRecords(newFaults);
            timer.ReadAndRecordTime(0);
            return Ok(new {
                faults = newFaults
            });
        }

        private async Task VaporiseCurrentRecords() {
            List<FaultRecord> savedRecords = await databaseContext.FaultRecords.ToListAsync();
            databaseContext.FaultRecords.RemoveRange(savedRecords);
        }

        private async Task AddNewRecords(IList<FaultRecord> newEvents) {
            foreach (var r in newEvents) {
                r.Row = 0;  // Clear out primary key so they can be written safely and uniquely
            }
            databaseContext.FaultRecords.AddRange(newEvents);
            await databaseContext.SaveChangesAsync();
        }

        private async Task<int> CountRecords() {
            return await databaseContext.FaultRecords.CountAsync();
        }
    }
}

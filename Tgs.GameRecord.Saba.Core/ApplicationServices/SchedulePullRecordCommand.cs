using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameRecord.Saba.Core.ApplicationServices;

public record SchedulePullRecordCommand(
    Uri ApiUrl,
    string OperatorId,
    string VendorId);

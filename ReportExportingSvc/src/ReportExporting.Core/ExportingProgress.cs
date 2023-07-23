using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportExporting.Core
{
    public enum ExportingProgress
    {
        Submitting, PutOnStore, FailToPutOnStore, PlaceOnQueue, FailToPlaceOnQueue, OnDeadLetterQueue, ExportingPdf, FailExportingPdf, ExportedPdf, ExportingWord, FailExportingWord, ExportedWord, Complete
    }
}

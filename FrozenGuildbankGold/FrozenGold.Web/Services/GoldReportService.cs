using System;

namespace FrozenGold.Web.Services
{
    public class GoldReportService
    {
        private GoldReport _report;

        public GoldReportService()
        {
        }

        public GoldReport Report
        {
            get
            {
                return _report;
            }
            set
            {
                _report = value;
                ReportUpdated(this, EventArgs.Empty);
            }
        }

        public event EventHandler ReportUpdated = delegate { };
    }
}

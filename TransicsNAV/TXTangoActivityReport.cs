using System;
using System.Linq;

namespace TransicsNAV
{
    public partial class TXTangoNAV
    {
        public ActivityReport[] GetActRep(Login login, int minutes)
        {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.ActivityReportSelection_V3 ActRepSel = new IWS.ActivityReportSelection_V3();

            IWS.DateTimeSelection dtSel = new IWS.DateTimeSelection();
            IWS.DateTimeAndIdSelection idSel = new IWS.DateTimeAndIdSelection();

            idSel.DateTimeType = IWS.enumSelectionDateTimeAndIdType.MINUTES;
            idSel.Value = minutes;

            dtSel.DateTimeType = IWS.enumSelectionDateTimeType.MINUTES;
            dtSel.Value = minutes;
            
            ActRepSel.DateTimeSelection = idSel;

            IWS.GetActivityReportResult_V9 ActRepRes = IWSService.Get_ActivityReport_V9(iwsLogin(login), ActRepSel);

            int i = 0;
            ActivityReport[] actRepList = new ActivityReport[ActRepRes.ActivityReportItems.Count()];

            foreach (IWS.ActivityReportItem_V9 act in ActRepRes.ActivityReportItems)
            {
                i = MoveActToActRep(act, ref actRepList, i);
            }
            return (actRepList);

        }

        public ActivityReport[] GetActRepVehicle(Login login, string vehicle, DateTime FromDate, DateTime ToDate)
        {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.ActivityReportSelection_V3 ActRepSel = new IWS.ActivityReportSelection_V3();

            IWS.PeriodSelectionWithDateType pSel = new IWS.PeriodSelectionWithDateType();

            pSel.StartDate = FromDate;
            pSel.EndDate = ToDate;

            ActRepSel.DateTimeRangeSelection = pSel;
            ActRepSel.IncludeRegistrations = true;

            IWS.IdentifierVehicle Vehicle = new IWS.IdentifierVehicle()
            {
                IdentifierVehicleType = IWS.enumIdentifierVehicleType.ID,
                Id = vehicle
            };

            ActRepSel.IncludeRegistrations = true;

            ActRepSel.Vehicles = new IWS.IdentifierVehicle[] { Vehicle };
            IWS.GetActivityReportResult_V9 ActRepRes = IWSService.Get_ActivityReport_V9(iwsLogin(login), ActRepSel);

            foreach (IWS.Error err in ActRepRes.Errors)
                throw new System.InvalidOperationException(err.ErrorCodeExplanation);

            int i = 0;
            ActivityReport[] actRepList = new ActivityReport[ActRepRes.ActivityReportItems.Count()];

            foreach (IWS.ActivityReportItem_V9 act in ActRepRes.ActivityReportItems)
            {
                i = MoveActToActRep(act, ref actRepList, i);
            }
            return (actRepList);
        }

        private int MoveActToActRep(IWS.ActivityReportItem_V9 act, ref ActivityReport[] actRepList, int i)
        {
            ActivityReport actRep = new ActivityReport();

            if (act.Position != null)
            {
                if (act.Position.AddressInfo != null)
                    actRep.AddressInfo = act.Position.AddressInfo;
                else if (act.Position.DistanceFromSmallCity != null)
                    actRep.AddressInfo = act.Position.DistanceFromLargeCity;
            }

            if (act.Place != null)
                actRep.Place = act.Place.PlaceID;

            if (act.Trip != null)
                actRep.Trip = act.Trip.TripID;

            if (act.Position != null)
            {
                if (act.Position.Latitude.HasValue)
                    actRep.Latitude = act.Position.Latitude.Value;
                if (act.Position.Longitude.HasValue)
                    actRep.Longitude = act.Position.Longitude.Value;
            }
            if (act.Driver != null)
                actRep.DriverCode = act.Driver.Code;

            if (act.CoDriver != null)
                actRep.BegeleiderCode = act.CoDriver.Code;

            actRep.ID = act.ID;
            actRep.VehicleID = act.Vehicle.ID;
            actRep.ActivityID = act.Activity.ID;
            actRep.ActivityName = act.Activity.Name;
            actRep.BeginDate = act.BeginDate;
            actRep.EndDate = act.EndDate;
            actRep.KmBegin = act.KmBegin;
            actRep.KmEnd = act.KmEnd;
            if (act.WorkingCode != null)
            {
                actRep.WorkingCodeCode = act.WorkingCode.Code;
                actRep.WorkingCodeDescription = act.WorkingCode.Description;
            }
            actRepList[i] = actRep;
            i += 1;

            return (i);
        }
        public ActivityReport[] GetActRepVehicleDetail(Login login, string vehicle, DateTime FromDate, DateTime ToDate) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.ActivityReportSelectionDetail_V4 ActRepSel = new IWS.ActivityReportSelectionDetail_V4();
            IWS.Period dsSel = new IWS.Period();

            dsSel.From = FromDate;
            dsSel.Until = ToDate;

            ActRepSel.DateStrategySelection = dsSel;
            ActRepSel.IncludeRegistrations = true;

            IWS.IdentifierVehicle Vehicle = new IWS.IdentifierVehicle() {
                IdentifierVehicleType = IWS.enumIdentifierVehicleType.ID,
                Id = vehicle
            };

            ActRepSel.Vehicles = new IWS.IdentifierVehicle[] { Vehicle };
            IWS.GetActivityReportDetailResult_V11 ActRepRes = IWSService.Get_ActivityReportDetail_V11(iwsLogin(login), ActRepSel);

            foreach (IWS.Error err in ActRepRes.Errors)
                throw new System.InvalidOperationException(err.ErrorCodeExplanation);

            int i = 0;
            ActivityReport[] actRepList = new ActivityReport[ActRepRes.ActivityReportDetailItems.Count()];

            foreach (IWS.ActivityReportDetailItem_V11 act in ActRepRes.ActivityReportDetailItems) {
                i = MoveActToActRepDetail(act, ref actRepList, i);
            }
            return (actRepList);
        }

        private int MoveActToActRepDetail(IWS.ActivityReportDetailItem_V11 act, ref ActivityReport[] actRepList, int i) {
            ActivityReport actRep = new ActivityReport();

            if (act.Position != null) {
                if (act.Position.AddressInfo != null)
                    actRep.AddressInfo = act.Position.AddressInfo;
                else if (act.Position.DistanceFromSmallCity != null)
                    actRep.AddressInfo = act.Position.DistanceFromLargeCity;
            }

            if (act.Place != null)
                actRep.Place = act.Place.PlaceID;

            if (act.Trip != null)
                actRep.Trip = act.Trip.TripID;

            if (act.Position != null) {
                if (act.Position.Latitude.HasValue)
                    actRep.Latitude = act.Position.Latitude.Value;
                if (act.Position.Longitude.HasValue)
                    actRep.Longitude = act.Position.Longitude.Value;
            }
            if (act.Driver != null)
                actRep.DriverCode = act.Driver.Code;

            if (act.CoDriver != null)
                actRep.BegeleiderCode = act.CoDriver.Code;

            actRep.ID = act.ID;
            actRep.VehicleID = act.Vehicle.ID;
            actRep.ActivityID = act.Activity.ID;
            actRep.ActivityName = act.Activity.Name;
            actRep.BeginDate = act.BeginDate;
            actRep.EndDate = act.EndDate;
            actRep.KmBegin = act.KmBegin;
            actRep.KmEnd = act.KmEnd;
            if (act.WorkingCode != null) {
                actRep.WorkingCodeCode = act.WorkingCode.Code;
                actRep.WorkingCodeDescription = act.WorkingCode.Description;
            }
            actRepList[i] = actRep;
            i += 1;

            return (i);
        }

    }
}

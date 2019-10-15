using System;
using System.Linq;

namespace TransicsNAV {
    public partial class TXTangoNAV {
        public string InsertVehicle(Login login, string NAVID, string Kenteken) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.VehicleInsert InsertVehicle = new IWS.VehicleInsert {
                VehicleID = NAVID,
                VehicleExternalCode = NAVID,
                LicensePlate = Kenteken
            };

            IWS.ResultInfo Insert_VehicleResult = IWSService.Insert_Vehicle(iwsLogin(login), InsertVehicle);
            string strError = handleError(Insert_VehicleResult);
            if (string.IsNullOrEmpty(strError))
                return (Insert_VehicleResult.ID);
            else
                return (strError);

        }

        public string UpdateVehicle(Login login, string NAVID, string Kenteken, String ChassisNo, String GSMNummer, string AutoFilter, string Brand) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            string strError = "";

            IWS.VehicleInsert UpdateVehicle = new IWS.VehicleInsert {
                VehicleID = NAVID,
                VehicleExternalCode = NAVID,
                LicensePlate = Kenteken,
                AutoFilter = AutoFilter,
                Category = IWS.VehicleCategory.GeneralCargo,
                GsmNumber = GSMNummer,
                TechnicalInfo = new IWS.VehicleTechnicalInfo {
                    ChassisNumber = ChassisNo,
                    BrandCode = Brand
                }
            };

            IWS.ResultInfo Update_VehicleResult = IWSService.Update_Vehicle(iwsLogin(login), UpdateVehicle);
            strError = handleError(Update_VehicleResult);
            if (!string.IsNullOrEmpty(strError))
                return ("Error Update " + strError);

            IWS.IdentifierVehicle Vehicle = new IWS.IdentifierVehicle() {
                IdentifierVehicleType = IWS.enumIdentifierVehicleType.ID,
                Id = NAVID
            };

            IWS.VehicleSelection_With_NextStop_Info GetVehicles = new IWS.VehicleSelection_With_NextStop_Info() {
                Identifiers = new IWS.IdentifierVehicle[] { Vehicle }
            };

            IWS.GetVehicleResult_With_NextStop_Info get_VehicleResult = IWSService.Get_Vehicles_V2(iwsLogin(login), GetVehicles);

            long strTransicsID = 0;

            foreach (IWS.VehicleResult_With_NextStop_Info veh in get_VehicleResult.Vehicles) {
                strTransicsID = veh.VehicleTransicsID;
            }

            return ("Ready " + strTransicsID.ToString());

        }
        public string UpdateVehicleOutOfDuty(Login login, string NAVID, DateTime OutOfService) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.VehicleInsert UpdateVehicle = new IWS.VehicleInsert {
                VehicleID = NAVID,
                Inactive = true,
                TechnicalInfo = new IWS.VehicleTechnicalInfo {
                    OutOfDuty = OutOfService,
                }
            };

            IWS.ResultInfo Update_VehicleResult = IWSService.Update_Vehicle(iwsLogin(login), UpdateVehicle);
            string strError = handleError(Update_VehicleResult);
            if (!string.IsNullOrEmpty(strError))
                return ("Error Out of Service " + strError);

            IWS.IdentifierVehicle Vehicle = new IWS.IdentifierVehicle() {
                IdentifierVehicleType = IWS.enumIdentifierVehicleType.ID,
                Id = NAVID
            };

            IWS.VehicleSelection_With_NextStop_Info GetVehicles = new IWS.VehicleSelection_With_NextStop_Info() {
                Identifiers = new IWS.IdentifierVehicle[] { Vehicle }
            };

            IWS.GetVehicleResult_With_NextStop_Info get_VehicleResult = IWSService.Get_Vehicles_V2(iwsLogin(login), GetVehicles);

            long strTransicsID = 0;

            foreach (IWS.VehicleResult_With_NextStop_Info veh in get_VehicleResult.Vehicles) {
                strTransicsID = veh.VehicleTransicsID;
            }

            return ("Ready " + strTransicsID.ToString());

        }

        public VehicleInfo[] GetVehicles(Login login) {

            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.DateTimeSelection DtSelection = new IWS.DateTimeSelection() {
                DateTimeType = IWS.enumSelectionDateTimeType.GET_ALL,
                //                IWS.enumSelectionDate.GET_MODIFIED_SINCE_LAST_REQUEST,
                Value = null
            };

            IWS.VehicleSelection_With_NextStop_Info GetVehicles = new IWS.VehicleSelection_With_NextStop_Info() {
                //Identifiers = new IWS.IdentifierVehicle[] { Vehicle },
                SelectionFromToday = DtSelection,
                Identifiers = null,
                IncludePosition = true,
                IncludeActivity = true,
                IncludeETAInfo = true,
                IncludeDrivers = true,
                IncludeNextStopInfo = true,
                IncludeUpdateDates = true
            };

            IWS.GetVehicleResult_With_NextStop_Info get_VehicleResult = IWSService.Get_Vehicles_V2(iwsLogin(login), GetVehicles);
            int i = 0;
            VehicleInfo[] vehInfoList = new VehicleInfo[get_VehicleResult.Vehicles.Count()];

            foreach (IWS.VehicleResult_With_NextStop_Info veh in get_VehicleResult.Vehicles) {

                VehicleInfo vehinfo = new VehicleInfo();
                vehinfo.VehicleID = veh.VehicleID;
                vehinfo.VehicleTransicsID = veh.VehicleTransicsID;
                vehinfo.CurrentKms = veh.CurrentKms;
                vehinfo.LastTrailerCode = veh.LastTrailerCode;

                if (veh.Position != null) {
                    vehinfo.addressInfo = veh.Position.AddressInfo;
                    vehinfo.distFromCapitol = veh.Position.DistanceFromCapitol;
                    vehinfo.distFromLargeCity = veh.Position.DistanceFromLargeCity;
                    vehinfo.distFromSmallcity = veh.Position.DistanceFromSmallCity;
                    vehinfo.distFromPointOfInterest = veh.Position.DistanceFromPointOfInterest;
                    vehinfo.countryCode = veh.Position.CountryCode;
                    vehinfo.Latitude = veh.Position.Latitude;
                    vehinfo.Longitude = veh.Position.Longitude;
                }
                if (veh.NextStopInfo != null) {
                    if (veh.NextStopInfo.Place != null)
                        vehinfo.strPlace = veh.NextStopInfo.Place.PlaceID;

                    if (veh.NextStopInfo.Trip != null)
                        vehinfo.strTrip = veh.NextStopInfo.Trip.TripID;
                }

                try {
                    vehinfo.ETALat = veh.ETAInfo.PositionDestination.Latitude.NullSafeToDouble();
                    vehinfo.ETALong = veh.ETAInfo.PositionDestination.Longitude.NullSafeToDouble();
                    vehinfo.ETAStatus = veh.ETAInfo.ETAStatus.NullSafeToString();
                    vehinfo.DistanceETA = veh.ETAInfo.DistanceETA;
                    vehinfo.ETA = veh.ETAInfo.ETA;
                    vehinfo.PosInfoDest = veh.ETAInfo.PositionInfoDestination;
                }
                catch { }
                if (veh.UpdateDates != null)
                    vehinfo.LastUpdate = veh.UpdateDates.Position;

                if (veh.Driver != null)
                    vehinfo.strDriverID = veh.Driver.ID;

                if (veh.Activity != null)
                    vehinfo.ActivityID = veh.Activity.ID.ToString();

                if (veh.UpdateDates.GPRS != null)
                    vehinfo.GPRSDate = veh.UpdateDates.GPRS;

                vehInfoList[i] = vehinfo;
                i += 1;

            }
            return (vehInfoList);
        }

    }
}

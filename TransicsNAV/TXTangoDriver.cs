using System;

namespace TransicsNAV {
    public partial class TXTangoNAV {
        public string InsertDriver(Login login, string NAVID, string VoorNaam, string AchterNaam) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.PersonInsert_V2 InsertPerson = new IWS.PersonInsert_V2 {
                PersonID = NAVID,
                Firstname = VoorNaam,
                Lastname = AchterNaam,
                Language = "NL",
                PersonExternalCode = NAVID
            };

            IWS.ResultInfo Insert_PersonResult = IWSService.Insert_Driver_V2(iwsLogin(login), InsertPerson);
            string strError = handleError(Insert_PersonResult);
            if (strError == null)
                return (Insert_PersonResult.ID);

            IWS.Identifier Driver = new IWS.Identifier() {
                IdentifierType = IWS.enumIdentifierType.ID,
                Id = NAVID
            };

            IWS.PersonSelection GetDrivers = new IWS.PersonSelection() {
                Persons = new IWS.Identifier[] { Driver },
            };

            IWS.GetPersonResult get_DriversResult = IWSService.Get_Drivers(iwsLogin(login), GetDrivers);

            long strTransicsID = 0;

            foreach (IWS.PersonResult driver in get_DriversResult.Persons) {
                strTransicsID = driver.PersonTransicsID;
            }

            return (strTransicsID.ToString());

        }

        public string UpdateDriver(Login login, string NAVID, string VoorNaam, string AchterNaam, string Language, string Street, string PostCode, string City, string email,
            string GSM, string DriversLicense, bool ADR, DateTime EmpDate, string TachoCardID) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.InterfacePersonUpdate_V3 UpdateDriver = new IWS.InterfacePersonUpdate_V3 {
                DriverToUpdate = new IWS.Identifier {
                    IdentifierType = IWS.enumIdentifierType.ID,
                    Id = NAVID
                },
                Lastname = AchterNaam,
                Firstname = VoorNaam,
                Language = Language,
                TachoCardInfo = new IWS.InterfaceTachoCardInfoUpdate {
                    CardId = TachoCardID
                },
                TechnicalInfo = new IWS.PersonTechnicalInfo {
                    DriverLicense = DriversLicense,
                    ADRCertificate = ADR,
                    Employed = EmpDate
                },
                ContactInfo = new IWS.InterfacePersonContactInfo_V3 {
                    Email = email,
                    GsmNumber = GSM,
                    Address = new IWS.PersonAddress_V3 {
                        Street = Street,
                        ZipCode = PostCode,
                        CountryCode = Language,
                        City = City
                    }
                }
            };

            IWS.ResultInfo Update_DriverResult = IWSService.Update_Driver_V4(iwsLogin(login), UpdateDriver);

            string strError = handleError(Update_DriverResult);
            if (!string.IsNullOrEmpty(strError))
                return (strError);

            IWS.Identifier Driver = new IWS.Identifier() {
                IdentifierType = IWS.enumIdentifierType.ID,
                Id = NAVID
            };

            IWS.PersonSelection GetDrivers = new IWS.PersonSelection() {
                Persons = new IWS.Identifier[] { Driver },
            };

            IWS.GetPersonResult get_DriversResult = IWSService.Get_Drivers(iwsLogin(login), GetDrivers);

            long strTransicsID = 0;

            foreach (IWS.PersonResult driver in get_DriversResult.Persons) {
                strTransicsID = driver.PersonTransicsID;
            }

            return (strTransicsID.ToString());
        }
        public string UpdateDriverResigned(Login login, string NAVID, DateTime TerminationDate) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.PersonInsert UpdateDriverRes = new IWS.PersonInsert {
                PersonID = NAVID,
                TechnicalInfo = new IWS.PersonTechnicalInfo {
                    Resigned = TerminationDate
                }
            };

            IWS.ResultInfo Update_DriverResultRes = IWSService.Update_Driver(iwsLogin(login), UpdateDriverRes);

            string strError = handleError(Update_DriverResultRes);
            if (!string.IsNullOrEmpty(strError))
                return (strError);

            IWS.Identifier Driver = new IWS.Identifier() {
                IdentifierType = IWS.enumIdentifierType.ID,
                Id = NAVID
            };

            IWS.PersonSelection GetDrivers = new IWS.PersonSelection() {
                Persons = new IWS.Identifier[] { Driver },
            };

            IWS.GetPersonResult get_DriversResult = IWSService.Get_Drivers(iwsLogin(login), GetDrivers);

            long strTransicsID = 0;

            foreach (IWS.PersonResult driver in get_DriversResult.Persons) {
                strTransicsID = driver.PersonTransicsID;
            }

            return (strTransicsID.ToString());
        }

        public IWS.GetRemainingDrivingRestingTimesResults_V4 GetDrivingHours(Login login, string driverID) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.RemainingDrivingRestingTimesSelection sel = new IWS.RemainingDrivingRestingTimesSelection();

            //one driver
            IWS.Identifier[] drivers = new IWS.Identifier[1];
            IWS.Identifier driver = new IWS.Identifier();
            driver.IdentifierType = IWS.enumIdentifierType.ID;
            driver.Id = driverID;
            drivers[0] = driver;
            sel.Drivers = drivers;


            sel.Edited_Data = false;
            sel.Original_Data = true;

            sel.OnBoardComputer_Data = false;
            sel.Tacho_Data = true;

            IWS.GetRemainingDrivingRestingTimesResults_V4 result = IWSService.Get_Remaining_Driving_Resting_Times_V4(iwsLogin(login), sel);
            return (result);
        }

    }
}

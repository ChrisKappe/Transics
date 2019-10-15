using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace TransicsNAV {
    public partial class TXTangoNAV {

        public string InsertPlanningEnhanced(Login login, Trip trip, bool Update, bool debug) {
            if (debug)
                return (trip.Dump());

            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.TripInsert tripInsert = new IWS.TripInsert() {
                DriverDisplay = trip.DriverDisplay ?? "",                                              
                TripId = trip.TripId ?? "",                                                          
                Places = trip.GetPlaces(),                                                       
                ExecutionDate = trip.ExecutionDate,
                StartTripAct = new IWS.Activity() { ID = trip.StartTripAct },                  
                StopTripAct = new IWS.Activity() { ID = trip.EndTripAct },                      
            };

            if (trip.PopUp1 != "" || trip.PopUp2 != "") 
                tripInsert.PlanningConfig = new IWS.PlanningConfig() {
                    Items = trip.GetPlanningConfigItem()
                };

            IWS.PlanningInsert PlanningData = new IWS.PlanningInsert() {
                Vehicle = new IWS.IdentifierVehicle()                                             
                {
                    IdentifierVehicleType = IWS.enumIdentifierVehicleType.ID,                   
                    Id = trip.Vehicle                                                           
                },
                Trips = new IWS.TripInsert[] { tripInsert }                                           
            };

            string strError = "";

            if (!login.IsTest) {
                if (Update) {
                    IWS.PlanningResultInsert PlanningResult = IWSService.Update_Planning(iwsLogin(login), PlanningData);
                    strError = handleError(PlanningResult);
                }
                else {
                    IWS.PlanningResultInsert PlanningInsertResult = IWSService.Insert_Planning(iwsLogin(login), PlanningData);
                    strError = handleError(PlanningInsertResult);
                }
            }

            if (string.IsNullOrEmpty(strError))
                if (login.IsTest) {
                    return ("Test Success. - " + trip.Dump());
                }
                else {
                    return ("Success.");

                }
            else
                return (strError);
        }
        public string CancelPlanning(Login login, string TripID) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.PlanningItemSelection CancelPlanningData = new IWS.PlanningItemSelection() {
                ID = TripID,
                PlanningSelectionType = IWS.enumPlanningItemSelectionType.TRIP
            };

            IWS.ExecutionResult PlanningCancelResult = IWSService.Cancel_Planning(iwsLogin(login), CancelPlanningData);

            return (handleError(PlanningCancelResult));

        }

        public string GetPlanning(Login login, ref Planning planning, string FromDate, ref string ToDate, bool Init) {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.PlanningModificationsSelection_V2 PlanningSelection = new IWS.PlanningModificationsSelection_V2();

            IWS.DateTimeSelection Selection = new IWS.DateTimeSelection();
            IWS.DateTimeRangeSelection dateRange = new IWS.DateTimeRangeSelection();
            if (Init) {
                Selection.DateTimeType = IWS.enumSelectionDateTimeType.GET_MODIFIED_SINCE_LAST_REQUEST;
                PlanningSelection.SelectionFromToday = Selection;
            }
            else {
                dateRange.StartDate = Convert.ToDateTime(FromDate);
                PlanningSelection.DateTimeRange = dateRange;
            }

            PlanningSelection.PlanningSelectionType = IWS.enumPlanningSelectionModificationsType.ALL;
            IWS.GetPlanningModificationsResult_V5 getPlanningResult = IWSService.Get_Planning_Modifications_V5(iwsLogin(login), PlanningSelection);

            string strError = handleError(getPlanningResult);

            if (getPlanningResult.MaximumModificationDate == null)
                ToDate = FromDate;
            else
                ToDate = getPlanningResult.MaximumModificationDate.Value.ToString();

            if (getPlanningResult.Trips != null)
                foreach (IWS.TripItemResult_V5 trip in getPlanningResult.Trips) {
                    planning.Trips.Add(new Trip {
                        TripId = trip.TripId,
                        Status = trip.Status.ToString(),
                        Vehicle = trip.Vehicle.ID ?? "",
                        Driver = trip.Driver?.ID ?? "",
                        StartDate = trip.StartDate,
                        EndDate = trip.EndDate,
                        TransferStatus = trip.TransferStatus.ToString(),
                        CancelStatus = trip.CancelStatus.ToString(),
                        ExecutionDate = (DateTime)trip.ExecutionDate
                    });
                }
            if (getPlanningResult.Places != null)
                foreach (IWS.PlaceItemResult_V4 place in getPlanningResult.Places) {
                    planning.Places.Add(new Place {
                        PlaceId = place.PlaceId,
                        Status = place.Status.ToString(),
                        ExecutionDate = (DateTime)place.ExecutionDate,
                        DriverDisplay = place.DriverDisplay,
                        VehicleCode = place.Vehicle.Code ?? "",
                        DriverID = place.Driver?.ID ?? "",
                        Author = place.Author,
                        ActivityID = (int)place.Activity.ID,
                        DispatcherDisplay = place.DispatcherDisplay,
                        Reference = place.References.InternalReference ?? "",
                        StartDate = place.StartDate,
                        EndDate = place.EndDate
                    });
                }
            if (getPlanningResult.Consultation != null)
                foreach (IWS.Consultation_V3 consultation in getPlanningResult.Consultation) {
                    Consultation thisCons = new Consultation();
                    if (consultation.Place != null)
                        thisCons.PlaceId = consultation.Place.PlaceID;
                    if (consultation.Position != null) {
                        thisCons.Latitude = consultation.Position.Latitude;
                        thisCons.Longitude = consultation.Position.Longitude;
                    }
                    thisCons.Km = consultation.Km;
                    if (consultation.Trip != null)
                        thisCons.TripId = consultation.Trip.TripID;
                    thisCons.VehicleId = consultation.Vehicle.ID;
                    thisCons.ActivityId = consultation.Activity.ID;
                    thisCons.ArrivalDate = consultation.ArrivalDate;
                    thisCons.LeavingDate = consultation.LeavingDate;
                    planning.Consultations.Add(thisCons);
                }
            if (getPlanningResult.Anomalies != null)
                foreach (IWS.Anomaly_V3 anomaly in getPlanningResult.Anomalies) {
                    if (anomaly.Place == null) {
                        planning.Anomalies.Add(new Anomaly {
                            CustomerID = anomaly.Product?.CustomerID ?? "",
                            //                        DriverId = anomaly.Driver.ID ?? "",
                            AnomalyCode = anomaly.AnomalyCode,
                            AnomalyDateTime = anomaly.AnomalyDateTime,
                            AnomalyDescription = anomaly.AnomalyDescription,
                            AnomalyID = anomaly.AnomalyID
                        });
                    } else {
                        planning.Anomalies.Add(new Anomaly {
                            PlaceId = anomaly.Place.CustomerID ?? "",
                            CustomerID = anomaly.Product?.CustomerID ?? "",
                            //                        DriverId = anomaly.Driver.ID ?? "",
                            AnomalyCode = anomaly.AnomalyCode,
                            AnomalyDateTime = anomaly.AnomalyDateTime,
                            AnomalyDescription = anomaly.AnomalyDescription,
                            AnomalyID = anomaly.AnomalyID
                        });
                    }
                }
            //if (getPlanningResult.Products != null)
            //    foreach (IWS.ProductItemResult_V3 product in getPlanningResult.Products) {
            //        Product thisProd = new Product();
            //        thisProd.ProductID = product.ProductID;
            //        thisProd.Status = product.Status.ToString();
            //        thisProd.DriverDisplay = product.DriverDisplay;
            //        //planning.Products.Add(thisProd);
            //    }
            if (getPlanningResult.Jobs != null)
                foreach (IWS.JobItemResult_V3 job in getPlanningResult.Jobs) {
                    planning.Jobs.Add(new Job {
                        JobId = job.JobId,
                        Comment = job.Comment,
                        DriverDisplay = job.DriverDisplay,
                        Status = job.Status.ToString()
                    });
                }
            return (strError);

        }


        public string ExecCancelPlaces(Login login, Cancel cancel) {
            IWS.ServiceSoapClient IWSService = InitWS(login);
            string strError = "";
            foreach (string PlaceID in cancel.PlaceID) {
                IWS.PlanningItemSelection CancelPlanningData = new IWS.PlanningItemSelection() {
                    ID = PlaceID,
                    PlanningSelectionType = IWS.enumPlanningItemSelectionType.PLACE
                };

                IWS.ExecutionResult PlanningCancelResult = IWSService.Cancel_Planning(iwsLogin(login), CancelPlanningData);

                strError = handleError(PlanningCancelResult);
            }

            return (strError);
        }

        static IWS.ServiceSoapClient InitWS(Login login) {

            BasicHttpBinding b = new BasicHttpBinding() {
                OpenTimeout = new TimeSpan(0, 5, 20),
                SendTimeout = new TimeSpan(0, 5, 20),
                ReceiveTimeout = new TimeSpan(0, 5, 20),
                CloseTimeout = new TimeSpan(0, 5, 20),
                MaxReceivedMessageSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue
            };
            b.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;
            b.ReaderQuotas.MaxStringContentLength = int.MaxValue;

            b.Security.Mode = BasicHttpSecurityMode.Transport;
            System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                (se, cert, chain, sslerror) => {
                    return true;
                };
            return new IWS.ServiceSoapClient(b, new EndpointAddress(login.WebServiceURL));
        }

        static IWS.Login iwsLogin(Login login) {
            IWS.Login login_Block = new IWS.Login() {
                Dispatcher = login.Dispatcher,
                Password = login.Password,
                Language = login.Language,
                Integrator = login.Integrator,
                SystemNr = login.SystemNr
            };

            return login_Block;
        }

        private string handleError(IWS.ExecutionResult response) {
            string strError = "";
            if (response.Errors.Length > 0) {
                foreach (IWS.Error err in response.Errors) {
                    strError = err.ErrorCode + ": " + err.ErrorCodeExplanation + "(" + err.Field + " - " + err.Value + ")";
                }
            }
            return strError;
        }
    }
}

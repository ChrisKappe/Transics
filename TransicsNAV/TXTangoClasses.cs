using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace TransicsNAV {
    public class Login {
        public string Dispatcher { get; set; }
        public string Password { get; set; }
        public string Language { get; set; }
        public string Integrator { get; set; }
        public int SystemNr { get; set; }
        public string WebServiceURL { get; set; }
        public bool IsTest { get; set; }
    }
    public class Message {
        public string Text { get; set; }
        public long? ID { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        public string VehicleID { get; set; }
        public string DriverID { get; set; }
    }
    public class VehicleInfo {
        public string VehicleID { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? ETALat { get; set; }
        public double? ETALong { get; set; }
        public string ETAStatus { get; set; }
        public int? DistanceETA { get; set; }
        public DateTime? ETA { get; set; }
        public string PosInfoDest { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string strDriverID { get; set; }
        public string ActivityID { get; set; }
        public string strTrip { get; set; }
        public string strPlace { get; set; }
        public string addressInfo { get; set; }
        public string LastTrailerCode { get; set; }
        public int? CurrentKms { get; set; }
        public long VehicleTransicsID { get; set; }
        public DateTime? GPRSDate { get; set; }
        public string distFromLargeCity { get; set; }
        public string distFromCapitol { get; set; }
        public string distFromPointOfInterest { get; set; }
        public string distFromSmallcity { get; set; }
        public string countryCode { get; set; }
    }
    public class Planning {
        public List<Trip> Trips { get; set; }
        public List<Place> Places { get; set; }
        public List<Consultation> Consultations { get; set; }
        public List<Anomaly> Anomalies { get; set; }
        public List<Product> Products { get; set; }
        public List<Job> Jobs { get; set; }
        public Planning() {
            Trips = new List<Trip>();
            Places = new List<Place>();
            Consultations = new List<Consultation>();
            Anomalies = new List<Anomaly>();
            Products = new List<Product>();
            Jobs = new List<Job>();
        }
    }
    public class Trip {
        public string TripId { get; set; }
        public string Status { get; set; }
        public string Vehicle { get; set; }
        public string Driver { get; set; }
        public string PopUp1 { get; set; }
        public string PopUp2 { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string TransferStatus { get; set; }
        public string CancelStatus { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string DriverDisplay { get; set; }
        public int StartTripAct { get; set; }
        public int EndTripAct { get; set; }
        public Place[] Places { get; set; }
        public string Dump() {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            return (json);
        }
        public IWS.PlaceInsert[] GetPlaces() {

            var PlaceInsertList = new List<IWS.PlaceInsert>();

            foreach (Place places in this.Places) {
                IWS.PlaceInsert place = new IWS.PlaceInsert() {                                  
                    PlaceId = places.PlaceId ?? "",                                                  
                    DriverDisplay = places.DriverDisplay ?? "",                                        
                    Comment = places.Comment ?? "",                                                   
                    Position = new IWS.Position {
                        Latitude = places.Latitude,
                        Longitude = places.Longitude
                    },
                    OrderSeq = places.OrderSeq,
                    ExecutionDate = places.ExecutionDate,
                    References = new IWS.Reference() { InternalReference = places.Reference },
                    Activity = new IWS.ActivityPlace() { ID = places.ActivityID },               
                    Products = places.GetProducts(),                                             
                    Jobs = places.GetJobs()
                };
//                throw new Exception("Popup :" + places.PopUp);
                if (places.PopUp != "")
                    place.PlanningConfig = new IWS.PlanningConfig() {
                        Items = places.GetPlanningConfigItem()
                    };

                PlaceInsertList.Add(place);

            }
            return PlaceInsertList.ToArray();
        }
        public IWS.PlanningConfigItem[] GetPlanningConfigItem() {
            var ItemList = new List<IWS.PlanningConfigItem>();
            if (this.PopUp1 != null) {
                IWS.PlanningConfigItem item1 = new IWS.PlanningConfigItem() {
                    Key = "1",
                    Value = "1"
                };
                ItemList.Add(item1);
                IWS.PlanningConfigItem item2 = new IWS.PlanningConfigItem() {
                    Key = "5",
                    Value = this.PopUp1
                };
                ItemList.Add(item2);
            }
            if (this.PopUp2 != null) {
                IWS.PlanningConfigItem item1 = new IWS.PlanningConfigItem() {
                    Key = "2",
                    Value = "1"
                };
                ItemList.Add(item1);
                IWS.PlanningConfigItem item2 = new IWS.PlanningConfigItem() {
                    Key = "6",
                    Value = this.PopUp2
                };
                ItemList.Add(item2);
            }
            return ItemList.ToArray();
        }

    }
    public class Place {
        public string PlaceId { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string VehicleCode { get; set; }
        public string DispatcherDisplay { get; set; }
        public string Author { get; set; }
        public string DriverID { get; set; }
        public int ActivityID { get; set; }
        public string DriverDisplay { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Comment { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PopUp { get; set; }
        public int OrderSeq { get; set; }
        public Product[] Products { get; set; }
        public Job[] Jobs { get; set; }
        public IWS.JobInsert[] GetJobs() {
            var JobInsertList = new List<IWS.JobInsert>();
            if (this.Jobs != null) {
                foreach (Job jobs in this.Jobs) {
                    IWS.JobInsert job = new IWS.JobInsert() {
                        JobId = jobs.JobId,
                        DriverDisplay = jobs.DriverDisplay,
                        Comment = jobs.Comment,
                        Products = GetProducts()
                    };
                    JobInsertList.Add(job);
                }

            }
            return JobInsertList.ToArray();
        }
        public IWS.ProductInsert[] GetProducts() {
            var ProductInsertList = new List<IWS.ProductInsert>();
            if (this.Products != null) {
                foreach (Product products in this.Products) {
                    IWS.ProductInsert product = new IWS.ProductInsert() {                      
                        ProductID = products.ProductID ?? "",                                        
                        DriverDisplay = products.DriverDisplay ?? "",                                
                        Comment = products.Comment ?? ""
                    };
                    ProductInsertList.Add(product);
                }
            }
            return ProductInsertList.ToArray();
        }
        public IWS.PlanningConfigItem[] GetPlanningConfigItem() {
            var ItemList = new List<IWS.PlanningConfigItem>();
            if (this.PopUp != null) {
                IWS.PlanningConfigItem item1 = new IWS.PlanningConfigItem() {
                    Key = getPopUpKey1(),
                    Value = "1"
                };
                ItemList.Add(item1);
                IWS.PlanningConfigItem item2 = new IWS.PlanningConfigItem() {
                    Key = getPopUpKey2(),
                    Value = this.PopUp
                };
                ItemList.Add(item2);
            }
            return ItemList.ToArray();
        }
        private string getPopUpKey1() {
            if (this.ActivityID == 102195) { //Laden
                return ("3");
            }
            else
                return ("4");
        }
        private string getPopUpKey2() {
            if (this.ActivityID == 102195) { //Laden
                return ("7");
            }
            else
                return ("8");
        }

    }
    public class Job {
        public string JobId { get; set; }
        public string DriverDisplay { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public Product[] Products { get; set; }
        public IWS.ProductInsert[] GetProducts() {
            var ProductInsertList = new List<IWS.ProductInsert>();
            if (this.Products != null) {
                foreach (Product products in this.Products) {
                    IWS.ProductInsert product = new IWS.ProductInsert() {                        // create a new product
                        ProductID = products.ProductID,                                          // the product ID  (set these lines in comment if you want to use the autonumbering)
                        DriverDisplay = products.DriverDisplay,                                  // the product name
                        Comment = products.Comment
                    };
                    ProductInsertList.Add(product);
                }
            }
            return ProductInsertList.ToArray();
        }

    }
    public class Consultation {
        public string PlaceId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Km { get; set; }
        public string TripId { get; set; }
        public string VehicleId { get; set; }
        public int? ActivityId { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? LeavingDate { get; set; }
    }
    public class Anomaly {
        public string PlaceId { get; set; }
        public string CustomerID { get; set; }
        public string DriverId { get; set; }
        public string AnomalyCode { get; set; }
        public DateTime? AnomalyDateTime { get; set; }
        public string AnomalyDescription { get; set; }
        public long AnomalyID { get; set; }
    }
    public class Product {
        public string ProductID { get; set; }
        public string Status { get; set; }
        public string DriverDisplay { get; set; }
        public string Comment { get; set; }
    }
    public class Document {
        public string ScanID { get; set; }
        public string SequenceID { get; set; }
        public string VehicleID { get; set; }
        public string DriverID { get; set; }
        public string PlaceID { get; set; }
        public string Comment { get; set; }
        public string DocumentType { get; set; }
        public string ID { get; set; }
        public string CreatedDateTime { get; set; }
        public Int64 DocumentID { get; set; }
        public MemoryStream Doc { get; set; }
        public string AzureURL { get; set; }
    }
    public class DocParameters {
        public string LastID { get; set; }
        public string NewLastID { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class ActivityReport {
        public long ID { get; set; }
        public string VehicleID { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public string Trip { get; set; }
        public string Place { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int KmBegin { get; set; }
        public int KmEnd { get; set; }
        public string AddressInfo { get; set; }
        public string WorkingCodeCode { get; set; }
        public string WorkingCodeDescription { get; set; }
        public string DriverCode { get; set; }
        public string BegeleiderCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
    public class Cancel {
        public string[] PlaceID { get; set; }
    }
}
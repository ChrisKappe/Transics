using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TransicsNAV
{
    public partial class TXTangoNAV
    {
        public List<Document> GetDocs(Login login, ref DocParameters docParameters)
        {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.ScannedDocumentsSelection_V2 docsSel = new IWS.ScannedDocumentsSelection_V2();

            IWS.DateTimeAndIdSelection Selection = new IWS.DateTimeAndIdSelection();
            if (docParameters.LastID == "")
                Selection.DateTimeType = IWS.enumSelectionDateTimeAndIdType.GET_MODIFIED_SINCE_LAST_REQUEST;
            else
            {
                Selection.DateTimeType = IWS.enumSelectionDateTimeAndIdType.GET_MODIFIED_SINCE_LAST_ID;
                Selection.Value = Convert.ToInt64(docParameters.LastID);
            }
            docsSel.SelectionFromToday = Selection;

            List<Document> documents = new List<Document>();

            IWS.ScannedDocumentSelection docSel = new IWS.ScannedDocumentSelection();

            IWS.GetScannedDocuments_V3 getScannedDocumentsResult = IWSService.Get_Scanned_Documents_V3(iwsLogin(login), docsSel);

            if (getScannedDocumentsResult.MaximumModificationID == null)
                docParameters.NewLastID = docParameters.LastID;
            else
                docParameters.NewLastID = getScannedDocumentsResult.MaximumModificationID.Value.ToString();

            if (getScannedDocumentsResult.Documents != null)
            {
                foreach (IWS.DocumentsResult_V3 doc in getScannedDocumentsResult.Documents)
                {
                    docSel.ScanID = Convert.ToInt32(doc.ScanID.Value);
                    docSel.ConvertToPdf = true;
                    Int64 ModificationCounter = doc.ModificationID;
                    IWS.GetScannedDocument getScannedDocumentResult = IWSService.Get_Scanned_Document(iwsLogin(login), docSel);
                    int i = 0;
                    foreach (IWS.DocumentResult document in getScannedDocumentResult.Documents)
                    {
                        i++;
                        string placeid = "";
                        string doctype = "";

                        if (doc.Place != null)
                            placeid = doc.Place.PlaceID.ToString();
                        if (doc.TypeDoc != null)
                            doctype = doc.TypeDoc.ToString();

                        MemoryStream stream = new MemoryStream(Convert.FromBase64String(document.Document1));

                        Document thisdoc = new Document();

                        thisdoc.ScanID = doc.ScanID.ToString();
                        thisdoc.SequenceID = i.ToString();
                        thisdoc.VehicleID = doc.Vehicle.ID.ToString();
                        if (doc.Driver != null)
                            thisdoc.DriverID = doc.Driver.ID.ToString();
                        thisdoc.PlaceID = placeid;
                        thisdoc.Comment = doc.Comment;
                        thisdoc.DocumentType = doctype;
                        thisdoc.Doc = stream;
                        thisdoc.DocumentID = ModificationCounter;
                        documents.Add(thisdoc);
                    }
                }
            }

            string strError = handleError(getScannedDocumentsResult);
            if (strError != null)
            {
                docParameters.ErrorMessage = strError;
            }
            return (documents);

        }

        public List<Document> GetDocsFromDates(Login login, ref DocParameters docParameters)
        {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.DateTimeRangeSelection dtRange = new IWS.DateTimeRangeSelection();

            List<Document> documents = new List<Document>();

            dtRange.StartDate = docParameters.FromDate;
            dtRange.EndDate = docParameters.ToDate;

            IWS.ScannedDocumentsSelection_V2 docsSel = new IWS.ScannedDocumentsSelection_V2()
            {
                SelectionDateRange = dtRange
            };
            
            IWS.ScannedDocumentSelection docSel = new IWS.ScannedDocumentSelection();

            IWS.GetScannedDocuments_V2 getScannedDocumentsResult = IWSService.Get_Scanned_Documents_V2(iwsLogin(login), docsSel);

            if (getScannedDocumentsResult.Documents != null)
            {
                foreach (IWS.Document_V3 doc in getScannedDocumentsResult.Documents)
                {
                    docSel.ScanID = Convert.ToInt32(doc.ScanID.Value);
                    docSel.ConvertToPdf = true;
                    Int64 ModificationCounter = doc.ModificationID;
                    IWS.GetScannedDocument getScannedDocumentResult = IWSService.Get_Scanned_Document(iwsLogin(login), docSel);
                    int i = 0;
                    foreach (IWS.DocumentResult document in getScannedDocumentResult.Documents)
                    {
                        i++;
                        string placeid = "";
                        string doctype = "";
                        if (doc.Place != null)
                            placeid = doc.Place.PlaceID.ToString();
                        if (doc.TypeDoc != null)
                            doctype = doc.TypeDoc.ToString();


                        Document thisdoc = new Document();

                        thisdoc.ScanID = doc.ScanID.ToString();
                        thisdoc.SequenceID = i.ToString();
                        thisdoc.VehicleID = doc.Vehicle.ID.ToString();
                        if (doc.Driver != null)
                            thisdoc.DriverID = doc.Driver.ID.ToString();
                        thisdoc.PlaceID = placeid;
                        thisdoc.Comment = doc.Comment;
                        thisdoc.DocumentType = doctype;
                        thisdoc.ID = doc.SerialNumber;
                        thisdoc.DocumentID = ModificationCounter;
                        DateTime dt = new DateTime(0, DateTimeKind.Utc);
                        dt = Convert.ToDateTime(doc.CreationDate);
                        dt.ToLocalTime();
                        thisdoc.CreatedDateTime = dt.ToString();
                        MemoryStream stream = new MemoryStream(Convert.FromBase64String(document.Document1));
                        thisdoc.Doc = stream;
                        documents.Add(thisdoc);
                    }
                }
            }

            string strError = handleError(getScannedDocumentsResult);
            if (strError != null)
            {
                docParameters.ErrorMessage = strError;
            }
            return (documents);

        }


    }
}

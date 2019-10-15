using System;
using System.Linq;

namespace TransicsNAV
{
    public partial class TXTangoNAV
    {
        public string SendMessage(Login login, string vehicle, string msg)
        {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.IdentifierVehicle Vehicle = new IWS.IdentifierVehicle()
            {
                IdentifierVehicleType = IWS.enumIdentifierVehicleType.ID,
                Id = vehicle
            };

            IWS.TextMessageSend Message = new IWS.TextMessageSend()
            {
                Vehicles = new IWS.IdentifierVehicle[] { Vehicle },
                VehicleType = IWS.enumVehicleType.SINGLE_VEHICLES,
                ReadConfirmation = false,
                Message = msg
            };

            IWS.SendTextMessageResult SendTextMessageResult = IWSService.Send_TextMessage_ReturnID(iwsLogin(login), Message);
            string strError = handleError(SendTextMessageResult);
            if (strError == null)
                return ("");
            else
                return (strError);

        }

        public Message[] GetMessagesOutbox(Login login, string LastID, ref string NewLastID)
        {
            IWS.ServiceSoapClient IWSService = InitWS(login);
            IWS.TextMessageOutboxSelection_V2 OutboxSelection = new IWS.TextMessageOutboxSelection_V2();

            IWS.DateTimeAndIdSelection Selection = new IWS.DateTimeAndIdSelection();
            if (LastID == "")
                Selection.DateTimeType = IWS.enumSelectionDateTimeAndIdType.GET_MODIFIED_SINCE_LAST_REQUEST;
            else
            {
                Selection.DateTimeType = IWS.enumSelectionDateTimeAndIdType.GET_MODIFIED_SINCE_LAST_ID;
                Selection.Value = Convert.ToInt64(LastID);
            }
            OutboxSelection.SelectionFromToday = Selection;

            IWS.GetTextMessagesOutbox_V5 OutBoxTextMessageResult = IWSService.Get_TextMessages_Outbox_V5(iwsLogin(login), OutboxSelection);

            if (OutBoxTextMessageResult.MaximumModificationID == null)
                NewLastID = LastID;
            else
                NewLastID = OutBoxTextMessageResult.MaximumModificationID.Value.ToString();

            int i = 0;
            Message[] messageList = new Message[OutBoxTextMessageResult.Outbox.Count()];

            foreach (IWS.TextMessageOutbox_V5 message in OutBoxTextMessageResult.Outbox)
            {
                Message msg = new Message();

                if (message.Vehicle != null)
                    msg.VehicleID = message.Vehicle.ID;
                if (message.Driver != null)
                    msg.DriverID = message.Driver.ID;

                msg.Text = message.Message;
                msg.ID = message.TextMessageID;
                msg.CreationDate = message.CreationDate;
                msg.DeliveredDate = message.DeliveredDate;
                messageList[i] = msg;
                i += 1;
            }
            return (messageList);
        }
        public Message[] GetMessagesInbox(Login login, string LastID, ref string NewLastID)
        {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            IWS.TextMessageInboxSelection_V2 InboxSelection = new IWS.TextMessageInboxSelection_V2();

            IWS.DateTimeAndIdSelection Selection = new IWS.DateTimeAndIdSelection();
            if (LastID == "")
                Selection.DateTimeType = IWS.enumSelectionDateTimeAndIdType.GET_MODIFIED_SINCE_LAST_REQUEST;
            else
            {
                Selection.DateTimeType = IWS.enumSelectionDateTimeAndIdType.GET_MODIFIED_SINCE_LAST_ID;
                Selection.Value = Convert.ToInt64(LastID);
            }
            InboxSelection.SelectionFromToday = Selection;

            IWS.GetTextMessagesInbox_V6 InboxTextMessageResult = IWSService.Get_TextMessages_Inbox_V6(iwsLogin(login), InboxSelection);

            if (InboxTextMessageResult.MaximumModificationID == null)
                NewLastID = LastID;
            else
                NewLastID = InboxTextMessageResult.MaximumModificationID.Value.ToString();

            int i = 0;
            Message[] messageList = new Message[InboxTextMessageResult.Inbox.Count()];

            foreach (IWS.TextMessageInbox_V6 message in InboxTextMessageResult.Inbox)
            {
                Message msg = new Message();

                if (message.Vehicle != null)
                    msg.VehicleID = message.Vehicle.ID;
                if (message.Driver != null)
                    msg.DriverID = message.Driver.ID;

                msg.Text = message.Message;
                msg.ID = message.TextMessageID;
                msg.CreationDate = message.CreationDate;
                msg.DeliveredDate = message.DeliveredDate;
                messageList[i] = msg;
                i += 1;
            }
            return (messageList);
        }
    }
}

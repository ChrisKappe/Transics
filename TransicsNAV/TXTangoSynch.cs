using System;

namespace TransicsNAV
{
    public partial class TXTangoNAV
    {
        public string AssignVehicleToPlanner(Login login, ref IWS.ServiceSoapClient IWSService, string VehID, string PlannerID)
        {
            IWS.IdentifierVehicle Veh = new IWS.IdentifierVehicle()
            {
                IdentifierVehicleType = IWS.enumIdentifierVehicleType.ID,
                Id = VehID
            };

            IWS.IdentifierVehicleItem item = new IWS.IdentifierVehicleItem()
            {
                Identifier = Veh
            };


            IWS.Identifier disp = new IWS.Identifier()
            {
                IdentifierType = IWS.enumIdentifierType.ID,
                Id = PlannerID
            };

            IWS.AssignIdentifiersToDispatchers ws = new IWS.AssignIdentifiersToDispatchers
            {
                VehicleItemList = new IWS.IdentifierVehicleItem[] { item },
                DispatcherItemList = new IWS.Identifier[] { disp }
            };

            IWS.ExecutionResult AssVehToPlanRes = IWSService.Assign_Vehicles_To_Dispatchers(iwsLogin(login), ws);
            string strError = handleError(AssVehToPlanRes);
            if (strError == null)
                return ("");
            else
                return (strError);

        }

        public string RemoveVehicleFromPlanner(Login login, ref IWS.ServiceSoapClient IWSService, string VehID, string PlannerID)
        {
            IWS.IdentifierVehicle Veh = new IWS.IdentifierVehicle()
            {
                IdentifierVehicleType = IWS.enumIdentifierVehicleType.ID,
                Id = VehID
            };

            IWS.Identifier disp = new IWS.Identifier()
            {
                IdentifierType = IWS.enumIdentifierType.ID,
                Id = PlannerID
            };

            IWS.RemoveIdentifiersFromDispatchers ws = new IWS.RemoveIdentifiersFromDispatchers
            {

                VehicleItemList = new IWS.IdentifierVehicle[] { Veh },
                DispatcherItemList = new IWS.Identifier[] { disp }
            };

            IWS.ExecutionResult RemVehToPlanRes = IWSService.Remove_Vehicles_From_Dispatchers(iwsLogin(login), ws);

            string strError = handleError(RemVehToPlanRes);
            if (strError == null)
                return ("");
            else
                return (strError);

        }

        public string DoVehicleSynch(Login login, string planner, bool debug, string[] VehToRemove, string[] VehToAdd)
        {
            IWS.ServiceSoapClient IWSService = InitWS(login);

            string ErrorMessage = "";
            foreach (string veh in VehToRemove)
            {
                try
                {
                    RemoveVehicleFromPlanner(login, ref IWSService, veh, planner);
                }
                catch (SystemException e)
                {
                    ErrorMessage = ErrorMessage + e.Message;
                };
            };

            foreach (string veh in VehToAdd)
            {
                try
                {
                    AssignVehicleToPlanner(login, ref IWSService, veh, planner);
                }
                catch (SystemException e)
                {
                    ErrorMessage = ErrorMessage + e.Message;
                };
            };

            if (debug)
                return (ErrorMessage + VehToAdd.ToString());

            return ("");

        }

    }
}

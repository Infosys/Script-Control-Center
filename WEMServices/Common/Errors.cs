/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
namespace Infosys.WEM.Infrastructure.Common
{
    public class Errors
    {
        //Enum to maintain application specific error code
        //Every error code listed here should have an entry 
        //created in the ErrorMessage resource file
        public enum ErrorCodes
        {
            Critical = 5000,
            Warning = 3000,
            Standard_Error = 1000,
            Document_Upload_Failed = 1001,
            WorkflowEntity_Not_Found = 1002,
            FileUpload_Validation_Failed = 1013,
            PKandRK_Validation_Failed = 1014,
            Request_Parameters_Invalid = 1015,            
            Data_NotFound = 1019,
            Companies_Not_Found = 1020,            
            Company_Name_Empty = 1024,           
            Client_Empty = 1026,
            Device_Empty = 1027,
            Title_Empty = 1028,
            Source_IP_Empty = 1029,            
            Message_Send_Failure = 1034,           
            Data_Format_Error = 1038,
            User_Exists = 1039,
            InvalidCharacter_Validation = 1040,
            Duplicate_Workflow_Name = 1041,
            Duplicate_Script_Name=1042,
            Method_Input_Parameters_Invalid = 1043,
            InvalidDataType_Validation=1044
        }
    }
}

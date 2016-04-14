using appez.exceptions;
using System;

namespace appez.model
{
    /// <summary>
    /// Model bean defining the response parameters for Smart
    /// Event request generated from the web layer
    /// </summary>
    public class SmartEventResponse
    {
        private bool isOperationComplete = false;
        public bool IsOperationComplete
        {
            set
            {
                isOperationComplete = value;
            }

            get
            {
                return isOperationComplete;
            }
        }
        private string serviceResponse = "";
        public string ServiceResponse
        {
            set
            {
                if (value == null)
                {
                    serviceResponse = "null";
                }
                else
                {
                    serviceResponse = value;
                }
            }

            get
            {
                return serviceResponse;
            }
        }
        private int exception = ExceptionTypes.UNKNOWN_EXCEPTION;
        public int ExceptionType
        {
            set
            {
                exception = value;
            }

            get
            {
                return exception;
            }
        }
        private string exceptionMessage = "";

        /// <summary>
        /// Exception message corresponding to the service operation exception
        /// </summary>
        public string ExceptionMessage
        {
            set
            {
                if (value == null)
                {
                    exceptionMessage = "";
                }
                else
                {
                    exceptionMessage = value;
                }
            }

            get
            {
                return exceptionMessage;
            }
        }

    }
}

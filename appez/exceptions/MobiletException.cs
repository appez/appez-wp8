using System;

namespace appez.exceptions
{
    /// <summary>
    /// Generic exception class for handling exceptions occurring
    /// due to the framework.
    /// </summary>
    public class MobiletException : Exception
    {
        public int exception = ExceptionTypes.UNKNOWN_EXCEPTION;
        public string errorMsg = "";

        public MobiletException()
            : base()
        {

        }

        public MobiletException(int exceptionType)
            : base()
        {

            this.ExceptionType = exceptionType;
        }

        public MobiletException(string detailMessage)
            : base(detailMessage)
        {
            this.ErrorMsg = detailMessage;
        }
        /// <summary>
        /// Error Message.
        /// </summary>
        private string ErrorMsg
        {
            get
            {
                return errorMsg;
            }
            set
            {
                this.errorMsg = value;
            }
        }

        /// <summary>
        /// Exception Type.
        /// </summary>
        private int ExceptionType
        {
            set
            {

                this.exception = value;
            }
            get
            {

                return exception;
            }
        }


    }
}

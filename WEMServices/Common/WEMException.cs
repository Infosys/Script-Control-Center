/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Infosys.WEM.Infrastructure.Common
{
    [Serializable]
    public class WEMException : System.Exception, ISerializable
    {
        public WEMException()
            : base()
        {

        }
        public WEMException(string message)
            : base(message)
        {

        }
        public WEMException(string message, Exception inner)
            : base(message, inner)
        {

        }
        protected WEMException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Add implementation.
        }
    }

    [Serializable]
    public class WEMCriticalException : System.Exception,ISerializable
    {
        public WEMCriticalException():base()
        {

        }
        public WEMCriticalException(string message):base(message)
        {

        }
        public WEMCriticalException(string message, Exception inner):base(message,inner)
        {

        }
        protected WEMCriticalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        // Add implementation.
        }
    }

    [Serializable]
    public class WEMDataItemNotFoundException : System.Exception,ISerializable
    {
        public WEMDataItemNotFoundException(): base()
        {

        }
        public WEMDataItemNotFoundException(string message): base(message)
        {

        }
        public WEMDataItemNotFoundException(string message, Exception inner): base(message, inner)
        {

        }
        protected WEMDataItemNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        // Add implementation.
        }

    }

    /// <summary>
    /// The exception to be rasied in case validation fails for input request data
    /// </summary>
    [Serializable]
    public class WEMValidationException : System.Exception, ISerializable
    {
        public WEMValidationException()
            : base()
        {

        }
        public WEMValidationException(string message)
            : base(message)
        {

        }
        public WEMValidationException(string message, Exception inner)
            : base(message, inner)
        {

        }
        protected WEMValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            // Add implementation.
        }

    }

}

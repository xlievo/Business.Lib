/*==================================
             ########
            ##########

             ########
            ##########
          ##############
         #######  #######
        ######      ######
        #####        #####
        ####          ####
        ####   ####   ####
        #####  ####  #####
         ################
          ##############
==================================*/

namespace Business.Logger
{
    public class log4netAdapter : ILogger
    {
        readonly log4net.ILog log;

        public log4netAdapter(log4net.ILog log)
        {
            this.log = log;
        }

        public void Debug(string message)
        {
            if (log.IsDebugEnabled) { log.Debug(message); }
        }

        public void Debug(string format, params object[] args)
        {
            if (log.IsDebugEnabled) { log.DebugFormat(format, args); }
        }

        public void Info(string message)
        {
            if (log.IsInfoEnabled) { log.Info(message); }
        }

        public void Info(string format, params object[] args)
        {
            if (log.IsInfoEnabled) { log.InfoFormat(format, args); }
        }

        public void Warn(string message)
        {
            if (log.IsWarnEnabled) { log.Warn(message); }
        }

        public void Warn(string format, params object[] args)
        {
            if (log.IsWarnEnabled) { log.WarnFormat(format, args); }
        }

        public void Error(string message)
        {
            if (log.IsErrorEnabled) { log.Error(message); }
        }

        public void Error(string format, params object[] args)
        {
            if (log.IsErrorEnabled) { log.ErrorFormat(format, args); }
        }

        public void Fatal(string message)
        {
            if (log.IsFatalEnabled) { log.Fatal(message); }
        }

        public void Fatal(string format, params object[] args)
        {
            if (log.IsFatalEnabled) { log.FatalFormat(format, args); }
        }
    }
}

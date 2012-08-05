using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IrrlichtLime;

public static class LoggerExtensions
{
    public static void Write(this Logger logger, string format, params object[] args)
    {
        if (logger == null)
            return;

        logger.Log(string.Format(format, args));
    }
}

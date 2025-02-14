﻿using Comdata.AppSupport.AppTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comdata.AppSupport.PPOLMorningChecklist
{
    interface IFileSystem
    {
        ILog Log { get; set; }

        void Copy(string source, string dest);

        void Move(string source, string dest);

        void Delete(string source);
    }
}

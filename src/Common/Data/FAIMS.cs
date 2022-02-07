﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPToolkit.Common.Data
{
    public class FAIMS
    {
        /// <summary>
        /// Compensation voltage
        /// </summary>
        public int CV { get; set; } = -55;

        public TriState Faims_State = TriState.None;
    }

    public enum TriState
    {
        On,
        Off,
        None
    }
}

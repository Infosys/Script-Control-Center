/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.ATR.ProcessRecorder.Entities
{
    public class UseCasePE  
    {
        [ReadOnly(true), PropertyGridBrowsable(true)] 
        public DateTime InitiatedOn { get; set; }

        [ReadOnly(true)]
        [Browsable(false)]
        public string Id { get; set; }

        [ReadOnly(true), PropertyGridBrowsable(true)]
        public string Name { get; set; }

        [ReadOnly(true), PropertyGridBrowsable(true)]
        public string CreatedBy { get; set; }

        [ReadOnly(true)]
        [DisplayName("Executed on"), PropertyGridBrowsable(true)]
        public string MachineName { get; set; }

        [ReadOnly(true), PropertyGridBrowsable(true)]
        public string OS { get; set; }

        [ReadOnly(true), PropertyGridBrowsable(true)]
        public string Domain { get; set; }

        [ReadOnly(true), PropertyGridBrowsable(true)]
        [DisplayName("Machine Type")]
        public string MachineType { get; set; }

        [ReadOnly(true), PropertyGridBrowsable(true)]
        public string OSVersion { get; set; }

        [ReadOnly(true), PropertyGridBrowsable(true)]
        [DisplayName("Screen Resolution")]
        public string ScreenResolution { get; set; }

        [ReadOnly(true)]
        [Browsable(false)]
        [DisplayName("UseCase File Name")]
        public string FileName { get; set; }

        [ReadOnly(true)]
        [Browsable(false)]
        [DisplayName("UseCase File Path")]
        public string FilePath { get; set; }

        [ReadOnly(true), PropertyGridBrowsable(true)]
        [DisplayName("Associated  Tasks")]
        public int AssociatedTasks { get; set; } 
    }

    public class PropertyGridBrowsableAttribute : Attribute
    {
        private bool browsable;
        public PropertyGridBrowsableAttribute(bool browsable)
        {
            this.browsable = browsable;
        }
    }
}

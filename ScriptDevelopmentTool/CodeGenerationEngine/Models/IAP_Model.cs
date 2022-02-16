/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerationEngine.Model
{
    public class Iap_Model
    {
        public string IAPModel { get; set; }

        public Iap_Model()
        {
            IAPModel = GetModel(); 
        }

        private string GetModel() 
        {
            StringBuilder _result = new StringBuilder();            
            foreach (var app in CodeGenerator.atrModel.AppConfigs)
            {
                foreach (var screen in app.ScreenConfigs)
                {
                    _result.Append("\n\n\t##############################################################################");
                    _result.Append(string.Format("\n\t### Application  :  {0}", app.AppName.Replace(" ", "")));
                    _result.Append(string.Format("\n\t### Screen       :  {0}", screen.ScreenName));
                    _result.Append("\n\t##############################################################################");
                    foreach (var entity in screen.EntityConfigs)
                    {
                        string _name = string.Format("{0}_{1}", screen.ScreenName, entity.EntityName).ToUpper();
                        string _value = string.Format("\"{0}.{1}.{2}\"", app.AppName, screen.ScreenName, entity.EntityName);
                        _result.Append(string.Format("\n\t{0} = {1}", _name, _value));

                        //if(entity.EntityControlConfig.ControlClass.ToString().ToUpper().Equals("EDIT"))
                        //    _result.Append(string.Format("\n\t{0} = \"{0}\"", entity.EntityControlConfig.ControlName));
                    }
                }
            }
            return _result.ToString();
        }
    }
}

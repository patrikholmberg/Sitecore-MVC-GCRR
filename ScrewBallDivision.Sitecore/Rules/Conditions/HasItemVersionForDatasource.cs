using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrewBallDivision.Sitecore.Rules.Conditions
{
    public class HasItemVersionForDatasource<T> : OperatorCondition<T>
      where T : ConditionalRenderingsRuleContext
    {
        /// <summary>
        /// Evaluates if a renderings potential datasource item have any versions in the current langugage
        /// </summary>
        /// <param name="ruleContext"></param>
        /// <returns></returns>
        protected override bool Execute(T ruleContext)
        {            
            var datasource = ruleContext.Reference.Settings.DataSource;

            //If no datasource is configured, the rendering probably does not require a datasource
            if (string.IsNullOrEmpty(datasource)) return true;
            ID datasourceId = ID.Null;
            //If the datasource starts with /sitecore/content or if it's an ID we check if the datasource item exist and if it has any versions
            if (datasource.StartsWith("/sitecore/content") || ID.TryParse(datasource, out datasourceId))
            {
                var datasoureItem = ruleContext.Item.Database.GetItem(datasource);
                if (datasoureItem == null) return false;
                return datasoureItem.Versions.Count > 0;
            }
            //If non of the above conditions are met, this is probably a query datasource which might be a larger collection of items
            return true;
        }
    }
}

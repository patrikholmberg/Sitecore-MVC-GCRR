using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Layouts;
using Sitecore.Mvc.Analytics.Pipelines.Response.CustomizeRendering;
using Sitecore.Mvc.Analytics.Presentation;
using Sitecore.Mvc.Extensions;
using Sitecore.Mvc.Presentation;
using Sitecore.Rules;
using Sitecore.Rules.ConditionalRenderings;
using Sitecore.Rules.Conditions;
using Sitecore.SecurityModel;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScrewBallDivision.Sitecore.Processors
{
    public class EvaluateGlobalConditionalRenderingRules : CustomizeRenderingProcessor
    {
        public override void Process(CustomizeRenderingArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            if (Context.PageMode.IsPageEditor)
                return;
            this.Evaluate(args);
        }

        /// <summary>
        /// Evaluates global rendering conditions for this rendering
        /// </summary>
        /// <param name="args"></param>
        protected virtual void Evaluate(CustomizeRenderingArgs args)
        {
            Assert.ArgumentNotNull((object)args, "args");
            Item obj = args.PageContext.Item;
            if (obj == null)
                return;
            var renderingReference = this.GetRenderingReference(args.Rendering, Context.Language, args.PageContext.Database);
            var rules = this.GetGlobalRules(obj);
            if (rules.Count == 0)
                return;
            var renderingsRuleContext = new ConditionalRenderingsRuleContext(new List<RenderingReference>() { renderingReference }, renderingReference);
            renderingsRuleContext.Item = obj;
            var context = renderingsRuleContext;
            rules.Run(renderingsRuleContext);
            this.ApplyActions(args, context);
            args.IsCustomized = true;
        }

        protected virtual void ApplyActions(CustomizeRenderingArgs args, ConditionalRenderingsRuleContext context)
        {
            Assert.ArgumentNotNull((object)args, "args");
            Assert.ArgumentNotNull((object)context, "context");
            RenderingReference reference = context.References.Find((Predicate<RenderingReference>)(r => r.UniqueId == context.Reference.UniqueId));
            if (reference == null)
                args.Renderer = (Renderer)new EmptyRenderer();
            else
                this.ApplyChanges(args.Rendering, reference);
        }
        protected virtual RenderingReference GetRenderingReference(Rendering rendering, Language language, Database database)
        {
            Assert.IsNotNull((object)rendering, "rendering");
            Assert.IsNotNull((object)language, "language");
            Assert.IsNotNull((object)database, "database");
            string text = rendering.Properties["RenderingXml"];
            if (string.IsNullOrEmpty(text))
                return (RenderingReference)null;
            else
                return new RenderingReference(XmlExtensions.ToXmlNode(XElement.Parse(text)), language, database);
        }

        /// <summary>
        /// Returns all global conditional rendering rules in a RuleList
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual RuleList<ConditionalRenderingsRuleContext> GetGlobalRules(Item item)
        {
            Assert.ArgumentNotNull((object)item, "item");
            RuleList<ConditionalRenderingsRuleContext> ruleList = (RuleList<ConditionalRenderingsRuleContext>)null;
            using (new SecurityDisabler())
            {
                Item parentItem = item.Database.GetItem(ItemIDs.ConditionalRenderingsGlobalRules);
                if (parentItem != null)
                    ruleList = RuleFactory.GetRules<ConditionalRenderingsRuleContext>(parentItem, "Rule");
            }
            return ruleList;
        }
    }
}

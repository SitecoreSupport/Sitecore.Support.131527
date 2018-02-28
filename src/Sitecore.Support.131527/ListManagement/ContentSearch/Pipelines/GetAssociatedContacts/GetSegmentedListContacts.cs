namespace Sitecore.Support.ListManagement.ContentSearch.Pipelines.GetAssociatedContacts
{
  using Sitecore.Analytics.Rules.SegmentBuilder;
  using Sitecore.ContentSearch;
  using Sitecore.ContentSearch.Analytics.Models;
  using Sitecore.Diagnostics;
  using Sitecore.ListManagement.Configuration;
  using Sitecore.ListManagement.ContentSearch;
  using Sitecore.ListManagement.ContentSearch.Model;
  using Sitecore.ListManagement.ContentSearch.Pipelines;
  using System.Linq;
  
  public class GetSegmentedListContacts
  {
    private readonly ISearchIndex index;
    private readonly IRuleEngine<IndexedContact, VisitorRuleContext<IndexedContact>> ruleEngine;

    public GetSegmentedListContacts(IRuleEngine<IndexedContact, VisitorRuleContext<IndexedContact>> ruleEngine)
    {
      Assert.ArgumentNotNull(ruleEngine, "ruleEngine");
      this.index = ContentSearchManager.GetIndex(ListManagementSettings.ContactsIndexName);
      this.ruleEngine = ruleEngine;
    }
    public virtual void Process(GetAssociatedContactsArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      SegmentedList segmentedList = args.ContactList as SegmentedList;
      if (segmentedList != null && !string.IsNullOrEmpty(segmentedList.Query))
      {
        args.Contacts = new QueryableProxy<ContactData>(new SegmentedListContactQueryProvider<ContactData>(this.index, this.ruleEngine, segmentedList.Query, segmentedList.Source));
        return;
      }
      args.Contacts = Enumerable.Empty<ContactData>().AsQueryable<ContactData>();
    }
  }
}
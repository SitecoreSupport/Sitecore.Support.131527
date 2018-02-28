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
        if (segmentedList.Source.IncludedListSources.Any<string>() && segmentedList.Source.IncludedListSources.First<string>().Contains('|'))
        {
          segmentedList.Source.IncludedListSources = segmentedList.Source.IncludedListSources.First<string>().Split(new char[]
          {
            '|'
          });
        }
        if (segmentedList.Source.ExcludedListSources.Any<string>() && segmentedList.Source.ExcludedListSources.First<string>().Contains('|'))
        {
          segmentedList.Source.ExcludedListSources=segmentedList.Source.ExcludedListSources.First<string>().Split(new char[]
          {
            '|'
          });
        }
        args.Contacts = new QueryableProxy<ContactData>(new SegmentedListContactQueryProvider<ContactData>(this.index, this.ruleEngine, segmentedList.Query, segmentedList.Source));
        return;
      }
      args.Contacts = Enumerable.Empty<ContactData>().AsQueryable<ContactData>();
    }
  }
}
/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                    Component : Use cases Layer                      *
*  Assembly : FinancialAccounting.AccountingRules.dll       Pattern   : Use case interactor class            *
*  Type     : AccountingClassificationsUseCases             License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Use cases for accounting classifications searching and retrieving.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Services;

using Empiria.FinancialAccounting.Transactions.Adapters;

namespace Empiria.FinancialAccounting.AccountingRules.UseCases {

  /// <summary>Use cases for accounting classifications searching and retrieving.</summary>
  public class AccountingClassificationsUseCases : UseCase {

    #region Constructors and parsers

    protected AccountingClassificationsUseCases() {
      // no-op
    }

    static public AccountingClassificationsUseCases UseCaseInteractor() {
      return CreateInstance<AccountingClassificationsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases

    public FixedList<AccountingClassificationDto> GetAccountingClassifications() {

      var results = new Dictionary<string, ClassificationGroup>(StringComparer.Ordinal);

      foreach (ClassificationGuide guide in ClassificationGuide.GetList()) {

        foreach (ClassificationRule rule in guide.Rules) {

          foreach (string selector in rule.SelectorValues) {

            ClassificationGroup group = GetOrCreateClassificationGroup(results, rule, selector);

            foreach (ClassificationEntry entry in rule.Entries) {

              string uid = entry.StandardAccount.Number;
              string displayName = GetDisplayName(entry);

              group.Items.Add(uid, displayName);
            }
          }
        }
      }

      return Map(results);
    }

    #endregion Use cases

    #region Helpers

    static private string GetDisplayName(ClassificationEntry entry) {

      string configuredName = entry.ExtData.Get("displayName", entry.StandardAccount.Name);

      string suffix = entry.StandardAccount.Number.Split('.').Last();

      return $"{suffix} - {configuredName}";
    }


    private ClassificationGroup GetOrCreateClassificationGroup(Dictionary<string, ClassificationGroup> results,
                                                               ClassificationRule rule,
                                                               string selector) {
      if (results.TryGetValue(selector, out ClassificationGroup group)) {
        return group;
      }


      var newGroup = new ClassificationGroup(rule.Name);

      results.Add(selector, newGroup);

      return newGroup;
    }


    static private FixedList<AccountingClassificationDto> Map(Dictionary<string, ClassificationGroup> results) {
      return results.OrderBy(x => x.Key)
                    .Where(x => x.Value.Items.Count != 0)
                    .Select(x => new AccountingClassificationDto {
                      SelectorValue = x.Key,
                      Name = x.Value.Name,
                      Classifications = x.Value.Items.OrderBy(y => y.Key)
                                                       .Select(y => new NamedEntityDto(y.Key, y.Value))
                                                       .ToFixedList()
                    })
                    .ToFixedList();
    }

    #endregion Helpers

    #region Nested types

    private sealed class ClassificationGroup {

      internal ClassificationGroup(string name) {
        Name = name;
        Items = new Dictionary<string, string>(StringComparer.Ordinal);
      }

      internal string Name {
        get;
      }

      internal Dictionary<string, string> Items {
        get;
      }
    }

    #endregion Nested types

  }  // class AccountingClassificationsUseCases

}  // namespace Empiria.FinancialAccounting.AccountingRules.UseCases

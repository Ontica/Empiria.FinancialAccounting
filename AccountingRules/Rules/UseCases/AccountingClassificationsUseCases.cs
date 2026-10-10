/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                    Component : Use cases Layer                      *
*  Assembly : FinancialAccounting.AccountingRules.dll       Pattern   : Use case interactor class            *
*  Type     : AccountingClassificationsUseCases             License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Use cases for accounting classifications searching and retrieving.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

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

      var accountsChart = AccountsChart.IFRS;

      FixedList<StandardAccount> accounts = accountsChart.GetStandardAccounts();

      var summaryAccounts = accounts.FindAll(x => x.Number.StartsWith("6.05") &&
                                                  x.Role == AccountRole.Sumaria &&
                                                  TryGetSelectorValue(x.Name) != null);


      var selectorValues = summaryAccounts.Select(x => GetSelectorValue(x.Name))
                                          .Distinct()
                                          .OrderBy(x => x)
                                          .ToList();

      var result = new List<AccountingClassificationDto>();

      foreach (string selectorValue in selectorValues) {

        FixedList<StandardAccount> parents = summaryAccounts.FindAll(x => GetSelectorValue(x.Name) == selectorValue);

        FixedList<StandardAccount> children = GetDetailChildren(accounts, parents);

        if (children.Count == 0) {
          continue;
        }

        result.Add(new AccountingClassificationDto {
          SelectorValue = selectorValue,
          Name = GetSelectorName(parents),
          Classifications = children.OrderBy(x => x.Number)
                                    .Select(x => MapClassification(x))
                                    .ToFixedList()
        });
      }

      return result.ToFixedList();
    }

    #endregion Use cases

    #region Helpers

    static private FixedList<StandardAccount> GetDetailChildren(FixedList<StandardAccount> accounts,
                                                                FixedList<StandardAccount> parents) {

      var children = new List<StandardAccount>();

      foreach (StandardAccount parent in parents) {

        FixedList<StandardAccount> parentChildren = accounts.FindAll(x => x.HasParent &&
                                                                          x.GetParent().Equals(parent) &&
                                                                          x.Role != AccountRole.Sumaria);

        children.AddRange(parentChildren);
      }

      return children.GroupBy(x => x.Number)
                     .Select(x => x.First())
                     .OrderBy(x => x.Number)
                     .ToFixedList();
    }


    static private string GetSelectorName(FixedList<StandardAccount> parents) {

      Assertion.Require(parents.Count > 0, nameof(parents));

      return parents[0].Name;
    }


    static private string GetSelectorValue(string accountName) {

      var selectorValue = TryGetSelectorValue(accountName);

      Assertion.Require(selectorValue, nameof(selectorValue));

      return selectorValue;
    }


    static private NamedEntityDto MapClassification(StandardAccount account) {

      string suffix = account.Number.Split('.')
                                    .Last();

      return new NamedEntityDto(account.Number, $"{suffix} - {account.Name}");
    }


    static private string TryGetSelectorValue(string accountName) {

      if (string.IsNullOrWhiteSpace(accountName)) {
        return null;
      }

      string candidate = accountName.Trim()
                                    .Split(' ')
                                    .FirstOrDefault();

      if (string.IsNullOrWhiteSpace(candidate)) {
        return null;
      }

      if (candidate.Length != 5) {
        return null;
      }

      if (!candidate.All(char.IsDigit)) {
        return null;
      }

      return candidate;
    }

    #endregion Helpers

  }  // class AccountingClassificationsUseCases

}  // namespace Empiria.FinancialAccounting.AccountingRules.UseCases

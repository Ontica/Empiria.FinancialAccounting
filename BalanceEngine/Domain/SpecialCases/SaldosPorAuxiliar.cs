/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Domain Layer                            *
*  Assembly : FinancialAccounting.BalanceEngine.dll      Pattern   : Service provider                        *
*  Type     : SaldosPorAuxiliar                          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Genera los datos para el reporte de saldos por auxiliar.                                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Collections;
using Empiria.FinancialAccounting.BalanceEngine.Adapters;

namespace Empiria.FinancialAccounting.BalanceEngine {

  /// <summary>Genera los datos para el reporte de saldos por auxiliar.</summary>
  internal class SaldosPorAuxiliar {

    private readonly TrialBalanceCommand _command;

    public SaldosPorAuxiliar(TrialBalanceCommand command) {
      _command = command;
    }


    internal TrialBalance Build() {
      var helper = new TrialBalanceHelper(_command);

      List<TrialBalanceEntry> trialBalance = helper.GetPostingEntries().ToList();

      EmpiriaHashTable<TrialBalanceEntry> summaryEntries = BalancesBySubsidiaryAccounts(trialBalance);

      EmpiriaHashTable<TrialBalanceEntry> combineEntries =
                                          CombineTotalSubsidiaryEntriesWithSummaryAccounts(summaryEntries);

      trialBalance = helper.RestrictLevels(combineEntries.ToFixedList().ToList());

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    internal TrialBalance BuildForBalancesGeneration() {
      var helper = new TrialBalanceHelper(_command);

      _command.WithSubledgerAccount = true;

      FixedList<TrialBalanceEntry> trialBalance = helper.GetPostingEntries();

      var returnBalance = new FixedList<ITrialBalanceEntry>(trialBalance.Select(x => (ITrialBalanceEntry) x));

      return new TrialBalance(_command, returnBalance);
    }


    #region Helper methods


    private void AccumulateSubledgerAccount(EmpiriaHashTable<TrialBalanceEntry> returnedEntries,
                                            TrialBalanceEntry entry,
                                            StandardAccount currentParent) {

      string hash = $"{entry.Currency.Code}||{currentParent.Number}";

      if (returnedEntries.ContainsKey(hash)) {
        if (returnedEntries[hash].SubledgerAccountIdParent == entry.SubledgerAccountId &&
            returnedEntries[hash].NotHasSector) {
          returnedEntries[hash].Sum(entry);
        }
      }
    }


    private List<TrialBalanceEntry> AddSummaryAccounts(EmpiriaHashTable<TrialBalanceEntry> summaryEntries,
                                                       List<TrialBalanceEntry> accumulatedEntries,
                                                       TrialBalanceEntry entry) {

      var summaryAccounts = summaryEntries.ToFixedList().Where(
              a => a.Account.GroupNumber == entry.Account.GroupNumber &&
              a.SubledgerAccountId == 0 &&
              a.SubledgerAccountIdParent == entry.SubledgerAccountIdParent &&
              a.Ledger.Number == entry.Ledger.Number &&
              a.Currency.Code == entry.Currency.Code &&
              a.ItemType == TrialBalanceItemType.BalanceEntry).ToList();

      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      accumulatedEntries.ForEach(item =>
                          hashReturnedEntries.Insert(GetHash(item, item.Account, item.Sector), item));

      foreach (var summary in summaryAccounts) {
        if (!hashReturnedEntries.ContainsKey(GetHash(summary, summary.Account, summary.Sector))) {
          hashReturnedEntries.Insert(GetHash(summary, summary.Account, summary.Sector), summary);
        }
      }

      return hashReturnedEntries.ToFixedList().ToList();
    }


    private EmpiriaHashTable<TrialBalanceEntry> BalancesBySubsidiaryAccounts(List<TrialBalanceEntry> trialBalance) {

      var subsidiaryEntries = trialBalance.Where(a => a.SubledgerAccountId > 0).ToList();

      var hashSubsidiaryEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var item in subsidiaryEntries) {
        string hash = $"{item.Ledger.Number}||{item.Currency.Code}||" +
                      $"{item.Account.Number}||{item.Sector.Code}||" +
                      $"{item.SubledgerAccountId}";

        hashSubsidiaryEntries.Insert(hash, item);
      }

      EmpiriaHashTable<TrialBalanceEntry> returnedSubsidiaryEntries = GenerateOrIncreaseParentEntries(hashSubsidiaryEntries);

      return returnedSubsidiaryEntries;
    }


    private void CreateOrAccumulateParentWithoutSector(
                  EmpiriaHashTable<TrialBalanceEntry> summaryParentEntries,
                  EmpiriaHashTable<TrialBalanceEntry> hashReturnedEntries,
                  TrialBalanceEntry entry,
                  StandardAccount currentParent) {

      var helper = new TrialBalanceHelper(_command);

      if (hashReturnedEntries.ContainsKey(GetHash(entry, currentParent, Sector.Empty))) {

        if (hashReturnedEntries[GetHash(entry, currentParent, Sector.Empty)
                               ].SubledgerAccountIdParent == entry.SubledgerAccountId &&
            hashReturnedEntries[GetHash(entry, currentParent, Sector.Empty)].NotHasSector) {

          hashReturnedEntries[GetHash(entry, currentParent, Sector.Empty)].Sum(entry);

        }
      } else {
        helper.SummaryByEntry(summaryParentEntries, entry, currentParent, Sector.Empty,
                             TrialBalanceItemType.BalanceSummary);
      }

    }


    private List<TrialBalanceEntry> CreateOrAccumulateTotalBySubsidiaryEntry(
                                      List<TrialBalanceEntry> returnedEntries,
                                      TrialBalanceEntry entry) {
      var helper = new TrialBalanceHelper(_command);

      var parentEntries = new EmpiriaHashTable<TrialBalanceEntry>();
      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      returnedEntries.ForEach(item =>
                          hashReturnedEntries.Insert(GetHash(item, item.Account, item.Sector), item));

      if (hashReturnedEntries.ContainsKey(GetHash(entry, StandardAccount.Empty, Sector.Empty))) {

        if (hashReturnedEntries[GetHash(entry, StandardAccount.Empty, Sector.Empty)
                               ].SubledgerAccountId == entry.SubledgerAccountIdParent) {

          if (entry.LastChangeDate > hashReturnedEntries[GetHash(entry, StandardAccount.Empty, Sector.Empty)
                                                        ].LastChangeDate) {

            hashReturnedEntries[GetHash(entry, StandardAccount.Empty, Sector.Empty)
                               ].LastChangeDate = entry.LastChangeDate;

          }
          hashReturnedEntries[GetHash(entry, StandardAccount.Empty, Sector.Empty)].Sum(entry);
        }

      } else {
        helper.SummaryBySubsidiaryEntry(parentEntries, entry, TrialBalanceItemType.BalanceSummary);

        var parent = parentEntries.Values.FirstOrDefault();
        parent.SubledgerAccountId = parent.SubledgerAccountIdParent;
        parent.SubledgerAccountNumber = entry.SubledgerAccountNumber;
        parent.LastChangeDate = entry.LastChangeDate;

        hashReturnedEntries.Insert(GetHash(parent, parent.Account, parent.Sector), parent);
      }

      return hashReturnedEntries.Values.ToList();
    }



    private EmpiriaHashTable<TrialBalanceEntry> CombineTotalSubsidiaryEntriesWithSummaryAccounts(
                                         EmpiriaHashTable<TrialBalanceEntry> summaryEntries) {

      var totaBySubsidiaryAccountList = OrderingSubsidiaryAccountsByNumber(summaryEntries);

      var balanceEntries = new List<TrialBalanceEntry>();

      foreach (var entry in totaBySubsidiaryAccountList) {
        balanceEntries = CreateOrAccumulateTotalBySubsidiaryEntry(balanceEntries, entry);
        balanceEntries = AddSummaryAccounts(summaryEntries, balanceEntries, entry);
      }

      EmpiriaHashTable<TrialBalanceEntry> hashReturnedEntries = GetHashSummaryEntries(balanceEntries);

      return hashReturnedEntries;
    }

    private EmpiriaHashTable<TrialBalanceEntry> GetHashSummaryEntries(List<TrialBalanceEntry> balanceEntries) {
      var returnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      balanceEntries.ForEach(entry => returnedEntries.Insert(
                              GetHash(entry, entry.Account, entry.Sector), entry));

      return returnedEntries;
    }

    private string GetHash(TrialBalanceEntry entry, StandardAccount account, Sector sector) {
      return $"{entry.Ledger.Number}||{entry.Currency.Code}||{account.Number}||" +
             $"{sector.Code}||{entry.SubledgerAccountIdParent}";
    }

    private EmpiriaHashTable<TrialBalanceEntry> GenerateOrIncreaseParentEntries(EmpiriaHashTable<TrialBalanceEntry> subsidiaryEntries) {
      var helper = new TrialBalanceHelper(_command);

      var hashReturnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();

      foreach (var entry in subsidiaryEntries.ToFixedList()) {
        entry.DebtorCreditor = entry.Account.DebtorCreditor;
        entry.SubledgerAccountIdParent = entry.SubledgerAccountId;

        var summaryParentEntries = new EmpiriaHashTable<TrialBalanceEntry>();

        StandardAccount account = entry.Account;

        int count = 0;
        while (true) {

          string hash = $"{entry.Ledger.Number}||{entry.Currency.Code}||" +
                        $"{account.Number}||{entry.Sector.Code}";

          if (hashReturnedEntries.ContainsKey(hash)) {

            SumInSubledgerAccounts(hashReturnedEntries, entry, hash, account);

          } else {
            count++;
            TrialBalanceItemType itemType = count == 1 ? TrialBalanceItemType.BalanceEntry :
                                                         TrialBalanceItemType.BalanceSummary;
            helper.SummaryByEntry(summaryParentEntries, entry, account, entry.Sector, itemType);

            if (!account.HasParent && entry.HasSector && entry.SubledgerAccountId > 0) {
              CreateOrAccumulateParentWithoutSector(summaryParentEntries, hashReturnedEntries, entry, account);
              break;
            } else if (!account.HasParent) {
              break;
            } else {
              account = account.GetParent();
            }
          }

        } // while

        foreach (var item in summaryParentEntries.Values.ToList()) {
          if (hashReturnedEntries.ContainsKey(GetHash(item, item.Account, item.Sector))) {
            hashReturnedEntries[GetHash(item, item.Account, item.Sector)].Sum(item);
          } else {
            hashReturnedEntries.Insert(GetHash(item, item.Account, item.Sector), item);
          }
        }
      } // foreach

      return hashReturnedEntries;
    }

    private void SumInSubledgerAccounts(EmpiriaHashTable<TrialBalanceEntry> hashReturnedEntries, TrialBalanceEntry entry, string hash, StandardAccount account) {
      do {
        if (hashReturnedEntries[hash].SubledgerAccountIdParent == entry.SubledgerAccountId &&
                hashReturnedEntries[hash].SubledgerAccountId == 0) {
          hashReturnedEntries[hash].Sum(entry);

        }

        if (!account.HasParent) {
          if (entry.HasSector) {
            AccumulateSubledgerAccount(hashReturnedEntries, entry, account);
          }
          break;
        } else {
          account = account.GetParent();
        }
      } while (false);
    }

    private List<TrialBalanceEntry> OrderingSubsidiaryAccountsByNumber(
                    EmpiriaHashTable<TrialBalanceEntry> totaBySubsidiaryAccountList) {

      var balanceEntries = totaBySubsidiaryAccountList.ToFixedList()
                            .Where(a => a.Level == 1 && a.NotHasSector).ToList();

      EmpiriaHashTable<TrialBalanceEntry> hashEntries = AssignSubsidiaryNumber(balanceEntries);

      var returnedEntries = hashEntries.ToFixedList()
                                       .Where(a => !a.SubledgerAccountNumber.Contains("undefined"))
                                       .OrderBy(a => a.Currency.Code)
                                       .ThenBy(a => a.SubledgerNumberOfDigits)
                                       .ThenBy(a => a.SubledgerAccountNumber)
                                       .ToList();
      return returnedEntries;
    }

    private EmpiriaHashTable<TrialBalanceEntry> AssignSubsidiaryNumber(
                                                List<TrialBalanceEntry> balanceEntries) {
      SubsidiaryAccount subsidiary;
      var returnedEntries = new EmpiriaHashTable<TrialBalanceEntry>();
      foreach (var entry in balanceEntries) {
        subsidiary = SubsidiaryAccount.Parse(entry.SubledgerAccountIdParent);
        entry.SubledgerAccountNumber = subsidiary.Number ?? "";
        entry.GroupName = subsidiary.Name ?? "";
        entry.SubledgerNumberOfDigits = entry.SubledgerAccountNumber != "" ?
                                 entry.SubledgerAccountNumber.Count() : 0;

        returnedEntries.Insert(GetHash(entry, entry.Account, entry.Sector), entry);
      }

      return returnedEntries;
    }

    #endregion Helper methods

  }  // class SaldosPorAuxiliar

}  // namespace Empiria.FinancialAccounting.BalanceEngine

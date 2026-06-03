/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Information Holder                      *
*  Type     : VoucherEntriesReclassificator              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Reclassifies the entries of an accounting voucher in operative transactions                    *
*             using a set of accounting rules.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;

using Empiria.FinancialAccounting.Vouchers;

using Empiria.FinancialAccounting.Reclassification.Data;

namespace Empiria.FinancialAccounting.Reclassification {

  /// <summary>Reclassifies the entries of an accounting voucher in
  /// operative transactions using a set of accounting rules.</summary>
  public class VoucherEntriesReclassificator {

    static readonly FixedList<AccountingRule> RULES = AccountingRule.GetList();

    private readonly FixedList<VoucherEntry> _toProcessEntries;

    #region Constructors and parsers

    internal VoucherEntriesReclassificator(Voucher voucher) {
      Assertion.Require(voucher, nameof(voucher));

      _toProcessEntries = new FixedList<VoucherEntry>(voucher.Entries);
    }

    #endregion Constructors and parsers

    #region Methods

    public void Execute() {

      foreach (var rule in RULES) {

        var debitEntries = FindDebitEntries(rule);

        foreach (var entry in debitEntries) {
          Group(entry, rule);
        }
      }
    }

    #endregion Methods

    #region Helpers

    private void Group(VoucherEntry debitEntry, AccountingRule rule) {

      var creditEntry = TryFindCreditEntry(rule, debitEntry);

      if (creditEntry == null) {
        return;
      }

      var entriesGroup = new List<VoucherEntry> {
        debitEntry, creditEntry
      };


      if (debitEntry.Currency.Equals(creditEntry.Currency)) {
        ProcessReclassifiedEntries(entriesGroup, rule);
        return;
      }

      if (debitEntry.Currency.Equals(Currency.UDI)) {

        var entryUDISDebe = TryFindEntry("9.01.03", debitEntry, isDebit: true);

        if (entryUDISDebe == null) {
          return;
        }

        var entryUDISHaber = TryFindEntry("9.01.04", debitEntry, isDebit: false);

        if (entryUDISHaber == null) {
          return;
        }

        entriesGroup.Add(entryUDISDebe);
        entriesGroup.Add(entryUDISHaber);

      } else {

        var entryMExtDebe = TryFindEntry("9.01.01", debitEntry, isDebit: true);

        if (entryMExtDebe == null) {
          return;
        }

        var entryMExtHaber = TryFindEntry("9.01.02", debitEntry, isDebit: false);

        if (entryMExtHaber == null) {
          return;
        }

        entriesGroup.Add(entryMExtDebe);
        entriesGroup.Add(entryMExtHaber);
      }

      ProcessReclassifiedEntries(entriesGroup, rule);
    }


    private void ProcessReclassifiedEntries(List<VoucherEntry> entries, AccountingRule rule) {

      string uid = Guid.NewGuid().ToString();

      var reclassifiedEntries = new List<ReclassifiedVoucherEntry>();

      foreach (var entry in entries) {

        var reclassifiedEntry = new ReclassifiedVoucherEntry(uid, entry, rule);

        reclassifiedEntries.Add(reclassifiedEntry);

        _toProcessEntries.Remove(entry);
      }

      reclassifiedEntries[0].ReclassifyCurrency(entries[0].Currency,
                                                entries[0].Amount);

      reclassifiedEntries[1].ReclassifyCurrency(entries[0].Currency,
                                                entries[0].Amount);

      ReclassificationDataService.UpdateReclassifiedVoucherEntries(reclassifiedEntries.ToFixedList());
    }



    private bool AmountsMatch(decimal a, decimal b) {
      return Math.Round(a, 2) == Math.Round(b, 2);
    }


    private VoucherEntry TryFindEntry(string accountNumber, VoucherEntry entry, bool isDebit) {
      if (isDebit) {
        return _toProcessEntries.Find(x => x.Debit > 0 && x.LedgerAccount.Number == accountNumber &&
                                         AmountsMatch(x.Amount * x.ExchangeRate,
                                                      entry.Amount * entry.ExchangeRate));
      } else {

        return _toProcessEntries.Find(x => x.Credit > 0 && x.LedgerAccount.Number == accountNumber &&
                                         AmountsMatch(x.Amount * x.ExchangeRate,
                                                      entry.Amount * entry.ExchangeRate));
      }
    }


    private VoucherEntry TryFindCreditEntry(AccountingRule rule, VoucherEntry debitEntry) {
      return _toProcessEntries.Find(x => x.Credit > 0 && x.LedgerAccount.Number == rule.CreditAccount &&
                                         x.SubledgerAccount.Number == debitEntry.SubledgerAccount.Number &&
                                         AmountsMatch(x.Amount * x.ExchangeRate,
                                                      debitEntry.Amount * debitEntry.ExchangeRate));
    }


    private FixedList<VoucherEntry> FindDebitEntries(AccountingRule rule) {
      return _toProcessEntries.FindAll(x => x.Debit > 0 && x.LedgerAccount.Number == rule.DebitAccount);
    }

    #endregion Helpers

  } // class VoucherEntriesReclassificator

} // namespace Empiria.FinancialAccounting.Reclassification

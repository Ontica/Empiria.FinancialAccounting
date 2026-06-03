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

    const string COMPRAVENTA_UDIS_DEBE = "9.01.03";
    const string COMPRAVENTA_UDIS_HABER = "9.01.04";
    const string COMPRAVENTA_EXTRANJERA_DEBE = "9.01.01";
    const string COMPRAVENTA_EXTRANJERA_HABER = "9.01.02";

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

        foreach (var debitEntry in debitEntries) {
          var creditEntry = TryFindCreditEntry(rule, debitEntry);

          if (creditEntry == null) {
            continue;
          }

          ExecuteRule(debitEntry, creditEntry, rule);
        }
      }
    }

    #endregion Methods

    #region Reclassificators

    private void ExecuteRule(VoucherEntry debitEntry, VoucherEntry creditEntry, AccountingRule rule) {

      var reclassifiedEntries = new List<VoucherEntry> {
        debitEntry, creditEntry
      };


      if (debitEntry.Currency.Equals(creditEntry.Currency)) {

        ProcessReclassifiedEntries(reclassifiedEntries, rule);

        return;
      }


      List<VoucherEntry> compraventaEntries = TryGetCompraventaEntries(debitEntry);

      if (compraventaEntries == null) {
        return;
      }

      reclassifiedEntries.AddRange(compraventaEntries);

      ProcessReclassifiedEntries(reclassifiedEntries, rule);
    }


    private void ProcessReclassifiedEntries(List<VoucherEntry> entries, AccountingRule rule) {

      string uid = Guid.NewGuid().ToString();

      var reclassifiedEntries = new List<ReclassifiedVoucherEntry>();

      foreach (var entry in entries) {

        var reclassifiedEntry = new ReclassifiedVoucherEntry(uid, entry, rule);

        reclassifiedEntries.Add(reclassifiedEntry);

        _toProcessEntries.Remove(entry);
      }

      reclassifiedEntries[0].ReclassifyCurrency(entries[0]);

      reclassifiedEntries[1].ReclassifyCurrency(entries[0]);

      ReclassificationDataService.UpdateReclassifiedVoucherEntries(reclassifiedEntries.ToFixedList());
    }


    private List<VoucherEntry> TryGetCompraventaEntries(VoucherEntry debitEntry) {

      var compraventaEntries = new List<VoucherEntry>();

      if (debitEntry.Currency.Equals(Currency.UDI)) {

        var entryUDISDebe = TryFindEntry(COMPRAVENTA_UDIS_DEBE, debitEntry, isDebit: true);

        if (entryUDISDebe == null) {
          return null;
        }

        var entryUDISHaber = TryFindEntry(COMPRAVENTA_UDIS_HABER, debitEntry, isDebit: false);

        if (entryUDISHaber == null) {
          return null;
        }

        compraventaEntries.Add(entryUDISDebe);
        compraventaEntries.Add(entryUDISHaber);

      } else {

        var entryMExtDebe = TryFindEntry(COMPRAVENTA_EXTRANJERA_DEBE, debitEntry, isDebit: true);

        if (entryMExtDebe == null) {
          return null;
        }

        var entryMExtHaber = TryFindEntry(COMPRAVENTA_EXTRANJERA_HABER, debitEntry, isDebit: false);

        if (entryMExtHaber == null) {
          return null;
        }

        compraventaEntries.Add(entryMExtDebe);
        compraventaEntries.Add(entryMExtHaber);
      }

      return compraventaEntries;
    }


    #endregion Reclassificators

    #region Helpers

    static private bool AmountsMatch(decimal a, decimal b) {
      return Math.Round(a, 2) == Math.Round(b, 2);
    }


    private FixedList<VoucherEntry> FindDebitEntries(AccountingRule rule) {
      return _toProcessEntries.FindAll(x => x.Debit > 0 && x.LedgerAccount.Number == rule.DebitAccount);
    }


    private VoucherEntry TryFindCreditEntry(AccountingRule rule, VoucherEntry debitEntry) {
      return _toProcessEntries.Find(x => x.Credit > 0 &&
                                         x.LedgerAccount.Number == rule.CreditAccount &&
                                         x.SubledgerAccount.Number == debitEntry.SubledgerAccount.Number &&
                                         AmountsMatch(x.Amount * x.ExchangeRate,
                                                      debitEntry.Amount * debitEntry.ExchangeRate));
    }


    private VoucherEntry TryFindEntry(string accountNumber, VoucherEntry entry, bool isDebit) {
      if (isDebit) {
        return _toProcessEntries.Find(x => x.Debit > 0 &&
                                           x.LedgerAccount.Number == accountNumber &&
                                           AmountsMatch(x.Amount * x.ExchangeRate,
                                                        entry.Amount * entry.ExchangeRate));
      } else {

        return _toProcessEntries.Find(x => x.Credit > 0 &&
                                           x.LedgerAccount.Number == accountNumber &&
                                           AmountsMatch(x.Amount * x.ExchangeRate,
                                                        entry.Amount * entry.ExchangeRate));
      }
    }

    #endregion Helpers

  } // class VoucherEntriesReclassificator

} // namespace Empiria.FinancialAccounting.Reclassification

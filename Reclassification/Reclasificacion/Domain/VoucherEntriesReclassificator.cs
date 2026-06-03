/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Service provider                        *
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

    const string COMPRAVENTA_EXTRANJERA = "9.01.02";
    const string COMPRAVENTA_EXTRANJERA_PESOS = "9.01.01";

    const string COMPRAVENTA_UDIS = "9.01.04";
    const string COMPRAVENTA_UDIS_PESOS = "9.01.03";

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

        var debitEntries = FindDebitEntries(rule.DebitAccount);

        foreach (var debitEntry in debitEntries) {

          var creditEntry = TryFindCreditEntry(rule.CreditAccount, debitEntry);

          if (creditEntry == null) {
            continue;
          }

          ExecuteRule(debitEntry, creditEntry, rule, false);

        }  // foreach debitEntry

      }  // foreach rule


      foreach (var cancelationRule in RULES) {

        var canceledEntries = FindDebitEntries(cancelationRule.CreditAccount);

        foreach (var canceledEntry in canceledEntries) {

          var creditEntry = TryFindCreditEntry(cancelationRule.DebitAccount, canceledEntry);

          if (creditEntry == null) {
            continue;
          }

          ExecuteRule(creditEntry, canceledEntry, cancelationRule, true);

        }  // foreach canceledEntry

      }  // foreach cancelationRule

    }

    #endregion Methods

    #region Reclassificators

    private void ExecuteRule(VoucherEntry baseEntry, VoucherEntry contraAccountEntry,
                             AccountingRule rule, bool isCancelation) {

      var reclassifiedEntries = new List<VoucherEntry> {
        baseEntry, contraAccountEntry
      };


      if (baseEntry.Currency.Equals(contraAccountEntry.Currency)) {

        ProcessReclassifiedEntries(reclassifiedEntries, rule);

        return;
      }

      FixedList<VoucherEntry> compraventaEntries = TryGetCompraventaEntries(baseEntry, !isCancelation);

      if (compraventaEntries == null) {
        return;
      }

      reclassifiedEntries.AddRange(compraventaEntries);

      ProcessReclassifiedEntries(reclassifiedEntries, rule);
    }


    private void ProcessReclassifiedEntries(List<VoucherEntry> entries, AccountingRule rule) {

      string entriesTxnUID = Guid.NewGuid().ToString();

      var reclassifiedEntries = new List<ReclassifiedVoucherEntry>();

      foreach (var entry in entries) {

        var reclassifiedEntry = new ReclassifiedVoucherEntry(entriesTxnUID, entry, rule);

        reclassifiedEntries.Add(reclassifiedEntry);

        _toProcessEntries.Remove(entry);
      }

      reclassifiedEntries[0].ReclassifyCurrency(entries[0]);

      reclassifiedEntries[1].ReclassifyCurrency(entries[0]);

      ReclassificationDataService.UpdateReclassifiedVoucherEntries(reclassifiedEntries.ToFixedList());
    }

    #endregion Reclassificators

    #region Helpers

    static private bool AmountsMatch(decimal a, decimal b) {
      return Math.Round(a, 2) == Math.Round(b, 2);
    }


    private FixedList<VoucherEntry> FindDebitEntries(string accountNumber) {
      return _toProcessEntries.FindAll(x => x.Debit > 0 && x.LedgerAccount.Number == accountNumber);
    }


    private VoucherEntry TryFindCreditEntry(string accountNumber, VoucherEntry debitEntry) {
      return _toProcessEntries.Find(x => x.Credit > 0 &&
                                         x.LedgerAccount.Number == accountNumber &&
                                         x.SubledgerAccount.Number == debitEntry.SubledgerAccount.Number &&
                                         AmountsMatch(x.Amount * x.ExchangeRate,
                                                      debitEntry.Amount * debitEntry.ExchangeRate));
    }


    private VoucherEntry TryFindEntry(string accountNumber, VoucherEntry entry, bool asDebit) {
      if (asDebit) {
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


    private FixedList<VoucherEntry> TryGetCompraventaEntries(VoucherEntry baseEntry, bool asDebit) {

      var compraventaEntries = new List<VoucherEntry>();

      if (baseEntry.Currency.Equals(Currency.UDI)) {

        return TryGetCompraventaPairEntries(COMPRAVENTA_UDIS_PESOS, COMPRAVENTA_UDIS, baseEntry, asDebit);

      } else {

        return TryGetCompraventaPairEntries(COMPRAVENTA_EXTRANJERA_PESOS, COMPRAVENTA_EXTRANJERA, baseEntry, asDebit);
      }
    }


    private FixedList<VoucherEntry> TryGetCompraventaPairEntries(string cuentaEnPesos, string cuentaMonedaBase,
                                                                 VoucherEntry baseEntry, bool asDebit) {

      var entryEnPesos = TryFindEntry(cuentaEnPesos, baseEntry, asDebit);

      if (entryEnPesos == null) {
        return null;
      }

      var entryEnMonedaBase = TryFindEntry(cuentaMonedaBase, baseEntry, !asDebit);

      if (entryEnMonedaBase == null) {
        return null;
      }

      return new VoucherEntry[] { entryEnPesos, entryEnMonedaBase }.ToFixedList();
    }

    #endregion Helpers

  } // class VoucherEntriesReclassificator

} // namespace Empiria.FinancialAccounting.Reclassification

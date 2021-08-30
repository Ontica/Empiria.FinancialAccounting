/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Service Provider                        *
*  Type     : VoucherValidator                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Validates a voucher before be sent to the ledger.                                              *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Validates a voucher before be sent to the ledger.</summary>
  internal class VoucherValidator {

    private readonly Voucher _voucher;

    public VoucherValidator(Voucher voucher) {
      _voucher = voucher;
    }

    internal bool IsValid() {
      return (ValidationResult().Count == 0);
    }

    internal FixedList<string> ValidationResult() {
      var resultList = new List<string>();

      var tempList = VoucherEntriesDataAreValid();
      if (tempList.Count > 0) {
        resultList.AddRange(tempList);
      }

      if (!HasEnoughVoucherEntries() ) {
        resultList.Add("La póliza debe tener cuando menos dos movimientos, un cargo y un abono.");
      }

      tempList = DebitsAndCreditsByCurrencyAreEqual();

      if (tempList.Count > 0) {
        resultList.AddRange(tempList);
      }
      if (!DebitsAndCreditsAreEqual()) {
        resultList.Add("La suma total de cargos y abonos no coincide.");
      }

      return new FixedList<string>(resultList);
    }

    #region Helper methods

    private bool DebitsAndCreditsAreEqual() {
      decimal debits = _voucher.Entries.Sum(x => x.Debit);
      decimal credits = _voucher.Entries.Sum(x => x.Credit);

      return debits == credits;
    }

    private FixedList<string> DebitsAndCreditsByCurrencyAreEqual() {
      var resultList = new List<string>();

      IEnumerable<Currency> currencies = _voucher.Entries.Select(x => x.Currency).Distinct();

      foreach (var currency in currencies) {
        var debitsEntries = _voucher.Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Debit && x.Currency.Equals(currency));
        var creditsEntries = _voucher.Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Credit && x.Currency.Equals(currency));

        decimal totalDebits = debitsEntries.Sum(x => x.Debit);
        decimal totalCredits = creditsEntries.Sum(x => x.Credit);

        if (totalDebits != totalCredits) {
          resultList.Add($"La suma de cargos no es igual a la suma de abonos en la moneda {currency.FullName}. " +
                          $"Diferencia de {(totalDebits - totalCredits).ToString("C")}");
        }

        totalDebits = debitsEntries.Sum(x => x.BaseCurrrencyAmount);
        totalCredits = creditsEntries.Sum(x => x.BaseCurrrencyAmount);

        if (debitsEntries.Sum(x => x.BaseCurrrencyAmount) != creditsEntries.Sum(x => x.BaseCurrrencyAmount)) {
          resultList.Add($"La suma de cargos y abonos en la moneda base no es igual para la moneda {currency.FullName}. " +
                         $"Diferencia de {(totalDebits - totalCredits).ToString("C")}. " +
                         $"Debe existir una diferencia en los tipos de cambio.");
        }
      }
      return resultList.ToFixedList();
    }


    private bool HasEnoughVoucherEntries() {
      return _voucher.Entries.Count >= 2;
    }

    private FixedList<string> VoucherEntriesDataAreValid() {
      var resultList = new List<string>();

      foreach (var entry in _voucher.Entries) {
        var entryResultList = VoucherEntryDataIsValid(entry);
        if (entryResultList.Count > 0) {
          resultList.AddRange(entryResultList);
        }
      }

      return resultList.ToFixedList();
    }

    private FixedList<string> VoucherEntryDataIsValid(VoucherEntry entry) {
      var resultList = new List<string>();

      var account = entry.LedgerAccount;
      var accountingDate = _voucher.AccountingDate;

      if (!account.Ledger.Equals(_voucher.Ledger)) {
        resultList.Add($"La cuenta {entry.LedgerAccount.Number} no pertenece a la contabilidad {_voucher.Ledger.Name}.");
      }

      try {
        account.CheckIsNotSummary(accountingDate);
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        account.CheckCurrencyRule(entry.Currency, accountingDate);
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (entry.HasSector) {
          account.CheckSectorRule(entry.Sector, accountingDate);
        } else {
          account.CheckNoSectorRule(accountingDate);
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (entry.HasSubledgerAccount && entry.HasSector) {
          account.CheckSubledgerAccountRule(entry.Sector, entry.SubledgerAccount, accountingDate);

        } else if (entry.HasSubledgerAccount && !entry.HasSector) {
          account.CheckSubledgerAccountRule(entry.SubledgerAccount, accountingDate);

        } else if (!entry.HasSubledgerAccount && entry.HasSector) {
          account.CheckNoSubledgerAccountRule(entry.Sector, accountingDate);

        } else if (!entry.HasSubledgerAccount && !entry.HasSector) {
          account.CheckNoSubledgerAccountRule(accountingDate);
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (!entry.HasEventType) {
          account.CheckNoEventTypeRule(accountingDate);
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      return resultList.ToFixedList();
    }


    #endregion Helper methods

  }  // class VoucherValidator

}  /// namespace Empiria.FinancialAccounting.Vouchers

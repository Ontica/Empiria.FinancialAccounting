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

using Empiria.FinancialAccounting.Vouchers.Adapters;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Validates a voucher before be sent to the ledger.</summary>
  internal class VoucherValidator {

    #region Public members

    public VoucherValidator(Ledger ledger, DateTime accountingDate) {
      this.Ledger = ledger;
      this.AccountingDate = accountingDate;
    }


    public DateTime AccountingDate {
      get;
    }


    public Ledger Ledger {
      get;
    }


    internal FixedList<string> Validate(FixedList<VoucherEntry> entries) {
      var converted = new FixedList<IVoucherEntry>(entries.Select(x => (IVoucherEntry) x));

      return Validate(converted);
    }


    internal FixedList<string> Validate(FixedList<VoucherEntryFields> entries) {
      var converted = new FixedList<IVoucherEntry>(entries.Select(x => (IVoucherEntry) x));

      return Validate(converted);
    }


    #endregion Public members

    #region Private methods

    private bool DebitsAndCreditsAreEqual(FixedList<IVoucherEntry> entries) {
      var debitsEntries = entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Debit);
      var creditsEntries = entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Credit);

      return debitsEntries.Sum(x => x.Amount) == creditsEntries.Sum(x => x.Amount);
    }


    private FixedList<string> DebitsAndCreditsByCurrencyAreEqual(FixedList<IVoucherEntry> entries) {
      var resultList = new List<string>();

      IEnumerable<Currency> currencies = entries.Select(x => x.Currency).Distinct();

      foreach (var currency in currencies) {
        var debitsEntries = entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Debit && x.Currency.Equals(currency));
        var creditsEntries = entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Credit && x.Currency.Equals(currency));

        decimal totalDebits = debitsEntries.Sum(x => x.Amount);
        decimal totalCredits = creditsEntries.Sum(x => x.Amount);

        if (totalDebits != totalCredits) {
          resultList.Add($"La suma de cargos no es igual a la suma de abonos en la moneda {currency.FullName}. " +
                          $"Diferencia de {(totalDebits - totalCredits).ToString("C")}");
          return resultList.ToFixedList();
        }

        totalDebits = debitsEntries.Sum(x => x.BaseCurrencyAmount);
        totalCredits = creditsEntries.Sum(x => x.BaseCurrencyAmount);

        if (totalDebits != totalCredits) {
          resultList.Add($"La suma de cargos y abonos en la moneda base no es igual para la moneda {currency.FullName}. " +
                         $"Diferencia de {(totalDebits - totalCredits).ToString("C")}. " +
                         $"Debe existir una diferencia en los tipos de cambio.");
        }
      }
      return resultList.ToFixedList();
    }


    private FixedList<string> Validate(FixedList<IVoucherEntry> entries) {
      var resultList = new List<string>();


      var tempList = VoucherEntriesDataAreValid(entries);
      if (tempList.Count > 0) {
        resultList.AddRange(tempList);
      }

      if (entries.Count < 2) {
        resultList.Add("La póliza debe tener cuando menos dos movimientos, un cargo y un abono.");
      }

      tempList = DebitsAndCreditsByCurrencyAreEqual(entries);

      if (tempList.Count > 0) {
        resultList.AddRange(tempList);
      }
      if (!DebitsAndCreditsAreEqual(entries)) {
        resultList.Add("La suma total de cargos y abonos no coincide.");
      }

      return new FixedList<string>(resultList);
    }


    private FixedList<string> VoucherEntriesDataAreValid(FixedList<IVoucherEntry> entries) {
      var resultList = new List<string>();

      var validator = new VoucherEntryValidator(this.Ledger, this.AccountingDate);

      foreach (var entry in entries) {
        FixedList<string> issues = validator.Validate(entry);

        if (issues.Count > 0) {
          resultList.AddRange(issues);
        }
      }

      return resultList.ToFixedList();
    }

    #endregion Private methods

  }  // class VoucherValidator

}  /// namespace Empiria.FinancialAccounting.Vouchers

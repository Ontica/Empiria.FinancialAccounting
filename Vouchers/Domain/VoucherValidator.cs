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
      List<string> resultList = new List<string>();

      if (!HasEnoughVoucherEntries() ) {
        resultList.Add("La póliza debe tener cuando menos dos movimientos, un cargo y un abono.");
      }
      var tempList = DebitsAndCreditsByCurrencyAreEqual();
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
      List<string> resultList = new List<string>();

      IEnumerable<Currency> currencies = _voucher.Entries.Select(x => x.Currency).Distinct();

      foreach (var currency in currencies) {
        var debitsEntries = _voucher.Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Debit && x.Currency.Equals(currency));
        var creditsEntries = _voucher.Entries.FindAll(x => x.VoucherEntryType == VoucherEntryType.Credit && x.Currency.Equals(currency));

        if (debitsEntries.Sum(x => x.Debit) != creditsEntries.Sum(x => x.Credit)) {
          resultList.Add($"La suma de cargos no es igual a la suma de abonos en la moneda {currency.FullName}.");
        }
      }
      return resultList.ToFixedList();
    }


    private bool HasEnoughVoucherEntries() {
      return _voucher.Entries.Count >= 2;
    }


    #endregion Helper methods

  }  // class VoucherValidator

}  /// namespace Empiria.FinancialAccounting.Vouchers

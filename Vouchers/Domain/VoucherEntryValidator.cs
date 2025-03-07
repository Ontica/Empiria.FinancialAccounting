/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Vouchers Management                        Component : Domain Layer                            *
*  Assembly : FinancialAccounting.Vouchers.dll           Pattern   : Service Provider                        *
*  Type     : VoucherEntryValidator                      License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Validates a voucher entry before be sent to the ledger.                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.Collections.Generic;

namespace Empiria.FinancialAccounting.Vouchers {

  /// <summary>Validates a voucher entry before be sent to the ledger.</summary>
  internal class VoucherEntryValidator {

    public VoucherEntryValidator(Ledger ledger, DateTime accountingDate) {
      this.Ledger = ledger;
      this.AccountingDate = accountingDate;
    }

    public Ledger Ledger {
      get;
    }

    public DateTime AccountingDate {
      get;
    }

    internal void EnsureValid(IVoucherEntry fields) {
      FixedList<string> resultList = Validate(fields);

      if (resultList.Count > 0) {
        Assertion.RequireFail(resultList[0]);
      }
    }


    internal FixedList<string> Validate(IVoucherEntry entry) {
      var resultList = new List<string>();

      Account account;

      try {
        account = entry.GetAccount(this.AccountingDate);

      } catch (Exception e) {
        resultList.Add(e.Message);

        return resultList.ToFixedList();
      }

      var accountAssertions = new AccountAssertions(account, this.AccountingDate);

      if (accountAssertions.CanSkipAssertionChecking()) {
        return resultList.ToFixedList();
      }

      try {
        accountAssertions.AssertIsNotSummary();
      } catch (Exception e) {
        resultList.Add(e.Message);

        return resultList.ToFixedList();
      }

      try {
        accountAssertions.AssertCurrencyRule(entry.Currency);
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (entry.HasSector) {
          accountAssertions.AssertSectorRule(entry.Sector);
        } else {
          accountAssertions.AssertNoSectorRule();
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (entry.HasSubledgerAccount && entry.HasSector) {
          accountAssertions.AssertSubledgerAccountRuleWithSector(entry.Sector);

        } else if (entry.HasSubledgerAccount && !entry.HasSector) {
          accountAssertions.AssertSubledgerAccountRuleWithNoSector();

        } else if (!entry.HasSubledgerAccount && entry.HasSector) {
          accountAssertions.AssertNoSubledgerAccountRuleWithSector(entry.Sector);

        } else if (!entry.HasSubledgerAccount && !entry.HasSector) {
          accountAssertions.AssertNoSubledgerAccountRuleWithNoSector();
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      if (entry.HasSubledgerAccount) {
        SubledgerAccount subledgerAccount = entry.GetSubledgerAccount();

        if (!subledgerAccount.IsEmptyInstance) {

          if (subledgerAccount.Suspended) {
            resultList.Add($"El auxiliar '{subledgerAccount.Number}' ({subledgerAccount.Name}) " +
                           $"está suspendido, por lo que no permite operaciones de registro.");
          }

          if (!subledgerAccount.BelongsTo(this.Ledger)) {
            resultList.Add($"El auxiliar '{subledgerAccount.Number}' ({subledgerAccount.Name}) " +
                           $"no pertenece a la contabilidad {Ledger.FullName}.");
          }
        }
      }

      try {
        if (!entry.HasEventType) {
          accountAssertions.AssertNoEventTypeRule();
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      if (entry.Amount <= 0) {
        resultList.Add($"El importe del cargo o abono debe ser mayor que cero.");
      }

      if (entry.Amount != Math.Round(entry.Amount, 2)) {
        resultList.Add($"El importe del movimiento tiene más de dos decimales: {entry.Amount}");
      }

      return resultList.ToFixedList();
    }

   }  // class VoucherEntryValidator

}  // namespace Empiria.FinancialAccounting.Vouchers

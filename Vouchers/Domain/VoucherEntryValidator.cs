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
        Assertion.AssertFail(resultList[0]);
      }
    }


    internal FixedList<string> Validate(IVoucherEntry entry) {
      var resultList = new List<string>();

      Account account = entry.GetAccount(this.AccountingDate);

      try {
        account.CheckIsNotSummary(this.AccountingDate);
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        account.CheckCurrencyRule(entry.Currency, this.AccountingDate);
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (entry.HasSector) {
          account.CheckSectorRule(entry.Sector, this.AccountingDate);
        } else {
          account.CheckNoSectorRule(this.AccountingDate);
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (entry.HasSubledgerAccount && entry.HasSector) {
          account.CheckSubledgerAccountRule(entry.Sector, this.AccountingDate);

        } else if (entry.HasSubledgerAccount && !entry.HasSector) {
          account.CheckSubledgerAccountRule(this.AccountingDate);

        } else if (!entry.HasSubledgerAccount && entry.HasSector) {
          account.CheckNoSubledgerAccountRule(entry.Sector, this.AccountingDate);

        } else if (!entry.HasSubledgerAccount && !entry.HasSector) {
          account.CheckNoSubledgerAccountRule(this.AccountingDate);
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (!entry.HasEventType) {
          account.CheckNoEventTypeRule(this.AccountingDate);
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      return resultList.ToFixedList();
    }

  }  // class VoucherEntryValidator

}  /// namespace Empiria.FinancialAccounting.Vouchers

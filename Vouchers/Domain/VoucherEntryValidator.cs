﻿/* Empiria Financial *****************************************************************************************
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

    public VoucherEntryValidator(Ledger ledger, DateTime accountingDate, bool checkProtectedAccounts) {
      this.Ledger = ledger;
      this.AccountingDate = accountingDate;
      this.CheckProtectedAccounts = checkProtectedAccounts;
    }

    public Ledger Ledger {
      get;
    }

    public DateTime AccountingDate {
      get;
    }

    public bool CheckProtectedAccounts {
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

      if (account.Number == "6.05.01.02.03.03" && this.AccountingDate == new DateTime(2023, 01, 01)) {
        return resultList.ToFixedList();
      }

      try {
        account.CheckIsNotSummary();
      } catch (Exception e) {
        resultList.Add(e.Message);

        return resultList.ToFixedList();
      }

      try {
        if (this.CheckProtectedAccounts) {
          account.CheckIsNotProtectedForEdition();
        }
      } catch (Exception e) {
        resultList.Add(e.Message);

        return resultList.ToFixedList();
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
          account.CheckNoSectorRule();
        }
      } catch (Exception e) {
        resultList.Add(e.Message);
      }

      try {
        if (entry.HasSubledgerAccount && entry.HasSector) {
          account.CheckSubledgerAccountRule(entry.Sector, this.AccountingDate);

        } else if (entry.HasSubledgerAccount && !entry.HasSector) {
          account.CheckSubledgerAccountRule();

        } else if (!entry.HasSubledgerAccount && entry.HasSector) {
          account.CheckNoSubledgerAccountRule(entry.Sector, this.AccountingDate);

        } else if (!entry.HasSubledgerAccount && !entry.HasSector) {
          account.CheckNoSubledgerAccountRule();
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
          account.CheckNoEventTypeRule();
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

/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts Chart Edition                     Component : Data Access Layer                       *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Data Service                            *
*  Type     : AccountEditionDataService                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides write data services for accounts edition.                                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition.Data {

  /// <summary>Provides write data services for accounts edition.</summary>
  static internal class AccountEditionDataService {


    static internal DataOperation AddAccountCurrencyOp(long stdAccountId, Currency currency,
                                                       DateTime applicationDate) {
      return DataOperation.Parse("write_cof_mapeo_moneda",
                                  stdAccountId, currency.Id,
                                  applicationDate.Date, Account.MAX_END_DATE);
    }


    static internal DataOperation AddAccountSectorOp(long stdAccountId, Sector sector,
                                                     AccountRole sectorRole,
                                                     DateTime applicationDate) {
      return DataOperation.Parse("write_cof_mapeo_sector",
                                  stdAccountId, sector.Id,
                                  (char) sectorRole, applicationDate.Date,
                                  Account.MAX_END_DATE);
    }


    static internal (long, DataOperation) CreateStandardAccountOp(AccountEditionCommand o) {

      long stdAccountId = 0;
      long stdAccountHistoryId = 0;

      if (!o.DryRun) {
        stdAccountId = CommonMethods.GetNextObjectId("SEC_ID_CUENTA_ESTANDAR");
        stdAccountHistoryId = CommonMethods.GetNextObjectId("SEC_ID_CUENTA_ESTANDAR_HIST");
      }


      var op = DataOperation.Parse("do_create_cuenta_estandar",
                         stdAccountId, o.GetAccountsChart().Id, o.AccountFields.AccountNumber,
                         o.AccountFields.Name, "Agregada con el importador en fase de pruebas",
                         (char) o.AccountFields.Role, o.AccountFields.GetAccountType().Id,
                         (char) o.AccountFields.DebtorCreditor,
                         o.ApplicationDate.Date, Account.MAX_END_DATE,
                         stdAccountHistoryId, Guid.NewGuid().ToString(),
                         BuildKeywords(o.AccountFields));


      return (stdAccountId, op);
    }


    static internal void Execute(FixedList<DataOperation> operations) {
      //no-op
    }


    static internal void Execute(DataOperationList list) {
      DataWriter.Execute(list);
    }


    static internal DataOperation FixStandardAccountNameOp(Account account, string newName) {
      return DataOperation.Parse("do_fix_nombre_cuenta_estandar",
                                 account.StandardAccountId, account.AccountsChart.Id,
                                 account.Number, newName, BuildKeywords(account, newName));
    }


    static internal DataOperation RemoveAccountCurrencyOp(CurrencyRule o, DateTime applicationDate) {
      return DataOperation.Parse("write_cof_mapeo_moneda",
                                  o.StandardAccountId, o.Currency,
                                  o.StartDate, applicationDate.Date.AddDays(-1));
    }


    static internal DataOperation RemoveAccountSectorOp(SectorRule o, DateTime applicationDate) {
      return DataOperation.Parse("write_cof_mapeo_sector",
                                  o.StandardAccountId, o.Sector.Id,
                                  (char) o.SectorRole,
                                  o.StartDate, applicationDate.Date.AddDays(-1));
    }


    static internal DataOperation UpdateStandardAccountOp(Account account, AccountEditionCommand o) {
      long stdAccountHistoryId = 0;

      if (!o.DryRun) {
        stdAccountHistoryId = CommonMethods.GetNextObjectId("SEC_ID_CUENTA_ESTANDAR_HIST");
      }

      var dataToBeUpdated = o.DataToBeUpdated.ToFixedList();

      string name = dataToBeUpdated.Contains(AccountDataToBeUpdated.Name) ?
                                                o.AccountFields.Name : account.Name;

      AccountType accountType = dataToBeUpdated.Contains(AccountDataToBeUpdated.AccountType) ?
                                                o.AccountFields.GetAccountType() : account.AccountType;

      DebtorCreditorType debtorCreditor = dataToBeUpdated.Contains(AccountDataToBeUpdated.DebtorCreditor) ?
                                                o.AccountFields.DebtorCreditor : account.DebtorCreditor;

      AccountRole role = dataToBeUpdated.Contains(AccountDataToBeUpdated.MainRole) ||
                         dataToBeUpdated.Contains(AccountDataToBeUpdated.SubledgerRole) ?
                                                o.AccountFields.Role : account.Role;

      string keywords = EmpiriaString.BuildKeywords(account.Number, name, accountType.Name,
                                                    debtorCreditor.ToString(), role.ToString());

      return DataOperation.Parse("do_update_cuenta_estandar",
                        account.Id, account.AccountsChart.Id, account.Number,
                        name, "Modificada con el importador en fase de pruebas",
                        (char) role, accountType.Id, (char) debtorCreditor,
                        o.ApplicationDate.Date, Account.MAX_END_DATE,
                        stdAccountHistoryId, Guid.NewGuid().ToString(), keywords);
    }


    #region Helpers

    static private string BuildKeywords(AccountFieldsDto o) {
      return EmpiriaString.BuildKeywords(o.AccountNumber, o.Name,
                                         o.GetAccountType().Name, o.Description,
                                         o.DebtorCreditor.ToString(),
                                         o.Role.ToString());
    }


    private static string BuildKeywords(Account o, string newName) {
      return EmpiriaString.BuildKeywords(o.Number, newName,
                                         o.AccountType.Name, o.Description,
                                         o.DebtorCreditor.ToString(),
                                         o.Role.ToString());
    }

    #endregion Helpers

  }  // class AccountEditionDataService

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.Data

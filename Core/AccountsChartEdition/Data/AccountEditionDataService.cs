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
using System.Collections.Generic;

using Empiria.Data;

using Empiria.FinancialAccounting.AccountsChartEdition.Adapters;

namespace Empiria.FinancialAccounting.AccountsChartEdition.Data {

  /// <summary>Provides write data services for accounts edition.</summary>
  static internal class AccountEditionDataService {


    static internal DataOperation AddAccountCurrencyOp(long stdAccountId, Currency currency, DateTime applicationDate) {
      return DataOperation.Parse("write_cof_mapeo_moneda",
                                  stdAccountId, currency.Id,
                                  applicationDate, Account.MAX_END_DATE);
    }


    static internal DataOperation AddAccountSectorOp(long stdAccountId, Sector sector,
                                                     AccountRole sectorRole,
                                                     DateTime applicationDate) {
      return DataOperation.Parse("write_cof_mapeo_sector",
                                  stdAccountId, sector.Id,
                                  (char) sectorRole, applicationDate,
                                  Account.MAX_END_DATE);
    }


    static internal (long, DataOperation) CreateStandardAccountOp(AccountEditionCommand o) {

      long stdAccountId = 0;
      long stdAccountHistoryId = 0;

      if (!o.DryRun) {
        stdAccountId = CommonMethods.GetNextObjectId("SEC_ID_CUENTA_ESTANDAR");
        stdAccountHistoryId = CommonMethods.GetNextObjectId("SEC_ID_CUENTA_ESTANDAR_HIST");
      }


      var op = DataOperation.Parse("do_create_cof_cuenta_estandar",
                         stdAccountId, o.GetAccountsChart().Id, o.AccountFields.AccountNumber,
                         o.AccountFields.Name, "Agregada con el importador en fase de pruebas",
                         (char) o.AccountFields.Role, o.AccountFields.GetAccountType().Id,
                         (char) o.AccountFields.DebtorCreditor, o.ApplicationDate, Account.MAX_END_DATE,
                         stdAccountHistoryId, Guid.NewGuid().ToString(), BuildKeywords(o.AccountFields));


      return (stdAccountId, op);
    }


    static internal void Execute(FixedList<DataOperation> operations) {
      //no-op
    }


    static internal void Execute(DataOperationList list) {
      DataWriter.Execute(list);
    }


    #region Helpers

    static private string BuildKeywords(AccountFieldsDto o) {
      return EmpiriaString.BuildKeywords(o.AccountNumber, o.Name,
                                         o.GetAccountType().Name, o.Description,
                                         o.DebtorCreditor.ToString(),
                                         o.Role.ToString());
    }


    #endregion Helpers

  }  // class AccountEditionDataService

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.Data

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


    static internal void Execute(FixedList<DataOperation> operations) {
      //no-op
    }

    static internal void Execute(DataOperationList list) {
      DataWriter.Execute(list);
    }


    static internal long GetNextStandardAccountHistoryId() {
      return CommonMethods.GetNextObjectId("SEC_ID_CUENTA_ESTANDAR_HIST");
    }


    static internal long GetNextStandardAccountId() {
      return CommonMethods.GetNextObjectId("SEC_ID_CUENTA_ESTANDAR");
    }


    static internal DataOperation WriteStandardAccountOp(long stdAccountId,
                                                         AccountEditionCommand o) {

      return DataOperation.Parse("write_cof_cuenta_estandar",
                    stdAccountId, o.GetAccountsChart().Id,
                    o.AccountFields.AccountNumber, o.AccountFields.Name,
                    "Agregada con el importador en fase de pruebas",
                    (char) o.AccountFields.Role, o.AccountFields.GetAccountType().Id,
                    (char) o.AccountFields.DebtorCreditor);
    }


    static internal FixedList<DataOperation> WriteStandardAccountHistoryOp(long stdAccountId,
                                                                           long stdAccountHistoryId,
                                                                           AccountEditionCommand o) {
      var op = DataOperation.Parse("write_cof_cuenta_estandar_hist",
                        stdAccountId, stdAccountHistoryId,
                        o.GetAccountsChart().Id, o.AccountFields.AccountNumber,
                        o.AccountFields.Name, "Agregada con el importador en fase de pruebas",
                        (char) o.AccountFields.Role, o.AccountFields.GetAccountType().Id,
                        (char) o.AccountFields.DebtorCreditor, o.ApplicationDate, Account.MAX_END_DATE);

      var op2 = DataOperation.Parse("write_cof_cuenta_est_hist_bis",
                        stdAccountHistoryId, Guid.NewGuid().ToString(),
                        BuildKeywords(o.AccountFields));

      return new[] { op, op2 }.ToFixedList();
    }


    static private string BuildKeywords(AccountFieldsDto o) {
      return EmpiriaString.BuildKeywords(o.AccountNumber, o.Name,
                                         o.GetAccountType().Name, o.Description,
                                         o.DebtorCreditor.ToString(),
                                         o.Role.ToString());
    }


    internal static DataOperation AddAccountSectorOp(long stdAccountId, Sector sector,
                                                     AccountRole sectorRole,
                                                     DateTime applicationDate) {
      return DataOperation.Parse("write_cof_mapeo_sector",
                                  stdAccountId, sector.Id,
                                  sectorRole, applicationDate,
                                  Account.MAX_END_DATE);
    }
  }  // class AccountEditionDataService

}  // namespace Empiria.FinancialAccounting.AccountsChartEdition.Data

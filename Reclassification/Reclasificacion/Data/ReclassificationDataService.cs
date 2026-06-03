/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Data Services                           *
*  Type     : ReclassificationDataService                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data access for reclassification accounting data.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.Reclassification.Data {

  /// <summary>Provides data access for reclassification accounting data.</summary>
  static internal class ReclassificationDataService {

    static internal FixedList<AccountingRule> GetAccountingRules() {

      var sql = "SELECT * FROM COF_REGLAS WHERE STATUS ='A' ";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<AccountingRule>(op);
    }


    static internal void UpdateReclassifiedVoucherEntries(FixedList<ReclassifiedVoucherEntry> entries) {

      foreach (var o in entries) {

        var op = DataOperation.Parse("write_cof_movimiento_bis",
          o.VoucherEntry.Id, -1, string.Empty, string.Empty, -1, -1,
          DateTime.Today, string.Empty, -1, o.UID, o.NewCurrency.Id,
          o.NewAmount, 0, o.OperationType.Id, o.AccountingRule.Id);

        DataWriter.Execute(op);

      }  // foreach
    }

  }  // class ReclassificationDataService

}  // namespace Empiria.FinancialAccounting.Reclassification.Data

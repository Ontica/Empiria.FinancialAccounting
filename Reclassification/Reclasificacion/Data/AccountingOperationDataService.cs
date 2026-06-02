/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Data Services                           *
*  Type     : AccountingOperationDataService             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data access for accounting operation data.                                            *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using Empiria.Data;

namespace Empiria.FinancialAccounting.Reclassification.Data {

  /// <summary>Provides data access for accounting operation data.</summary>
  static internal class AccountingOperationDataService {

    static internal FixedList<AccountingOperation> GetOperations() {


      var sql = "SELECT * FROM COF_REGLAS WHERE STATUS ='A' ";

      var op = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<AccountingOperation>(op);
    }


    static internal void UpdateTransaction(RealTransaction realTransaction) {


      var op = DataOperation.Parse("write_cof_movimiento_bis", realTransaction.Transaction.Id, -1,
                            string.Empty, string.Empty, -1, -1,
                            DateTime.Today, string.Empty, -1,
                           realTransaction.UID, realTransaction.IdMonedaReal, realTransaction.MontoReal, 0, realTransaction.AccountingOperationType.Id,
                           realTransaction.AccountingOperation.Id

              );

      DataWriter.Execute(op);
    }



  }  // class AccountingOperationDataService

}  // namespace Empiria.FinancialAccounting.Reclassification.Data
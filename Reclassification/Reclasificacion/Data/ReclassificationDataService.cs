/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Reclassification Services                  Component : Data Layer                              *
*  Assembly : FinancialAccounting.Reclassification.dll   Pattern   : Data Services                           *
*  Type     : ReclassificationDataService                License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides data access for reclassification accounting data.                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

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

        var sql = "UPDATE COF_MOVIMIENTO_BIS SET " +
                      $"UID_TXN_REAL = '{o.UID}', " +
                      $"ID_TIPO_TXN_REAL = {o.OperationType.Id}, " +
                      $"ID_MONEDA_REAL = '{o.NewCurrency.Id}', " +
                      $"MONTO_REAL = {o.NewAmount:0.00}, " +
                      $"TIPO_CAMBIO_REAL = 0, " +
                      $"ID_GUIA_CONTABLE = {o.AccountingRule.Id} " +
                  $"WHERE ID_MOVIMIENTO = {o.VoucherEntry.Id}";

        var op = DataOperation.Parse(sql);

        DataWriter.Execute(op);

      }  // foreach
    }

  }  // class ReclassificationDataService

}  // namespace Empiria.FinancialAccounting.Reclassification.Data

/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Transaction Slips                             Component : Data service                         *
*  Assembly : FinancialAccounting.BanobrasIntegration.dll   Pattern   : Data service                         *
*  Type     : TransactionSlipSearcher                       License   : Please read LICENSE.txt file         *
*                                                                                                            *
*  Summary  : Provides search services over transaction slips (volantes).                                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Data;

namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips {

  static internal class TransactionSlipData {


    static internal FixedList<TransactionSlip> GetPendingTransactionSlips(string filter, string sort) {
      string sql = "SELECT VW_MC_ENCABEZADOS.*, -1 AS ID_VOLANTE, -1 AS ID_TRANSACCION, " +
                   "'1900-01-01' AS FECHA_PROCESO, 'P' AS STATUS " +
                   "FROM VW_MC_ENCABEZADOS";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionSlip>(operation);
    }


    static internal FixedList<TransactionSlip> GetProcessedTransactionSlips(string filter, string sort) {
      string sql = "SELECT * FROM COF_VOLANTES";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionSlip>(operation);
    }


    static internal FixedList<TransactionSlipEntry> GetTransactionSlipEntries(TransactionSlip slip) {
      string sql = "SELECT * " +
                   "FROM COF_VOLANTES_MOVIMIENTOS " +
                  $"WHERE ID_VOLANTE = {slip.Id} " +
                   "ORDER BY MCO_FOLIO";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionSlipEntry>(operation);
    }


  }  // class TransactionSlipData

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips

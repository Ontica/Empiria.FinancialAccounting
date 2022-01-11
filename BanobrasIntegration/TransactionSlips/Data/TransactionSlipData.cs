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


    static internal TransactionSlip GetPendingTransactionSlip(string uid) {
      string sql = "SELECT * " +
                   "FROM VW_MC_ENCABEZADOS " +
                  $"WHERE ENC_VOLANTE_UID = '{uid}'";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<TransactionSlip>(operation);
    }


    static internal FixedList<TransactionSlip> GetPendingTransactionSlips(string filter) {
      string sql = "SELECT * " +
                   "FROM VW_MC_ENCABEZADOS";

      if (filter.Length > 0) {
        sql += $" WHERE {filter}";
      }

      sql += $" ORDER BY ENC_FECHA_VOL, ENC_NUM_VOL";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionSlip>(operation);
    }


    static internal FixedList<TransactionSlipEntry> GetPendingTransactionSlipEntries(TransactionSlip slip) {
      string sql = "SELECT * " +
                   "FROM VW_MC_MOVIMIENTOS " +
                  $"WHERE ENC_VOLANTE_UID = '{slip.UID}' " +
                   "ORDER BY MCO_FOLIO";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionSlipEntry>(operation);
    }


    static internal TransactionSlip GetProcessedTransactionSlip(string slipUID) {
      string sql = "SELECT * " +
                   "FROM VW_COF_VOLANTES " +
                  $"WHERE ENC_VOLANTE_UID = '{slipUID}'";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObject<TransactionSlip>(operation);
    }


    static internal FixedList<TransactionSlip> GetProcessedTransactionSlips(string filter) {
      string sql = "SELECT * " +
                   "FROM VW_COF_VOLANTES";

      if (filter.Length > 0) {
        sql += $" WHERE {filter}";
      }

      sql += $" ORDER BY ENC_FECHA_VOL, ENC_NUM_VOL";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionSlip>(operation);
    }


    static internal FixedList<TransactionSlipEntry> GetProcessedTransactionSlipEntries(TransactionSlip slip) {
      string sql = "SELECT * " +
                   "FROM COF_VOLANTES_MOVIMIENTOS " +
                  $"WHERE ID_VOLANTE = {slip.Id} " +
                   "ORDER BY MCO_FOLIO";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionSlipEntry>(operation);
    }


    static internal FixedList<TransactionSlipIssue> GetTransactionSlipIssues(TransactionSlip slip) {
      string sql = "SELECT * " +
                   "FROM COF_VOLANTES_ISSUES " +
                  $"WHERE ID_VOLANTE = {slip.Id} " +
                   "ORDER BY ID_VOLANTE_ISSUE";

      var operation = DataOperation.Parse(sql);

      return DataReader.GetPlainObjectFixedList<TransactionSlipIssue>(operation);
    }

  }  // class TransactionSlipData

}  // namespace Empiria.FinancialAccounting.BanobrasIntegration.TransactionSlips
